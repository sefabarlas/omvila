import { ChevronDown, XCircle } from 'lucide-react';
import { useId } from 'react';
import type { SelectHTMLAttributes } from 'react';
import { classNames } from './Button';
import styles from './Field.module.css';

export interface SelectOption {
  value: string;
  label: string;
}

export interface SelectProps extends SelectHTMLAttributes<HTMLSelectElement> {
  label: string;
  /** Metin dizisi veya {value, label}. */
  options: readonly (string | SelectOption)[];
  hint?: string | undefined;
  error?: string | undefined;
  /** Etiket ekranda başka yerde yazıyorsa (ayar satırı) yalnız ekran okuyucuya. */
  hideLabel?: boolean;
}

/** Etiketli yerel açılır liste; 7'den az seçenekte. Uzun listeler için aramalı menü kullanılır. */
export function Select({
  label,
  options,
  hint,
  error,
  hideLabel = false,
  id,
  className,
  ...rest
}: SelectProps) {
  const generated = useId();
  const selectId = id ?? generated;
  const hintId = `${selectId}-hint`;
  const message = error ?? hint;
  return (
    <div className={classNames(styles.field, className)}>
      <label className={hideLabel ? styles.visuallyHidden : styles.label} htmlFor={selectId}>
        {label}
      </label>
      <div className={styles.wrap}>
        <select
          id={selectId}
          className={classNames(styles.input, styles.select)}
          aria-invalid={error ? true : undefined}
          aria-describedby={message ? hintId : undefined}
          {...rest}
        >
          {options.map((option) => {
            const { value, label: text } =
              typeof option === 'string' ? { value: option, label: option } : option;
            return (
              <option key={value} value={value}>
                {text}
              </option>
            );
          })}
        </select>
        <span className={styles.chevron} aria-hidden="true">
          <ChevronDown size={16} />
        </span>
      </div>
      {message ? (
        <p id={hintId} className={classNames(styles.hint, error !== undefined && styles.error)}>
          {error !== undefined ? <XCircle size={14} aria-hidden="true" /> : null}
          {message}
        </p>
      ) : null}
    </div>
  );
}
