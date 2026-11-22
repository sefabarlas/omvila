import type { NextConfig } from 'next';

// Statik site (web/tanitim/AGENTS.md): `next build` → out/. Sunucu yok; Caddy dosyaları sunar (Dockerfile).
// Çalışma alanı paketleri TypeScript kaynağıyla dışa aktarılır; Next onları derler.
const config: NextConfig = {
  output: 'export',
  // /fiyatlar → out/fiyatlar/index.html: statik sunucu dizinin index.html'ini kendiliğinden bulur.
  trailingSlash: true,
  reactStrictMode: true,
  poweredByHeader: false,
  // Geliştirme rozeti ekran görüntülerine girmesin (e2e/ekranlar.e2e.ts).
  devIndicators: false,
  transpilePackages: ['@omvila/api-client-ts', '@omvila/design-tokens', '@omvila/ui-web'],
};

export default config;
