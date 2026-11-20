import type { InputHTMLAttributes } from 'react';
import { classNames } from './Button';
import styles from './Toggle.module.css';

export interface SwitchProps extends Omit<InputHTMLAttributes<HTMLInputElement>, 'type' | 'role'> {
  /** Ayarın açık hâli: "Online randevu açık". "Açık/Kapalı" durum metni eklenmez. */
  label: string;
  description?: string;
}

/** Anında kaydedilen açık/kapalı ayar. */
export function Switch({ label, description, className, ...rest }: SwitchProps) {
  return (
    <label className={classNames(styles.control, className)}>
      <input type="checkbox" role="switch" className={styles.native} {...rest} />
      <span className={styles.track} aria-hidden="true">
        <span className={styles.thumb} />
      </span>
      <span className={styles.text}>
        {label}
        {description ? <span className={styles.description}>{description}</span> : null}
      </span>
    </label>
  );
}
