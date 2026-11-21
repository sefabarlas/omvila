import '@omvila/design-tokens/fonts.css';
import '@omvila/design-tokens/mobil.css';
import './global.css';
import type { Metadata, Viewport } from 'next';
import type { ReactNode } from 'react';
import { ThemeSync } from '@/components/ThemeSync';
import { siteUrl } from '@/lib/metadata';

export const metadata: Metadata = {
  metadataBase: new URL(siteUrl),
  title: 'OMVILA',
  applicationName: 'OMVILA',
};

export const viewport: Viewport = {
  width: 'device-width',
  initialScale: 1,
  colorScheme: 'light dark',
};

// Koyu tema tokenları [data-theme='dark'] ile açılır (design-tokens). Sayfa sistem temasını izler; ilk boyamadan
// önce çalışsın diye <head>'de eşzamanlı satır içi betik (dosya isteği yok, titreme yok). Metin sabit,
// kullanıcı verisi içermez; React metin kaçışı betiği bozmasın diye < > & ' kullanılmaz.
// Hidrasyondan sonrasını ThemeSync üstlenir.
const themeScript =
  'document.documentElement.dataset.theme=' +
  'matchMedia("(prefers-color-scheme: dark)").matches?"dark":"light"';

export default function RootLayout({ children }: { children: ReactNode }) {
  return (
    <html lang="tr" suppressHydrationWarning>
      <head>
        <script>{themeScript}</script>
      </head>
      <body>
        <ThemeSync />
        {children}
      </body>
    </html>
  );
}
