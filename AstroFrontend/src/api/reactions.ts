import { apiFetch } from './client';
import type {
  ReactionSummary,
  ReactionType,
  ToggleReactionRequest,
  ToggleReactionResponse,
} from '@/data/reactions';

/**
 * HTTP client for the Reactions module, mirroring the backend controller:
 * reactions live on the post aggregate, so the routes hang off /api/posts.
 * Until the backend module exists the UI is driven by src/data/reactions.
 */
export const reactionsApi = {
  /** GET /api/posts/{postId}/reactions — read model for the bar. */
  summary: (postId: number) =>
    apiFetch<ReactionSummary>(`/api/posts/${postId}/reactions`),

  /** POST /api/posts/{postId}/reactions — toggle the viewer's reaction. */
  toggle: (postId: number, reaction: ReactionType) =>
    apiFetch<ToggleReactionResponse>(`/api/posts/${postId}/reactions`, {
      method: 'POST',
      body: { postId, reaction } satisfies ToggleReactionRequest,
    }),
};
