import type { NextConfig } from 'next';

// Çalışma alanı paketleri TypeScript kaynağıyla dışa aktarılır (derleme adımı yok); Next onları derler.
const config: NextConfig = {
  reactStrictMode: true,
  poweredByHeader: false,
  // Geliştirme rozeti ekran görüntülerine girmesin (e2e/ekranlar.e2e.ts).
  devIndicators: false,
  transpilePackages: ['@omvila/api-client-ts', '@omvila/design-tokens', '@omvila/ui-web'],
};

export default config;
