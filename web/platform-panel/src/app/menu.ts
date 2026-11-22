import type { AdminPermission } from './api';

// Kenar menüsü (platform-paneli/Main): PLATFORM ve DESTEK grupları. Henüz yapılmamış bölümler (onay kuyrukları,
// abonelikler, denetim kaydı, planlar) `available: false` ile gizli ve rotasızdır; ekranları gelince açılır (T-123).

export interface MenuEntry {
  key: string;
  label: string;
  to: string;
  permission: AdminPermission;
  available: boolean;
  group: 'platform' | 'destek';
}

export const menuEntries: readonly MenuEntry[] = [
  {
    key: 'genel',
    label: 'Genel bakış',
    to: '/',
    permission: 'platform.brands.view',
    available: true,
    group: 'platform',
  },
  {
    key: 'markalar',
    label: 'Markalar',
    to: '/markalar',
    permission: 'platform.brands.view',
    available: true,
    group: 'platform',
  },
  {
    key: 'onay',
    label: 'Onay kuyrukları',
    to: '/onay',
    permission: 'platform.payments.match',
    available: false,
    group: 'platform',
  },
  {
    key: 'abonelikler',
    label: 'Abonelikler',
    to: '/abonelikler',
    permission: 'platform.brands.view',
    available: false,
    group: 'platform',
  },
  {
    key: 'denetim',
    label: 'Denetim kaydı',
    to: '/denetim',
    permission: 'platform.audit.view',
    available: false,
    group: 'destek',
  },
  {
    key: 'planlar',
    label: 'Planlar ve fiyat',
    to: '/planlar',
    permission: 'platform.plans.view',
    available: false,
    group: 'destek',
  },
];

/** Kullanıcının görebileceği öğeler; `ignoreAvailability` henüz yapılmamış bölümleri de verir (test). */
export function visibleMenu(
  can: (permission: AdminPermission) => boolean,
  ignoreAvailability = false,
): MenuEntry[] {
  return menuEntries.filter(
    (entry) => (ignoreAvailability || entry.available) && can(entry.permission),
  );
}

/** Yoldan etkin öğe ("/" yalnız tam eşleşmede; /markalar/… "Markalar"). */
export function activeMenuKey(pathname: string): string {
  const match = menuEntries.find(
    (entry) => entry.to !== '/' && (pathname === entry.to || pathname.startsWith(`${entry.to}/`)),
  );
  return match?.key ?? (pathname === '/' ? 'genel' : '');
}
