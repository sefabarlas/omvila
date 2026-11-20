import type { ButtonHTMLAttributes, ReactNode } from 'react';
import styles from './Button.module.css';

export type ButtonVariant = 'primary' | 'secondary' | 'ghost' | 'danger';
export type ButtonSize = 'sm' | 'md' | 'lg';

export interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: ButtonVariant;
  size?: ButtonSize;
  /** Önde gösterilen ikon (Lucide). */
  icon?: ReactNode;
  /** Uzun süren işlem: buton etkileşime kapanır, metin loadingLabel olur ("Kaydediliyor…"). */
  loading?: boolean;
  loadingLabel?: string;
}

export function classNames(...names: (string | false | null | undefined)[]): string {
  return names.filter(Boolean).join(' ');
}

/** Eylem butonu; bir ekranda en fazla bir primary. Metin fiille başlar ("Randevu ekle"). */
export function Button({
  variant = 'secondary',
  size = 'md',
  icon,
  loading = false,
  loadingLabel,
  type = 'button',
  className,
  disabled,
  children,
  onClick,
  ...rest
}: ButtonProps) {
  // Yüklenirken buton rengini korur (tasarım), tıklama yok sayılır; ekran okuyucuya meşgul ve devre dışı bildirilir.
  return (
    <button
      type={type}
      className={classNames(
        styles.button,
        styles[variant],
        size !== 'md' && styles[size],
        className,
      )}
      disabled={disabled}
      aria-busy={loading || undefined}
      aria-disabled={loading || undefined}
      onClick={loading ? undefined : onClick}
      {...rest}
    >
      {loading ? <span className={styles.spinner} aria-hidden="true" /> : icon}
      {loading && loadingLabel ? loadingLabel : children}
    </button>
  );
}
