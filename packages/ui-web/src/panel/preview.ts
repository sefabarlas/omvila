import type { CSSProperties } from 'react';

// Yalnız önizleme hikâyeleri için yerleşim (tokenlarla).
export const previewRow: CSSProperties = {
  display: 'flex',
  flexWrap: 'wrap',
  gap: 'var(--bosluk-3)',
  alignItems: 'center',
};
export const previewStack: CSSProperties = {
  display: 'flex',
  flexDirection: 'column',
  gap: 'var(--bosluk-4)',
  maxWidth: '420px',
};
