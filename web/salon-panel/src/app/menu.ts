// Kenar menüsü (Sidebar README: sıra ve gruplar). Öğe izinle gösterilir (roller-yetkiler.md § Menü); henüz yapılmamış
// bölümler Dilim 1'de gizlidir (acik-sorular.md, Dilim 1 varsayımı 1). `available` dilimi gelince açılır.

export interface MenuEntry {
  key: string;
  label: string;
  to: string;
  /** Görmek için gereken izin; boş = her oturum. */
  permission?: string;
  available: boolean;
  group: 'isletme' | 'yonetim';
}

export const menuEntries: readonly MenuEntry[] = [
  { key: 'genel', label: 'Genel bakış', to: '/', available: true, group: 'isletme' },
  {
    key: 'takvim',
    label: 'Takvim',
    to: '/takvim',
    permission: 'calendar.view',
    available: false,
    group: 'isletme',
  },
  {
    key: 'musteriler',
    label: 'Müşteriler',
    to: '/musteriler',
    permission: 'customer.view',
    available: false,
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

/** Kullanıcının görebileceği öğeler. */
export function visibleMenu(can: (permission: string) => boolean): MenuEntry[] {
  return menuEntries.filter(
    (entry) => entry.available && (entry.permission === undefined || can(entry.permission)),
  );
}

/** Yoldan etkin öğe ("/" yalnız tam eşleşmede). */
export function activeMenuKey(pathname: string): string {
  const match = menuEntries.find(
    (entry) => entry.to !== '/' && (pathname === entry.to || pathname.startsWith(`${entry.to}/`)),
  );
  return match?.key ?? (pathname === '/' ? 'genel' : '');
}
