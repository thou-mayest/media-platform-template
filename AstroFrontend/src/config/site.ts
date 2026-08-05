// Site-wide constants: things that describe the product, not the data.
//
// The absolute origin is deliberately NOT here. It comes from astro.config's
// `site` and is read as `Astro.site`, so there is exactly one source of truth.

export const siteName = 'Verso';
export const siteLocale = 'en';

/** Appended to every page title. Titles are truncated around 60 characters
 *  in results, so the suffix stays short. */
export function pageTitle(title: string): string {
  return `${title} — ${siteName}`;
}

/** Used for twitter:site. Empty string omits the tag entirely — an invalid
 *  handle is worse than none. */
export const twitterHandle = '';

/** Album cards per page on the profile grid. Becomes the API's pageSize. */
export const ALBUMS_PER_PAGE = 6;

/** How recently an album must have been published to earn a "New" badge. */
export const NEW_BADGE_DAYS = 90;