import { classNames } from './Button';
import { initials } from '../format/name';
import styles from './Avatar.module.css';

export { initials };

export interface AvatarProps {
  name: string;
  /** Fotoğraf; yoksa baş harfler. */
  src?: string | null;
  size?: 'sm' | 'md' | 'lg';
  className?: string;
}

function tone(name: string): 't0' | 't1' | 't2' {
  let hash = 0;
  for (const char of name) {
    hash = (hash * 31 + (char.codePointAt(0) ?? 0)) % 997;
  }

  return (['t0', 't1', 't2'] as const)[hash % 3] ?? 't0';
}

/** Kişinin fotoğrafı veya baş harfleri; ad ekran okuyucuya okunur. */
export function Avatar({ name, src, size = 'md', className }: AvatarProps) {
  return (
    <span
      className={classNames(styles.avatar, styles[size], !src && styles[tone(name)], className)}
      role="img"
      aria-label={name}
    >
      {src ? <img src={src} alt="" /> : <span aria-hidden="true">{initials(name)}</span>}
    </span>
  );
}
