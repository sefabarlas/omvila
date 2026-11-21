import { Button, Modal, TextField } from '@omvila/ui-web/panel';
import { addDays } from '@omvila/ui-web/time';
import { useState } from 'react';
import styles from './Cash.module.css';

interface PreviousDaysModalProps {
  /** Şubede bugün; ileri bir gün seçilemez. */
  today: string;
  onClose: () => void;
  onPick: (date: string) => void;
}

/** "Önceki günler": kasası açılacak günü seçer (varsayılan dün). Tasarımda ayrı ekranı yok (acik-sorular.md T-97). */
export function PreviousDaysModal({ today, onClose, onPick }: PreviousDaysModalProps) {
  const [date, setDate] = useState(() => addDays(today, -1));
  const [error, setError] = useState<string | undefined>();
  const submit = () => {
    if (!/^\d{4}-\d{2}-\d{2}$/.test(date) || date > today) {
      setError('Bugün veya daha önceki bir gün seçin.');
      return;
    }

    onPick(date);
  };

  return (
    <Modal
      open
      onClose={onClose}
      title="Önceki günler"
      description="Kasasını görmek istediğiniz günü seçin."
      footer={
        <>
          <Button onClick={onClose}>Vazgeç</Button>
          <Button variant="primary" onClick={submit}>
            Günü aç
          </Button>
        </>
      }
    >
      <div className={styles.picker}>
        <TextField
          label="Gün"
          type="date"
          max={today}
          value={date}
          error={error}
          onChange={(event) => {
            setDate(event.target.value);
            setError(undefined);
          }}
        />
      </div>
    </Modal>
  );
}
