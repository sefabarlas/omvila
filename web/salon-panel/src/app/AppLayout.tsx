import { Sidebar } from '@omvila/ui-web/panel';
import type { SidebarGroup } from '@omvila/ui-web/panel';
import { useQueryClient } from '@tanstack/react-query';
import { Outlet, useLocation, useNavigate } from '@tanstack/react-router';
import { useState } from 'react';
import { useApi } from './api-context';
import { auth } from './auth';
import { useTheme } from './theme';
import { BranchContext } from './branch';
import styles from './AppLayout.module.css';
import { menuIcons } from './menu-icons';
import { activeMenuKey, isPersonalView, visibleMenu } from './menu';
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
  const api = useApi();
  const queryClient = useQueryClient();
  const theme = useTheme();
  const { pathname } = useLocation();
  const entries = visibleMenu(can);
  // Çıkış: sunucu yenileme çerezini siler ve zinciri iptal eder; istek başarısız olsa da yerel oturum kapanır.
  const signOut = async () => {
    try {
      await api.POST('/api/v1/manage/auth/logout');
    } catch {
      // Ağ hatasında da yerel oturum kapanır.
    }
    auth.setToken(null);
    queryClient.clear();
    await navigate({ to: '/giris' });
  };
  const personal = isPersonalView(can);
  const groups: SidebarGroup[] = (
    [
      ['benim', 'Benim'],
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
            active={activeMenuKey(pathname, personal)}
            onNavigate={(item) => void navigate({ to: item.href })}
            branches={session.branches}
            {...(branchId ? { branchId } : {})}
            onBranchChange={setBranchId}
            user={{
              name: session.user.fullName || session.brand.name,
              role: roleLabels[session.membership.role] ?? '',
            }}
            onSignOut={() => void signOut()}
            theme={{ dark: theme.dark, onToggle: theme.toggle }}
          />
        </div>
        <main id="icerik" className={styles.main}>
          <Outlet />
        </main>
      </div>
    </BranchContext.Provider>
  );
}
