// Types for the Profiles and Albums mock data.
//
// Contains only fields the backend will return. Presentation concerns
// (render size, badges, gradients) are derived by the UI; URLs are built by
// src/lib/images.ts and src/lib/routes.ts. Swapping this module for real API
// responses should require no template changes.

export type SocialPlatform = 'instagram' | 'youtube' | 'x' | 'website';
export type SocialLink = { platform: SocialPlatform; url: string };

/** Read model over the ActorProfile aggregate. */
export type Actor = {
  id: number;
  slug: string;
  displayName: string;
  profession: string;
  bio: string;
  /** ActorProfile.AvatarStorageKey. Empty until real media exists. */
  avatarKey: string;
  socialLinks: SocialLink[];
  albumCount: number;
  mediaCount: number;
  followerCount: number;
  updatedAt: string;
};

/**
 * Read model over the Album aggregate.
 * actorSlug/actorName are denormalised into the list DTO so rendering a card
 * never needs a second lookup.
 */
export type Album = {
  id: number;
  actorId: number;
  actorSlug: string;
  actorName: string;
  slug: string;
  title: string;
  description: string;
  /** Storage key of Album.CoverPostId. Empty until real media exists. */
  coverKey: string;
  /** Authored per album. Describes the image, not the album. */
  coverAlt: string;
  /** Post.AspectRatio — width / height. The UI picks render width and
   *  derives height from this, so the box is reserved without CLS. */
  coverAspectRatio: number;
  photoCount: number;
  videoCount: number;
  isSeries: boolean;
  tags: string[];
  /** Drives the "new" badge (UI decides the window) and JSON-LD datePublished. */
  publishedAt: string;
  /** Sitemap lastmod, JSON-LD dateModified, Last-Modified header. */
  updatedAt: string;
};

/** Mirrors the paged envelope the API will return — not a client-side slice
 *  contract. Production must never fetch-all-then-slice. */
export type PagedResult<T> = {
  items: T[];
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
  hasPrev: boolean;
  hasNext: boolean;
};

export type RawActor = readonly [
  name: string, profession: string, bio: string,
  followers: number, updatedAt: string,
];

export type RawAlbum = readonly [
  actorId: number, title: string, description: string, coverAlt: string,
  coverAspectRatio: number, photos: number, videos: number,
  isSeries: boolean, tags: string, publishedAt: string, updatedAt: string,
];