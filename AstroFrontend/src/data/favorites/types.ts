// Contracts for the Favorites module.
//
// Mirrors the shape the backend Favorites module will return. A favorite is a
// simple bookmark — no metadata beyond the post id and when it was saved.

/** A single saved post. */
export type Favorite = {
  postId: number;
  savedAt: string;
};
