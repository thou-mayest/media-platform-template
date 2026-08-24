import type { ReactionCounts } from './types';

// Mock read-model data for the Reactions module.
//
// Counts are derived deterministically from the post id so SSR output is
// stable across rebuilds and HTTP caching keeps working. When the backend
// module exists, this file is deleted and the read models come from the API
// (src/api/reactions.ts) instead.

/** Seeded fraction in [0, 1) — stable per (postId, salt). */
function fraction(postId: number, salt: number): number {
  const x = Math.sin(postId * salt + 31.7) * 43758.5453;
  return x - Math.floor(x);
}

export function mockCounts(postId: number): ReactionCounts {
  return {
    like: Math.round(14 + fraction(postId, 127.1) * 260),
    dislike: Math.round(1 + fraction(postId, 311.7) * 24),
  };
}
