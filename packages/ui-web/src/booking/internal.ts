import { useCallback, useRef } from 'react';
import type { KeyboardEvent } from 'react';

// Rezervasyon bileşenlerinin ortak yardımcıları. Panel bileşenlerinden içe aktarılmaz (panel CSS'i panel ölçeğinin
// tokenlarını kullanır; rezervasyon sayfası yalnız mobil ölçeği yükler).

export function cx(...names: (string | false | null | undefined)[]): string {
  return names.filter(Boolean).join(' ');
}

export interface RovingItem {
  value: string;
  disabled?: boolean | undefined;
}

/**
 * Radyo grubu klavye davranışı (WAI-ARIA radio group): grupta tek sekme durağı (seçili ya da ilk etkin öğe);
 * oklar sonraki/önceki etkin öğeye geçip seçer, Home/End ilk/son etkin öğe. Devre dışı öğeler atlanır.
 */
export function useRovingRadio(
  items: readonly RovingItem[],
  value: string | null,
  onChange: (value: string) => void,
) {
  const refs = useRef(new Map<string, HTMLElement>());
  const enabled = items.filter((item) => !item.disabled);
  const tabStop = enabled.some((item) => item.value === value) ? value : enabled[0]?.value;

  const select = useCallback(
    (next: string) => {
      onChange(next);
      refs.current.get(next)?.focus();
    },
    [onChange],
  );

  const onKeyDown = (event: KeyboardEvent<HTMLElement>, current: string) => {
    const index = enabled.findIndex((item) => item.value === current);
    let target: RovingItem | undefined;
    switch (event.key) {
      case 'ArrowRight':
      case 'ArrowDown':
        target = enabled[(index + 1) % enabled.length];
        break;
      case 'ArrowLeft':
      case 'ArrowUp':
        target = enabled[(index - 1 + enabled.length) % enabled.length];
        break;
      case 'Home':
        target = enabled[0];
        break;
      case 'End':
        target = enabled.at(-1);
        break;
      default:
        return;
    }

    event.preventDefault();
    if (target) {
      select(target.value);
    }
  };

  const itemProps = (item: RovingItem) => ({
    ref: (element: HTMLElement | null) => {
      if (element) {
        refs.current.set(item.value, element);
      } else {
        refs.current.delete(item.value);
      }
    },
    role: 'radio' as const,
    'aria-checked': value === item.value,
    tabIndex: item.value === tabStop ? 0 : -1,
    disabled: item.disabled,
    onClick: () => {
      onChange(item.value);
    },
    onKeyDown: (event: KeyboardEvent<HTMLElement>) => {
      onKeyDown(event, item.value);
    },
  });

  return { itemProps };
}
