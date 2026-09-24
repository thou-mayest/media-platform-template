import { getToken } from '@/lib/auth';
import { apiFetch } from './client';
import { createApiError } from './errors';
import { showErrorToast } from '@/lib/toast';

const BASE_URL = import.meta.env.PUBLIC_API_BASE_URL ?? 'http://localhost:5000';

export interface UploadResponse {
  id: string;
}

export interface FileDto {
  id: string;
  fileName: string;
  originalFileName: string;
  contentType: string;
  fileSize: number;
  storageProvider: string;
  bucketName: string;
  storageKey: string;
  url: string;
  createdAt: string;
}

function authHeaders(): Record<string, string> {
  const token = getToken();
  return token ? { Authorization: `Bearer ${token}` } : {};
}

export const filesApi = {

  upload(file: File, onProgress?: (percent: number) => void): Promise<UploadResponse> {
    return new Promise((resolve, reject) => {
      const formData = new FormData();
      formData.append('file', file);

      const xhr = new XMLHttpRequest();
      xhr.open('POST', `${BASE_URL}/api/files`);
      const authorization = authHeaders().Authorization;
      if (authorization) xhr.setRequestHeader('Authorization', authorization);

      xhr.upload.addEventListener('progress', (e) => {
        if (e.lengthComputable && onProgress) {
          onProgress(Math.round((e.loaded / e.total) * 100));
        }
      });

      xhr.addEventListener('load', () => {
        if (xhr.status >= 200 && xhr.status < 300) {
          try {
            resolve(JSON.parse(xhr.responseText) as UploadResponse);
          } catch {
            const error = createApiError(xhr.status);
            showErrorToast(error);
            reject(error);
          }
        } else {
          const error = createApiError(xhr.status);
          showErrorToast(error);
          reject(error);
        }
      });
      xhr.addEventListener('error', () => {
        const error = createApiError(0);
        showErrorToast(error);
        reject(error);
      });
      xhr.addEventListener('abort', () => reject(new DOMException('Upload aborted', 'AbortError')));

      xhr.send(formData);
    });
  },

  /** List all uploaded files. Requires an authenticated admin. */
  list: () => apiFetch<FileDto[]>('/api/files', { headers: authHeaders() }),

 /**
 * Sends an array of strings to your backend.
 * Replace the URL and adjust the request format as needed.
 */
  sendStringList: (urls: string[]) =>
    apiFetch<unknown>('/api/files/import', {
      method: 'POST',
      headers: {
        ...authHeaders(),
      },
      body: { urls },
    }),

  /** Fetch metadata (including the public url) for an uploaded file. */
  getById: (id: string) => apiFetch<FileDto>(`/api/files/${id}`, { headers: authHeaders() }),
};
