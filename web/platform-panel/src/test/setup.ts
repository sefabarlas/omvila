import { configure } from '@testing-library/react';

// jsdom'da olmayan tarayıcı API'leri (tema ve yönlendirici kaydırması).
Object.defineProperty(window, 'matchMedia', {
  writable: true,
  value: (query: string) => ({
    matches: false,
    media: query,
    addEventListener: () => undefined,
    removeEventListener: () => undefined,
  }),
});
window.scrollTo = () => undefined;

// İlk ekran (rota ağacı, oturum, sorgular) CI'nın soğuk başlangıcında 1 sn'yi aşabiliyor; findBy* beklemesi 5 sn.
// Beklentiler değişmez, yalnız bekleme süresi uzar.
configure({ asyncUtilTimeout: 5000 });
