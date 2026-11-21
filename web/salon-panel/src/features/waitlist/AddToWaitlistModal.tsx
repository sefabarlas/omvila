import { ApiProblem, idempotencyKey } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { formatDate, parseMobilePhone } from '@omvila/ui-web/format';
import {
  Banner,
  Button,
  Checkbox,
  Modal,
  Select,
  Skeleton,
  TextField,
  useToast,
} from '@omvila/ui-web/panel';
import { addDays, branchToday } from '@omvila/ui-web/time';
import { useQuery, useQueryClient } from '@tanstack/react-query';
import { Check } from 'lucide-react';
import { useRef, useState } from 'react';
import { useApi } from '@/app/api-context';
import { useClock } from '@/app/clock';
import { defaultTimeZone } from '@/app/config';
import { CustomerPicker } from '@/features/appointments/CustomerPicker';
import type { CustomerChoice } from '@/features/appointments/CustomerPicker';
import appointmentStyles from '@/features/appointments/NewAppointment.module.css';
import { calendarQuery } from '@/features/calendar/api';
import { customerQuery } from '@/features/customers/api';
import { servicesQuery } from '@/features/services/api';
import styles from '@/shared/ListPage.module.css';
import { useAddToWaitlist } from './api';
import type { WaitlistCreate, WaitlistTimeWindow } from './api';
import { timeWindowOptions } from './labels';

/** Sözleşme: en çok 5 hizmet, 14 gün. */
const maxServices = 5;
const dayCount = 14;
const anyStaff = '';

/**
 * "Listeye ekle" (manageAddToWaitlist; Source = Panel, müşteriye bildirim gitmez): müşteri (kayıtlı veya yeni),
 * hizmetler, personel ("Fark etmez" veya kişi + "Başka personel de olur"), günler (önümüzdeki 14 gün; seçilmezse
 * herhangi bir gün), saat dilimleri (seçilmezse her saat). Tasarımı olmayan pencere; yeni randevu desenleriyle (T-98).
 */
export function AddToWaitlistModal({
  branchId,
  onClose,
}: {
  branchId: string;
  onClose: () => void;
}) {
  const api = useApi();
  const clock = useClock();
  const toast = useToast();
  const queryClient = useQueryClient();
  const add = useAddToWaitlist();
  const today = branchToday(clock, defaultTimeZone);
  const [choice, setChoice] = useState<CustomerChoice>({ kind: 'search' });
  const [serviceIds, setServiceIds] = useState<string[]>([]);
  const [staffId, setStaffId] = useState(anyStaff);
  const [acceptOther, setAcceptOther] = useState(false);
  const [dates, setDates] = useState<string[]>([]);
  const [windows, setWindows] = useState<WaitlistTimeWindow[]>([]);
  const [note, setNote] = useState('');
  const [showErrors, setShowErrors] = useState(false);
  const [problem, setProblem] = useState<string | null>(null);
  const sent = useRef<{ key: string; body: string } | null>(null);

  const servicesData = useQuery(servicesQuery(api, branchId));
  const services = (servicesData.data?.items ?? []).filter((s) => s.isActive);
  const staff =
    useQuery(calendarQuery(api, { branchId, from: today, to: today })).data?.staff ?? [];
  const selected = services.filter((s) => serviceIds.includes(s.id));
  const offering = staff.filter((m) => selected.every((s) => s.staffIds.includes(m.staffId)));
  const days = Array.from({ length: dayCount }, (_, i) => addDays(today, i));

  const draftInvalid =
    choice.kind === 'new' &&
    (choice.draft.fullName.trim().length < 2 || parseMobilePhone(choice.draft.phone) === null);
  const errors = {
    customer: choice.kind === 'search' ? 'Müşteri seçin ya da yeni müşteri ekleyin.' : undefined,
    services: serviceIds.length === 0 ? 'En az bir hizmet seçin.' : undefined,
  };

  const toggle = <T,>(list: T[], value: T, max: number) =>
    list.includes(value)
      ? list.filter((v) => v !== value)
      : list.length < max
        ? [...list, value]
        : list;

  const keyFor = (body: WaitlistCreate) => {
    const text = JSON.stringify(body);
    if (sent.current?.body !== text) {
      sent.current = { key: idempotencyKey()['Idempotency-Key'], body: text };
    }

    return sent.current.key;
  };

  const submit = async () => {
    setShowErrors(true);
    if (errors.customer || errors.services || draftInvalid) {
      return;
    }

    const body: WaitlistCreate = {
      branchId,
      ...(choice.kind === 'existing'
        ? { brandCustomerId: choice.customer.id }
        : choice.kind === 'new'
          ? {
              newCustomer: {
                fullName: choice.draft.fullName.trim(),
                phone: choice.draft.phone.trim(),
              },
            }
          : {}),
      preferences: {
        serviceIds,
        preferredStaffId: staffId === anyStaff ? null : staffId,
        acceptOtherStaff: staffId === anyStaff ? true : acceptOther,
        dates: [...dates].sort(),
        timeWindows: timeWindowOptions.map((o) => o.value).filter((w) => windows.includes(w)),
      },
      ...(note.trim() !== '' ? { note: note.trim() } : {}),
    };
    setProblem(null);
    try {
      const created = await add.mutateAsync({ key: keyFor(body), body });
      toast.show({
        tone: 'basari',
        title: 'Listeye eklendi',
        message: `${created.customer.fullName} · ${created.serviceNames.join(', ')}`,
      });
      onClose();
    } catch (error) {
      if (
        error instanceof ApiProblem &&
        error.code === 'phone_already_exists' &&
        typeof error.meta.brandCustomerId === 'string'
      ) {
        const existing = await queryClient.query(customerQuery(api, error.meta.brandCustomerId));
        setChoice({ kind: 'existing', customer: existing });
        setProblem('Bu numara kayıtlı; kayıtlı müşteri seçildi. Bilgileri kontrol edip ekleyin.');
        return;
      }

      setProblem(errorMessage(error instanceof ApiProblem ? error.code : undefined));
    }
  };

  return (
    <Modal
      open
      onClose={onClose}
      title="Listeye ekle"
      description="Uygun saat boşalınca sıradaki kişiye teklif gider."
      size="lg"
      footer={
        <>
          <Button onClick={onClose}>Vazgeç</Button>
          <Button
            variant="primary"
            loading={add.isPending}
            loadingLabel="Ekleniyor…"
            onClick={() => void submit()}
          >
            Listeye ekle
          </Button>
        </>
      }
    >
      <div className={styles.form}>
        {problem ? <Banner tone="hata" title={problem} /> : null}
        <CustomerPicker
          value={choice}
          onChange={setChoice}
          today={today}
          error={showErrors ? errors.customer : undefined}
          draftErrors={
            showErrors && choice.kind === 'new'
              ? {
                  fullName:
                    choice.draft.fullName.trim().length < 2
                      ? 'Ad soyad en az 2 karakter olmalı.'
                      : undefined,
                  phone:
                    parseMobilePhone(choice.draft.phone) === null
                      ? 'Cep telefonu 10 haneli olmalı: 0532 901 44 18'
                      : undefined,
                }
              : {}
          }
          newCustomerHint="Müşteri kartı kayıtla birlikte açılır."
        />
        <ChipGroup
          id="bekleme-hizmetler"
          label="Hizmetler"
          loading={servicesData.isPending}
          empty="Henüz hizmet yok. Hizmetler sayfasından ekleyin."
          options={services.map((s) => ({ value: s.id, label: s.name }))}
          selected={serviceIds}
          max={maxServices}
          error={showErrors ? errors.services : undefined}
          onToggle={(value) => {
            setServiceIds((list) => toggle(list, value, maxServices));
          }}
        />
        <div className={appointmentStyles.row}>
          <Select
            label="Personel"
            value={offering.some((m) => m.staffId === staffId) ? staffId : anyStaff}
            options={[
              { value: anyStaff, label: 'Fark etmez' },
              ...offering.map((m) => ({ value: m.staffId, label: m.fullName })),
            ]}
            onChange={(event) => {
              setStaffId(event.target.value);
            }}
          />
          {staffId !== anyStaff && offering.some((m) => m.staffId === staffId) ? (
            <Checkbox
              label="Başka personel de olur"
              checked={acceptOther}
              onChange={(event) => {
                setAcceptOther(event.target.checked);
              }}
            />
          ) : null}
        </div>
        <ChipGroup
          id="bekleme-gunler"
          label="Günler"
          hint="Seçmezseniz herhangi bir gün uyar."
          options={days.map((d) => ({
            value: d,
            label: `${formatDate(d, 'weekdayShort')} ${formatDate(d, 'day')}`,
          }))}
          selected={dates}
          max={dayCount}
          onToggle={(value) => {
            setDates((list) => toggle(list, value, dayCount));
          }}
        />
        <ChipGroup
          id="bekleme-saatler"
          label="Saatler"
          hint="Seçmezseniz her saat uyar."
          options={timeWindowOptions}
          selected={windows}
          max={timeWindowOptions.length}
          onToggle={(value) => {
            setWindows((list) => toggle(list, value, timeWindowOptions.length));
          }}
        />
        <TextField
          label="Not (isteğe bağlı)"
          maxLength={300}
          value={note}
          onChange={(event) => {
            setNote(event.target.value);
          }}
        />
      </div>
    </Modal>
  );
}

interface ChipGroupProps<T extends string> {
  id: string;
  label: string;
  hint?: string;
  loading?: boolean;
  empty?: string;
  options: readonly { value: T; label: string }[];
  selected: readonly T[];
  max: number;
  error?: string | undefined;
  onToggle: (value: T) => void;
}

/** Çok seçimli çipler (yeni randevudaki hizmet çipleriyle aynı görünüm). */
function ChipGroup<T extends string>({
  id,
  label,
  hint,
  loading = false,
  empty,
  options,
  selected,
  max,
  error,
  onToggle,
}: ChipGroupProps<T>) {
  return (
    <div className={appointmentStyles.section} role="group" aria-labelledby={id}>
      <h3 id={id} className={appointmentStyles.label}>
        {label}
      </h3>
      {loading ? (
        <Skeleton variant="block" height="36px" />
      ) : options.length === 0 && empty ? (
        <p className={appointmentStyles.subLabel}>{empty}</p>
      ) : (
        <div className={appointmentStyles.chips}>
          {options.map((option) => {
            const on = selected.includes(option.value);
            return (
              <button
                key={option.value}
                type="button"
                className={appointmentStyles.chip}
                aria-pressed={on}
                disabled={!on && selected.length >= max}
                onClick={() => {
                  onToggle(option.value);
                }}
              >
                {on ? <Check size={14} aria-hidden="true" /> : null}
                {option.label}
              </button>
            );
          })}
        </div>
      )}
      {hint ? <p className={appointmentStyles.subLabel}>{hint}</p> : null}
      {error ? <p className={appointmentStyles.error}>{error}</p> : null}
    </div>
  );
}
