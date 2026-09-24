import type { ApiError } from '../api/errors.ts';

export const TOAST_EVENT = 'verso:toast';

export interface ToastDetail {
  message: string;
  severity: 'error';
}

export function showErrorToast(error: ApiError): void {
  if (typeof window === 'undefined') return;

  window.dispatchEvent(
    new CustomEvent<ToastDetail>(TOAST_EVENT, {
      detail: { message: error.message, severity: 'error' },
    }),
  );
}
