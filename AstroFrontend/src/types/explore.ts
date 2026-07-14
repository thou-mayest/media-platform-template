export const EXPLORE_SORT_MODES = ['newest', 'oldest', 'title', 'artist'] as const;

export type ExploreSortMode = (typeof EXPLORE_SORT_MODES)[number];
