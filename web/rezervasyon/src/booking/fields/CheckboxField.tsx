import { Check } from 'lucide-react';
import { useId } from 'react';
import type { ReactNode } from 'react';
import styles from './fields.module.css';

interface CheckboxFieldProps {
  checked: boolean;
  onCheckedChange: (checked: boolean) => void;
  children: ReactNode;
  error?: string | undefined;
  required?: boolean;
}

/** Onay kutusu (aydınlatma metni, kampanya izni). Kutu 24 px, dokunma alanı satırın tamamı. */
export function CheckboxField({
  checked,
  onCheckedChange,
  children,
  error,
  required = false,
}: CheckboxFieldProps) {
  const id = useId();
  return (
    <div className={styles.checkField}>
      <label className={styles.checkRow}>
        <input
          type="checkbox"
          className={styles.checkNative}
          checked={checked}
          required={required}
          aria-invalid={error ? true : undefined}
          aria-describedby={error ? `${id}-hata` : undefined}
          onChange={(event) => {
            onCheckedChange(event.target.checked);
          }}
        />
        <span className={styles.checkBox} aria-hidden="true">
          {checked ? <Check size={18} strokeWidth={3} /> : null}
        </span>
        <span className={styles.checkText}>{children}</span>
      </label>
      {error ? (
        <p id={`${id}-hata`} className={styles.error}>
          {error}
        </p>
      ) : null}
    </div>
  );
}
