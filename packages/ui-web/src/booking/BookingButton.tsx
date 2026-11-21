import type { ButtonHTMLAttributes, ReactNode } from 'react';
import { cx } from './internal';
import styles from './BookingButton.module.css';

export type BookingButtonVariant = 'primary' | 'secondary' | 'ghost' | 'danger';
export type BookingButtonSize = 'sm' | 'md' | 'lg';

export interface BookingButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  /** primary: ekranın tek ana eylemi; secondary: gri dolgu; ghost: marka renginde metin; danger: "Randevuyu iptal et". */
  variant?: BookingButtonVariant;
  /** lg: ana eylem (52), md: kart içi (48), sm: satır içi (36 görünür, 44 dokunma alanı). */
  size?: BookingButtonSize;
  /** Tam genişlik. */
  block?: boolean;
  /** Metnin solundaki ikon (lucide). */
  icon?: ReactNode;
  /** Süren işlem: tıklama yok sayılır, metin loadingLabel olur ("Oluşturuluyor…"). */
  loading?: boolean;
  loadingLabel?: string;
}

/** Mobil tasarım sistemi butonu (components/Button). Metin fiille başlar: "Devam et", "Saati değiştir". */
export function BookingButton({
  variant = 'primary',
  size = 'lg',
  block = false,
  icon,
  loading = false,
  loadingLabel,
  className,
  children,
  disabled,
  onClick,
  type = 'button',
  ...rest
}: BookingButtonProps) {
  return (
    <button
      type={type === 'submit' ? 'submit' : 'button'}
      className={cx(styles.button, styles[variant], styles[size], block && styles.block, className)}
      disabled={disabled}
      aria-busy={loading || undefined}
      aria-disabled={loading || undefined}
      onClick={loading ? undefined : onClick}
      {...rest}
    >
      {icon ? (
        <span className={styles.icon} aria-hidden="true">
          {icon}
        </span>
      ) : null}
      {loading && loadingLabel ? loadingLabel : children}
    </button>
  );
}
