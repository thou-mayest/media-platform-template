// Contracts for the Reactions module.
//
// Mirrors the shape the backend Reactions module will return (the .NET side
// keeps these as public contracts so other modules can reference them without
// depending on the module's internals). Contains only data the API returns;
// presentation concerns (labels, icons, active styling) live in the UI
// component.

export type ReactionType = 'like' | 'dislike';

/** Per-type count. `Record` keeps the enum open — adding a reaction type
 *  requires no shape change, only the type union. */
export type ReactionCounts = Record<ReactionType, number>;

/** Read model over the PostReactions aggregate. */
export type ReactionSummary = {
  postId: number;
  counts: ReactionCounts;
  total: number;
  /** Comments are a separate aggregate — counted here for the bar, but not a
   *  togglable reaction. */
  commentCount: number;
  /** The requesting viewer's reaction, if any. Null for anonymous viewers. */
  viewerReaction: ReactionType | null;
};

/** Body of the toggle command — mirrors Reactions.Application's command. */
export type ToggleReactionRequest = {
  postId: number;
  reaction: ReactionType;
};

export type ToggleReactionResponse = {
  summary: ReactionSummary;
  /** True when the reaction was removed (toggled off) rather than added. */
  removed: boolean;
};
