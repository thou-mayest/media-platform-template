import {
  allTags,
  artists as galleryArtists,
  artistsByTag,
  artworks as galleryArtworks,
  artworksByArtist,
  artworksByCategory,
  artworksByTag,
  categories as galleryCategories,
} from '@/data/gallery';
import type { Artist as GalleryArtist, Artwork as GalleryArtwork } from '@/data/gallery';
import type {
  AdminCategory,
  AdminTag,
  Artist,
  ArtistProfile,
  Artwork,
  ArtworkDetail,
  CatalogSnapshot,
  Category,
  CategoryDetail,
  HomeFeed,
  SearchResults,
  TagDetail,
} from '@/lib/catalog-types';

const HERO_IDS = [0, 24, 12];
const RECENT_IDS = [4, 20, 28, 8, 16, 32, 14, 2, 27, 18, 6, 35];
const SPOTLIGHT_SLUG = 'mira-okafor';
const REQUEST_TIMEOUT_MS = 1800;

const apiBaseUrl = normalizeBaseUrl(
  import.meta.env.PUBLIC_API_BASE_URL ?? import.meta.env.API_BASE_URL ?? 'http://127.0.0.1:5048',
);

let snapshotPromise: Promise<CatalogSnapshot> | undefined;
let homeFeedPromise: Promise<HomeFeed> | undefined;

type ApiArtist = {
  id: string;
  name: string;
  location: string;
  disciplines: string;
  bio: string;
  accent: string;
  initials: string;
  since: string;
  slug: string;
  workCount: number;
  sampleBackgrounds: string[];
};

type ApiArtwork = {
  id: string;
  creatorId: string;
  creatorName: string;
  creatorSlug: string;
  title: string;
  year: number;
  category: string;
  categorySlug: string;
  medium: string;
  tags: string[];
  aspectRatio: number;
  background: string;
  size: string;
  slug: string;
};

type ApiCategory = {
  name: string;
  count: number;
  background: string;
  slug: string;
};

type ApiSnapshot = {
  creators: ApiArtist[];
  works: ApiArtwork[];
  categories: ApiCategory[];
  tags: string[];
};

type ApiHomeFeed = {
  heroWorks: ApiArtwork[];
  spotlight: {
    creator: ApiArtist;
    works: ApiArtwork[];
  };
  recentWorks: ApiArtwork[];
  categories: ApiCategory[];
};

type ApiArtistProfile = {
  creator: ApiArtist;
  works: ApiArtwork[];
  tags: string[];
};

type ApiCategoryDetail = {
  category: ApiCategory;
  works: ApiArtwork[];
  tags: string[];
};

type ApiArtworkDetail = {
  video: ApiArtwork;
  creator: ApiArtist;
  related: ApiArtwork[];
};

type ApiTagDetail = {
  name: string;
  slug: string;
  creators: Array<Pick<ApiArtist, 'id' | 'name' | 'slug'>>;
  works: ApiArtwork[];
};

type ApiSearchResults = {
  query: string;
  creators: ApiArtist[];
  works: ApiArtwork[];
};

export async function getCatalogSnapshot(): Promise<CatalogSnapshot> {
  if (!snapshotPromise) {
    snapshotPromise = loadCatalogSnapshot();
  }

  return snapshotPromise;
}

export async function getHomeFeed(): Promise<HomeFeed> {
  if (!homeFeedPromise) {
    homeFeedPromise = loadHomeFeed();
  }

  return homeFeedPromise;
}

export async function getArtistProfile(slug: string): Promise<ArtistProfile | null> {
  const apiProfile = await fetchJson<ApiArtistProfile>(`/api/v1/creators/${encodeURIComponent(slug)}`);
  if (apiProfile) {
    return {
      artist: mapArtist(apiProfile.creator),
      works: apiProfile.works.map(mapArtwork),
      tags: apiProfile.tags,
    };
  }

  const artist = galleryArtists.find((item) => item.slug === slug);
  if (!artist) {
    return null;
  }

  const works = artworksByArtist(artist.id).map(mapFallbackArtwork);
  return {
    artist: mapFallbackArtist(artist),
    works,
    tags: [...new Set(works.flatMap((work) => work.tags))].sort(),
  };
}

export async function getCategoryDetail(slug: string): Promise<CategoryDetail | null> {
  const apiCategory = await fetchJson<ApiCategoryDetail>(`/api/v1/categories/${encodeURIComponent(slug)}`);
  if (apiCategory) {
    return {
      category: mapCategory(apiCategory.category),
      works: apiCategory.works.map(mapArtwork),
      tags: apiCategory.tags,
    };
  }

  const category = galleryCategories.find((item) => item.slug === slug);
  if (!category) {
    return null;
  }

  const works = artworksByCategory(category.name).map(mapFallbackArtwork);
  return {
    category: mapFallbackCategory(category),
    works,
    tags: [...new Set(works.flatMap((work) => work.tags))].sort(),
  };
}

export async function getArtworkDetail(slug: string): Promise<ArtworkDetail | null> {
  const apiArtwork = await fetchJson<ApiArtworkDetail>(`/api/v1/videos/${encodeURIComponent(slug)}`);
  if (apiArtwork) {
    return {
      work: mapArtwork(apiArtwork.video),
      artist: mapArtist(apiArtwork.creator),
      related: apiArtwork.related.map(mapArtwork),
    };
  }

  const work = galleryArtworks.find((item) => item.slug === slug);
  if (!work) {
    return null;
  }

  const artist = galleryArtists.find((item) => item.slug === work.artistSlug);
  if (!artist) {
    return null;
  }

  const related = galleryArtworks
    .filter(
      (candidate) =>
        candidate.id !== work.id &&
        (candidate.category === work.category ||
          candidate.tags.some((tag) => work.tags.includes(tag))),
    )
    .slice(0, 4)
    .map(mapFallbackArtwork);

  return {
    work: mapFallbackArtwork(work),
    artist: mapFallbackArtist(artist),
    related,
  };
}

export async function getTagDetail(slug: string): Promise<TagDetail | null> {
  const apiTag = await fetchJson<ApiTagDetail>(`/api/v1/tags/${encodeURIComponent(slug)}`);
  if (apiTag) {
    return {
      name: apiTag.name,
      slug: apiTag.slug,
      artists: apiTag.creators.map((creator) => ({
        id: creator.id,
        name: creator.name,
        slug: creator.slug,
      })),
      works: apiTag.works.map(mapArtwork),
    };
  }

  const tagName = allTags.find((item) => item.toLowerCase() === slug.toLowerCase());
  if (!tagName) {
    return null;
  }

  return {
    name: tagName,
    slug,
    artists: artistsByTag(tagName).map((artist) => ({
      id: artist.id,
      name: artist.name,
      slug: artist.slug,
    })),
    works: artworksByTag(tagName).map(mapFallbackArtwork),
  };
}

export async function searchCatalog(query: string): Promise<SearchResults> {
  const trimmedQuery = query.trim();
  if (!trimmedQuery) {
    return { query: '', artists: [], works: [] };
  }

  const apiResults = await fetchJson<ApiSearchResults>(`/api/v1/search?q=${encodeURIComponent(trimmedQuery)}`);
  if (apiResults) {
    return {
      query: apiResults.query,
      artists: apiResults.creators.map(mapArtist),
      works: apiResults.works.map(mapArtwork),
    };
  }

  const snapshot = await getCatalogSnapshot();
  const q = trimmedQuery.toLowerCase();

  return {
    query: trimmedQuery,
    artists: snapshot.artists.filter((artist) =>
      `${artist.name} ${artist.location} ${artist.disciplines} ${artist.bio}`.toLowerCase().includes(q),
    ),
    works: snapshot.artworks.filter((work) =>
      `${work.title} ${work.artistName} ${work.medium} ${work.category} ${work.tags.join(' ')}`
        .toLowerCase()
        .includes(q),
    ),
  };
}

async function loadCatalogSnapshot(): Promise<CatalogSnapshot> {
  const apiSnapshot = await fetchJson<ApiSnapshot>('/api/v1/discovery/snapshot');
  if (apiSnapshot) {
    return {
      artists: apiSnapshot.creators.map(mapArtist),
      artworks: apiSnapshot.works.map(mapArtwork),
      categories: apiSnapshot.categories.map(mapCategory),
      tags: apiSnapshot.tags,
    };
  }

  return buildFallbackSnapshot();
}

async function loadHomeFeed(): Promise<HomeFeed> {
  const apiHomeFeed = await fetchJson<ApiHomeFeed>('/api/v1/discovery/home');
  if (apiHomeFeed) {
    return {
      heroWorks: apiHomeFeed.heroWorks.map(mapArtwork),
      spotlight: {
        artist: mapArtist(apiHomeFeed.spotlight.creator),
        works: apiHomeFeed.spotlight.works.map(mapArtwork),
      },
      recentWorks: apiHomeFeed.recentWorks.map(mapArtwork),
      categories: apiHomeFeed.categories.map(mapCategory),
    };
  }

  return buildFallbackHomeFeed();
}

function buildFallbackSnapshot(): CatalogSnapshot {
  return {
    artists: galleryArtists.map(mapFallbackArtist),
    artworks: galleryArtworks.map(mapFallbackArtwork),
    categories: galleryCategories.map(mapFallbackCategory),
    tags: [...allTags],
  };
}

function buildFallbackHomeFeed(): HomeFeed {
  const snapshot = buildFallbackSnapshot();
  const heroWorks = HERO_IDS
    .map((index) => snapshot.artworks[index])
    .filter(Boolean);

  const spotlightArtist =
    snapshot.artists.find((artist) => artist.slug === SPOTLIGHT_SLUG) ?? snapshot.artists[0]!;
  const spotlightWorks = snapshot.artworks
    .filter((work) => work.artistSlug === spotlightArtist.slug)
    .slice(0, 3);

  const recentWorks = RECENT_IDS
    .map((index) => snapshot.artworks[index])
    .filter(Boolean);

  return {
    heroWorks,
    spotlight: {
      artist: spotlightArtist,
      works: spotlightWorks,
    },
    recentWorks,
    categories: snapshot.categories,
  };
}

function mapArtist(artist: ApiArtist): Artist {
  return {
    id: artist.id,
    name: artist.name,
    location: artist.location,
    disciplines: artist.disciplines,
    bio: artist.bio,
    accent: artist.accent,
    initials: artist.initials,
    since: artist.since,
    slug: artist.slug,
    workCount: artist.workCount,
    sampleBackgrounds: artist.sampleBackgrounds,
  };
}

function mapArtwork(work: ApiArtwork): Artwork {
  return {
    id: work.id,
    artistId: work.creatorId,
    artistName: work.creatorName,
    artistSlug: work.creatorSlug,
    title: work.title,
    year: work.year,
    category: work.category,
    categorySlug: work.categorySlug,
    medium: work.medium,
    tags: work.tags,
    ar: work.aspectRatio,
    bg: work.background,
    size: work.size,
    slug: work.slug,
  };
}

function mapCategory(category: ApiCategory): Category {
  return {
    name: category.name,
    count: category.count,
    bg: category.background,
    slug: category.slug,
  };
}

function mapFallbackArtist(artist: GalleryArtist): Artist {
  const works = artworksByArtist(artist.id);
  return {
    ...artist,
    workCount: works.length,
    sampleBackgrounds: works.slice(0, 3).map((work) => work.bg),
  };
}

function mapFallbackArtwork(work: GalleryArtwork): Artwork {
  return { ...work };
}

function mapFallbackCategory(category: (typeof galleryCategories)[number]): Category {
  return { ...category };
}

async function fetchJson<T>(path: string): Promise<T | null> {
  if (!apiBaseUrl) {
    return null;
  }

  const controller = new AbortController();
  const timeoutId = setTimeout(() => controller.abort(), REQUEST_TIMEOUT_MS);

  try {
    const response = await fetch(`${apiBaseUrl}${path}`, {
      headers: {
        accept: 'application/json',
      },
      signal: controller.signal,
    });

    if (!response.ok) {
      return null;
    }

    return (await response.json()) as T;
  } catch {
    return null;
  } finally {
    clearTimeout(timeoutId);
  }
}

function normalizeBaseUrl(value: string): string {
  return value.trim().replace(/\/$/, '');
}

export async function getAllAdminCategories(): Promise<AdminCategory[]> {
  const result = await fetchJson<AdminCategory[]>('/api/v1/categories/all');
  if (result) {
    return result;
  }

  return galleryCategories.map((c, index) => ({
    id: `cat-${index}`,
    name: c.name,
    slug: c.slug,
    workCount: c.count,
  }));
}

export async function getAllAdminTags(): Promise<AdminTag[]> {
  const result = await fetchJson<AdminTag[]>('/api/v1/tags');
  if (result) {
    return result;
  }

  return allTags.map((name, index) => ({
    id: `tag-${index}`,
    name,
    slug: name.toLowerCase().replace(/\s+/g, '-'),
    workCount: artworksByTag(name).length,
  }));
}
