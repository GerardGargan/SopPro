import axios from "axios";

// TODO: Set up a environment variable for baseURL
export const api = axios.create({
  baseURL: "http://192.168.1.46:5000/api",
  headers: {
    "Content-Type": "application/json",
  },
});
