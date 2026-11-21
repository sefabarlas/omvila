import { useId } from 'react';
import type { InputHTMLAttributes, ReactNode } from 'react';
import styles from './fields.module.css';

interface TextFieldProps extends Omit<
  InputHTMLAttributes<HTMLInputElement>,
  'id' | 'className' | 'prefix'
> {
  label: string;
  /** Alanın altındaki açıklama ("Doğrulama kodu bu numaraya SMS ile gelir."). */
  hint?: string;
  error?: string | undefined;
  /** Alanın solundaki sabit ön ek ("+90"). */
  prefix?: ReactNode;
}

/** Etiketli metin alanı (rezervasyon-sayfasi/Telefon). Hata metni alana `aria-describedby` ile bağlıdır. */
export function TextField({ label, hint, error, prefix, ...input }: TextFieldProps) {
  const id = useId();
  const describedBy = [error ? `${id}-hata` : null, hint ? `${id}-not` : null]
    .filter(Boolean)
    .join(' ');
  return (
    <div className={styles.field}>
      <label className={styles.label} htmlFor={id}>
        {label}
      </label>
      <div className={styles.control} data-invalid={error ? 'true' : undefined}>
        {prefix ? (
          <span className={styles.prefix} aria-hidden="true">
            {prefix}
          </span>
        ) : null}
        <input
          id={id}
          className={styles.input}
          aria-invalid={error ? true : undefined}
          aria-describedby={describedBy || undefined}
          {...input}
        />
      </div>
      {error ? (
        <p id={`${id}-hata`} className={styles.error}>
          {error}
        </p>
      ) : null}
      {hint ? (
        <p id={`${id}-not`} className={styles.hint}>
          {hint}
        </p>
      ) : null}
    </div>
  );
}
