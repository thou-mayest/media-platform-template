import { artworks as galleryArtworks } from '@/data/gallery';

export interface PostItem {
  id: string;
  authorId: string;
  mediaAssetId?: string;
  title: string;
  description: string;
  mediaUrl: string;
  altText: string;
  category: string;
  tags: string[];
  status: 'Draft' | 'Published' | 'Archived';
  viewCount: number;
  createdDate: string;
  publishedAt: string | null;
}

export interface CreatePostPayload {
  mediaAssetId: string;
  mediaUrl: string;
  title: string;
  description: string;
  category: string;
  altText: string;
  tags?: string[];
  publishImmediately?: boolean;
}

const apiBaseUrl = (
  import.meta.env.PUBLIC_API_BASE_URL ??
  import.meta.env.API_BASE_URL ??
  'http://127.0.0.1:5048'
).replace(/\/$/, '');

const REQUEST_TIMEOUT_MS = 2500;

async function fetchJson<T>(path: string, init?: RequestInit): Promise<T | undefined> {
  const controller = new AbortController();
  const timeoutId = setTimeout(() => controller.abort(), REQUEST_TIMEOUT_MS);

  try {
    const res = await fetch(`${apiBaseUrl}${path}`, {
      ...init,
      signal: controller.signal,
      headers: {
        Accept: 'application/json',
        ...init?.headers,
      },
    });

    if (!res.ok) return undefined;
    if (res.status === 204) return undefined;
    return (await res.json()) as T;
  } catch {
    return undefined;
  } finally {
    clearTimeout(timeoutId);
  }
}

function getFallbackPosts(): PostItem[] {
  return galleryArtworks.slice(0, 15).map((w, index) => ({
    id: `post-sample-${w.id}`,
    authorId: String(w.artistId),
    title: w.title,
    description: `Original work in ${w.medium}. Created in ${w.year}. Dimensions: ${w.size}.`,
    mediaUrl: '',
    altText: `${w.title} by ${w.artistName}`,
    category: w.category,
    tags: w.tags,
    status: index % 4 === 0 ? 'Draft' : 'Published',
    viewCount: (index + 1) * 142,
    createdDate: new Date(Date.now() - index * 86400000).toISOString(),
    publishedAt: index % 4 === 0 ? null : new Date(Date.now() - index * 86400000).toISOString(),
  }));
}

export async function getAllAdminPosts(): Promise<PostItem[]> {
  type PagedResponse = {
    items?: PostItem[];
    page?: number;
    totalCount?: number;
  };

  const response = await fetchJson<PagedResponse | PostItem[]>('/api/posts?pageSize=100');
  if (response) {
    if (Array.isArray(response)) return response;
    if (Array.isArray(response.items)) return response.items;
  }

  return getFallbackPosts();
}

export async function getAdminPostById(id: string): Promise<PostItem | undefined> {
  const result = await fetchJson<PostItem>(`/api/posts/${encodeURIComponent(id)}`);
  if (result) return result;

  const fallback = getFallbackPosts().find((p) => p.id === id);
  return fallback;
}
