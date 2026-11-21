import { errorMessage } from '@omvila/ui-web/errors';
import { Button, Modal } from '@omvila/ui-web/panel';
import styles from './OutsideHoursDialog.module.css';

interface OutsideHoursDialogProps {
  /** 409 kodu: outside_working_hours veya staff_on_time_off. */
  code: string;
  /** "Yine de taşı", "Yine de ekle". */
  confirmLabel: string;
  /** Onayın sonucu: "Yine de taşırsanız randevu bu saate yazılır." */
  consequence: string;
  onCancel: () => void;
  onConfirm: () => void;
}

/** 409 outside_working_hours / staff_on_time_off: kullanıcı onaylarsa allowOutsideWorkingHours ile tekrar. */
export function OutsideHoursDialog({
  code,
  confirmLabel,
  consequence,
  onCancel,
  onConfirm,
}: OutsideHoursDialogProps) {
  return (
    <Modal
      open
      onClose={onCancel}
      title={code === 'staff_on_time_off' ? 'Personel bu saatte izinli' : 'Çalışma saati dışında'}
      footer={
        <>
          <Button onClick={onCancel}>Vazgeç</Button>
          <Button variant="primary" onClick={onConfirm}>
            {confirmLabel}
          </Button>
        </>
      }
    >
      <p className={styles.text}>
        {errorMessage(code)} {consequence}
      </p>
    </Modal>
  );
}
