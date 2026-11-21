import type { NextConfig } from 'next';

// Çalışma alanı paketleri TypeScript kaynağıyla dışa aktarılır (derleme adımı yok); Next onları derler.
const config: NextConfig = {
  reactStrictMode: true,
  poweredByHeader: false,
  transpilePackages: ['@omvila/api-client-ts', '@omvila/design-tokens'],
};

export default config;
