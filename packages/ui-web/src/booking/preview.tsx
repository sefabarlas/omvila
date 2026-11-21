import type { CSSProperties, ReactNode } from 'react';

// Yalnız önizleme hikâyeleri için telefon çerçevesi ve yerleşim (tokenlarla).
const phone: CSSProperties = {
  display: 'flex',
  flexDirection: 'column',
  boxSizing: 'border-box',
  width: '100%',
  maxWidth: 'var(--ekran-genislik)',
  minHeight: '100vh',
  margin: '0 auto',
  background: 'var(--zemin)',
  color: 'var(--metin)',
};

export const screenPad: CSSProperties = {
  display: 'flex',
  flexDirection: 'column',
  gap: 'var(--bosluk-4)',
  padding: 'var(--bosluk-4)',
  flex: 1,
};

export const sectionTitle: CSSProperties = {
  margin: 0,
  fontSize: 'var(--yazi-buyuk-baslik-boyut)',
  fontWeight: 800,
  lineHeight: 'var(--yazi-buyuk-baslik-satir)',
  letterSpacing: 'var(--yazi-buyuk-baslik-harf)',
};

export const groupTitle: CSSProperties = {
  margin: 'var(--bosluk-2) var(--bosluk-1) 0',
  fontSize: 'var(--yazi-baslik-3-boyut)',
  fontWeight: 800,
};

export const stack: CSSProperties = {
  display: 'flex',
  flexDirection: 'column',
  gap: 'var(--bosluk-2)',
};

export function Phone({ children }: { children: ReactNode }) {
  return <div style={phone}>{children}</div>;
}
