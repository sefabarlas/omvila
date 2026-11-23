import emblemUrl from '@omvila/design-tokens/assets/logos/omvila-amblem-ters.svg';
import logoUrl from '@omvila/design-tokens/assets/logos/omvila-logo-yatay-ters.svg';
import { ChevronDown, LogOut, Store } from 'lucide-react';
import { useId } from 'react';
import type { MouseEvent, ReactNode } from 'react';
import { Avatar } from './Avatar';
import styles from './Sidebar.module.css';

export interface SidebarItem {
  key: string;
  label: string;
  icon: ReactNode;
  href: string;
  /** Yalnız eylem bekleyen şeyler (onay bekleyen randevu); 0 veya boşken gösterilmez. */
  count?: number;
}

export interface SidebarGroup {
  /** "YÖNETİM"; ilk grup başlıksız olabilir. */
  title?: string;
  items: readonly SidebarItem[];
}

export interface SidebarBranch {
  id: string;
  name: string;
}

export interface SidebarProps {
  groups: readonly SidebarGroup[];
  /** Etkin öğenin anahtarı (aria-current="page"). */
  active: string;
  /** Uygulama yönlendiricisi: verilirse bağlantı tıklaması ona devredilir. */
  onNavigate?: (item: SidebarItem) => void;
  /** Şube seçici; tek şubeli işletmede gizlenir. */
  branches?: readonly SidebarBranch[];
  branchId?: string;
  onBranchChange?: (branchId: string) => void;
  user?: { name: string; role: string };
  /** Verilirse kullanıcı satırında "Çıkış yap" butonu çıkar. */
  onSignOut?: () => void;
  /** Menü bölgesinin adı. */
  label?: string;
}

/** Panelin ana menüsü. 1199 px altında ikon şeridine daralır; daralmış öğe adı ipucu ve ekran okuyucuda kalır. */
export function Sidebar({
  groups,
  active,
  onNavigate,
  branches = [],
  branchId,
  onBranchChange,
  user,
  onSignOut,
  label = 'Ana menü',
}: SidebarProps) {
  const branchSelectId = useId();
  const onClick = (event: MouseEvent<HTMLAnchorElement>, item: SidebarItem) => {
    if (!onNavigate || event.metaKey || event.ctrlKey || event.shiftKey || event.button !== 0) {
      return;
    }

    event.preventDefault();
    onNavigate(item);
  };

  return (
    <nav className={styles.sidebar} aria-label={label}>
      <div className={styles.brand}>
        <img className={styles.logo} src={logoUrl} alt="OMVILA" />
        <img className={styles.emblem} src={emblemUrl} alt="OMVILA" />
      </div>
      {branches.length > 1 ? (
        <div className={styles.branch}>
          <label htmlFor={branchSelectId} className={styles.visuallyHidden}>
            Şube
          </label>
          <span className={styles.branchIcon} aria-hidden="true">
            <Store size={16} />
          </span>
          <select
            id={branchSelectId}
            className={styles.branchSelect}
            value={branchId}
            onChange={(event) => onBranchChange?.(event.target.value)}
          >
            {branches.map((branch) => (
              <option key={branch.id} value={branch.id}>
                {branch.name}
              </option>
            ))}
          </select>
          <span className={styles.branchChevron} aria-hidden="true">
            <ChevronDown size={16} />
          </span>
        </div>
      ) : null}
      <div className={styles.groups}>
        {groups.map((group, index) => (
          <div key={group.title ?? index}>
            {group.title ? <div className={styles.title}>{group.title}</div> : null}
            <ul className={styles.group}>
              {group.items.map((item) => (
                <li key={item.key}>
                  <a
                    href={item.href}
                    className={styles.item}
                    title={item.label}
                    aria-current={item.key === active ? 'page' : undefined}
                    onClick={(event) => {
                      onClick(event, item);
                    }}
                  >
                    <span className={styles.icon} aria-hidden="true">
                      {item.icon}
                    </span>
                    <span className={styles.label}>{item.label}</span>
                    {item.count ? <span className={styles.count}>{item.count}</span> : null}
                  </a>
                </li>
              ))}
            </ul>
          </div>
        ))}
      </div>
      {user || onSignOut ? (
        <div className={styles.user}>
          {user ? (
            <>
              <Avatar name={user.name} size="sm" />
              <div className={styles.userText}>
                <span className={styles.userName}>{user.name}</span>
                <span className={styles.userRole}>{user.role}</span>
              </div>
            </>
          ) : null}
          {onSignOut ? (
            <button
              type="button"
              className={styles.signOut}
              aria-label="Çıkış yap"
              title="Çıkış yap"
              onClick={onSignOut}
            >
              <LogOut size={16} aria-hidden="true" />
            </button>
          ) : null}
        </div>
      ) : null}
    </nav>
  );
}
