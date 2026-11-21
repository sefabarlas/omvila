import { fileURLToPath, URL } from 'node:url';
import react from '@vitejs/plugin-react';
import { defineConfig } from 'vitest/config';

// Sunucu bileşenleri testte doğrudan çağrılır (`await Page({ params })`) ve sonucu jsdom'da çizilir.
export default defineConfig({
  plugins: [react()],
  resolve: { alias: { '@': fileURLToPath(new URL('./src', import.meta.url)) } },
  test: {
    environment: 'jsdom',
    // Uçtan uca akış testleri (çok adım) CI'da 5 sn'yi aşabiliyor; beklentiler değişmez, yalnız süre.
    testTimeout: 15_000,
    setupFiles: ['./src/test/setup.ts'],
    css: { modules: { classNameStrategy: 'non-scoped' } },
  },
});
