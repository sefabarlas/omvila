import { zodResolver } from '@hookform/resolvers/zod';
import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { Banner, Button, Modal, Select, Switch, TextField, useToast } from '@omvila/ui-web/panel';
import { localDateTimeToUtc } from '@omvila/ui-web/time';
import { useState } from 'react';
import { useForm, useWatch } from 'react-hook-form';
import { z } from 'zod';
import { useCreateTimeOff } from './api';
import type { StaffSummary } from './api';
import styles from './Staff.module.css';

const types = [
  { value: 'Leave', label: 'Yıllık izin' },
  { value: 'Sick', label: 'Hastalık' },
  { value: 'Training', label: 'Eğitim' },
  { value: 'Break', label: 'Mola' },
  { value: 'Other', label: 'Diğer' },
] as const;

const schema = z
  .object({
    staffId: z.string().min(1),
    type: z.enum(['Leave', 'Sick', 'Training', 'Break', 'Other']),
    fromDate: z.string().min(1, 'Başlangıç tarihini seçin.'),
    toDate: z.string(),
    allDay: z.boolean(),
    start: z.string(),
    end: z.string(),
    note: z.string().trim().max(200),
  })
  .superRefine((v, ctx) => {
    if (v.allDay && v.toDate !== '' && v.toDate < v.fromDate) {
      ctx.addIssue({
        code: 'custom',
        path: ['toDate'],
        message: 'Bitiş başlangıçtan önce olamaz.',
      });
    }

    if (!v.allDay && (v.start === '' || v.end === '' || v.end <= v.start)) {
      ctx.addIssue({
        code: 'custom',
        path: ['end'],
        message: 'Bitiş saati başlangıçtan sonra olmalı.',
      });
    }
  });

type FormValues = z.infer<typeof schema>;

/**
 * İzin veya tatil ekle (salon-paneli/IzinEkle, kabul kriteri 12). Randevular iptal edilmez; yanıttaki etkilenen
 * randevular şeritte sayılır. İzin bütün şubelerde geçerlidir. Saatli izin şubenin saat diliminden UTC'ye çevrilir.
 */
export function TimeOffModal({
  staff,
  initialStaffId,
  timeZone,
  today,
  onClose,
}: {
  staff: readonly StaffSummary[];
  initialStaffId: string;
  timeZone: string;
  today: string;
  onClose: () => void;
}) {
  const toast = useToast();
  const create = useCreateTimeOff();
  const [affected, setAffected] = useState<number>();
  const form = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      staffId: initialStaffId,
      type: 'Leave',
      fromDate: '',
      toDate: '',
      allDay: true,
      start: '',
      end: '',
      note: '',
    },
  });
  const { errors } = form.formState;
  const allDay = useWatch({ control: form.control, name: 'allDay' });

  const onSubmit = form.handleSubmit(async (v) => {
    try {
      const result = await create.mutateAsync({
        staffId: v.staffId,
        body: v.allDay
          ? {
              type: v.type,
              isAllDay: true,
              fromDate: v.fromDate,
              toDate: v.toDate === '' ? v.fromDate : v.toDate,
              note: v.note === '' ? null : v.note,
            }
          : {
              type: v.type,
              isAllDay: false,
              startUtc: localDateTimeToUtc(v.fromDate, v.start, timeZone),
              endUtc: localDateTimeToUtc(v.fromDate, v.end, timeZone),
              note: v.note === '' ? null : v.note,
            },
      });
      if (result.affectedAppointments.length > 0) {
        setAffected(result.affectedAppointments.length);
        return;
      }

      toast.show({ tone: 'basari', title: 'İzin eklendi' });
      onClose();
    } catch (problem) {
      toast.show({
        tone: 'hata',
        title: errorMessage(problem instanceof ApiProblem ? problem.code : undefined),
      });
    }
  });

  const person = staff.find((s) => s.id === form.getValues('staffId'));

  return (
    <Modal
      open
      onClose={onClose}
      title="İzin veya tatil ekle"
      footer={
        affected !== undefined ? (
          <Button variant="primary" onClick={onClose}>
            Kapat
          </Button>
        ) : (
          <>
            <Button onClick={onClose}>Vazgeç</Button>
            <Button
              variant="primary"
              type="submit"
              form="izin-ekle"
              loading={create.isPending}
              loadingLabel="Kaydediliyor…"
            >
              İzni kaydet
            </Button>
          </>
        )
      }
    >
      <form id="izin-ekle" className={styles.form} noValidate onSubmit={(e) => void onSubmit(e)}>
        <Select
          label="Personel"
          options={staff.map((s) => ({ value: s.id, label: s.displayName ?? s.fullName }))}
          {...form.register('staffId')}
        />
        <Select label="Tür" options={types} {...form.register('type')} />
        <TextField
          label={allDay ? 'Başlangıç' : 'Tarih'}
          type="date"
          min={today}
          error={errors.fromDate?.message}
          {...form.register('fromDate')}
        />
        {allDay ? (
          <TextField
            label="Bitiş"
            type="date"
            min={today}
            hint="Tek günse boş bırakın."
            error={errors.toDate?.message}
            {...form.register('toDate')}
          />
        ) : (
          <div className={styles.form}>
            <TextField label="Başlangıç saati" type="time" {...form.register('start')} />
            <TextField
              label="Bitiş saati"
              type="time"
              error={errors.end?.message}
              {...form.register('end')}
            />
          </div>
        )}
        <div className={styles.full}>
          <Switch label="Tüm gün" {...form.register('allDay')} />
        </div>
        <TextField
          className={styles.full}
          label="Not (isteğe bağlı)"
          maxLength={200}
          {...form.register('note')}
        />
        {affected !== undefined ? (
          <div className={styles.full}>
            <Banner
              tone="uyari"
              title={`Bu tarihlerde ${person?.fullName ?? 'kişinin'} ${String(affected)} randevusu var.`}
            >
              İzin kaydedildi. Her randevuyu takvimden başka personele taşıyın veya müşteriyle iptal
              edin.
            </Banner>
          </div>
        ) : null}
      </form>
    </Modal>
  );
}
