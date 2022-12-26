import axios from "axios";

export const login = payload => axios.post('/api/login', payload);

export const logout = () => axios.get('/api/logout');

export const validateCredentials = () => axios.get('/api/validate');