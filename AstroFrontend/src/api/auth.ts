import { apiFetch } from './client';

export type LoginRequest = {
  email: string;
  password: string;
};

export type RegisterRequest = {
  name: string;
  email: string;
  password: string;
};

export type AuthResponse = {
  token: string;
  userId: string;
  name: string;
  email: string;
};

export const authApi = {
  login: (body: LoginRequest) =>
    apiFetch<AuthResponse>('/api/users/login', { method: 'POST', body }),

  register: (body: RegisterRequest) =>
    apiFetch<AuthResponse>('/api/users/signup', { method: 'POST', body }),
};
