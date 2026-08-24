// Public surface of the Favorites module. Pages import '@/data/favorites'
// only, so the mock layer can be swapped for the real API without touching
// templates — the same rule the other modules follow.

import type { Favorite } from './types';

export type { Favorite } from './types';

const STORAGE_KEY = 'verso:favorites';

function readStore(): Favorite[] {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) return [];
    const parsed = JSON.parse(raw) as unknown;
    return Array.isArray(parsed) ? (parsed as Favorite[]) : [];
  } catch {
    return [];
  }
}

function writeStore(favorites: Favorite[]): void {
  try {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(favorites));
  } catch {
    /* storage unavailable */
  }
}

/** Check if a post is bookmarked. */
export function isFavorite(postId: number): boolean {
  return readStore().some((f) => f.postId === postId);
}

/** Toggle a post's bookmark state. Returns true if now favorited. */
export function toggleFavorite(postId: number): boolean {
  const store = readStore();
  const idx = store.findIndex((f) => f.postId === postId);
  if (idx >= 0) {
    store.splice(idx, 1);
    writeStore(store);
    return false;
  }
  store.push({ postId, savedAt: new Date().toISOString() });
  writeStore(store);
  return true;
}

/** Total number of bookmarked posts. */
export function favoritesCount(): number {
  return readStore().length;
}
