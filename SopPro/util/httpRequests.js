import { api } from '../api/axiosApi';

export async function registerCompany(data) {
    try {
        const response = await api.post('/auth/signuporganisation', data);
        console.log(response.data);

    } catch(e) {
        const error = new Error(e.response.data.errorMessage || "Oops something went wrong!");
        throw error;
    }
}