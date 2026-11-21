import { fileURLToPath } from 'node:url';
import type { NextConfig } from 'next';

// Çalışma alanı paketleri TypeScript kaynağıyla dışa aktarılır (derleme adımı yok); Next onları derler.
const config: NextConfig = {
  reactStrictMode: true,
  poweredByHeader: false,
  // Geliştirme rozeti ekran görüntülerine girmesin (e2e/ekranlar.e2e.ts).
  devIndicators: false,
  transpilePackages: ['@omvila/api-client-ts', '@omvila/design-tokens', '@omvila/ui-web'],
  // Kapsayıcı imajı (web/rezervasyon/Dockerfile) için kendi kendine yeten sunucu. `next dev` etkilenmez.
  output: 'standalone',
  // İzleme kökü depo kökü (pnpm çalışma alanı): standalone çıktısı web/rezervasyon/server.js olur.
  outputFileTracingRoot: fileURLToPath(new URL('../..', import.meta.url)),
};

export default config;
