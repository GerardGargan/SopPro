import axios from "axios";

// TODO: Set up a environment variable for baseURL
export const api = axios.create({
  baseURL: "http://192.168.1.198:5160/api",
  headers: {
    "Content-Type": "application/json",
  },
});
