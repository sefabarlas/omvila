// Kenar menüsü (Sidebar README: sıra ve gruplar). Öğe izinle gösterilir (roller-yetkiler.md § Menü); henüz yapılmamış
// bölümler Dilim 1'de gizlidir (acik-sorular.md, Dilim 1 varsayımı 1). `available` dilimi gelince açılır.

export interface MenuEntry {
  key: string;
  label: string;
  to: string;
  /** Görmek için gereken izin; boş = her oturum. */
  permission?: string;
  available: boolean;
  group: 'isletme' | 'yonetim' | 'benim';
}

export const menuEntries: readonly MenuEntry[] = [
  { key: 'genel', label: 'Genel bakış', to: '/', available: true, group: 'isletme' },
  {
    key: 'takvim',
    label: 'Takvim',
    to: '/takvim',
    permission: 'calendar.view',
    available: true,
    group: 'isletme',
  },
  {
    key: 'musteriler',
    label: 'Müşteriler',
    to: '/musteriler',
    permission: 'customer.view',
    available: true,
    group: 'isletme',
  },
  {
    key: 'bekleme',
    label: 'Bekleme listesi',
    to: '/bekleme',
    permission: 'waitlist.manage',
    available: false,
    group: 'isletme',
  },
  {
    key: 'kasa',
    label: 'Kasa',
    to: '/kasa',
    permission: 'payment.collect',
    available: false,
    group: 'isletme',
  },
  {
    key: 'degerlendirmeler',
    label: 'Değerlendirmeler',
    to: '/degerlendirmeler',
    permission: 'review.view',
    available: false,
    group: 'isletme',
  },
  {
    key: 'hizmetler',
    label: 'Hizmetler',
    to: '/hizmetler',
    permission: 'service.manage',
    available: true,
    group: 'yonetim',
  },
  {
    key: 'personel',
    label: 'Personel',
    to: '/personel',
    permission: 'staff.manage',
    available: true,
    group: 'yonetim',
  },
  {
    key: 'paketler',
    label: 'Paketler',
    to: '/paketler',
    permission: 'package.manage',
    available: false,
    group: 'yonetim',
  },
  {
    key: 'raporlar',
    label: 'Raporlar ve prim',
    to: '/raporlar',
    permission: 'report.view',
    available: false,
    group: 'yonetim',
  },
  {
    key: 'ayarlar',
    label: 'Ayarlar',
    to: '/ayarlar',
    permission: 'settings.manage',
    available: true,
    group: 'yonetim',
  },
];

/**
 * Kişisel menü (salon-paneli/PersonelRolu, kabul kriteri 10): raporları göremeyen kullanıcı (personel) işletme
 * menüsü yerine "Benim" grubunu görür. "Takvimim" takvimi açar (sunucu yalnız kendi kolonunu döner); ana sayfa
 * ("/", girişten sonra) kişisel özettir ve menüde Takvimim altında sayılır (acik-sorular.md T-45, T-54). Müşteriler
 * D2'de açıldı (T-57); Kapıda sıra (D3) ve Primim (D6) sayfaları gelince açılır.
 */
export const personalEntries: readonly MenuEntry[] = [
  {
    key: 'takvimim',
    label: 'Takvimim',
    to: '/takvim',
    permission: 'calendar.view',
    available: true,
    group: 'benim',
  },
  {
    key: 'musteriler',
    label: 'Müşteriler',
    to: '/musteriler',
    permission: 'customer.view',
    available: true,
    group: 'benim',
  },
  {
    key: 'kapida',
    label: 'Kapıda sıra',
    to: '/kapida-sira',
    permission: 'walkin.manage',
    available: false,
    group: 'benim',
  },
  {
    key: 'primim',
    label: 'Primim',
    to: '/primim',
    permission: 'commission.view',
    available: false,
    group: 'benim',
  },
];

/** Raporları görmeyen kullanıcı kişisel menüyü görür (roller-yetkiler.md § Menü görünürlüğü). */
export function isPersonalView(can: (permission: string) => boolean): boolean {
  return !can('report.view');
}

/** Kullanıcının görebileceği öğeler; `ignoreAvailability` henüz yapılmamış bölümleri de verir (test, plan). */
export function visibleMenu(
  can: (permission: string) => boolean,
  ignoreAvailability = false,
): MenuEntry[] {
  const source = isPersonalView(can) ? personalEntries : menuEntries;
  return source.filter(
    (entry) =>
      (ignoreAvailability || entry.available) &&
      (entry.permission === undefined || can(entry.permission)),
  );
}

/** Yoldan etkin öğe ("/" yalnız tam eşleşmede; kişisel menüde "Takvimim"). */
export function activeMenuKey(pathname: string, personal = false): string {
  const source = personal ? personalEntries : menuEntries;
  const match = source.find(
    (entry) => entry.to !== '/' && (pathname === entry.to || pathname.startsWith(`${entry.to}/`)),
  );
  return match?.key ?? (pathname === '/' ? (personal ? 'takvimim' : 'genel') : '');
}
