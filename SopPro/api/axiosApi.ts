import axios from "axios";

// TODO: Set up a environment variable for baseURL
const api = axios.create({
  baseURL: "http://192.168.1.46:5000/api",
  headers: {
    "Content-Type": "application/json",
  },
});

api.interceptors.response.use(
  (response) => {
    console.log("Response Data:", response.data);
    return response;
  },
  (error) => {
    if (error.response) {
      // Server responded with a status other than 2xx
      console.error("Response Error Data:", error.response.data);
      console.error("Response Error Status:", error.response.status);
      console.error("Response Error Headers:", error.response.headers);
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