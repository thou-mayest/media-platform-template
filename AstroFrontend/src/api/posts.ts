import { getToken } from '@/lib/auth';
import { apiFetch } from './client';

/** Body of POST /api/posts (mirrors backend CreatePostRequest). */
export interface CreatePostRequest {
  mediaAssetId: string;
  title: string;
  description: string;
  categoryId: string | null;
  mediaUrl: string;
  altText: string;
  /** Free-form tags; the backend strips leading '#' and matches existing tags case-insensitively. */
  tags: string[];
}

function authHeaders(): Record<string, string> {
  const token = getToken();
  return token ? { Authorization: `Bearer ${token}` } : {};
}

export const postsApi = {
  /** Create a post (POST /api/posts). Requires an authenticated user. Resolves to the new post's id. */
  create: (request: CreatePostRequest) =>
    apiFetch<string>('/api/posts', {
      method: 'POST',
      headers: authHeaders(),
      body: request,
      errorMessages: {
        unauthorized: 'You must be logged in to create a post.',
        unexpected: 'The server rejected the post. Please check the fields and try again.',
      },
    }),
};
