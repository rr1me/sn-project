import axios from "axios";

export const authenticate = (username, password) => {
    return axios.post('/api/login', {
        username: username,
        password: password
    });
}