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

export async function updateSop(sop) {
  try {
    const response = await api.put(`/sop/${sop.id}`, sop);
    return response.data;
  } catch (e) {
    const error = new Error(
      e.response?.data?.errorMessage || "Error updating SOP"
    );
    throw error;
  }
}

export async function createSop(sop) {
  try {
    const response = await api.post(`/sop`, sop);
    return response.data;
  } catch (e) {
    const error = new Error(
      e.response?.data?.errorMessage || "Error creating SOP"
    );
    throw error;
  }
}

export async function fetchSops({ searchQuery, status }) {
  try {
    const params = {};
    if (searchQuery) params.search = searchQuery;
    if (status !== null) params.status = status;

    const response = await api.get("/sop", {
      params,
    });
    return response.data.result;
  } catch (e) {
    const error = new Error(
      e.response?.data?.errorMessage || "Error fetching SOPs"
    );
    throw error;
  }
}

export async function uploadImage(formData) {
  try {
    console.log("sending");

    const response = await api.post("/sop/upload", formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
    return response.data;
  } catch (e) {
    const error = new Error(
      e.response?.data?.errorMessage || "Error uploading image"
    );
    throw error;
  }
}

export async function fetchDepartments() {
  try {
    const response = await api.get("/department");
    return response.data.result;
  } catch (e) {
    const error = new Error(
      e.response?.data?.errorMessage || "Error fetching departments"
    );
    throw error;
  }
}

export async function deleteSops(ids) {
  try {
    if (!ids || ids.length === 0) throw Error("No ids provided");
    const response = await api.delete("/sop/delete", ids);
    return response.data;
  } catch (e) {
    const error = new Error(
      e.response?.data?.errorMessage || "Error deleting SOPs"
    );
    throw error;
  }
}
