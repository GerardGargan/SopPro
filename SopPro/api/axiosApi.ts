import axios from "axios";
import store from '../store/index';

// TODO: Set up a environment variable for baseURL
const api = axios.create({
  baseURL: "http://192.168.1.46:5000/api",
  headers: {
    "Content-Type": "application/json",
  },
});

api.interceptors.request.use(
  (config) => {
  const token = store.getState().auth.token;

  if(token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
},
(error) => {
  return Promise.reject(error);
});
 // interceptor for loggout out response data to the console for inspection
api.interceptors.response.use(
  (response) => {
    console.log("Response Data:", JSON.stringify(response.data, null, 2));
    return response;
  },
  (error) => {
    if (error.response) {
      // Server responded with a status other than 2xx
      console.error("Response Error Data:", JSON.stringify(error.response.data, null, 2));
      console.error("Response Error Status:", error.response.status);
      console.error("Response Error Headers:", JSON.stringify(error.response.headers, null, 2));
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

// interceptor for logging out request data to the console for inspection
api.interceptors.request.use(
  (config) => {
    // Log the full request details
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


export default api;