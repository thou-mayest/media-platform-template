// Public surface of the Reactions module. Pages import '@/data/reactions'
// only, so the mock layer can be swapped for the real API without touching
// templates — the same rule the media module follows.

import { commentCountFor } from '@/data/comments';
import { mockCounts } from './fixtures';
import type { ReactionSummary, ReactionType } from './types';

export type {
  ReactionType,
  ReactionCounts,
  ReactionSummary,
  ToggleReactionRequest,
  ToggleReactionResponse,
} from './types';

/**
 * Query read model for a post's reactions.
 * `viewerReaction` is the authenticated viewer's own reaction; SSR passes null
 * and the client paints it from localStorage (anonymous state, same as
 * follow/save). With a real backend this resolves through
 * `reactionsApi.summary()`. `commentCount` is a read over the Comments
 * module — comments are a separate aggregate, only counted here for the bar.
 */
export function reactionSummary(
  postId: number,
  viewerReaction: ReactionType | null = null,
): ReactionSummary {
  const counts = mockCounts(postId);
  return {
    postId,
    counts,
    total: counts.like + counts.dislike,
    commentCount: commentCountFor(postId),
    viewerReaction,
  };
}
