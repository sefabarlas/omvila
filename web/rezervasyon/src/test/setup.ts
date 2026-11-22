import { cleanup, configure } from '@testing-library/react';
import { afterEach, vi } from 'vitest';

// Adımlar `lazy()` ile yüklenir; dosyadaki ilk test adımın modülünü CI'nın soğuk başlangıcında dönüştürürken
// ilk ekran 1 sn'yi aşabiliyor (BookingFlowD5 "dolu gün" testi, findByRole 1712 ms'de düştü; T-113).
// findBy* beklemesi salon panelindeki gibi 5 sn. Beklentiler değişmez, yalnız bekleme süresi uzar.
configure({ asyncUtilTimeout: 5000 });

afterEach(() => {
  cleanup();
  vi.unstubAllGlobals();
  vi.restoreAllMocks();
  // Sahte zamanlayıcı kullanan bir test beklentide düşerse kendi useRealTimers satırına ulaşmaz; sonraki
  // testlere sızmasın.
  vi.useRealTimers();
});
