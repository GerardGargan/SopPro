import axios from "axios";
import store from "../store/index";
import { authActions } from "../store/authSlice";

const api = axios.create({
  baseURL: `${process.env.EXPO_PUBLIC_API_URL}`,
  headers: {
    "Content-Type": "application/json",
  },
});

// Flag to prevent multiple refresh requests
let isRefreshing = false;
// Queue of failed requests to retry after token refresh
let failedQueue = [];

const processQueue = (error, token = null) => {
  failedQueue.forEach((prom) => {
    if (error) {
      prom.reject(error);
    } else {
      prom.resolve(token);
    }
  });

  failedQueue = [];
};

// attach the users bearer token to requests being sent
api.interceptors.request.use(
  (config) => {
    const token = store.getState().auth.token;

    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Logging interceptor for requests
api.interceptors.request.use(
  (config) => {
    console.log("API Request:");
    console.log("URL:", config.url);
    console.log("Method:", config.method);
    console.log("Headers:", config.headers);
    if (config.data) {
      console.log("Data:", JSON.stringify(config.data, null, 2));
    }

    return config;
  },
  (error) => {
    console.error("Request Error:", error);
    return Promise.reject(error);
  }
);

// Response interceptor with token refresh logic
api.interceptors.response.use(
  (response) => {
    console.log("Response Data:", JSON.stringify(response.data, null, 2));
    return response;
  },
  async (error) => {
    const originalRequest = error.config;

    // Log error details
    if (error.response) {
      console.error(
        "Response Error Data:",
        JSON.stringify(error.response.data, null, 2)
      );
      console.error("Response Error Status:", error.response.status);
      console.error(
        "Response Error Headers:",
        JSON.stringify(error.response.headers, null, 2)
      );

      // Checking the authentication header to differentiate between unauthorized status and invalid token specifically
      const authHeader = error.response.headers["www-authenticate"];
      const isTokenExpired =
        authHeader &&
        authHeader.includes('Bearer error="invalid_token"') &&
        authHeader.includes('error_description="The token expired at');

      if (
        error.response.status === 401 &&
        isTokenExpired &&
        !originalRequest._retry
      ) {
        if (isRefreshing) {
          // If already refreshing, add request to queue
          return new Promise((resolve, reject) => {
            failedQueue.push({ resolve, reject });
          })
            .then((token) => {
              originalRequest.headers["Authorization"] = `Bearer ${token}`;
              return api(originalRequest);
            })
            .catch((err) => {
              return Promise.reject(err);
            });
        }

        originalRequest._retry = true;
        isRefreshing = true;

        try {
          // Get refresh token from store
          const refreshToken = store.getState().auth.refreshToken;
          const currentToken = store.getState().auth.token;

          if (!refreshToken) {
            // No refresh token available, log out the user
            store.dispatch(authActions.logout());
            throw new Error("No refresh token available, logging out");
          }

          // Request new token
          const response = await axios.post(
            `${process.env.EXPO_PUBLIC_API_URL}/auth/refresh`,
            {
              token: currentToken,
              refreshToken: refreshToken,
            },
            {
              headers: { "Content-Type": "application/json" },
            }
          );

          // Extract new tokens from response
          const { token, refreshToken: newRefreshToken } = response.data;

          // Update tokens in store
          store.dispatch(
            authActions.setToken({ token, refreshToken: newRefreshToken })
          );

          // Update Authorization header for original request
          originalRequest.headers["Authorization"] = `Bearer ${token}`;

          // Process queued requests
          processQueue(null, token);

          // Retry the original request with new token
          return api(originalRequest);
        } catch (refreshError) {
          // Refresh failed, reject all queued requests
          processQueue(refreshError, null);

          console.error("Token refresh failed:", refreshError);

          // Check if the refresh error is due to an expired refresh token
          if (refreshError.response && refreshError.response.status === 401) {
            console.log("Refresh token expired or invalid. Logging out user.");
            store.dispatch(authActions.logout());
          }

          return Promise.reject(refreshError);
        } finally {
          isRefreshing = false;
        }
      }
    } else if (error.request) {
      // No response received
      console.error("No Response Received:", error.request);
    } else {
      // Something went wrong in setting up the request
      console.error("Error Config:", error.message);
    }

    return Promise.reject(error);
  }
);

export default api;
