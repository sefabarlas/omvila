'use client';

import { Check, Monitor, Moon, Sun } from 'lucide-react';
import { useEffect, useId, useRef, useState } from 'react';
import type { KeyboardEvent as ReactKeyboardEvent, ReactNode } from 'react';
import { setThemePreference, useResolvedDark, useThemePreference } from '@/lib/theme';
import type { ThemePreference } from '@/lib/theme';
import styles from './ThemeMenu.module.css';

export const themeOptions: readonly {
  value: ThemePreference;
  label: string;
  icon: (size: number) => ReactNode;
}[] = [
  { value: 'light', label: 'Açık', icon: (size) => <Sun size={size} aria-hidden="true" /> },
  { value: 'dark', label: 'Koyu', icon: (size) => <Moon size={size} aria-hidden="true" /> },
  {
    value: 'system',
    label: 'Sistem',
    icon: (size) => <Monitor size={size} aria-hidden="true" />,
  },
];

/** Üst bardaki tema düğmesi ve menüsü (tasarım TemaMenu). Esc ve dışarı tıklama kapatır; ok tuşları gezer. */
export function ThemeMenu() {
  const preference = useThemePreference();
  const dark = useResolvedDark();
  const [open, setOpen] = useState(false);
  const rootRef = useRef<HTMLDivElement>(null);
  const menuRef = useRef<HTMLDivElement>(null);
  const triggerRef = useRef<HTMLButtonElement>(null);
  const menuId = useId();
  const label = themeOptions.find((o) => o.value === preference)?.label ?? 'Sistem';

  useEffect(() => {
    if (!open) {
      return;
    }

    menuRef.current?.querySelector<HTMLButtonElement>('[aria-checked="true"]')?.focus();
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
    const step = event.key === 'ArrowDown' ? 1 : event.key === 'ArrowUp' ? -1 : 0;
    if (!step || !menuRef.current) {
      return;
    }

    event.preventDefault();
    const items = Array.from(menuRef.current.querySelectorAll<HTMLButtonElement>('button'));
    const index = items.indexOf(document.activeElement as HTMLButtonElement);
    items[(index + step + items.length) % items.length]?.focus();
  };

  return (
    <div className={styles.root} ref={rootRef}>
      <button
        ref={triggerRef}
        type="button"
        className={styles.trigger}
        aria-haspopup="menu"
        aria-expanded={open}
        aria-controls={open ? menuId : undefined}
        aria-label={`Tema: ${label}`}
        onClick={() => {
          setOpen((value) => !value);
        }}
      >
        {dark ? <Moon size={20} aria-hidden="true" /> : <Sun size={20} aria-hidden="true" />}
      </button>
      {open ? (
        <div
          id={menuId}
          ref={menuRef}
          role="menu"
          aria-label="Tema"
          tabIndex={-1}
          className={styles.menu}
          onKeyDown={onMenuKeyDown}
        >
          <div className={styles.label}>Tema</div>
          {themeOptions.map((option) => (
            <button
              key={option.value}
              type="button"
              role="menuitemradio"
              aria-checked={preference === option.value}
              className={styles.option}
              onClick={() => {
                setThemePreference(option.value);
              }}
            >
              {option.icon(20)}
              <span className={styles.optionLabel}>{option.label}</span>
              {preference === option.value ? <Check size={18} aria-hidden="true" /> : null}
            </button>
          ))}
          <p className={styles.hint}>Sistem, cihazınızın ayarını izler.</p>
        </div>
      ) : null}
    </div>
  );
}

/** Alt bilgideki tema seçici (tasarım TemaMenu, footer). */
export function ThemeSwitch() {
  const preference = useThemePreference();
  return (
    <div role="group" aria-label="Tema" className={styles.switch}>
      {themeOptions.map((option) => (
        <button
          key={option.value}
          type="button"
          aria-pressed={preference === option.value}
          className={styles.segment}
          onClick={() => {
            setThemePreference(option.value);
          }}
        >
          {option.icon(16)}
          {option.label}
        </button>
      ))}
    </div>
  );
}
