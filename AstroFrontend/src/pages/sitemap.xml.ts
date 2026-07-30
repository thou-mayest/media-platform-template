import type { APIRoute } from 'astro';
import { artists, artworks, categories, galleryTags } from '@/data/gallery';

export const GET: APIRoute = ({ site }) => {
  const origin = site ?? new URL('http://localhost:4321');
  const paths = [
    '/',
    '/explore',
    '/artists',
    ...artists.map((artist) => `/artists/${artist.slug}`),
    '/categories',
    ...categories.map((category) => `/categories/${category.slug}`),
    '/tags',
    ...galleryTags.map((tag) => `/tags/${tag.slug}`),
    ...artworks.map((work) => `/works/${work.slug}`),
  ];
  const urls = paths
    .map((path) => `  <url><loc>${new URL(path, origin).toString()}</loc></url>`)
    .join('\n');

  return new Response(
    `<?xml version="1.0" encoding="UTF-8"?>\n<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">\n${urls}\n</urlset>`,
    { headers: { 'Content-Type': 'application/xml; charset=utf-8' } },
  );
};
