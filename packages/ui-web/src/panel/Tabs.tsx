import { useId, useRef } from 'react';
import type { KeyboardEvent } from 'react';
import styles from './Tabs.module.css';

export interface TabItem {
  value: string;
  label: string;
  /** Sayı rozeti; yalnız gerçek sayı (boşken gösterilmez). */
  count?: number;
}

export interface TabsProps {
  items: readonly TabItem[];
  value: string;
  onChange: (value: string) => void;
  label: string;
  /** Sekme paneli kimliği öneki (aria-controls). */
  idPrefix?: string;
}

/** Aynı sayfadaki görünümler arası geçiş; ok tuşlarıyla dolaşılır (WAI-ARIA tabs, elle seçim). */
export function Tabs({ items, value, onChange, label, idPrefix }: TabsProps) {
  const generated = useId();
  const prefix = idPrefix ?? generated;
  const refs = useRef<(HTMLButtonElement | null)[]>([]);

  const onKeyDown = (event: KeyboardEvent, index: number) => {
    const last = items.length - 1;
    const target =
      event.key === 'ArrowRight'
        ? index === last
          ? 0
          : index + 1
        : event.key === 'ArrowLeft'
          ? index === 0
            ? last
            : index - 1
          : event.key === 'Home'
            ? 0
            : event.key === 'End'
              ? last
              : null;
    if (target === null) {
      return;
    }

    event.preventDefault();
    refs.current[target]?.focus();
    const item = items[target];
    if (item) {
      onChange(item.value);
    }
  };

  return (
    <div role="tablist" aria-label={label} className={styles.list}>
      {items.map((item, index) => {
        const selected = item.value === value;
        return (
          <button
            key={item.value}
            ref={(el) => {
              refs.current[index] = el;
            }}
            type="button"
            role="tab"
            id={`${prefix}-tab-${item.value}`}
            aria-selected={selected}
            aria-controls={`${prefix}-panel-${item.value}`}
            tabIndex={selected ? 0 : -1}
            className={styles.tab}
            onClick={() => {
              onChange(item.value);
            }}
            onKeyDown={(event) => {
              onKeyDown(event, index);
            }}
          >
            {item.label}
            {item.count !== undefined && item.count > 0 ? (
              <span className={styles.count}>{item.count}</span>
            ) : null}
          </button>
        );
      })}
    </div>
  );
}
