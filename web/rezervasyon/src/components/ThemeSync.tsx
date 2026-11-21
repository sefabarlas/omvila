'use client';

import { useLayoutEffect } from 'react';

const darkQuery = '(prefers-color-scheme: dark)';

/**
 * `data-theme`'i sistem temasıyla eşler. Satır içi betik ilk boyamayı karşılar; React 19 <html>'i hidrasyonda
 * veya istemcide baştan çizerken (404 sayfaları hata kabuğuyla gelir) sunucuda olmayan özniteliği siler.
 * Bu bileşen değeri boyamadan önce geri yazar ve tema değişikliğini izler.
 */
export function ThemeSync() {
  useLayoutEffect(() => {
    const media = window.matchMedia(darkQuery);
    const root = document.documentElement;
    const apply = () => {
      root.dataset.theme = media.matches ? 'dark' : 'light';
    };
    apply();
    media.addEventListener('change', apply);
    return () => {
      media.removeEventListener('change', apply);
    };
  }, []);
  return null;
}
