import { XCircle } from 'lucide-react';
import { useId } from 'react';
import type { TextareaHTMLAttributes } from 'react';
import { classNames } from './Button';
import styles from './Field.module.css';

export interface TextAreaProps extends TextareaHTMLAttributes<HTMLTextAreaElement> {
  /** Her zaman görünür etiket. */
  label: string;
  /** Yardımcı metin (kullanılabilir değişkenler, karakter sınırı). */
  hint?: string | undefined;
  /** Hata: ne olduğunu ve ne yapılacağını söyler. */
  error?: string | undefined;
  hideLabel?: boolean;
}

/** Etiketli çok satırlı giriş alanı (SMS şablonu, not). TextField ile aynı ölçü ve durumlar. */
export function TextArea({
  label,
  hint,
  error,
  hideLabel = false,
  id,
  className,
  rows = 4,
  ...rest
}: TextAreaProps) {
  const generated = useId();
  const inputId = id ?? generated;
  const hintId = `${inputId}-hint`;
  const message = error ?? hint;
  return (
    <div className={classNames(styles.field, className)}>
      <label className={hideLabel ? styles.visuallyHidden : styles.label} htmlFor={inputId}>
        {label}
      </label>
      <textarea
        id={inputId}
        rows={rows}
        className={classNames(styles.input, styles.textarea)}
        aria-invalid={error ? true : undefined}
        aria-describedby={message ? hintId : undefined}
        {...rest}
      />
      {message ? (
        <p id={hintId} className={classNames(styles.hint, error !== undefined && styles.error)}>
          {error !== undefined ? <XCircle size={14} aria-hidden="true" /> : null}
          {message}
        </p>
      ) : null}
    </div>
  );
}
