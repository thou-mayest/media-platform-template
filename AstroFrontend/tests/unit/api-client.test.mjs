import assert from 'node:assert/strict';
import test from 'node:test';

import { apiFetch } from '../../src/api/client.ts';
import { ApiError, DEFAULT_API_ERROR_MESSAGES } from '../../src/api/errors.ts';
import { TOAST_EVENT } from '../../src/lib/toast.ts';

test('reports HTTP failures with the corresponding safe toast', async (t) => {
  const originalFetch = globalThis.fetch;
  const originalWindow = globalThis.window;

  globalThis.window = new EventTarget();
  t.after(() => {
    globalThis.fetch = originalFetch;
    if (originalWindow === undefined) delete globalThis.window;
    else globalThis.window = originalWindow;
  });

  const cases = [
    [401, 'unauthorized'],
    [403, 'forbidden'],
    [404, 'not-found'],
    [500, 'server'],
    [400, 'unexpected'],
  ];

  for (const [status, kind] of cases) {
    const messages = [];
    const onToast = (event) => messages.push(event.detail.message);
    window.addEventListener(TOAST_EVENT, onToast);
    globalThis.fetch = async () => new Response('Sensitive backend details', { status });

    await assert.rejects(apiFetch('/api/test'), (error) => {
      assert.ok(error instanceof ApiError);
      assert.equal(error.status, status);
      assert.equal(error.kind, kind);
      assert.equal(error.message, DEFAULT_API_ERROR_MESSAGES[kind]);
      assert.equal(error.message.includes('Sensitive backend details'), false);
      return true;
    });

    assert.deepEqual(messages, [DEFAULT_API_ERROR_MESSAGES[kind]]);
    window.removeEventListener(TOAST_EVENT, onToast);
  }
});

test('reports connection failures with the network toast', async (t) => {
  const originalFetch = globalThis.fetch;
  const originalWindow = globalThis.window;

  globalThis.window = new EventTarget();
  globalThis.fetch = async () => {
    throw new TypeError('Internal connection details');
  };
  t.after(() => {
    globalThis.fetch = originalFetch;
    if (originalWindow === undefined) delete globalThis.window;
    else globalThis.window = originalWindow;
  });

  let message = '';
  window.addEventListener(TOAST_EVENT, (event) => {
    message = event.detail.message;
  });

  await assert.rejects(apiFetch('/api/test'), (error) => {
    assert.ok(error instanceof ApiError);
    assert.equal(error.kind, 'network');
    assert.equal(error.message, DEFAULT_API_ERROR_MESSAGES.network);
    return true;
  });

  assert.equal(message, DEFAULT_API_ERROR_MESSAGES.network);
});

test('keeps cancelled requests silent', async (t) => {
  const originalFetch = globalThis.fetch;
  const originalWindow = globalThis.window;

  globalThis.window = new EventTarget();
  globalThis.fetch = async () => {
    throw new DOMException('Cancelled', 'AbortError');
  };
  t.after(() => {
    globalThis.fetch = originalFetch;
    if (originalWindow === undefined) delete globalThis.window;
    else globalThis.window = originalWindow;
  });

  let toastCount = 0;
  window.addEventListener(TOAST_EVENT, () => {
    toastCount += 1;
  });

  await assert.rejects(apiFetch('/api/test'), { name: 'AbortError' });
  assert.equal(toastCount, 0);
});
