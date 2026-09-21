import assert from 'node:assert/strict';
import test from 'node:test';

import {
  ApiError,
  createApiError,
  DEFAULT_API_ERROR_MESSAGES,
  getApiErrorKind,
  isAbortError,
} from '../../src/api/errors.ts';

test('maps supported HTTP and network failures to safe user-facing messages', () => {
  const cases = [
    [0, 'network'],
    [401, 'unauthorized'],
    [403, 'forbidden'],
    [404, 'not-found'],
    [500, 'server'],
    [503, 'server'],
    [400, 'unexpected'],
  ];

  for (const [status, kind] of cases) {
    assert.equal(getApiErrorKind(status), kind);
    assert.equal(createApiError(status).message, DEFAULT_API_ERROR_MESSAGES[kind]);
  }
});

test('creates errors with safe messages instead of backend details', () => {
  const error = createApiError(500);

  assert.ok(error instanceof ApiError);
  assert.equal(error.status, 500);
  assert.equal(error.kind, 'server');
  assert.equal(error.message, DEFAULT_API_ERROR_MESSAGES.server);
  assert.equal(error.message.includes('/api/'), false);
});

test('supports a contextual message without changing the error category', () => {
  const error = createApiError(401, {
    unauthorized: 'The email or password is incorrect.',
  });

  assert.equal(error.kind, 'unauthorized');
  assert.equal(error.message, 'The email or password is incorrect.');
});

test('recognizes cancellations so they can remain silent', () => {
  assert.equal(isAbortError(new DOMException('Cancelled', 'AbortError')), true);
  assert.equal(isAbortError(new Error('Network failure')), false);
});
