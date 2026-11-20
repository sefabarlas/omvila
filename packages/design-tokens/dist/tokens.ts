// Üretildi: pnpm --filter @omvila/design-tokens build. Elle düzenlemeyin.
// Token adları CSS değişkeni adlarıdır: var(--<ad>).

export const markaTokens = {
  renk: ['zemin', 'zemin-yukseltilmis', 'zemin-ikincil', 'zemin-marka', 'zemin-marka-yumusak', 'zemin-vurgu-yumusak', 'metin', 'metin-ikincil', 'metin-marka', 'metin-ters', 'marka', 'marka-hover', 'marka-ustu', 'vurgu', 'vurgu-ustu', 'cizgi', 'cizgi-guclu', 'odak', 'basari', 'uyari', 'hata', 'bilgi', 'petrol-50', 'petrol-100', 'petrol-200', 'petrol-300', 'petrol-400', 'petrol-500', 'petrol-600', 'petrol-700', 'petrol-800', 'petrol-900', 'petrol-950', 'kum-50', 'kum-100', 'kum-200', 'kum-300', 'kum-400', 'kum-500', 'kum-600', 'kum-700', 'kum-800', 'kum-900', 'mercan-50', 'mercan-100', 'mercan-200', 'mercan-300', 'mercan-400', 'mercan-500', 'mercan-600', 'mercan-700', 'mercan-800', 'mercan-900', 'gri-0', 'gri-50', 'gri-100', 'gri-200', 'gri-300', 'gri-400', 'gri-500', 'gri-600', 'gri-700', 'gri-800', 'gri-900', 'basari-100', 'basari-600', 'basari-800', 'uyari-100', 'uyari-400', 'uyari-700', 'hata-100', 'hata-600', 'hata-800', 'bilgi-100', 'bilgi-600', 'bilgi-800'],
  golge: ['golge-sm', 'golge-md', 'golge-lg'],
  bosluk: ['bosluk-1', 'bosluk-2', 'bosluk-3', 'bosluk-4', 'bosluk-5', 'bosluk-6', 'bosluk-8', 'bosluk-10', 'bosluk-12', 'bosluk-16', 'bosluk-20'],
  kose: ['kose-xs', 'kose-sm', 'kose-md', 'kose-lg', 'kose-xl', 'kose-tam'],
  boyut: [],
  yazi: ['display', 'baslik-1', 'baslik-2', 'baslik-3', 'baslik-4', 'govde-buyuk', 'govde', 'govde-kucuk', 'etiket', 'aciklama', 'ust-etiket', 'rakam'],
} as const;

export type MarkaColorToken = (typeof markaTokens.renk)[number];

export const panelTokens = {
  renk: ['zemin', 'zemin-yukseltilmis', 'zemin-ikincil', 'zemin-marka', 'zemin-marka-yumusak', 'zemin-vurgu-yumusak', 'metin-vurgu', 'metin', 'metin-ikincil', 'metin-marka', 'metin-ters', 'marka', 'marka-hover', 'marka-ustu', 'vurgu', 'vurgu-ustu', 'cizgi', 'cizgi-guclu', 'odak', 'basari', 'uyari', 'hata', 'bilgi', 'blok-marka', 'blok-vurgu', 'blok-kum', 'blok-kum-metin', 'zemin-hover', 'zemin-secili', 'zemin-devre-disi', 'metin-devre-disi', 'sidebar-zemin', 'sidebar-metin', 'sidebar-metin-aktif', 'sidebar-hover', 'sidebar-secili', 'sidebar-cizgi', 'basari-zemin', 'uyari-zemin', 'hata-zemin', 'bilgi-zemin', 'odak-sidebar', 'ust-katman', 'durum-onay-zemin', 'durum-onay-metin', 'durum-onay-nokta', 'durum-kapora-zemin', 'durum-kapora-metin', 'durum-kapora-nokta', 'durum-onaylandi-zemin', 'durum-onaylandi-metin', 'durum-onaylandi-nokta', 'durum-geldi-zemin', 'durum-geldi-metin', 'durum-geldi-nokta', 'durum-tamamlandi-zemin', 'durum-tamamlandi-metin', 'durum-tamamlandi-nokta', 'durum-gelmedi-zemin', 'durum-gelmedi-metin', 'durum-gelmedi-nokta', 'durum-iptal-zemin', 'durum-iptal-metin', 'durum-iptal-nokta', 'avatar-1-zemin', 'avatar-1-metin', 'avatar-2-zemin', 'avatar-2-metin', 'avatar-3-zemin', 'avatar-3-metin', 'sidebar-ikon-aktif', 'blok-vurgu-metin', 'petrol-50', 'petrol-100', 'petrol-200', 'petrol-300', 'petrol-400', 'petrol-500', 'petrol-600', 'petrol-700', 'petrol-800', 'petrol-900', 'petrol-950', 'kum-50', 'kum-100', 'kum-200', 'kum-300', 'kum-400', 'kum-500', 'kum-600', 'kum-700', 'kum-800', 'kum-900', 'mercan-50', 'mercan-100', 'mercan-200', 'mercan-300', 'mercan-400', 'mercan-500', 'mercan-600', 'mercan-700', 'mercan-800', 'mercan-900', 'gri-0', 'gri-50', 'gri-100', 'gri-200', 'gri-300', 'gri-400', 'gri-500', 'gri-600', 'gri-700', 'gri-800', 'gri-900', 'basari-100', 'basari-600', 'basari-800', 'uyari-100', 'uyari-400', 'uyari-700', 'hata-100', 'hata-600', 'hata-800', 'bilgi-100', 'bilgi-600', 'bilgi-800'],
  golge: ['golge-sm', 'golge-md', 'golge-lg', 'odak-halkasi'],
  bosluk: ['bosluk-0-5', 'bosluk-1', 'bosluk-1-5', 'bosluk-2', 'bosluk-3', 'bosluk-4', 'bosluk-5', 'bosluk-6', 'bosluk-8', 'bosluk-12'],
  kose: ['kose-xs', 'kose-sm', 'kose-md', 'kose-lg', 'kose-xl', 'kose-tam'],
  boyut: ['kontrol-sm', 'kontrol-md', 'kontrol-lg', 'sidebar-genislik', 'ustbar-yukseklik', 'tablo-satir', 'takvim-saat', 'icerik-genislik'],
  yazi: ['sayfa-basligi', 'bolum-basligi', 'kart-basligi', 'metrik', 'govde', 'govde-guclu', 'kucuk', 'etiket', 'aciklama', 'ust-etiket', 'rakam'],
} as const;

export type PanelColorToken = (typeof panelTokens.renk)[number];

export const mobilTokens = {
  renk: ['zemin', 'zemin-yukseltilmis', 'zemin-ikincil', 'zemin-marka', 'zemin-marka-yumusak', 'zemin-vurgu-yumusak', 'metin-vurgu', 'metin', 'metin-ikincil', 'metin-marka', 'metin-ters', 'marka', 'marka-hover', 'marka-ustu', 'vurgu', 'vurgu-ustu', 'cizgi', 'cizgi-guclu', 'odak', 'basari', 'uyari', 'hata', 'bilgi', 'zemin-hover', 'zemin-secili', 'zemin-devre-disi', 'metin-devre-disi', 'basari-zemin', 'uyari-zemin', 'hata-zemin', 'bilgi-zemin', 'ust-katman', 'durum-onay-zemin', 'durum-onay-metin', 'durum-onay-nokta', 'durum-kapora-zemin', 'durum-kapora-metin', 'durum-kapora-nokta', 'durum-onaylandi-zemin', 'durum-onaylandi-metin', 'durum-onaylandi-nokta', 'durum-geldi-zemin', 'durum-geldi-metin', 'durum-geldi-nokta', 'durum-tamamlandi-zemin', 'durum-tamamlandi-metin', 'durum-tamamlandi-nokta', 'durum-gelmedi-zemin', 'durum-gelmedi-metin', 'durum-gelmedi-nokta', 'durum-iptal-zemin', 'durum-iptal-metin', 'durum-iptal-nokta', 'tabbar-zemin', 'tabbar-pasif', 'tabbar-aktif', 'tabbar-aktif-zemin', 'blok-marka', 'blok-vurgu', 'blok-kum', 'blok-kum-metin', 'iskelet', 'tutamak', 'yildiz', 'avatar-1-zemin', 'avatar-1-metin', 'avatar-2-zemin', 'avatar-2-metin', 'avatar-3-zemin', 'avatar-3-metin', 'sidebar-ikon-aktif', 'blok-vurgu-metin', 'petrol-50', 'petrol-100', 'petrol-200', 'petrol-300', 'petrol-400', 'petrol-500', 'petrol-600', 'petrol-700', 'petrol-800', 'petrol-900', 'petrol-950', 'kum-50', 'kum-100', 'kum-200', 'kum-300', 'kum-400', 'kum-500', 'kum-600', 'kum-700', 'kum-800', 'kum-900', 'mercan-50', 'mercan-100', 'mercan-200', 'mercan-300', 'mercan-400', 'mercan-500', 'mercan-600', 'mercan-700', 'mercan-800', 'mercan-900', 'gri-0', 'gri-50', 'gri-100', 'gri-200', 'gri-300', 'gri-400', 'gri-500', 'gri-600', 'gri-700', 'gri-800', 'gri-900', 'basari-100', 'basari-600', 'basari-800', 'uyari-100', 'uyari-400', 'uyari-700', 'hata-100', 'hata-600', 'hata-800', 'bilgi-100', 'bilgi-600', 'bilgi-800'],
  golge: ['golge-sm', 'golge-md', 'golge-lg', 'odak-halkasi'],
  bosluk: ['bosluk-1', 'bosluk-2', 'bosluk-3', 'bosluk-4', 'bosluk-5', 'bosluk-6', 'bosluk-8', 'bosluk-12'],
  kose: ['kose-xs', 'kose-sm', 'kose-md', 'kose-lg', 'kose-xl', 'kose-tam'],
  boyut: ['dokunma-min', 'kontrol-sm', 'kontrol-md', 'kontrol-lg', 'ust-bar', 'tabbar', 'saat-dilimi', 'ekran-genislik'],
  yazi: ['buyuk-baslik', 'baslik-1', 'baslik-2', 'baslik-3', 'ust-bar-basligi', 'metrik', 'govde', 'govde-guclu', 'ikincil', 'etiket', 'etiket-kucuk', 'aciklama', 'sekme', 'rakam'],
} as const;

export type MobilColorToken = (typeof mobilTokens.renk)[number];
