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
    apiFetch<AuthResponse>('/api/auth/login', {
      method: 'POST',
      body,
      errorMessages: { unauthorized: 'The email or password is incorrect.' },
    }),

  register: (body: RegisterRequest) =>
    apiFetch<AuthResponse>('/api/auth/signup', {
      method: 'POST',
      body,
      errorMessages: { unexpected: 'Unable to create your account. Please check your details and try again.' },
    }),
};
