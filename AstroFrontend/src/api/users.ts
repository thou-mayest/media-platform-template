import { getToken } from '@/lib/auth';
import { apiFetch } from './client';

export type UserRole = 'Admin' | 'User' | 'PremiumUser';

export interface UserDto {
  id: string;
  name: string;
  email: string;
  role: UserRole;
  createdDate: string;
  updateDate: string | null;
}

function authHeaders(): Record<string, string> {
  const token = getToken();
  return token ? { Authorization: `Bearer ${token}` } : {};
}

export const usersApi = {
  /** List all users (GET /api/users). Requires an authenticated admin. */
  list: () => apiFetch<UserDto[]>('/api/users', { headers: authHeaders() }),

  /** Delete a user (DELETE /api/users/{id}). Requires an authenticated admin. */
  delete: (id: string) =>
    apiFetch<void>(`/api/users/${id}`, {
      method: 'DELETE',
      headers: authHeaders(),
    }),
};
