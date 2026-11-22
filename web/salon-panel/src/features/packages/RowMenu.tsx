import { MoreHorizontal } from 'lucide-react';
import { useEffect, useId, useRef, useState } from 'react';
import type { KeyboardEvent } from 'react';
import styles from './Packages.module.css';

export interface RowMenuItem {
  label: string;
  onSelect: () => void;
  danger?: boolean;
}

/**
 * Satır "…" menüsü (salon-paneli/Paketler). ui-web'de açılır menü bileşeni yok; tek kullanım olduğu için burada.
 * Escape ve dışarı tıklama kapatır; açılınca ilk öğe odak alır, ok tuşlarıyla gezilir.
 */
export function RowMenu({ label, items }: { label: string; items: readonly RowMenuItem[] }) {
  const [open, setOpen] = useState(false);
  const id = useId();
  const root = useRef<HTMLDivElement>(null);
  const menu = useRef<HTMLDivElement>(null);
  const trigger = useRef<HTMLButtonElement>(null);

  useEffect(() => {
    if (!open) {
      return undefined;
    }

    menu.current?.querySelector<HTMLButtonElement>('[role="menuitem"]')?.focus();
    const close = (event: MouseEvent) => {
      if (!root.current?.contains(event.target as Node)) {
        setOpen(false);
      }
    };
    document.addEventListener('mousedown', close);
    return () => {
      document.removeEventListener('mousedown', close);
    };
  }, [open]);

  const onKeyDown = (event: KeyboardEvent) => {
    if (event.key === 'Escape') {
      setOpen(false);
      trigger.current?.focus();
      return;
    }

    if (event.key !== 'ArrowDown' && event.key !== 'ArrowUp') {
      return;
    }

    event.preventDefault();
    const buttons = Array.from(
      menu.current?.querySelectorAll<HTMLButtonElement>('[role="menuitem"]') ?? [],
    );
    const index = buttons.indexOf(document.activeElement as HTMLButtonElement);
    const next = event.key === 'ArrowDown' ? index + 1 : index - 1;
    buttons[(next + buttons.length) % buttons.length]?.focus();
  };

  if (items.length === 0) {
    return null;
  }

  return (
    <div ref={root} className={styles.menuRoot}>
      <button
        ref={trigger}
        type="button"
        className={styles.menuTrigger}
        aria-label={label}
        aria-haspopup="menu"
        aria-expanded={open}
        aria-controls={open ? id : undefined}
        onClick={() => {
          setOpen((value) => !value);
        }}
      >
        <MoreHorizontal size={18} aria-hidden="true" />
      </button>
      {open ? (
        <div
          ref={menu}
          id={id}
          role="menu"
          aria-label={label}
          tabIndex={-1}
          className={styles.menu}
          onKeyDown={onKeyDown}
        >
          {items.map((item) => (
            <button
              key={item.label}
              type="button"
              role="menuitem"
              className={[styles.menuItem, item.danger ? styles.danger : '']
                .filter(Boolean)
                .join(' ')}
              onClick={() => {
                setOpen(false);
                item.onSelect();
              }}
            >
              {item.label}
            </button>
          ))}
        </div>
      ) : null}
    </div>
  );
}
