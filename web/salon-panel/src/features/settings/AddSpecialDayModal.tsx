import { zodResolver } from '@hookform/resolvers/zod';
import { Button, Modal, Select, TextField, useToast } from '@omvila/ui-web/panel';
import { useForm, useWatch } from 'react-hook-form';
import { z } from 'zod';
import { useCreateOverride } from './branch-api';
import { describeSaveError } from './save-error';
import styles from './Settings.module.css';

const kinds = [
  { value: 'Closure', label: 'Kapalı' },
  { value: 'ShortDay', label: 'Özel saat' },
] as const;

const schema = z
  .object({
    kind: z.enum(['Closure', 'ShortDay']),
    fromDate: z.string().min(1, 'Tarih seçin.'),
    toDate: z.string(),
    start: z.string(),
    end: z.string(),
    note: z.string().trim().max(200, 'Not en fazla 200 karakter olabilir.'),
  })
  .superRefine((values, ctx) => {
    if (values.toDate !== '' && values.toDate < values.fromDate) {
      ctx.addIssue({
        code: 'custom',
        path: ['toDate'],
        message: 'Bitiş tarihi başlangıçtan önce olamaz.',
      });
    }

    if (values.kind === 'ShortDay') {
      if (values.start === '' || values.end === '') {
        ctx.addIssue({
          code: 'custom',
          path: ['end'],
          message: 'Başlangıç ve bitiş saatini girin.',
        });
      } else if (values.end <= values.start) {
        ctx.addIssue({
          code: 'custom',
          path: ['end'],
          message: 'Bitiş saati başlangıçtan sonra olmalı.',
        });
      }
    }
  });

type FormValues = z.infer<typeof schema>;

export interface AddSpecialDayModalProps {
  branchId: string;
  branchName: string;
  today: string;
  onClose: () => void;
}

/**
 * Özel gün ekle: kapalı gün (tek veya çok günlü, "Ramazan Bayramı · 3 gün") ya da o gün için özel saat (kısa gün).
 * "Yalnızca bu gün için aç" takvimde (Dilim 2, TakvimKapali). Açıkken bağlanır; her açılışta form boştur.
 */
export function AddSpecialDayModal({
  branchId,
  branchName,
  today,
  onClose,
}: AddSpecialDayModalProps) {
  const toast = useToast();
  const create = useCreateOverride(branchId);
  const form = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: { kind: 'Closure', fromDate: '', toDate: '', start: '', end: '', note: '' },
  });
  const kind = useWatch({ control: form.control, name: 'kind' });
  const { errors } = form.formState;

  const onSubmit = form.handleSubmit(async (values) => {
    const closed = values.kind === 'Closure';
    try {
      await create.mutateAsync({
        fromDate: values.fromDate,
        ...(closed && values.toDate !== '' && values.toDate !== values.fromDate
          ? { toDate: values.toDate }
          : {}),
        kind: values.kind,
        isClosed: closed,
        intervals: closed ? [] : [{ start: values.start, end: values.end }],
        note: values.note === '' ? null : values.note,
      });
      toast.show({ tone: 'basari', title: 'Özel gün eklendi' });
      onClose();
    } catch (error) {
      toast.show({ tone: 'hata', title: describeSaveError(error).message });
    }
  });

  return (
    <Modal
      open
      onClose={onClose}
      title="Özel gün ekle"
      description={`${branchName} şubesi`}
      footer={
        <>
          <Button onClick={onClose}>Vazgeç</Button>
          <Button
            variant="primary"
            type="submit"
            form="ozel-gun"
            loading={create.isPending}
            loadingLabel="Ekleniyor…"
          >
            Özel günü ekle
          </Button>
        </>
      }
    >
      <form id="ozel-gun" className={styles.grid} noValidate onSubmit={(e) => void onSubmit(e)}>
        <Select className={styles.full} label="Tür" options={kinds} {...form.register('kind')} />
        <TextField
          label={kind === 'Closure' ? 'Başlangıç tarihi' : 'Tarih'}
          type="date"
          min={today}
          error={errors.fromDate?.message}
          {...form.register('fromDate')}
        />
        {kind === 'Closure' ? (
          <TextField
            label="Bitiş tarihi"
            type="date"
            min={today}
            hint="Tek günse boş bırakın."
            error={errors.toDate?.message}
            {...form.register('toDate')}
          />
        ) : (
          <div className={styles.interval}>
            <TextField
              className={styles.timeField}
              label="Açılış"
              type="time"
              {...form.register('start')}
            />
            <TextField
              className={styles.timeField}
              label="Kapanış"
              type="time"
              error={errors.end?.message}
              {...form.register('end')}
            />
          </div>
        )}
        <TextField
          className={styles.full}
          label="Not"
          placeholder="Ramazan Bayramı"
          maxLength={200}
          hint="Listede ve takvimde görünür."
          error={errors.note?.message}
          {...form.register('note')}
        />
      </form>
    </Modal>
  );
}
