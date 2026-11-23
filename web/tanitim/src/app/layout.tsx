import '@omvila/design-tokens/fonts.css';
import '@omvila/design-tokens/marka.css';
import './global.css';
import type { Metadata, Viewport } from 'next';
import type { ReactNode } from 'react';
import { SiteFooter } from '@/components/SiteFooter';
import { SiteHeader } from '@/components/SiteHeader';
import { ThemeSync } from '@/components/ThemeSync';
import { siteUrl } from '@/lib/site';

export const metadata: Metadata = {
  metadataBase: new URL(siteUrl()),
  title: 'OMVILA',
  applicationName: 'OMVILA',
};

export const viewport: Viewport = {
  width: 'device-width',
  initialScale: 1,
  colorScheme: 'light dark',
};

// Koyu tema tokenları [data-theme='dark'] ile açılır (design-tokens). Kayıtlı tercih (Açık/Koyu; tema menüsü) yoksa
// sistem teması izlenir; ilk boyamadan önce çalışsın diye <head>'de eşzamanlı satır içi betik. Hidrasyondan sonrasını
// ThemeSync üstlenir.
const themeScript =
  'try{var p=localStorage.getItem("omvila.tema")}catch(e){}' +
  'document.documentElement.dataset.theme=p==="dark"||(p!=="light"&&' +
  'matchMedia("(prefers-color-scheme: dark)").matches)?"dark":"light"';

export default function RootLayout({ children }: { children: ReactNode }) {
  return (
    <html lang="tr" data-scroll-behavior="smooth" suppressHydrationWarning>
      <head>
        <script>{themeScript}</script>
      </head>
      <body>
        <ThemeSync />
        <SiteHeader />
        <main>{children}</main>
        <SiteFooter />
      </body>
    </html>
  );
}
