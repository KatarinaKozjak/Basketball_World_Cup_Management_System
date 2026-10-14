import axios from 'axios';

export const api = axios.create({ baseURL: process.env.REACT_APP_API_URL ?? 'http://localhost:5260/api' });
export const message = (e: any) => e?.response?.data?.message ?? e?.message ?? 'Došlo je do greške.';