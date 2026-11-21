import { initials } from '../format/name';
import { cx } from './internal';
import styles from './BookingAvatar.module.css';

export interface BookingAvatarProps {
  name: string;
  /** Fotoğraf; yoksa baş harfler. */
  src?: string | null;
  /** sm 32, md 48, lg 56. */
  size?: 'sm' | 'md' | 'lg';
  /** Ad yanında yazıyorsa ekran okuyucudan gizlenir. */
  decorative?: boolean;
  className?: string;
}

function tone(name: string): 't0' | 't1' | 't2' {
  let hash = 0;
  for (const char of name) {
    hash = (hash * 31 + (char.codePointAt(0) ?? 0)) % 997;
  }

  return (['t0', 't1', 't2'] as const)[hash % 3] ?? 't0';
}

/** Mobil Avatar (components/Avatar): fotoğraf veya baş harfler; ton addan türetilir, anlam taşımaz. */
export function BookingAvatar({
  name,
  src,
  size = 'md',
  decorative = false,
  className,
}: BookingAvatarProps) {
  const a11y = decorative
    ? { 'aria-hidden': true as const }
    : { role: 'img' as const, 'aria-label': name };
  return (
    <span
      className={cx(styles.avatar, styles[size], !src && styles[tone(name)], className)}
      {...a11y}
    >
      {src ? <img src={src} alt="" /> : <span aria-hidden="true">{initials(name)}</span>}
    </span>
  );
}
