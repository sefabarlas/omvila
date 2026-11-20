import { useId } from 'react';
import type { InputHTMLAttributes, ReactNode } from 'react';
import { classNames } from './Button';
import styles from './Field.module.css';

export interface TextFieldProps extends InputHTMLAttributes<HTMLInputElement> {
  /** Her zaman görünür etiket. */
  label: string;
  /** Yardımcı metin. */
  hint?: string;
  /** Hata: ne olduğunu ve ne yapılacağını söyler ("Telefon numarası 10 haneli olmalı."). */
  error?: string | undefined;
  /** Arama ve telefon alanlarında ikon. */
  icon?: ReactNode;
  /** Birim: ₺, dk. */
  suffix?: string;
}

/** Etiketli tek satır giriş alanı. Placeholder gerçek bir örnektir, etiketin tekrarı değildir. */
export function TextField({
  label,
  hint,
  error,
  icon,
  suffix,
  id,
  className,
  ...rest
}: TextFieldProps) {
  const generated = useId();
  const inputId = id ?? generated;
  const hintId = `${inputId}-hint`;
  const message = error ?? hint;
  return (
    <div className={classNames(styles.field, className)}>
      <label className={styles.label} htmlFor={inputId}>
        {label}
      </label>
      <div className={styles.wrap}>
        {icon ? (
          <span className={styles.icon} aria-hidden="true">
            {icon}
          </span>
        ) : null}
        <input
          id={inputId}
          className={classNames(
            styles.input,
            icon !== undefined && styles.hasIcon,
            suffix !== undefined && styles.hasSuffix,
          )}
          aria-invalid={error ? true : undefined}
          aria-describedby={message ? hintId : undefined}
          {...rest}
        />
        {suffix ? (
          <span className={styles.suffix} aria-hidden="true">
            {suffix}
          </span>
        ) : null}
      </div>
      {message ? (
        <p id={hintId} className={classNames(styles.hint, error !== undefined && styles.error)}>
          {message}
        </p>
      ) : null}
    </div>
  );
}
