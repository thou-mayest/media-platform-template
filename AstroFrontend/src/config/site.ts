// Site-wide constants: things that describe the product, not the data.
//
// The absolute origin is deliberately NOT here. It comes from astro.config's
// `site` and is read as `Astro.site`, so there is exactly one source of truth.

export const siteName = 'Verso';
export const siteLocale = 'en';

/** Open Graph requires language_TERRITORY, unlike <html lang>. */
export const siteOgLocale = 'en_US';

/** Appended to every page title. Titles are truncated around 60 characters
 *  in results, so the suffix stays short. */
export function pageTitle(title: string): string {
  return `${title} — ${siteName}`;
}

/** Used for twitter:site. Empty string omits the tag entirely — an invalid
 *  handle is worse than none. */
export const twitterHandle = '';

/** Album cards per page. 10 albums + 2 ad slots = 12 cells, which fills the
 *  4-column grid exactly, matching the design's row rhythm. */
export const ALBUMS_PER_PAGE = 10;

/** How recently an album must have been published to earn a "New" badge. */
export const NEW_BADGE_DAYS = 90;

/** Media items per page in the album feed. */
export const POSTS_PER_PAGE = 8;