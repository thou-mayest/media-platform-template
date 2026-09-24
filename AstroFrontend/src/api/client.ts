import { createApiError, isAbortError, type ApiErrorMessages } from './errors.ts';
import { showErrorToast } from '../lib/toast.ts';

const BASE_URL = import.meta.env?.PUBLIC_API_BASE_URL ?? 'http://localhost:5000';

type RequestOptions = Omit<RequestInit, 'body'> & {
  body?: unknown;
  errorMessages?: ApiErrorMessages;
};

export async function apiFetch<T>(path: string, options: RequestOptions = {}): Promise<T> {
  const { body, headers, errorMessages, ...rest } = options;
  const requestHeaders = new Headers(headers);
  let res: Response;

  if (body !== undefined && !requestHeaders.has('Content-Type')) {
    requestHeaders.set('Content-Type', 'application/json');
  }

  try {
    res = await fetch(`${BASE_URL}${path}`, {
      ...rest,
      headers: requestHeaders,
      body: body !== undefined ? JSON.stringify(body) : undefined,
    });
  } catch (error) {
    if (isAbortError(error)) throw error;
    const apiError = createApiError(0, errorMessages);
    showErrorToast(apiError);
    throw apiError;
  }

  if (!res.ok) {
    const apiError = createApiError(res.status, errorMessages);
    showErrorToast(apiError);
    throw apiError;
  }

  if (res.status === 204) return undefined as T;

  try {
    return (await res.json()) as T;
  } catch {
    const apiError = createApiError(res.status, errorMessages);
    showErrorToast(apiError);
    throw apiError;
  }
}
