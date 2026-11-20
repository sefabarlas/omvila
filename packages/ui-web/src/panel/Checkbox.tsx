import { Check } from 'lucide-react';
import type { InputHTMLAttributes } from 'react';
import { classNames } from './Button';
import styles from './Toggle.module.css';

export interface CheckboxProps extends Omit<InputHTMLAttributes<HTMLInputElement>, 'type'> {
  label: string;
  /** İkinci satır açıklama. */
  description?: string;
  /** Etiket yalnız ekran okuyucuya (tablo satırı seçimi gibi bağlamı görünen yerler). */
  hideLabel?: boolean;
}

/** Formla kaydedilen bağımsız seçenek. Hemen etki eden ayar için Switch. */
export function Checkbox({
  label,
  description,
  hideLabel = false,
  className,
  ...rest
}: CheckboxProps) {
  return (
    <label className={classNames(styles.control, className)}>
      <input type="checkbox" className={styles.native} {...rest} />
      <span className={styles.box} aria-hidden="true">
        <Check size={14} strokeWidth={3} />
      </span>
      <span className={hideLabel ? styles.visuallyHidden : styles.text}>
        {label}
        {description ? <span className={styles.description}>{description}</span> : null}
      </span>
    </label>
  );
}
