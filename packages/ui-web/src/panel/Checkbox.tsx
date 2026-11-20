import { Check } from 'lucide-react';
import type { InputHTMLAttributes } from 'react';
import { classNames } from './Button';
import styles from './Toggle.module.css';

export interface CheckboxProps extends Omit<InputHTMLAttributes<HTMLInputElement>, 'type'> {
  label: string;
  /** İkinci satır açıklama. */
  description?: string;
}

/** Formla kaydedilen bağımsız seçenek. Hemen etki eden ayar için Switch. */
export function Checkbox({ label, description, className, ...rest }: CheckboxProps) {
  return (
    <label className={classNames(styles.control, className)}>
      <input type="checkbox" className={styles.native} {...rest} />
      <span className={styles.box} aria-hidden="true">
        <Check size={14} strokeWidth={3} />
      </span>
      <span className={styles.text}>
        {label}
        {description ? <span className={styles.description}>{description}</span> : null}
      </span>
    </label>
  );
}
