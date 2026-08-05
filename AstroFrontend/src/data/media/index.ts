// Public surface of the media mock dataset. Pages import '@/data/media' only,
// so the internal layout can change without touching templates.

import { rawActors, rawAlbums, rawPosts } from './fixtures';
import type { Actor, Album, Post, PagedResult } from './types';

export type { Actor, Album, Post, MediaType, PagedResult, SocialLink, SocialPlatform } from './types';
// Duplicated from data/gallery.ts — slugify is not exported there, and
// exporting it would mean editing a shared file.
function slugify(s: string): string {
  return s
    .toLowerCase()
    .normalize('NFKD')
    .replace(/[\u0300-\u036f]/g, '')
    .replace(/[^a-z0-9]+/g, '-')
    .replace(/(^-|-$)/g, '');
}

export const actors: Actor[] = rawActors.map((a, i) => {
  const owned = rawAlbums.filter((r) => r[0] === i);
  const slug = slugify(a[0]);
  return {
    id: i,
    slug,
    displayName: a[0],
    profession: a[1],
    bio: a[2],
    avatarKey: '',
    socialLinks: [
      { platform: 'instagram', url: `https://instagram.com/${slug}` },
      { platform: 'website', url: `https://${slug}.example.com` },
    ],
    albumCount: owned.length,
    mediaCount: owned.reduce((n, r) => n + r[5] + r[6], 0),
    followerCount: a[3],
    updatedAt: a[4],
  };
});

export const albums: Album[] = rawAlbums.map((r, i) => {
  const actor = actors[r[0]]!;
  return {
    id: i,
    actorId: r[0],
    actorSlug: actor.slug,
    actorName: actor.displayName,
    slug: `${slugify(r[1])}-${i}`,
    title: r[1],
    description: r[2],
    coverKey: '',
    coverAlt: r[3],
    coverAspectRatio: r[4],
    photoCount: r[5],
    videoCount: r[6],
    isSeries: r[7],
    tags: r[8].split(','),
    publishedAt: r[9],
    updatedAt: r[10],
  };
});

export function getActorBySlug(slug: string): Actor | undefined {
  return actors.find((a) => a.slug === slug);
}

/** Newest first — what a real endpoint returns, so page 1 stays correct. */
export function albumsByActor(actorId: number): Album[] {
  return albums
    .filter((al) => al.actorId === actorId)
    .sort((a, b) => b.publishedAt.localeCompare(a.publishedAt));
}

export function getAlbumBySlug(actorSlug: string, albumSlug: string): Album | undefined {
  return albums.find((al) => al.actorSlug === actorSlug && al.slug === albumSlug);
}

export function suggestedActors(excludeId: number, limit = 6): Actor[] {
  return actors.filter((a) => a.id !== excludeId).slice(0, limit);
}

/** Most recent updatedAt across a set — the Last-Modified source for a page. */
export function lastModified(items: { updatedAt: string }[]): string | undefined {
  return items.map((i) => i.updatedAt).sort().at(-1);
}

/** Simulates server-side paging. Clamps out-of-range pages rather than
 *  404-ing, keeping guessed ?page=N crawlable. */
export function paginate<T>(items: T[], page: number, pageSize: number): PagedResult<T> {
  const totalItems = items.length;
  const totalPages = Math.max(1, Math.ceil(totalItems / pageSize));
  const current = Math.min(Math.max(1, Math.trunc(page) || 1), totalPages);
  const start = (current - 1) * pageSize;
  return {
    items: items.slice(start, start + pageSize),
    page: current,
    pageSize,
    totalItems,
    totalPages,
    hasPrev: current > 1,
    hasNext: current < totalPages,
  };
}

export const posts: Post[] = (() => {
  const perAlbum = new Map<number, number>();
  return rawPosts.map((r, i) => {
    const order = perAlbum.get(r[0]) ?? 0;
    perAlbum.set(r[0], order + 1);
    const album = albums[r[0]]!;
    return {
      id: i,
      albumId: r[0],
      actorId: album.actorId,
      storageKey: '',
      mediaType: r[1],
      aspectRatio: r[2],
      durationSeconds: r[3],
      altText: r[4],
      caption: r[5],
      displayOrder: order,
      publishedAt: r[6],
      // Mock treats posts as immutable after publish. The API returns these
      // independently.
      updatedAt: r[6],
    };
  });
})();

export function postsByAlbum(albumId: number): Post[] {
  return posts
    .filter((p) => p.albumId === albumId)
    .sort((a, b) => a.displayOrder - b.displayOrder);
}