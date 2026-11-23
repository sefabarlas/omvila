import emblemUrl from '@omvila/design-tokens/assets/logos/omvila-amblem-ters.svg';
import logoUrl from '@omvila/design-tokens/assets/logos/omvila-logo-yatay-ters.svg';
import { ChevronDown, ChevronUp, LogOut, Monitor, Moon, Store, Sun } from 'lucide-react';
import { useEffect, useId, useRef, useState } from 'react';
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

export type ThemePreference = 'system' | 'light' | 'dark';

const themeOptions: readonly { value: ThemePreference; label: string; icon: ReactNode }[] = [
  { value: 'system', label: 'Sistem', icon: <Monitor size={14} aria-hidden="true" /> },
  { value: 'light', label: 'Açık', icon: <Sun size={14} aria-hidden="true" /> },
  { value: 'dark', label: 'Koyu', icon: <Moon size={14} aria-hidden="true" /> },
];

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
  /** Verilirse kullanıcı menüsünde tema seçici çıkar (Sistem / Açık / Koyu). */
  theme?: { preference: ThemePreference; onChange: (preference: ThemePreference) => void };
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
  theme,
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
      {user ? (
        <UserMenu user={user} {...(onSignOut ? { onSignOut } : {})} {...(theme ? { theme } : {})} />
      ) : null}
    </nav>
  );
}

interface UserMenuProps {
  user: { name: string; role: string };
  onSignOut?: () => void;
  theme?: SidebarProps['theme'];
}

/** Kullanıcı satırı; menü varsa tıklayınca tema seçici ve "Çıkış yap" açılır (Esc ve dışarı tıklama kapatır). */
function UserMenu({ user, onSignOut, theme }: UserMenuProps) {
  const [open, setOpen] = useState(false);
  const rootRef = useRef<HTMLDivElement>(null);
  const triggerRef = useRef<HTMLButtonElement>(null);
  const menuId = useId();
  const themeName = useId();
  const hasMenu = onSignOut !== undefined || theme !== undefined;

  useEffect(() => {
    if (!open) {
      return;
    }

    const onPointerDown = (event: PointerEvent) => {
      if (!rootRef.current?.contains(event.target as Node)) {
        setOpen(false);
      }
    };
    const onKeyDown = (event: KeyboardEvent) => {
      if (event.key === 'Escape') {
        setOpen(false);
        triggerRef.current?.focus();
      }
    };
    document.addEventListener('pointerdown', onPointerDown);
    document.addEventListener('keydown', onKeyDown);
    return () => {
      document.removeEventListener('pointerdown', onPointerDown);
      document.removeEventListener('keydown', onKeyDown);
    };
  }, [open]);

  const identity = (
    <>
      <Avatar name={user.name} size="sm" />
      <span className={styles.userText}>
        <span className={styles.userName}>{user.name}</span>
        <span className={styles.userRole}>{user.role}</span>
      </span>
    </>
  );

  if (!hasMenu) {
    return <div className={styles.user}>{identity}</div>;
  }

  return (
    <div className={styles.userMenu} ref={rootRef}>
      {open ? (
        <div id={menuId} className={styles.menu} role="group" aria-label="Hesap">
          {theme ? (
            <fieldset className={styles.themeGroup}>
              <legend className={styles.menuLabel}>Tema</legend>
              <div className={styles.segmented}>
                {themeOptions.map((option) => (
                  <label key={option.value} className={styles.segment}>
                    <input
                      type="radio"
                      name={themeName}
                      value={option.value}
                      checked={theme.preference === option.value}
                      onChange={() => {
                        theme.onChange(option.value);
                      }}
                    />
                    {option.icon}
                    <span>{option.label}</span>
                  </label>
                ))}
              </div>
            </fieldset>
          ) : null}
          {onSignOut ? (
            <button type="button" className={styles.menuItem} onClick={onSignOut}>
              <LogOut size={16} aria-hidden="true" />
              Çıkış yap
            </button>
          ) : null}
        </div>
      ) : null}
      <button
        ref={triggerRef}
        type="button"
        className={[styles.user, styles.userButton].join(' ')}
        aria-expanded={open}
        aria-controls={open ? menuId : undefined}
        aria-label={`${user.name}, hesap menüsü`}
        title={user.name}
        onClick={() => {
          setOpen((value) => !value);
        }}
      >
        {identity}
        <span className={styles.userChevron} aria-hidden="true">
          <ChevronUp size={16} />
        </span>
      </button>
    </div>
  );
}
