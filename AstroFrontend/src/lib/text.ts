/** Trims to a word boundary. Used for meta descriptions, which are truncated
 *  around 155 characters in results. */
export function clamp(s: string, max = 155): string {
  if (s.length <= max) return s;
  const cut = s.slice(0, max - 1);
  return `${cut.slice(0, cut.lastIndexOf(' ')).trimEnd()}…`;
}

/** 38200 -> "38.2k". Presentation only — counts stay numbers in the model. */
export function formatCompact(n: number): string {
  if (n < 1000) return String(n);
  const k = n / 1000;
  return `${k >= 10 ? k.toFixed(1) : k.toFixed(2)}k`.replace('.0k', 'k');
}