import { cleanup, configure } from '@testing-library/react';
import { afterEach, vi } from 'vitest';

// İlk ekran (akış, sorgular) CI'nın soğuk başlangıcında 1 sn'yi aşabiliyor; findBy* beklemesi 5 sn (salon panelindeki gibi).
// Beklentiler değişmez, yalnız bekleme süresi uzar.
configure({ asyncUtilTimeout: 5000 });

afterEach(() => {
  cleanup();
  vi.unstubAllGlobals();
  vi.restoreAllMocks();
});
