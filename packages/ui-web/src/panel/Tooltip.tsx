import { cloneElement, isValidElement, useId } from 'react';
import type { ReactElement } from 'react';
import styles from './Tooltip.module.css';

export interface TooltipProps {
  /** Tek satır, 60 karakteri geçmez; zorunlu bilgi için kullanılmaz. */
  content: string;
  /** Odaklanabilir tek öğe (buton, bağlantı). */
  children: ReactElement<{ 'aria-describedby'?: string }>;
}

/** Üzerine gelince veya odaklanınca kısa açıklama; öğeye aria-describedby ile bağlanır. */
export function Tooltip({ content, children }: TooltipProps) {
  const id = useId();
  return (
    <span className={styles.wrap}>
      {isValidElement(children) ? cloneElement(children, { 'aria-describedby': id }) : children}
      <span role="tooltip" id={id} className={styles.bubble}>
        {content}
      </span>
    </span>
  );
}
