function formatSize(bytes: number): string {
  if (bytes < 1024) return `${bytes} B`;
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
  return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
}

function fileExtension(name: string): string {
  const i = name.lastIndexOf('.');
  return i > 0 ? name.slice(i + 1).toLowerCase() : '';
}

const IMAGE_ICON =
  '<svg width="40" height="40" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.4" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="3" width="18" height="18" rx="2"/><circle cx="8.5" cy="8.5" r="1.5"/><path d="M21 15l-5-5L5 21"/></svg>';

const VIDEO_ICON =
  '<svg width="40" height="40" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.4" stroke-linecap="round" stroke-linejoin="round"><rect x="2" y="2" width="20" height="20" rx="2.18"/><path d="m10 8 6 4-6 4V8z"/></svg>';

const FILE_ICON =
  '<svg width="40" height="40" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.4" stroke-linecap="round" stroke-linejoin="round"><path d="M14 3H6a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V9z"/><path d="M14 3v6h6"/></svg>';

const PLAY_ICON =
  '<svg width="32" height="32" viewBox="0 0 24 24" fill="currentColor"><path d="M8 5v14l11-7z"/></svg>';

const EXTERNAL_ICON =
  '<svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M18 13v6a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h6"/><path d="M15 3h6v6"/><path d="M10 14 21 3"/></svg>';

const STYLE = `
  :host {
    display: block;
    font-family: inherit;
  }

  .card {
    display: grid;
    grid-template-rows: auto 1fr;
    height: 100%;
    background: var(--surface);
    border: 1px solid var(--border-soft);
    border-radius: 16px;
    overflow: hidden;
    transition: transform 0.25s ease, box-shadow 0.25s ease, border-color 0.25s ease;
  }

  .card:hover {
    transform: translateY(-4px);
    box-shadow: 0 22px 50px -26px var(--shadow);
    border-color: var(--border);
  }

  .preview {
    position: relative;
    aspect-ratio: 4 / 3;
    background: linear-gradient(135deg, var(--bg) 0%, var(--surface) 100%);
    display: flex;
    align-items: center;
    justify-content: center;
    overflow: hidden;
  }

  .preview img,
  .preview video {
    width: 100%;
    height: 100%;
    object-fit: cover;
    transition: transform 0.4s ease;
  }

  .card:hover .preview img,
  .card:hover .preview video {
    transform: scale(1.03);
  }

  .icon {
    color: var(--faint);
  }

  .ext {
    position: absolute;
    bottom: 10px;
    right: 10px;
    background: var(--ink);
    color: var(--on-ink);
    padding: 3px 8px;
    border-radius: 6px;
    font-size: 10px;
    font-weight: 700;
    text-transform: uppercase;
    letter-spacing: 0.04em;
  }

  .overlay {
    position: absolute;
    inset: 0;
    display: flex;
    align-items: center;
    justify-content: center;
    background: rgba(0, 0, 0, 0.35);
    color: #f5f2ec;
    opacity: 0;
    transition: opacity 0.2s ease;
    pointer-events: none;
  }

  .card:hover .overlay {
    opacity: 1;
  }

  .body {
    padding: 1rem;
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
  }

  .name {
    font-family: 'Spectral', serif;
    font-size: 1.05rem;
    line-height: 1.25;
    color: var(--text);
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
    margin: 0;
  }

  .row {
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: 0.75rem;
    flex-wrap: wrap;
  }

  .type-pill {
    font-size: 0.68rem;
    font-weight: 700;
    text-transform: uppercase;
    letter-spacing: 0.06em;
    padding: 0.25rem 0.55rem;
    border-radius: 999px;
    background: var(--chip-bg);
    color: var(--text-2);
  }

  .size {
    font-size: 0.8rem;
    color: var(--muted);
    margin-left: auto;
  }

  .date {
    font-size: 0.8rem;
    color: var(--muted);
    margin: 0;
  }

  .link {
    display: inline-flex;
    align-items: center;
    gap: 0.35rem;
    margin-top: 0.25rem;
    font-size: 0.85rem;
    font-weight: 600;
    color: var(--accent);
    text-decoration: none;
  }

  .link:hover {
    text-decoration: underline;
    text-underline-offset: 3px;
  }
`;

export class MediaCard extends HTMLElement {
  constructor() {
    super();
    this.attachShadow({ mode: 'open' });
  }

  connectedCallback(): void {
    this.render();
  }

  private render(): void {
    const url = this.getAttribute('data-url') || '';
    const name = this.getAttribute('data-name') || '';
    const type = this.getAttribute('data-type') || '';
    const size = Number(this.getAttribute('data-size') || '0');
    const created = this.getAttribute('data-created') || '';

    const isImage = type.startsWith('image/');
    const isVideo = type.startsWith('video/');
    const ext = fileExtension(name);

    let preview = '';
    if (isImage) {
      preview = `
        <div class="preview">
          <img src="${url}" alt="${name}" loading="lazy" />
        </div>
      `;
    } else if (isVideo) {
      preview = `
        <div class="preview">
          <video src="${url}" preload="metadata"></video>
          <div class="overlay">${PLAY_ICON}</div>
        </div>
      `;
    } else {
      preview = `
        <div class="preview">
          <div class="icon">${FILE_ICON}</div>
          ${ext ? `<span class="ext">${ext}</span>` : ''}
        </div>
      `;
    }

    const dateText = created ? new Date(created).toLocaleString() : '';

    this.shadowRoot!.innerHTML = `
      <style>${STYLE}</style>
      <article class="card">
        ${preview}
        <div class="body">
          <p class="name" title="${name}">${name}</p>
          <div class="row">
            <span class="type-pill">${type || 'unknown'}</span>
            <span class="size">${formatSize(size)}</span>
          </div>
          ${dateText ? `<p class="date">${dateText}</p>` : ''}
          <a class="link" href="${url}" target="_blank" rel="noopener">
            Open file ${EXTERNAL_ICON}
          </a>
        </div>
      </article>
    `;
  }
}

if (!customElements.get('media-card')) {
  customElements.define('media-card', MediaCard);
}
