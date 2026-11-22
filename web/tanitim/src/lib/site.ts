import siteJson from '@content/site.json';

// Adresler derlemede ortamdan okunur (statik çıktıya gömülür; ortam başına ayrı derleme):
//   NEXT_PUBLIC_SITE_URL   sitenin kendi kökü (canonical, Open Graph, sitemap)
//   NEXT_PUBLIC_PANEL_URL  salon panelinin kökü; "Giriş yap" → /giris, "Ücretsiz dene" → /kayit
//   NEXT_PUBLIC_NOINDEX    "true" ise arama motorlarına kapalı (test ortamı)
// Yoksa yerel geliştirme adresleri (salon paneli Vite :5173).

function trimSlash(url: string): string {
  return url.replace(/\/+$/, '');
}

export function siteUrl(): string {
  return trimSlash(process.env.NEXT_PUBLIC_SITE_URL ?? 'http://localhost:3001');
}

export function panelUrl(): string {
  return trimSlash(process.env.NEXT_PUBLIC_PANEL_URL ?? 'http://localhost:5173');
}

export function signInUrl(): string {
  return `${panelUrl()}/giris`;
}

export function signUpUrl(): string {
  return `${panelUrl()}/kayit`;
}

export function noIndex(): boolean {
  return process.env.NEXT_PUBLIC_NOINDEX === 'true';
}

export interface SiteContent {
  copyrightYear: number;
  reminderOffsetsMinutes: number[];
  waitlistOfferHoldMinutes: number;
}

export const site: SiteContent = siteJson;

const words = ['', 'bir', 'iki', 'üç', 'dört', 'beş', 'altı', 'yedi', 'sekiz', 'dokuz', 'on'];

function count(n: number, unit: string): string {
  return `${words[n] ?? String(n)} ${unit}`;
}

/** 1440 → "bir gün", 120 → "iki saat", 30 → "30 dakika". */
export function offsetText(minutes: number): string {
  if (minutes % 1440 === 0) return count(minutes / 1440, 'gün');
  if (minutes % 60 === 0) return count(minutes / 60, 'saat');
  return `${String(minutes)} dakika`;
}

/** [1440, 120] → "bir gün ve iki saat"; tek değerde yalnız o. */
export function reminderText(offsets: number[] = site.reminderOffsetsMinutes): string {
  const parts = offsets.map(offsetText);
  if (parts.length <= 1) return parts.join('');
  return `${parts.slice(0, -1).join(', ')} ve ${parts.at(-1) ?? ''}`;
}
