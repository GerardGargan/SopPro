import { api } from '../api/axiosApi';

export async function registerCompany(data) {
    try {
        const response = await api.post('/auth/signuporganisation', data);
        console.log(response.data);

    } catch(e) {
        const error = new Error("Error, could not register user!");
        error.code = response.status;
        throw error;
    }
}