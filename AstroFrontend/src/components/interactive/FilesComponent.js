// src/components/FileCard.js

// --- Icons (same as before, moved here) ---
const FILE_ICON = `<svg width="36" height="36" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.4" stroke-linecap="round" stroke-linejoin="round"><path d="M14 3H6a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V9z"/><path d="M14 3v6h6"/></svg>`;
const IMAGE_ICON = `<svg width="36" height="36" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.4" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="3" width="18" height="18" rx="2"/><circle cx="8.5" cy="8.5" r="1.5"/><path d="M21 15l-5-5L5 21"/></svg>`;
const VIDEO_ICON = `<svg width="36" height="36" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.4" stroke-linecap="round" stroke-linejoin="round"><rect x="2" y="2" width="20" height="20" rx="2.18"/><path d="m10 8 6 4-6 4V8z"/></svg>`;
const PLAY_ICON = `<svg width="28" height="28" viewBox="0 0 24 24" fill="currentColor"><path d="M8 5v14l11-7z"/></svg>`;
const EXTERNAL_ICON = `<svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M18 13v6a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h6"/><path d="M15 3h6v6"/><path d="M10 14 21 3"/></svg>`;
const DOWNLOAD_ICON = `<svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"/><path d="M7 10l5 5 5-5"/><path d="M12 15V3"/></svg>`;
const DELETE_ICON = `<svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M3 6h18"/><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0h10"/><path d="M10 11v6"/><path d="M14 11v6"/></svg>`;

// --- Helper functions ---
function formatSize(bytes) {
  if (bytes < 1024) return `${bytes} B`;
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
  return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
}

function isImage(type) { return type.startsWith('image/'); }
function isVideo(type) { return type.startsWith('video/'); }

/**
 * Creates a card element for a file with action buttons.
 * @param {FileDto} file - the file data
 * @param {Function} onDelete - callback when delete is clicked (receives file id)
 * @param {Function} onDownload - callback when download is clicked (receives file url)
 * @returns {HTMLLIElement}
 */
export function createFileCard(file, onDelete, onDownload) {
  const li = document.createElement('li');
  li.className = 'card';

  // --- Preview ---
  const preview = document.createElement('div');
  preview.className = 'preview';

  if (isImage(file.contentType)) {
    const img = document.createElement('img');
    img.src = file.url;
    img.alt = file.originalFileName;
    img.loading = 'lazy';
    preview.appendChild(img);
  } else if (isVideo(file.contentType)) {
    const video = document.createElement('video');
    video.src = file.url;
    video.preload = 'metadata';
    preview.appendChild(video);
    const overlay = document.createElement('div');
    overlay.className = 'preview__overlay';
    overlay.innerHTML = PLAY_ICON;
    preview.appendChild(overlay);
  } else {
    preview.innerHTML = `<div class="preview__icon">${FILE_ICON}</div>`;
  }

  // --- Info ---
  const info = document.createElement('div');
  info.className = 'info';

  // File name
  const name = document.createElement('p');
  name.className = 'name';
  name.title = file.originalFileName;
  name.textContent = file.originalFileName;

  // Meta (size, type, date)
  const meta = document.createElement('p');
  meta.className = 'meta';
  const typeLabel = file.contentType || 'unknown';
  const date = new Date(file.createdAt).toLocaleString();
  meta.innerHTML = `<strong>${formatSize(file.fileSize)}</strong> · ${typeLabel}<br/>${date}`;

  // --- NEW: Type badge label ---
  const badge = document.createElement('span');
  badge.className = 'badge';
  if (isImage(file.contentType)) {
    badge.textContent = '🖼 Image';
  } else if (isVideo(file.contentType)) {
    badge.textContent = '🎬 Video';
  } else {
    badge.textContent = '📄 Document';
  }

  // --- Action buttons row ---
  const actions = document.createElement('div');
  actions.className = 'card__actions';

  // Open link (existing)
  const link = document.createElement('a');
  link.className = 'card__link';
  link.href = file.url;
  link.target = '_blank';
  link.rel = 'noopener';
  link.innerHTML = `Open ${EXTERNAL_ICON}`;

  // Download button
  const downloadBtn = document.createElement('button');
  downloadBtn.className = 'btn btn-ghost btn-sm card__btn';
  downloadBtn.innerHTML = `${DOWNLOAD_ICON} Download`;
  downloadBtn.type = 'button';
  downloadBtn.addEventListener('click', (e) => {
    e.stopPropagation();
    if (onDownload) onDownload(file.url);
  });

  // Delete button
  const deleteBtn = document.createElement('button');
  deleteBtn.className = 'btn btn-danger btn-sm card__btn';
  deleteBtn.innerHTML = `${DELETE_ICON} Delete`;
  deleteBtn.type = 'button';
  deleteBtn.addEventListener('click', (e) => {
    e.stopPropagation();
    if (confirm(`Delete "${file.originalFileName}"?`)) {
      if (onDelete) onDelete(file.id);
    }
  });

  actions.append(link, downloadBtn, deleteBtn);

  // Assemble info
  info.append(name, badge, meta, actions);
  li.append(preview, info);
  return li;
}