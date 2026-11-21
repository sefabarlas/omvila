import { ApiProblem, idempotencyKey } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { formatDuration, parseMobilePhone } from '@omvila/ui-web/format';
import { Banner, Button, Modal, Skeleton, TextField, useToast } from '@omvila/ui-web/panel';
import { branchToday } from '@omvila/ui-web/time';
import { useQuery } from '@tanstack/react-query';
import { Check } from 'lucide-react';
import { useRef, useState } from 'react';
import { useApi } from '@/app/api-context';
import { useClock } from '@/app/clock';
import { defaultTimeZone } from '@/app/config';
import appointmentStyles from '@/features/appointments/NewAppointment.module.css';
import { calendarQuery } from '@/features/calendar/api';
import { servicesQuery } from '@/features/services/api';
import styles from '@/shared/ListPage.module.css';
import { useCreateWalkIn } from './api';
import type { WalkInCreate, WalkInQueue } from './api';

const anyStaff = '';

/**
 * "Sıraya ekle" (manageCreateWalkIn; alanlar isletme-uygulamasi/SiraEkle): ad, telefon (isteğe bağlı; girilirse sırası
 * yaklaşınca SMS), hizmet, personel ("Fark etmez"). Başlıktaki numara gösterimdir; asıl numara yanıttan gelir ve
 * bildirimde yazar. Aynı gövde aynı Idempotency-Key ile gider.
 */
export function AddWalkInModal({
  branchId,
  queue,
  onClose,
}: {
  branchId: string;
  queue: WalkInQueue | undefined;
  onClose: () => void;
}) {
  const api = useApi();
  const clock = useClock();
  const toast = useToast();
  const create = useCreateWalkIn();
  const today = branchToday(clock, queue?.timeZone ?? defaultTimeZone);
  const [fullName, setFullName] = useState('');
  const [phone, setPhone] = useState('');
  const [serviceId, setServiceId] = useState<string | null>(null);
  const [staffId, setStaffId] = useState(anyStaff);
  const [showErrors, setShowErrors] = useState(false);
  const [problem, setProblem] = useState<string | null>(null);
  const sent = useRef<{ key: string; body: string } | null>(null);

  const servicesData = useQuery(servicesQuery(api, branchId));
  const services = (servicesData.data?.items ?? []).filter((s) => s.isActive);
  const staff =
    useQuery(calendarQuery(api, { branchId, from: today, to: today })).data?.staff ?? [];
  const service = services.find((s) => s.id === serviceId);
  const offering = service ? staff.filter((m) => service.staffIds.includes(m.staffId)) : staff;
  const effectiveStaff = offering.some((m) => m.staffId === staffId) ? staffId : anyStaff;

  const nameError = fullName.trim().length < 2 ? 'Ad en az 2 karakter olmalı.' : undefined;
  const phoneError =
    phone.trim() !== '' && parseMobilePhone(phone) === null
      ? 'Cep telefonu 10 haneli olmalı: 0532 123 45 67'
      : undefined;

  const submit = async () => {
    setShowErrors(true);
    if (nameError || phoneError) {
      return;
    }

    const body: WalkInCreate = {
      branchId,
      fullName: fullName.trim(),
      ...(phone.trim() !== '' ? { phone: phone.trim() } : {}),
      serviceId,
      preferredStaffId: effectiveStaff === anyStaff ? null : effectiveStaff,
    };
    const text = JSON.stringify(body);
    if (sent.current?.body !== text) {
      sent.current = { key: idempotencyKey()['Idempotency-Key'], body: text };
    }

    setProblem(null);
    try {
      const ticket = await create.mutateAsync({ key: sent.current.key, body });
      toast.show({
        tone: 'basari',
        title: `Sıraya eklendi · No ${String(ticket.queueNumber)}`,
        message: ticket.phoneMasked
          ? `${ticket.customerName}; sırası yaklaşınca SMS gider.`
          : ticket.customerName,
      });
      onClose();
    } catch (error) {
      setProblem(errorMessage(error instanceof ApiProblem ? error.code : undefined));
    }
  };

  const estimate = queue?.estimatedWaitMinutesForNew;
  return (
    <Modal
      open
      onClose={onClose}
      title="Sıraya ekle"
      {...(queue ? { description: `Sıradaki numara: No ${String(queue.nextQueueNumber)}` } : {})}
      footer={
        <>
          <Button onClick={onClose}>Vazgeç</Button>
          <Button
            variant="primary"
            loading={create.isPending}
            loadingLabel="Ekleniyor…"
            onClick={() => void submit()}
          >
            Sıraya ekle
          </Button>
        </>
      }
    >
      <div className={styles.form}>
        {problem ? <Banner tone="hata" title={problem} /> : null}
        <TextField
          label="Ad"
          autoComplete="off"
          maxLength={120}
          value={fullName}
          hint="Telefon isteğe bağlı; girilirse sıra gelince SMS gider"
          error={showErrors ? nameError : undefined}
          onChange={(event) => {
            setFullName(event.target.value);
          }}
        />
        <TextField
          label="Telefon (isteğe bağlı)"
          type="tel"
          inputMode="tel"
          autoComplete="off"
          placeholder="0532 123 45 67"
          value={phone}
          error={showErrors ? phoneError : undefined}
          onChange={(event) => {
            setPhone(event.target.value);
          }}
        />
        <SingleChoice
          id="sira-hizmet"
          label="Hizmet"
          loading={servicesData.isPending}
          options={services.map((s) => ({ value: s.id, label: s.name }))}
          value={serviceId}
          onChange={setServiceId}
        />
        <SingleChoice
          id="sira-personel"
          label="Personel"
          options={[
            { value: anyStaff, label: 'Fark etmez' },
            ...offering.map((m) => ({ value: m.staffId, label: m.fullName.split(' ')[0] ?? '' })),
          ]}
          value={effectiveStaff}
          required
          onChange={(value) => {
            setStaffId(value ?? anyStaff);
          }}
        />
        {queue && estimate !== null && estimate !== undefined ? (
          <p className={styles.note}>
            {`Tahmini bekleme ~${formatDuration(estimate)} · önünde ${String(queue.waitingCount)} kişi var.`}
          </p>
        ) : null}
      </div>
    </Modal>
  );
}

/** Tek seçimli çipler; `required` değilse seçili çipe yeniden basmak seçimi kaldırır. */
function SingleChoice({
  id,
  label,
  loading = false,
  options,
  value,
  required = false,
  onChange,
}: {
  id: string;
  label: string;
  loading?: boolean;
  options: readonly { value: string; label: string }[];
  value: string | null;
  required?: boolean;
  onChange: (value: string | null) => void;
}) {
  return (
    <div className={appointmentStyles.section} role="group" aria-labelledby={id}>
      <h3 id={id} className={appointmentStyles.label}>
        {label}
      </h3>
      {loading ? (
        <Skeleton variant="block" height="36px" />
      ) : (
        <div className={appointmentStyles.chips}>
          {options.map((option) => {
            const on = option.value === value;
            return (
              <button
                key={option.value}
                type="button"
                className={appointmentStyles.chip}
                aria-pressed={on}
                onClick={() => {
                  onChange(on && !required ? null : option.value);
                }}
              >
                {on ? <Check size={14} aria-hidden="true" /> : null}
                {option.label}
              </button>
            );
          })}
        </div>
      )}
    </div>
  );
}
