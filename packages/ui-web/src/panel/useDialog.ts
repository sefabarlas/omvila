import { useEffect, useRef } from 'react';
import type { RefObject } from 'react';

const focusable =
  'a[href], button:not([disabled]), input:not([disabled]), select:not([disabled]), textarea:not([disabled]), [tabindex]:not([tabindex="-1"])';

/**
 * Açık diyaloğun ortak davranışı: ilk odaklanabilir öğeye odak, Tab ile içeride dolaşma, Esc veya arka plana
 * tıklamayla kapanma, kapanınca odağın açan öğeye dönmesi ve sayfa kaydırmasının kilitlenmesi.
 */
export function useDialog(open: boolean, onClose: () => void): RefObject<HTMLDivElement | null> {
  const ref = useRef<HTMLDivElement>(null);
  const closeRef = useRef(onClose);
  closeRef.current = onClose;

  useEffect(() => {
    if (!open) {
      return;
    }

    const opener = document.activeElement instanceof HTMLElement ? document.activeElement : null;
    const panel = ref.current;
    const items = () => Array.from(panel?.querySelectorAll<HTMLElement>(focusable) ?? []);
    (items()[0] ?? panel)?.focus();
    const overflow = document.body.style.overflow;
    document.body.style.overflow = 'hidden';

    const onKey = (event: KeyboardEvent) => {
      if (event.key === 'Escape') {
        event.stopPropagation();
        closeRef.current();
        return;
      }

      if (event.key !== 'Tab') {
        return;
      }

      const list = items();
      const first = list[0];
      const last = list[list.length - 1];
      if (!first || !last) {
        event.preventDefault();
        return;
      }

      if (event.shiftKey && document.activeElement === first) {
        event.preventDefault();
        last.focus();
      } else if (!event.shiftKey && document.activeElement === last) {
        event.preventDefault();
        first.focus();
      }
    };

    // Arka plan (diyaloğun kapsayıcısı) tıklaması; klavye için Esc var.
    const onPointer = (event: MouseEvent) => {
      if (event.target === panel?.parentElement) {
        closeRef.current();
      }
    };

    document.addEventListener('keydown', onKey);
    document.addEventListener('mousedown', onPointer);
    return () => {
      document.removeEventListener('keydown', onKey);
      document.removeEventListener('mousedown', onPointer);
      document.body.style.overflow = overflow;
      opener?.focus();
    };
  }, [open]);

  return ref;
}
