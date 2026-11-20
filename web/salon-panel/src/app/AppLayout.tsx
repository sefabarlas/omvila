import { Sidebar } from '@omvila/ui-web/panel';
import type { SidebarGroup } from '@omvila/ui-web/panel';
import { Outlet, useLocation, useNavigate } from '@tanstack/react-router';
import { useState } from 'react';
import { BranchContext } from './branch';
import styles from './AppLayout.module.css';
import { menuIcons } from './menu-icons';
import { activeMenuKey, visibleMenu } from './menu';
import { useSession } from './session';

const roleLabels: Record<string, string> = {
  Owner: 'İşletme sahibi',
  BranchManager: 'Şube yöneticisi',
  Staff: 'Personel',
};

/** Oturumlu panel düzeni: izinle süzülmüş kenar menü, abonelik şeridi, sayfa içeriği. */
export function AppLayout() {
  const { session, can } = useSession();
  const navigate = useNavigate();
  const { pathname } = useLocation();
  const entries = visibleMenu(can);
  const groups: SidebarGroup[] = (
    [
      ['isletme', 'İşletme'],
      ['yonetim', 'Yönetim'],
    ] as const
  )
    .map(([key, title]) => ({
      title,
      items: entries
        .filter((e) => e.group === key)
        .map((e) => ({ key: e.key, label: e.label, href: e.to, icon: menuIcons[e.key] })),
    }))
    .filter((group) => group.items.length > 0);
  const [branchId, setBranchId] = useState(session.branches[0]?.id);

  return (
    <BranchContext.Provider value={{ branchId, setBranchId }}>
      <div className={styles.shell}>
        <a className={styles.skip} href="#icerik">
          İçeriğe geç
        </a>
        <div className={styles.sidebar}>
          <Sidebar
            groups={groups}
            active={activeMenuKey(pathname)}
            onNavigate={(item) => void navigate({ to: item.href })}
            branches={session.branches}
            {...(branchId ? { branchId } : {})}
            onBranchChange={setBranchId}
            user={{
              name: session.user.fullName || session.brand.name,
              role: roleLabels[session.membership.role] ?? '',
            }}
          />
        </div>
        <main id="icerik" className={styles.main}>
          <Outlet />
        </main>
      </div>
    </BranchContext.Provider>
  );
}
