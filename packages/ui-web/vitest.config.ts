import { defineConfig } from 'vitest/config';

export default defineConfig({
  test: {
    environment: 'jsdom',
    // Klavyeyle sürükleme gibi çok adımlı testler CI'da 5 sn'yi aşabiliyor; beklentiler değişmez, yalnız süre.
    testTimeout: 15_000,
    css: { modules: { classNameStrategy: 'non-scoped' } },
  },
});
