import emblemUrl from '@omvila/design-tokens/assets/logos/omvila-amblem-ters.svg';
import logoUrl from '@omvila/design-tokens/assets/logos/omvila-logo-yatay-ters.svg';
import { ChevronDown, ChevronsUpDown, LogOut, Monitor, Moon, Store, Sun } from 'lucide-react';
import { useEffect, useId, useRef, useState } from 'react';
import type { KeyboardEvent as ReactKeyboardEvent, MouseEvent, ReactNode } from 'react';
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

// Tasarım sistemi ProfileMenu: Açık · Koyu · Sistem.
const themeOptions: readonly { value: ThemePreference; label: string; icon: ReactNode }[] = [
  { value: 'light', label: 'Açık', icon: <Sun size={18} aria-hidden="true" /> },
  { value: 'dark', label: 'Koyu', icon: <Moon size={18} aria-hidden="true" /> },
  { value: 'system', label: 'Sistem', icon: <Monitor size={18} aria-hidden="true" /> },
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
  /** Verilirse profil menüsünde tema seçici çıkar (Açık / Koyu / Sistem). */
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

/**
 * Kullanıcı satırı ve profil menüsü (tasarım sistemi ProfileMenu): kimlik, tema, "Oturumu kapat".
 * Esc ve dışarı tıklama kapatır, odak açan düğmeye döner; ok tuşları öğeler arasında gezer.
 */
function UserMenu({ user, onSignOut, theme }: UserMenuProps) {
  const [open, setOpen] = useState(false);
  const rootRef = useRef<HTMLDivElement>(null);
  const menuRef = useRef<HTMLDivElement>(null);
  const triggerRef = useRef<HTMLButtonElement>(null);
  const menuId = useId();
  const themeLabelId = useId();
  const hasMenu = onSignOut !== undefined || theme !== undefined;

  useEffect(() => {
    if (!open) {
      return;
    }

    const items = menuRef.current?.querySelectorAll<HTMLButtonElement>('[role^="menuitem"]');
    const selected = menuRef.current?.querySelector<HTMLButtonElement>('[aria-checked="true"]');
    (selected ?? items?.[0])?.focus();
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

  const onMenuKeyDown = (event: ReactKeyboardEvent<HTMLDivElement>) => {
    const keys: Record<string, number> = {
      ArrowDown: 1,
      ArrowRight: 1,
      ArrowUp: -1,
      ArrowLeft: -1,
    };
    const step = keys[event.key];
    if (step === undefined || !menuRef.current) {
      return;
    }

    event.preventDefault();
    const items = Array.from(
      menuRef.current.querySelectorAll<HTMLButtonElement>('[role^="menuitem"]'),
    );
    const index = items.indexOf(document.activeElement as HTMLButtonElement);
    items[(index + step + items.length) % items.length]?.focus();
  };

  const avatar = <Avatar name={user.name} size="sm" />;
  const text = (
    <span className={styles.userText}>
      <span className={styles.userName}>{user.name}</span>
      <span className={styles.userRole}>{user.role}</span>
    </span>
  );

  if (!hasMenu) {
    return (
      <div className={styles.user}>
        {avatar}
        {text}
      </div>
    );
  }

  return (
    <div className={styles.userMenu} ref={rootRef}>
      {open ? (
        <div
          id={menuId}
          ref={menuRef}
          className={styles.menu}
          role="menu"
          aria-label="Profil menüsü"
          tabIndex={-1}
          onKeyDown={onMenuKeyDown}
        >
          <div className={styles.menuHead}>
            <Avatar name={user.name} size="md" />
            <span className={styles.menuId}>
              <strong>{user.name}</strong>
              {user.role ? <small>{user.role}</small> : null}
            </span>
          </div>
          {theme ? (
            <>
              <div className={styles.menuSep} role="separator" />
              <div className={styles.menuLabel} id={themeLabelId}>
                Tema
              </div>
              <div className={styles.themeGrid} role="group" aria-labelledby={themeLabelId}>
                {themeOptions.map((option) => (
                  <button
                    key={option.value}
                    type="button"
                    role="menuitemradio"
                    aria-checked={theme.preference === option.value}
                    className={styles.themeOption}
                    onClick={() => {
                      theme.onChange(option.value);
                    }}
                  >
                    {option.icon}
                    <span>{option.label}</span>
                  </button>
                ))}
              </div>
              <p className={styles.menuHint}>Sistem, cihazınızın ayarını izler.</p>
            </>
          ) : null}
          {onSignOut ? (
            <>
              <div className={styles.menuSep} role="separator" />
              <button type="button" role="menuitem" className={styles.menuItem} onClick={onSignOut}>
                <LogOut size={18} aria-hidden="true" />
                <span>Oturumu kapat</span>
              </button>
            </>
          ) : null}
        </div>
      ) : null}
      <button
        ref={triggerRef}
        type="button"
        className={styles.userButton}
        aria-haspopup="menu"
        aria-expanded={open}
        aria-controls={open ? menuId : undefined}
        aria-label="Profil menüsü"
        title={user.name}
        onClick={() => {
          setOpen((value) => !value);
        }}
      >
        {avatar}
        {text}
        <span className={styles.userChevron} aria-hidden="true">
          <ChevronsUpDown size={16} />
        </span>
      </button>
    </div>
  );
}
