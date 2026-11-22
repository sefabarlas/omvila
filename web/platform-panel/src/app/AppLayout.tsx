import { Sidebar } from '@omvila/ui-web/panel';
import type { SidebarGroup } from '@omvila/ui-web/panel';
import { Outlet, useLocation, useNavigate } from '@tanstack/react-router';
import { CreditCard, History, Inbox, LayoutGrid, Store, Tag } from 'lucide-react';
import type { ReactNode } from 'react';
import styles from './AppLayout.module.css';
import { activeMenuKey, visibleMenu } from './menu';
import { roleLabels, useSession } from './session';

// Ekran tasarımlarındaki ikonlar (Lucide; platform-paneli/Main).
const icons: Record<string, ReactNode> = {
  genel: <LayoutGrid size={18} />,
  markalar: <Store size={18} />,
  onay: <Inbox size={18} />,
  abonelikler: <CreditCard size={18} />,
  denetim: <History size={18} />,
  planlar: <Tag size={18} />,
};

/** Oturumlu panel düzeni: izinle süzülmüş kenar menü ve sayfa içeriği. */
export function AppLayout() {
  const { session, can } = useSession();
  const navigate = useNavigate();
  const { pathname } = useLocation();
  const entries = visibleMenu(can);
  const groups: SidebarGroup[] = (
    [
      ['platform', 'Platform'],
      ['destek', 'Destek'],
    ] as const
  )
    .map(([key, title]) => ({
      title,
      items: entries
        .filter((e) => e.group === key)
        .map((e) => ({
          key: e.key,
          label: e.label,
          href: e.to,
          icon: icons[e.key],
          ...(e.key === 'onay' ? { count: session.pendingApprovalCount } : {}),
        })),
    }))
    .filter((group) => group.items.length > 0);

  return (
    <div className={styles.shell}>
      <a className={styles.skip} href="#icerik">
        İçeriğe geç
      </a>
      <div className={styles.sidebar}>
        <Sidebar
          groups={groups}
          active={activeMenuKey(pathname)}
          onNavigate={(item) => void navigate({ to: item.href })}
          user={{ name: session.user.fullName, role: `${roleLabels[session.role]} · OMVILA` }}
        />
      </div>
      <main id="icerik" className={styles.main}>
        <Outlet />
      </main>
    </div>
  );
}
