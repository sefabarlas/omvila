import { zodResolver } from '@hookform/resolvers/zod';
import { ApiProblem, idempotencyKey } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { parseMobilePhone } from '@omvila/ui-web/format';
import { Banner, Button, Checkbox, Modal, TextField, useToast } from '@omvila/ui-web/panel';
import { branchToday } from '@omvila/ui-web/time';
import { useQuery } from '@tanstack/react-query';
import { useState } from 'react';
import { useForm, useWatch } from 'react-hook-form';
import { z } from 'zod';
import { useApi } from '@/app/api-context';
import { useClock } from '@/app/clock';
import { defaultTimeZone } from '@/app/config';
import { customerQuery, useCreateCustomer } from './api';
import styles from './Customers.module.css';
import { inlineDayLabel } from './labels';

export interface CustomerPrefill {
  fullName?: string;
  phone?: string;
}

const phoneMessage = 'Cep telefonu 10 haneli olmalı: 0532 901 44 18';

function schemaFor(currentYear: number) {
  return z.object({
    fullName: z.string().trim().min(2, 'Ad soyad en az 2 karakter olmalı.').max(120),
    phone: z
      .string()
      .trim()
      .refine((v) => parseMobilePhone(v) !== null, phoneMessage),
    birthYear: z
      .string()
      .trim()
      .refine(
        (v) => v === '' || (/^\d{4}$/.test(v) && Number(v) >= 1900 && Number(v) <= currentYear),
        `Doğum yılı 1900 ile ${String(currentYear)} arasında olmalı.`,
      ),
    notes: z.string().trim().max(2000),
    sendAppointmentSms: z.boolean(),
  });
}

type FormValues = z.infer<ReturnType<typeof schemaFor>>;

const formId = 'musteri-ekle';

/**
 * Müşteri ekle (salon-paneli/MusteriEkle, kabul kriteri 15). Doğum tarihi değil yalnız yıl (ürün kuralı 4); notta
 * sağlık bilgisi yazılmaz. Kampanya izni salondan verilmez, müşteriden alınır. Kayıtlı numarada 409
 * phone_already_exists → "Bu numara zaten kayıtlı" ve "Mevcut kartı aç". Pencere başına bir Idempotency-Key.
 */
export function AddCustomerModal({
  prefill,
  onClose,
  onOpenCard,
}: {
  prefill: CustomerPrefill;
  onClose: () => void;
  onOpenCard: (brandCustomerId: string) => void;
}) {
  const api = useApi();
  const clock = useClock();
  const toast = useToast();
  const create = useCreateCustomer();
  const [key] = useState(() => idempotencyKey()['Idempotency-Key']);
  // Kayıtlı numara: mevcut kartın kimliği (meta yoksa null: eşzamanlı ekleme, T-46 (7)).
  const [conflict, setConflict] = useState<{ id: string | null; phone: string } | null>(null);
  const today = branchToday(clock, defaultTimeZone);
  const form = useForm<FormValues>({
    resolver: zodResolver(schemaFor(Number(today.slice(0, 4)))),
    defaultValues: {
      fullName: prefill.fullName ?? '',
      phone: prefill.phone ?? '',
      birthYear: '',
      notes: '',
      sendAppointmentSms: true,
    },
  });
  const { errors } = form.formState;
  const phone = useWatch({ control: form.control, name: 'phone' });
  // Numara değişince uyarı kalkar; yeni numarayla tekrar denenebilir.
  const existing = conflict?.phone === phone.trim() ? conflict : null;
  const existingQuery = useQuery({
    ...customerQuery(api, existing?.id ?? ''),
    enabled: !!existing?.id,
  });

  const onSubmit = form.handleSubmit(async (values) => {
    try {
      const created = await create.mutateAsync({
        key,
        body: {
          fullName: values.fullName,
          phone: values.phone,
          birthYear: values.birthYear === '' ? null : Number(values.birthYear),
          notes: values.notes === '' ? null : values.notes,
          sendAppointmentSms: values.sendAppointmentSms,
        },
      });
      toast.show({ tone: 'basari', title: `${created.fullName} eklendi` });
      onOpenCard(created.id);
    } catch (problem) {
      if (problem instanceof ApiProblem && problem.code === 'phone_already_exists') {
        const id = problem.meta.brandCustomerId;
        setConflict({ id: typeof id === 'string' ? id : null, phone: values.phone });
        return;
      }

      if (problem instanceof ApiProblem && problem.errors.phone) {
        form.setError('phone', { message: phoneMessage });
        return;
      }

      if (problem instanceof ApiProblem && problem.errors.birthYear) {
        form.setError('birthYear', { message: 'Doğum yılını kontrol edin.' });
        return;
      }

      toast.show({
        tone: 'hata',
        title: errorMessage(problem instanceof ApiProblem ? problem.code : undefined),
      });
    }
  });

  const match = existingQuery.data;
  const matchLine = match
    ? [
        match.fullName,
        `${String(match.visitCount)} ziyaret`,
        match.lastVisitOn ? `son ${inlineDayLabel(match.lastVisitOn, today)}` : null,
      ]
        .filter(Boolean)
        .join(', ')
    : null;

  return (
    <Modal
      open
      onClose={onClose}
      title="Müşteri ekle"
      footer={
        <>
          <Button onClick={onClose}>Vazgeç</Button>
          {existing?.id ? (
            <Button
              variant="primary"
              onClick={() => {
                if (existing.id) {
                  onOpenCard(existing.id);
                }
              }}
            >
              Mevcut kartı aç
            </Button>
          ) : (
            <Button
              variant="primary"
              type="submit"
              form={formId}
              loading={create.isPending}
              loadingLabel="Ekleniyor…"
            >
              Müşteriyi ekle
            </Button>
          )}
        </>
      }
    >
      <form id={formId} className={styles.form} noValidate onSubmit={(e) => void onSubmit(e)}>
        <TextField
          label="Ad soyad"
          placeholder="Tülin Aksoy"
          autoComplete="off"
          error={errors.fullName?.message}
          {...form.register('fullName')}
        />
        <TextField
          label="Cep telefonu"
          type="tel"
          inputMode="tel"
          placeholder="0532 901 44 18"
          autoComplete="off"
          error={errors.phone?.message}
          {...form.register('phone')}
        />
        {existing ? (
          <div className={styles.full}>
            <Banner tone="uyari" title="Bu numara zaten kayıtlı.">
              {existing.id
                ? `${matchLine ? `${matchLine}. ` : ''}Yeni kayıt açılmaz, mevcut karta gidersiniz.`
                : 'Yeni kayıt açılmaz. Müşteriyi listede numarasıyla arayabilirsiniz.'}
            </Banner>
          </div>
        ) : null}
        <TextField
          label="Doğum yılı (isteğe bağlı)"
          inputMode="numeric"
          placeholder="1994"
          maxLength={4}
          autoComplete="off"
          error={errors.birthYear?.message}
          {...form.register('birthYear')}
        />
        <TextField
          className={styles.full}
          label="Not (isteğe bağlı)"
          placeholder="Tercih edilen ürün, renk kodu…"
          hint="Sağlık bilgisi yazmayın."
          maxLength={2000}
          autoComplete="off"
          error={errors.notes?.message}
          {...form.register('notes')}
        />
        <div className={styles.full}>
          <Checkbox
            label="Randevu bilgilendirme SMS'leri gönderilsin"
            description="Onay, hatırlatma ve değişiklik mesajları."
            {...form.register('sendAppointmentSms')}
          />
        </div>
        <p className={styles.formNote}>
          Kampanya mesajı izni müşteriden alınır. Müşteri ilk randevusunda SMS ile onay verebilir.
        </p>
      </form>
    </Modal>
  );
}
