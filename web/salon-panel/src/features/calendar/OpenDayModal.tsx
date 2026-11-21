import { formatDate } from '@omvila/ui-web/format';
import { Button, Modal, TextField, useToast } from '@omvila/ui-web/panel';
import { useState } from 'react';
import { describeSaveError } from '@/features/settings/save-error';
import { useOpenDay } from './api';
import styles from './Calendar.module.css';

interface OpenDayModalProps {
  branchId: string;
  date: string;
  /** Öneri: şubenin bugünkü saatleri (varsa ilk açılış, son kapanış). */
  defaults: readonly { start: string; end: string }[];
  onClose: () => void;
}

/**
 * "Yalnızca bu gün için aç" (TakvimKapali, kabul kriteri 16): o tarihe ExtraOpening özel günü; haftalık düzen
 * değişmez. Saati olmayan personel o gün salonun bu saatleriyle çalışır (acik-sorular.md T-48 (4)).
 */
export function OpenDayModal({ branchId, date, defaults, onClose }: OpenDayModalProps) {
  const toast = useToast();
  const open = useOpenDay(branchId);
  const [start, setStart] = useState(defaults[0]?.start ?? '');
  const [end, setEnd] = useState(defaults.at(-1)?.end ?? '');
  const [error, setError] = useState<string | undefined>();

  const onSubmit = async (event: { preventDefault: () => void }) => {
    event.preventDefault();
    if (start === '' || end === '') {
      setError('Açılış ve kapanış saatini girin.');
      return;
    }

    if (end <= start) {
      setError('Kapanış saati açılıştan sonra olmalı.');
      return;
    }

    setError(undefined);
    try {
      await open.mutateAsync({ date, start, end });
      toast.show({ tone: 'basari', title: `${formatDate(date, 'dayMonth')} randevuya açıldı` });
      onClose();
    } catch (failure) {
      toast.show({ tone: 'hata', title: describeSaveError(failure).message });
    }
  };

  return (
    <Modal
      open
      onClose={onClose}
      title="Yalnızca bu gün için aç"
      description={`${formatDate(date, 'panel')} · haftalık düzen değişmez`}
      footer={
        <>
          <Button onClick={onClose}>Vazgeç</Button>
          <Button
            variant="primary"
            type="submit"
            form="gunu-ac"
            loading={open.isPending}
            loadingLabel="Açılıyor…"
          >
            Günü aç
          </Button>
        </>
      }
    >
      <form id="gunu-ac" className={styles.interval} noValidate onSubmit={(e) => void onSubmit(e)}>
        <TextField
          label="Açılış"
          type="time"
          value={start}
          onChange={(event) => {
            setStart(event.target.value);
          }}
        />
        <TextField
          label="Kapanış"
          type="time"
          value={end}
          error={error}
          onChange={(event) => {
            setEnd(event.target.value);
          }}
        />
      </form>
    </Modal>
  );
}
