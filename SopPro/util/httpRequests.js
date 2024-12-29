import api from "../api/axiosApi";

export async function registerCompany(data) {
  try {
    const response = await api.post("/auth/signuporganisation", data);
  } catch (e) {
    const error = new Error(
      e.response?.data?.errorMessage || "Oops, something went wrong!"
    );
    throw error;
  }
}

export async function login(data) {
  try {
    const response = await api.post("/auth/login", data);
    return response.data;
  } catch (e) {
    const error = new Error(
      e.response?.data?.errorMessage || "Error logging in"
    );
    throw error;
  }
}

export async function fetchSop(id) {
  try {
    const response = await api.get(`/sop/${id}`);
    return response.data.result;
  } catch (e) {
    const error = new Error(
      e.response?.data?.errorMessage || "Error fetching SOP"
    );
    throw error;
  }
}
