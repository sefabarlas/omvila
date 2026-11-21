import { Button } from '@omvila/ui-web/panel';
import { Plus } from 'lucide-react';
import { useId } from 'react';
import type { ReactNode } from 'react';
import styles from './NewAppointment.module.css';

/**
 * Üst bardaki "Randevu ekle" (sayfanın tek primary eylemi). Salt okunur abonelikte pasiftir ve nedeni yanında yazar
 * (kabul kriteri 17; durum-makinesi.md § Abonelik, S-05).
 */
export function NewAppointmentButton({
  readOnly,
  onClick,
  label = 'Randevu ekle',
  icon = <Plus size={16} aria-hidden="true" />,
}: {
  readOnly: boolean;
  onClick: () => void;
  /** Müşteri kartında "Randevu ver". */
  label?: string;
  icon?: ReactNode;
}) {
  const reasonId = useId();
  return (
    <span className={styles.topbarAction}>
      {readOnly ? (
        <span id={reasonId} className={styles.reason}>
          Abonelik salt okunur; yeni randevu eklenemez.
        </span>
      ) : null}
      <Button
        variant="primary"
        icon={icon}
        disabled={readOnly}
        aria-describedby={readOnly ? reasonId : undefined}
        onClick={onClick}
      >
        {label}
      </Button>
    </span>
  );
}
