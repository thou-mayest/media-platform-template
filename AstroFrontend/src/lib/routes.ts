// Every application URL is built here. Nothing else constructs paths, so a
// routing change is a single-file edit and internal links cannot drift apart.

const ACTORS = '/actors';

/** Page 1 never carries ?page=1. That would create a second URL for identical
 *  content, competing with the canonical and splitting its signals. */
function withPage(path: string, page: number): string {
  return page > 1 ? `${path}?page=${page}` : path;
}

export function actorPath(slug: string, page = 1): string {
  return withPage(`${ACTORS}/${slug}`, page);
}

export function albumPath(actorSlug: string, albumSlug: string, page = 1): string {
  return withPage(`${ACTORS}/${actorSlug}/a/${albumSlug}`, page);
}

export function postPath(
  actorSlug: string,
  albumSlug: string,
  postId: number | string,
): string {
  return `${ACTORS}/${actorSlug}/a/${albumSlug}/p/${postId}`;
}

/**
 * Absolute URL for canonical, og:url and sitemap entries.
 * Throws rather than silently emitting a relative canonical — a missing origin
 * is a deploy misconfiguration that should fail loudly, not degrade quietly.
 */
export function absoluteUrl(path: string, site: URL | undefined): string {
  if (!site) {
    throw new Error(
      'astro.config `site` is not set — canonical URLs, og:url and the sitemap all require an absolute origin.',
    );
  }
  return new URL(path, site).toString();
}