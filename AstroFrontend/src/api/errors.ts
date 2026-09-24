export type ApiErrorKind =
  | 'unauthorized'
  | 'forbidden'
  | 'not-found'
  | 'server'
  | 'network'
  | 'unexpected';

export type ApiErrorMessages = Partial<Record<ApiErrorKind, string>>;

export const DEFAULT_API_ERROR_MESSAGES: Record<ApiErrorKind, string> = {
  unauthorized: 'Your session has expired. Please log in again.',
  forbidden: "You don't have permission to perform this action.",
  'not-found': 'The requested resource could not be found.',
  server: 'Something went wrong on the server. Please try again later.',
  network: 'Unable to connect to the server. Please check your connection and try again.',
  unexpected: 'Something went wrong. Please try again.',
};

export class ApiError extends Error {
  readonly status: number;
  readonly kind: ApiErrorKind;

  constructor(
    status: number,
    kind: ApiErrorKind,
    message: string,
  ) {
    super(message);
    this.name = 'ApiError';
    this.status = status;
    this.kind = kind;
  }
}

export function getApiErrorKind(status: number): ApiErrorKind {
  if (status === 0) return 'network';
  if (status === 401) return 'unauthorized';
  if (status === 403) return 'forbidden';
  if (status === 404) return 'not-found';
  if (status >= 500) return 'server';
  return 'unexpected';
}

export function createApiError(status: number, messages: ApiErrorMessages = {}): ApiError {
  const kind = getApiErrorKind(status);
  return new ApiError(status, kind, messages[kind] ?? DEFAULT_API_ERROR_MESSAGES[kind]);
}

export function isAbortError(error: unknown): boolean {
  return typeof DOMException !== 'undefined' && error instanceof DOMException && error.name === 'AbortError';
}
