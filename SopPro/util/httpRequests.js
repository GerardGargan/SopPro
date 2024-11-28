import { api } from '../api/axiosApi';

export async function registerCompany(data) {
    try {
        const response = await api.post('/auth/signuporganisation', data);
    } catch(e) {
        const error = new Error(e.response.data.errorMessage || "Oops something went wrong!");
        throw error;
    }
}

export async function login(data) {
    try {
        const response = await api.post('/auth/login', data);
        console.log("response", response.data);
        return response.data;
    } catch(e) {
        const error = new Error(e.response.data.errorMessage || "Error logging in");
        throw error;
    }
}