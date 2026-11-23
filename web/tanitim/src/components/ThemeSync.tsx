'use client';

import { useLayoutEffect } from 'react';
import { applyTheme, darkMediaQuery } from '@/lib/theme';

/**
 * `data-theme`'i kayıtlı tercihe (yoksa sistem temasına) göre yazar. Satır içi betik ilk boyamayı karşılar; React 19
 * <html>'i hidrasyonda veya istemcide baştan çizerken (404 sayfaları hata kabuğuyla gelir) sunucuda olmayan özniteliği
 * siler. Bu bileşen değeri boyamadan önce geri yazar ve sistem teması değişikliğini izler.
 */
export function ThemeSync() {
  useLayoutEffect(() => {
    const apply = () => {
      applyTheme();
    };
    apply();
    if (typeof window.matchMedia !== 'function') {
      return;
    }

    const media = window.matchMedia(darkMediaQuery);
    media.addEventListener('change', apply);
    return () => {
      media.removeEventListener('change', apply);
    };
  }, []);
  return null;
}
