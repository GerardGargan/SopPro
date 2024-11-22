import axios from "axios";

export const api = axios.create({
  baseURL: "http://localhost:5160/api",
  headers: {
    "Content-Type": "application/json",
  },
});
