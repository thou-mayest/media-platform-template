export type EntityId = string | number;

export type Artist = {
  id: EntityId;
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

export type Artwork = {
  id: EntityId;
  artistId: EntityId;
  artistName: string;
  artistSlug: string;
  title: string;
  year: number;
  category: string;
  categorySlug: string;
  medium: string;
  tags: string[];
  ar: number;
  bg: string;
  size: string;
  slug: string;
};

export type Category = {
  name: string;
  count: number;
  bg: string;
  slug: string;
};

export type HomeFeed = {
  heroWorks: Artwork[];
  spotlight: {
    artist: Artist;
    works: Artwork[];
  };
  recentWorks: Artwork[];
  categories: Category[];
};

export type CatalogSnapshot = {
  artists: Artist[];
  artworks: Artwork[];
  categories: Category[];
  tags: string[];
};

export type ArtistProfile = {
  artist: Artist;
  works: Artwork[];
  tags: string[];
};

export type CategoryDetail = {
  category: Category;
  works: Artwork[];
  tags: string[];
};

export type ArtworkDetail = {
  work: Artwork;
  artist: Artist;
  related: Artwork[];
};

export type TagDetail = {
  name: string;
  slug: string;
  artists: Array<Pick<Artist, 'id' | 'name' | 'slug'>>;
  works: Artwork[];
};

export type SearchResults = {
  query: string;
  artists: Artist[];
  works: Artwork[];
};

export type AdminCategory = {
  id: string;
  name: string;
  slug: string;
  workCount: number;
};

export type AdminTag = {
  id: string;
  name: string;
  slug: string;
  workCount: number;
};
