// Public surface of the Comments module. Pages import '@/data/comments'
// only, so the mock layer can be swapped for the real API without touching
// templates — the same rule the media and reactions modules follow.

import { fixtureCommentsFor } from './fixtures';
import { VIEWER_NAME, type Comment, type NewComment, type StoredComment } from './types';

export type { Comment, NewComment } from './types';
export { VIEWER_NAME } from './types';

// Anonymous viewer state, keyed `verso:comments` — same mechanism as
// reactions/follow/save. With a real backend, viewer comments come from the
// API (the write path posts to the Comments module instead of storage).
const STORAGE_KEY = 'verso:comments';

function readStore(): Record<string, StoredComment[]> {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) return {};
    const parsed = JSON.parse(raw) as unknown;
    return parsed !== null && typeof parsed === 'object'
      ? (parsed as Record<string, StoredComment[]>)
      : {};
  } catch {
    return {};
  }
}

function writeStore(store: Record<string, StoredComment[]>): void {
  try {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(store));
  } catch {
    /* storage unavailable — the viewer's comments just don't persist */
  }
}

function toComment(postId: number, s: StoredComment): Comment {
  return {
    id: s.id,
    postId,
    parentId: s.parentId,
    authorName: VIEWER_NAME,
    content: s.content,
    createdAt: s.createdAt,
    updatedAt: s.updatedAt ?? s.createdAt,
  };
}

/**
 * Full thread for a post: seeded comments plus the viewer's own, appended at
 * the end. SSR renders the seeded part; the client re-reads after mount to
 * paint any comments this viewer posted earlier (see CommentSection.astro).
 */
export function commentsFor(postId: number): Comment[] {
  const stored = readStore()[String(postId)] ?? [];
  return [...fixtureCommentsFor(postId), ...stored.map((s) => toComment(postId, s))];
}

/** Total comment count — the number the ReactionBar's pill shows. */
export function commentCountFor(postId: number): number {
  const stored = readStore()[String(postId)]?.length ?? 0;
  return fixtureCommentsFor(postId).length + stored;
}

/** Only the viewer's own stored comments — SSR never renders these; the
 *  CommentSection script hydrates them on mount. */
export function viewerCommentsFor(postId: number): Comment[] {
  const stored = readStore()[String(postId)] ?? [];
  return stored.map((s) => toComment(postId, s));
}

/**
 * Create a comment. Mirrors the backend's create-comment command; returns the
 * persisted row so the caller can render it optimistically. Ids are negative
 * and decrementing so they can never collide with seeded (positive) ids.
 */
export function addComment(input: NewComment): Comment {
  const store = readStore();
  const key = String(input.postId);
  const list = store[key] ?? [];
  const id = list.length > 0 ? Math.min(...list.map((c) => c.id)) - 1 : -1;
  const now = new Date().toISOString();
  const stored: StoredComment = {
    id,
    parentId: input.parentId,
    content: input.content,
    createdAt: now,
  };
  writeStore({ ...store, [key]: [...list, stored] });
  return toComment(input.postId, stored);
}

/**
 * Delete a viewer comment. Only works for stored (viewer) comments — seeded
 * comments are immutable. Returns true if the comment was found and removed.
 */
export function deleteComment(id: number): boolean {
  const store = readStore();
  for (const [key, list] of Object.entries(store)) {
    const idx = list.findIndex((c) => c.id === id);
    if (idx < 0) continue;
    list.splice(idx, 1);
    if (list.length === 0) delete store[key];
    else store[key] = list;
    writeStore(store);
    return true;
  }
  return false;
}
