import type { Comment } from './types';

// Mock read-model data for the Comments module.
//
// Comments are derived deterministically from the post id so SSR output is
// stable across rebuilds and HTTP caching keeps working — the same rule the
// reactions fixtures follow. Dates hang off a fixed base date rather than
// `Date.now()` for the same reason. When the backend module exists, this
// file is deleted and the read models come from the API instead.

/** Seeded fraction in [0, 1) — stable per (postId, salt). */
function fraction(postId: number, salt: number): number {
  const x = Math.sin(postId * salt + 31.7) * 43758.5453;
  return x - Math.floor(x);
}

/** Deterministic pick from a pool. */
function pick<T>(pool: readonly T[], postId: number, salt: number): T {
  const i = Math.floor(fraction(postId, salt) * pool.length);
  return pool[i];
}

/** Days-ago in [min, max), deterministic per (postId, salt). */
function daysAgo(postId: number, salt: number, min: number, max: number): number {
  return min + Math.floor(fraction(postId, salt) * (max - min));
}

/** Deterministic ISO timestamp: base date minus `days`, plus a seeded hour
 *  offset so same-day comments still sort stably. */
function isoFor(postId: number, days: number, salt: number): string {
  const BASE = Date.parse('2026-08-12T10:00:00.000Z');
  const hourMs = Math.floor(fraction(postId, salt) * 10) * 3_600_000;
  return new Date(BASE - days * 86_400_000 + hourMs).toISOString();
}

const AUTHORS = [
  'Maya Lindqvist',
  'Olu Fadipe',
  'Elise Moreau',
  'Jonas Weber',
  'Priya Raman',
  'Dani Okonjo',
  'Camille Roy',
  'Tadeo Silva',
  'Ines Berg',
  'Ravi Chandra',
] as const;

const TOP_COMMENTS = [
  'The light in this one — I keep coming back to it. Nothing else on the page is fighting the frame.',
  'Print quality must be something else. The grain reads like film, not noise.',
  'This is the frame I wish I had made on my last trip. Saving it.',
  'There is a stillness here that the thumbnail does not prepare you for.',
  'The composition is deceptively simple. The more you look, the more it holds.',
  'Gorgeous tones. Which stock was this shot on?',
  'The negative space does all the work. Brave crop.',
  'I would hang this in my hallway. Seriously.',
  'Every element in the frame earns its place — that is rarer than people think.',
  'The patience this must have taken. It shows.',
  'Something about the horizon line here feels off in the best way.',
  'This belongs in a book. Which volume though?',
  'The way the colour drops out at the edges — was that in camera?',
  'I have been staring at this for five minutes and I am not done yet.',
] as const;

const REPLIES = [
  'Could not agree more. The restraint is the whole point.',
  'Agreed — the thumbnail genuinely does not do it justice.',
  'Yes! Exactly what I was thinking.',
  'I was about to say the same thing.',
  'Right? The shadows alone are worth the look.',
  'Came here to say this.',
  'You have a better eye than me then, I missed that entirely.',
  'That is a good question, I would love to know too.',
  'Seconded. And the caption helps a lot as well.',
  'Beautifully put.',
] as const;

/**
 * Deterministic seeded thread for a post: 2–4 top-level comments, each with
 * 0–2 replies. Ids are namespaced under `postId * 1000` so the viewer's own
 * comments (negative ids) can never collide.
 */
export function fixtureCommentsFor(postId: number): Comment[] {
  const comments: Comment[] = [];

  const topCount = 2 + Math.floor(fraction(postId, 41.3) * 3); // 2..4
  for (let i = 0; i < topCount; i++) {
    const id = postId * 1000 + i + 1;
    const parentDays = daysAgo(postId, i * 3.3 + 1.7, 1, 40);
    comments.push({
      id,
      postId,
      parentId: null,
      authorName: pick(AUTHORS, postId, i * 7.1 + 3),
      content: pick(TOP_COMMENTS, postId, i * 13.7 + 5),
      createdAt: isoFor(postId, parentDays, i * 3.3 + 1.7),
      updatedAt: isoFor(postId, parentDays, i * 3.3 + 1.9),
    });

    const replyCount = Math.floor(fraction(postId, i * 5.9 + 8) * 3); // 0..2
    for (let j = 0; j < replyCount; j++) {
      // Replies never predate their parent: cap at parentDays - 1 (same day
      // when the parent is one day old).
      const maxDays = Math.max(parentDays - 1, 0);
      comments.push({
        id: postId * 1000 + 100 + i * 10 + j + 1,
        postId,
        parentId: id,
        authorName: pick(AUTHORS, postId, i * 9.7 + j * 5.1 + 2),
        content: pick(REPLIES, postId, i * 11.3 + j * 3.7 + 6),
        createdAt: isoFor(postId, daysAgo(postId, i * 6.7 + j * 2.3 + 4.1, 0, maxDays + 1), i * 6.7 + j * 2.3 + 4.2),
        updatedAt: isoFor(postId, daysAgo(postId, i * 6.7 + j * 2.3 + 4.3, 0, maxDays + 1), i * 6.7 + j * 2.3 + 4.4),
      });
    }
  }

  return comments;
}
