// Contracts for the Comments module.
//
// Mirrors the shape the backend Comments module will return (the .NET side
// keeps these as public contracts so other modules can reference them without
// depending on the module's internals). Contains only data the API returns;
// presentation concerns (avatars, indentation, time formatting) live in the
// UI component.
//
// Comments are a separate aggregate from reactions: they are posted, not
// toggled, so the read model carries full rows instead of counts.

/** Read model over the Comment aggregate. Threading is expressed with
 *  `parentId` — replies point at their parent's id (one level deep in this
 *  version). */
export type Comment = {
  id: number;
  postId: number;
  parentId: number | null;
  authorName: string;
  content: string;
  createdAt: string;
  updatedAt: string;
};

/** New-comment input — mirrors the backend's create-comment command. */
export type NewComment = {
  postId: number;
  parentId: number | null;
  content: string;
};

/** Storage shape for the viewer's own comments (anonymous, same mechanism as
 *  reactions/follow/save). Keyed by postId so each post keeps its own thread. */
export type StoredComment = {
  id: number;
  parentId: number | null;
  content: string;
  createdAt: string;
  updatedAt?: string;
};

/** The requesting viewer's identity when posting — anonymous mocks sign as
 *  "You". A real backend resolves this from the JWT. */
export const VIEWER_NAME = 'You';
