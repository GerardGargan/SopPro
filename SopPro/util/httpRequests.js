import api from "../api/axiosApi";

// This file contains exported functions for performing Http requests to API endpoints and handles formatting of errors
// These functions are plugged in to the Tanstack Query and Mutation hooks in the app to manage data fetching and caching

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

export async function fetchSops({
  searchQuery,
  status,
  page = 1,
  pageSize = 20,
  isFavourite = false,
}) {
  try {
    const params = {};
    if (searchQuery) params.search = searchQuery;
    if (status !== null) params.status = status;
    params.page = page;
    params.pageSize = pageSize;
    params.isFavourite = isFavourite;

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

export async function fetchDepartment(id) {
  try {
    const response = await api.get(`/department/${id}`);
    return response.data.result;
  } catch (e) {
    const error = new Error(
      e.response?.data?.errorMessage || "Error fetching departments"
    );
    throw error;
  }
}

export async function createDepartment({ name }) {
  const data = { name };
  try {
    const response = await api.post(`/department`, data);
    return response.data;
  } catch (e) {
    const error = new Error(
      e.response?.data?.errorMessage || "Error creating department"
    );
    throw error;
  }
}

export async function updateDepartment({ name, id }) {
  const data = { name, id };
  try {
    const response = await api.put(`/department/${id}`, data);
    return response.data;
  } catch (e) {
    const error = new Error(
      e.response?.data?.errorMessage || "Error updating department"
    );
    throw error;
  }
}

export async function deleteDepartment({ id }) {
  try {
    const response = await api.delete(`/department/${id}`);
    return response.data;
  } catch (e) {
    const error = new Error(
      e.response?.data?.errorMessage || "Error deleting department"
    );
    throw error;
  }
}

export async function deleteSops(ids) {
  try {
    if (!ids || ids.length === 0) throw Error("No ids provided");

    const response = await api.delete("/sop/delete", {
      data: ids,
    });

    return response.data;
  } catch (e) {
    const error = new Error(
      e.response?.data?.errorMessage || "Error deleting SOPs"
    );
    throw error;
  }
}

export async function revertSopVersion({ versionId }) {
  const data = { versionId };
  try {
    const response = await api.post("/sop/revert", data);
    return response.data;
  } catch (e) {
    const error = new Error(
      e.response?.data?.errorMessage || "Error reverting version"
    );
    throw error;
  }
}

export async function fetchPpe() {
  try {
    const response = await api.get("/ppe");
    return response.data.result;
  } catch (e) {
    const error = new Error(
      e.response?.data?.errorMessage || "Error fetching PPE"
    );
    throw error;
  }
}

export async function addSopToFavourites(id) {
  try {
    const response = await api.get(`/sop/${id}/favourite`);
    return response.data;
  } catch (e) {
    const error = new Error(
      e.response?.data?.errorMessage || "Error adding SOP to favourites"
    );
    throw error;
  }
}

export async function removeSopFromFavourites(id) {
  try {
    const response = await api.delete(`/sop/${id}/favourite`);
    return response.data;
  } catch (e) {
    const error = new Error(
      e.response?.data?.errorMessage || "Error removing SOP from favourites"
    );
    throw error;
  }
}

export async function approveSop(id) {
  try {
    const response = await api.get(`/sop/${id}/approve`);
    return response.data;
  } catch (e) {
    const error = new Error(
      e.response?.data?.errorMessage || "Error approving sop"
    );
    throw error;
  }
}

export async function rejectSop(id) {
  try {
    const response = await api.get(`/sop/${id}/reject`);
    return response.data;
  } catch (e) {
    const error = new Error(
      e.response?.data?.errorMessage || "Error rejecting sop"
    );
    throw error;
  }
}

export async function requestApproval(id) {
  try {
    const response = await api.get(`/sop/${id}/requestapproval`);
    return response.data;
  } catch (e) {
    const error = new Error(
      e.response?.data?.errorMessage || "Error requesting approval"
    );
    throw error;
  }
}

export async function changePasswordRequest({
  oldPassword,
  newPassword,
  confirmNewPassword,
}) {
  const data = { oldPassword, newPassword, confirmNewPassword };
  try {
    const response = await api.post(`/auth/password`, data);
    return response.data;
  } catch (e) {
    const error = new Error(
      e.response?.data?.errorMessage || "Error changing password"
    );
    throw error;
  }
}

export async function forgotPassword(email) {
  try {
    const response = await api.post("/auth/forgot", {
      email,
    });

    return response.data;
  } catch (e) {
    const error = new Error(
      e.response?.data?.errorMessage || "Error requesting reset"
    );
    throw error;
  }
}

export async function resetPassword({ email, formattedToken, password }) {
  const data = { email, resetCode: formattedToken, newPassword: password };

  try {
    const response = await api.post("/auth/reset", data);
    return response.data;
  } catch (e) {
    const error = new Error(
      e.response?.data?.errorMessage || "Error reseting password"
    );
    throw error;
  }
}

export async function inviteUser({ email, role }) {
  const data = { email, role };
  try {
    const response = await api.post("/auth/inviteuser", data);
    return response.data;
  } catch (e) {
    const error = new Error(
      e.response?.data?.errorMessage || "Error sending invite"
    );
    throw error;
  }
}

export async function fetchAllUsers() {
  try {
    const response = await api.get("/auth");
    return response.data.result;
  } catch (e) {
    const error = new Error(
      e.response?.data?.errorMessage || "Error fetching users"
    );
    throw error;
  }
}

export async function fetchUser({ id }) {
  try {
    const response = await api.get(`/auth/${id}`);
    return response.data.result;
  } catch (e) {
    const error = new Error(
      e.response?.data?.errorMessage || "Error fetching users"
    );
    throw error;
  }
}

export async function updateUser(id, { forename, surname, roleName }) {
  const data = { id, forename, surname, roleName };
  try {
    const response = await api.put(`/auth/${id}`, data);
    return response.data;
  } catch (e) {
    const error = new Error(
      e.response?.data?.errorMessage || "Error updating user"
    );
    throw error;
  }
}

export async function deleteUser({ id }) {
  try {
    const response = await api.delete(`/auth/${id}`);
    return response.data;
  } catch (e) {
    const error = new Error(
      e.response?.data?.errorMessage || "Error updating user"
    );
    throw error;
  }
}

export async function completeRegistration({
  forename,
  surname,
  password,
  token,
}) {
  const data = { forename, surname, password, token };
  try {
    const response = await api.post("/auth/registerinvite", data);
    return response.data;
  } catch (e) {
    const error = new Error(
      e.response?.data?.errorMessage || "Error sending invite"
    );
    throw error;
  }
}

export async function generateAiSop({ jobDescription, keyRisks, primaryGoal }) {
  const data = { jobDescription, keyRisks, primaryGoal };
  try {
    const response = await api.post("/sop/aigenerator", data);
    return response.data;
  } catch (e) {
    const error = new Error(
      e.response?.data?.errorMessage || "Error sending invite"
    );
    throw error;
  }
}

export async function getAnalytics() {
  try {
    const response = await api.get("/sop/analytics");
    return response.data;
  } catch (e) {
    const error = new Error(
      e.response?.data?.errorMessage || "Error loading analytics"
    );
    throw error;
  }
}

export async function getSettingByKey(key) {
  try {
    const response = await api.get(`/setting/${key}`);
    return response.data;
  } catch (e) {
    if (e.response?.status === 404) {
      return null;
    }

    const error = new Error(
      e.response?.data?.errorMessage || "Error fetching setting"
    );
    throw error;
  }
}

export async function createSetting({ type, key, value }) {
  const data = { type, key, value };
  try {
    const response = await api.post(`/setting`, data);
  } catch (e) {
    const error = new Error(
      e.response?.data?.errorMessage || "Error creating setting"
    );
    throw error;
  }
}

export async function updateSetting({ type, key, value }) {
  const data = { type, key, value };
  try {
    const response = await api.put(`/setting/${key}`, data);
  } catch (e) {
    const error = new Error(
      e.response?.data?.errorMessage || "Error updating setting"
    );
    throw error;
  }
}
