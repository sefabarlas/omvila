import type { ButtonHTMLAttributes, ReactNode } from 'react';
import { classNames } from './Button';
import type { ButtonSize, ButtonVariant } from './Button';
import styles from './Button.module.css';

export interface IconButtonProps extends Omit<ButtonHTMLAttributes<HTMLButtonElement>, 'children'> {
  /** Ekran okuyucu ve ipucu metni; zorunlu (yalnız ikonlu buton adsız kalmaz). */
  label: string;
  icon: ReactNode;
  variant?: ButtonVariant;
  size?: ButtonSize;
}

export function IconButton({
  label,
  icon,
  variant = 'ghost',
  size = 'md',
  type = 'button',
  className,
  ...rest
}: IconButtonProps) {
  return (
    <button
      type={type}
      aria-label={label}
      title={label}
      className={classNames(
        styles.button,
        styles.iconOnly,
        styles[variant],
        size !== 'md' && styles[size],
        className,
      )}
      {...rest}
    >
      {icon}
    </button>
  );
}
