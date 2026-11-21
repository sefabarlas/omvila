import type { ReactNode } from 'react';
import { useId } from 'react';
import { cx } from './internal';
import shared from './shared.module.css';
import styles from './SwitchCard.module.css';

export interface SwitchCardProps {
  /** Açık hâlin adı: "İki hizmet de aynı kişiden". */
  label: string;
  /** "Saç kesimi, Fön", "Kapatırsanız her hizmete ayrı kişi seçersiniz". */
  description?: ReactNode;
  checked: boolean;
  onCheckedChange: (checked: boolean) => void;
  disabled?: boolean;
  /** Soldaki ikon kutusu (ör. paket ikonu). */
  icon?: ReactNode;
  className?: string;
}

/** Kart içinde açıklamalı anahtar (components/Switch): personel adımı, paketten düşme, bekleme listesi. */
export function SwitchCard({
  label,
  description,
  checked,
  onCheckedChange,
  disabled = false,
  icon,
  className,
}: SwitchCardProps) {
  const id = useId();
  const labelId = `${id}-ad`;
  const descriptionId = `${id}-aciklama`;
  return (
    <label className={cx(styles.card, disabled && styles.disabled, className)}>
      {icon ? (
        <span className={styles.icon} aria-hidden="true">
          {icon}
        </span>
      ) : null}
      <span className={shared.cardText}>
        <span id={labelId} className={shared.cardTitle}>
          {label}
        </span>
        {description ? (
          <span id={descriptionId} className={shared.cardMeta}>
            {description}
          </span>
        ) : null}
      </span>
      <input
        type="checkbox"
        role="switch"
        className={styles.native}
        checked={checked}
        disabled={disabled}
        aria-labelledby={labelId}
        aria-describedby={description ? descriptionId : undefined}
        onChange={(event) => {
          onCheckedChange(event.target.checked);
        }}
      />
      <span className={styles.track} aria-hidden="true">
        <span className={styles.thumb} />
      </span>
    </label>
  );
}
