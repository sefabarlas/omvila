import { fileURLToPath, URL } from 'node:url';
import { tanstackRouter } from '@tanstack/router-plugin/vite';
import react from '@vitejs/plugin-react';
import { defineConfig } from 'vitest/config';

// Dosya tabanlı rotalar src/routes altında; üretilen ağaç src/routeTree.gen.ts (commit edilir, elle düzenlenmez).
export default defineConfig({
  plugins: [
    tanstackRouter({ target: 'react', autoCodeSplitting: true, quoteStyle: 'single' }),
    react(),
  ],
  resolve: { alias: { '@': fileURLToPath(new URL('./src', import.meta.url)) } },
  test: {
    environment: 'jsdom',
    // Çok adımlı form testleri (aşamalı hizmet) CI'da 5 sn'yi aşabiliyor; beklentiler değişmez, yalnız süre.
    testTimeout: 15_000,
    setupFiles: ['./src/test/setup.ts'],
    css: { modules: { classNameStrategy: 'non-scoped' } },
  },
});
