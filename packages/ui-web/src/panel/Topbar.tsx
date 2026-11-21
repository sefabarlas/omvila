import { Bell, Search } from 'lucide-react';
import { useEffect, useRef } from 'react';
import type { ReactNode } from 'react';
import { IconButton } from './IconButton';
import styles from './Topbar.module.css';

export interface TopbarProps {
  /** Sayfanın adı (sayfada tek h1). */
  title: string;
  /** Bağlam: tarih, şube. */
  context?: string;
  /** Genel arama; verilmezse alan gösterilmez. ⌘K / Ctrl+K odaklar. */
  search?: { placeholder: string; label: string; onSearch: (query: string) => void };
  /**
   * Okunmamış bildirim sayısı; boş veya 0 ise rozet yok. `panel` zilin altında açılan bildirim paneli
   * (salon-paneli/BildirimPaneli); `expanded` ve `controls` zilin aria-expanded / aria-controls değerleridir.
   */
  notifications?: {
    count?: number;
    onOpen: () => void;
    expanded?: boolean;
    controls?: string;
    panel?: ReactNode;
  };
  /** Sayfanın tek primary eylemi ("Randevu ekle"). */
  action?: ReactNode;
}

/** Sayfa başlığı, genel arama, bildirimler ve sayfanın ana eylemi. */
export function Topbar({ title, context, search, notifications, action }: TopbarProps) {
  const searchRef = useRef<HTMLInputElement>(null);
  useEffect(() => {
    if (!search) {
      return;
    }

    const onKey = (event: KeyboardEvent) => {
      if ((event.metaKey || event.ctrlKey) && event.key.toLowerCase() === 'k') {
        event.preventDefault();
        searchRef.current?.focus();
      }
    };
    document.addEventListener('keydown', onKey);
    return () => {
      document.removeEventListener('keydown', onKey);
    };
  }, [search]);

  const count = notifications?.count ?? 0;
  return (
    <header className={styles.topbar}>
      <div className={styles.heading}>
        <h1 className={styles.title}>{title}</h1>
        {context ? <span className={styles.context}>{context}</span> : null}
      </div>
      <div className={styles.right}>
        {search ? (
          <form
            role="search"
            className={styles.search}
            onSubmit={(event) => {
              event.preventDefault();
              search.onSearch(searchRef.current?.value ?? '');
            }}
          >
            <Search size={16} aria-hidden="true" />
            <input
              ref={searchRef}
              type="search"
              className={styles.searchInput}
              placeholder={search.placeholder}
              aria-label={search.label}
            />
            <kbd className={styles.kbd} aria-hidden="true">
              ⌘K
            </kbd>
          </form>
        ) : null}
        {notifications ? (
          <span className={styles.bell}>
            <IconButton
              label={count > 0 ? `Bildirimler, ${String(count)} okunmamış` : 'Bildirimler'}
              icon={<Bell size={18} />}
              onClick={notifications.onOpen}
              {...(notifications.expanded === undefined
                ? {}
                : { 'aria-expanded': notifications.expanded, 'aria-haspopup': 'dialog' as const })}
              {...(notifications.controls ? { 'aria-controls': notifications.controls } : {})}
            />
            {count > 0 ? (
              <span className={styles.badge} aria-hidden="true">
                {count > 99 ? '99+' : count}
              </span>
            ) : null}
            {notifications.panel}
          </span>
        ) : null}
        {action}
      </div>
    </header>
  );
}
