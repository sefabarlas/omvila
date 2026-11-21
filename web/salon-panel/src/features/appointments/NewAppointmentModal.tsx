import { ApiProblem, idempotencyKey } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { formatDate, formatDuration, formatMoney, parseMobilePhone } from '@omvila/ui-web/format';
import {
  Banner,
  Button,
  Modal,
  Select,
  Skeleton,
  Switch,
  TextField,
  useToast,
} from '@omvila/ui-web/panel';
import {
  branchToday,
  formatClock,
  localDateTimeToUtc,
  localMinutesOfDay,
  parseClock,
  toBranchDate,
} from '@omvila/ui-web/time';
import { useQuery, useQueryClient } from '@tanstack/react-query';
import { Check, Plus } from 'lucide-react';
import { useRef, useState } from 'react';
import { useApi } from '@/app/api-context';
import { useClock } from '@/app/clock';
import { defaultTimeZone } from '@/app/config';
import { useSession } from '@/app/session';
import { calendarQuery } from '@/features/calendar/api';
import { customerQuery } from '@/features/customers/api';
import { servicesQuery } from '@/features/services/api';
import { OutsideHoursDialog } from '@/shared/OutsideHoursDialog';
import { slotsQuery, useCreateAppointment } from './api';
import type { AppointmentCreate } from './api';
import { CustomerPicker } from './CustomerPicker';
import type { CustomerChoice, PickedCustomer } from './CustomerPicker';
import { channelOptions, parseRepeat, repeatOptions, skippedMessage } from './labels';
import type { PanelChannel, RepeatWeeks } from './labels';
import styles from './NewAppointment.module.css';
import { SlotGrid } from './SlotGrid';

export interface NewAppointmentPrefill {
  /** Takvimde tıklanan kolonun personeli. */
  staffId?: string | undefined;
  /** Şubenin yerel günü ("2026-11-25"); yoksa şubede bugün. */
  date?: string | undefined;
  /** Yerel saat ("14:30"). */
  time?: string | undefined;
  /** Müşteri kartındaki "Randevu ver". */
  customer?: PickedCustomer | undefined;
}

/** Hizmet çiplerinde "+ Diğer" öncesi gösterilen sayı (tasarım: dört çip). */
const visibleChips = 4;
/** Sözleşme: bir randevuda en çok 5 kalem. */
const maxItems = 5;
const phoneMessage = 'Cep telefonu 10 haneli olmalı: 0532 901 44 18';
const isClock = (value: string) => /^\d{2}:\d{2}$/.test(value);

type Problem =
  | { kind: 'slot'; time: string; alternatives: string[] }
  | { kind: 'readOnly' }
  | { kind: 'existingPhone' }
  | { kind: 'error'; code: string | undefined };

/**
 * Yeni randevu (salon-paneli/YeniRandevu, kabul kriterleri 11 ve 13): müşteri, hizmetler, personel, "Nasıl geldi?",
 * boş saatler, tekrar, not, "Müşteriye SMS gönder". Panel randevusu Confirmed oluşur. Çalışma saati dışında önce 409
 * gelir, onaylanınca allowOutsideWorkingHours ile tekrar gönderilir; 409 slot_taken "Saat az önce doldu" ve
 * alternatifler; salt okunur abonelikte 403 subscription_read_only. Aynı gövde aynı Idempotency-Key ile gider.
 */
export function NewAppointmentModal({
  branchId,
  prefill = {},
  onClose,
}: {
  branchId: string;
  prefill?: NewAppointmentPrefill;
  onClose: () => void;
}) {
  const api = useApi();
  const clock = useClock();
  const toast = useToast();
  const queryClient = useQueryClient();
  const { session } = useSession();
  const create = useCreateAppointment();
  const [choice, setChoice] = useState<CustomerChoice>(
    prefill.customer ? { kind: 'existing', customer: prefill.customer } : { kind: 'search' },
  );
  const [serviceIds, setServiceIds] = useState<string[]>([]);
  const [showAllServices, setShowAllServices] = useState(false);
  const [staffId, setStaffId] = useState(prefill.staffId ?? session.membership.staffId ?? '');
  const [channel, setChannel] = useState<PanelChannel>('Phone');
  const [date, setDate] = useState(prefill.date ?? branchToday(clock, defaultTimeZone));
  const [time, setTime] = useState<string | null>(prefill.time ?? null);
  const [customTime, setCustomTime] = useState(false);
  const [repeat, setRepeat] = useState<RepeatWeeks>(0);
  const [note, setNote] = useState('');
  const [notify, setNotify] = useState(true);
  const [showErrors, setShowErrors] = useState(false);
  const [problem, setProblem] = useState<Problem | null>(null);
  const [confirm, setConfirm] = useState<{ code: string; body: AppointmentCreate } | null>(null);
  // Pencere başına bir anahtar; gövde değişirse (ör. onaydan sonra) yeni anahtar (aynı anahtar + farklı gövde 422).
  const sent = useRef<{ key: string; body: string } | null>(null);

  const servicesData = useQuery(servicesQuery(api, branchId));
  const services = (servicesData.data?.items ?? [])
    .filter((service) => service.isActive)
    .sort((a, b) => (a.sortOrder ?? 0) - (b.sortOrder ?? 0));
  const selected = serviceIds
    .map((id) => services.find((service) => service.id === id))
    .filter((service) => service !== undefined);
  // Personel listesi o günün takviminden (personel rolünde sunucu yalnız kendi kolonunu döner).
  const day = useQuery(calendarQuery(api, { branchId, from: date, to: date }));
  const staff = day.data?.staff ?? [];
  const offering = staff.filter((member) =>
    selected.every((service) => service.staffIds.includes(member.staffId)),
  );
  const chosenStaff = offering.find((member) => member.staffId === staffId) ?? offering[0];
  const effectiveStaffId = chosenStaff?.staffId ?? '';
  const staffName = chosenStaff?.fullName ?? '';
  const slots = useQuery({
    ...slotsQuery(api, branchId, date, serviceIds, effectiveStaffId),
    enabled: serviceIds.length > 0 && effectiveStaffId !== '',
  });
  const timeZone = slots.data?.timeZone ?? day.data?.timeZone ?? defaultTimeZone;
  const slot = time ? slots.data?.slots.find((s) => s.localTime === time) : undefined;
  // Listede olmayan saat (takvimde tıklanan saat, çalışma saati dışı): elle yazılır.
  const offered = time !== null && slots.data?.slots.some((s) => s.localTime === time) === true;
  const showCustomTime = customTime || (time !== null && slots.isSuccess && !offered);
  const duration = selected.reduce((sum, service) => sum + service.durationMinutes, 0);
  const endTime =
    slot?.endLocalTime ??
    (time && isClock(time) ? formatClock((parseClock(time) + duration) % (24 * 60)) : null);
  const total = slot?.total ?? {
    amountMinor: selected.reduce((sum, service) => sum + service.price.amountMinor, 0),
    currency: selected[0]?.price.currency ?? 'TRY',
  };

  const draftErrors =
    showErrors && choice.kind === 'new'
      ? {
          fullName:
            choice.draft.fullName.trim().length < 2
              ? 'Ad soyad en az 2 karakter olmalı.'
              : undefined,
          phone: parseMobilePhone(choice.draft.phone) === null ? phoneMessage : undefined,
        }
      : {};
  const errors = {
    customer: choice.kind === 'search' ? 'Müşteri seçin ya da yeni müşteri ekleyin.' : undefined,
    services: selected.length === 0 ? 'En az bir hizmet seçin.' : undefined,
    staff: effectiveStaffId === '' ? 'Seçilen hizmetleri veren personel yok.' : undefined,
    time: time === null || !isClock(time) ? 'Saat seçin.' : undefined,
  };
  const invalid =
    Object.values(errors).some(Boolean) ||
    (choice.kind === 'new' &&
      (choice.draft.fullName.trim().length < 2 || parseMobilePhone(choice.draft.phone) === null));

  const summaryTitle =
    time && endTime
      ? `${formatDate(date, 'compact')} · ${time}–${endTime}`
      : formatDate(date, 'compact');
  const summaryText =
    selected.length > 0 && time
      ? [selected.map((s) => s.name).join(', '), staffName, formatMoney(total)]
          .filter(Boolean)
          .join(' · ')
      : 'Hizmet ve saat seçin';
  const toggleService = (id: string) => {
    setProblem(null);
    setServiceIds((current) =>
      current.includes(id)
        ? current.filter((value) => value !== id)
        : current.length < maxItems
          ? [...current, id]
          : current,
    );
  };

  const keyFor = (body: AppointmentCreate) => {
    const text = JSON.stringify(body);
    if (sent.current?.body === text) {
      return sent.current.key;
    }

    const key = idempotencyKey()['Idempotency-Key'];
    sent.current = { key, body: text };
    return key;
  };

  const send = async (body: AppointmentCreate) => {
    setProblem(null);
    try {
      const created = await create.mutateAsync({ key: keyFor(body), body });
      const skipped = created.skippedOccurrences;
      toast.show(
        skipped.length > 0
          ? { tone: 'uyari', title: 'Randevu oluşturuldu', message: skippedMessage(skipped) }
          : {
              tone: 'basari',
              title: 'Randevu oluşturuldu',
              message: `${created.appointment.customer.fullName} · ${summaryTitle}`,
            },
      );
      onClose();
    } catch (error) {
      if (!(error instanceof ApiProblem)) {
        setProblem({ kind: 'error', code: undefined });
        return;
      }

      if (error.code === 'outside_working_hours' || error.code === 'staff_on_time_off') {
        setConfirm({ code: error.code, body });
      } else if (error.code === 'slot_taken') {
        const alternatives = Array.isArray(error.meta.alternatives)
          ? error.meta.alternatives.filter((value): value is string => typeof value === 'string')
          : [];
        setProblem({ kind: 'slot', time: time ?? '', alternatives });
        void slots.refetch();
      } else if (error.code === 'subscription_read_only') {
        setProblem({ kind: 'readOnly' });
      } else if (
        error.code === 'phone_already_exists' &&
        typeof error.meta.brandCustomerId === 'string'
      ) {
        const existing = await queryClient.query(customerQuery(api, error.meta.brandCustomerId));
        setChoice({ kind: 'existing', customer: existing });
        setProblem({ kind: 'existingPhone' });
      } else {
        setProblem({ kind: 'error', code: error.code });
      }
    }
  };

  const submit = () => {
    setShowErrors(true);
    if (invalid || time === null) {
      return;
    }

    const body: AppointmentCreate = {
      branchId,
      channel,
      startUtc: slot?.startUtc ?? localDateTimeToUtc(date, time, timeZone),
      // Üretilen tip allOf + required'ı `& Record<string, never>` olarak çıkarıyor; gövde sözleşmeye uygundur.
      items: serviceIds.map((serviceId) => ({
        serviceId,
        staffId: effectiveStaffId,
      })) as AppointmentCreate['items'],
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
      ...(note.trim() !== '' ? { staffNote: note.trim() } : {}),
      ...(repeat !== 0 && channel !== 'WalkIn'
        ? { recurrence: { frequency: 'Weekly' as const, intervalWeeks: repeat } }
        : {}),
      notifyCustomer: notify,
    };
    void send(body);
  };

  const pickAlternative = (instantUtc: string) => {
    setDate(toBranchDate(new Date(instantUtc), timeZone));
    setTime(localTimeOf(instantUtc, timeZone));
    setProblem(null);
  };

  const chips = showAllServices
    ? services
    : services.filter((s, i) => i < visibleChips || serviceIds.includes(s.id));
  const firstName = staffName.split(' ')[0] ?? staffName;

  return (
    <>
      <Modal
        open={confirm === null}
        onClose={onClose}
        title="Yeni randevu"
        size="lg"
        footer={
          <>
            <div className={styles.summary} aria-live="polite">
              <span className={styles.summaryTitle}>{summaryTitle}</span>
              <span className={styles.summaryText}>{summaryText}</span>
            </div>
            <Button onClick={onClose}>Vazgeç</Button>
            <Button
              variant="primary"
              loading={create.isPending}
              loadingLabel="Oluşturuluyor…"
              onClick={submit}
            >
              Randevu oluştur
            </Button>
          </>
        }
      >
        {problem ? (
          <ProblemBanner
            problem={problem}
            timeZone={timeZone}
            onPick={pickAlternative}
            onDismiss={() => {
              setProblem(null);
            }}
          />
        ) : null}
        <CustomerPicker
          value={choice}
          onChange={(value) => {
            setChoice(value);
            if (problem?.kind === 'existingPhone') {
              setProblem(null);
            }
          }}
          today={branchToday(clock, timeZone)}
          error={showErrors ? errors.customer : undefined}
          draftErrors={draftErrors}
        />

        <div className={styles.section} role="group" aria-labelledby="yeni-randevu-hizmetler">
          <h3 id="yeni-randevu-hizmetler" className={styles.label}>
            Hizmetler
          </h3>
          {servicesData.isPending ? (
            <Skeleton variant="block" height="36px" />
          ) : services.length === 0 ? (
            <p className={styles.subLabel}>Henüz hizmet yok. Hizmetler sayfasından ekleyin.</p>
          ) : (
            <div className={styles.chips}>
              {chips.map((service) => {
                const on = serviceIds.includes(service.id);
                return (
                  <button
                    key={service.id}
                    type="button"
                    className={styles.chip}
                    aria-pressed={on}
                    disabled={!on && serviceIds.length >= maxItems}
                    onClick={() => {
                      toggleService(service.id);
                    }}
                  >
                    {on ? <Check size={14} aria-hidden="true" /> : null}
                    {`${service.name} · ${formatDuration(service.durationMinutes)}`}
                  </button>
                );
              })}
              {!showAllServices && services.length > chips.length ? (
                <button
                  type="button"
                  className={styles.chip}
                  aria-expanded={false}
                  onClick={() => {
                    setShowAllServices(true);
                  }}
                >
                  <Plus size={14} aria-hidden="true" />
                  Diğer
                </button>
              ) : null}
            </div>
          )}
          {showErrors && errors.services ? <p className={styles.error}>{errors.services}</p> : null}
        </div>

        <div className={styles.row}>
          <Select
            label="Personel"
            value={effectiveStaffId}
            disabled={offering.length === 0}
            options={
              offering.length > 0
                ? offering.map((member) => ({ value: member.staffId, label: member.fullName }))
                : [{ value: '', label: day.isPending ? 'Yükleniyor…' : 'Personel yok' }]
            }
            error={showErrors && selected.length > 0 ? errors.staff : undefined}
            onChange={(event) => {
              setStaffId(event.target.value);
            }}
          />
          <Select
            label="Nasıl geldi?"
            value={channel}
            options={channelOptions}
            onChange={(event) => {
              const next = channelOptions.find((o) => o.value === event.target.value)?.value;
              setChannel(next ?? 'Phone');
            }}
          />
        </div>

        <div className={styles.section} role="group" aria-labelledby="yeni-randevu-saatler">
          <div className={styles.actions}>
            <h3 id="yeni-randevu-saatler" className={styles.label}>
              {staffName
                ? `${formatDate(date, 'panel')} · ${firstName} için boş saatler`
                : `${formatDate(date, 'panel')} · boş saatler`}
            </h3>
            <TextField
              className={styles.customTime}
              label="Tarih"
              hideLabel
              type="date"
              value={date}
              onChange={(event) => {
                if (/^\d{4}-\d{2}-\d{2}$/.test(event.target.value)) {
                  setDate(event.target.value);
                  setProblem(null);
                }
              }}
            />
          </div>
          {serviceIds.length === 0 ? (
            <p className={styles.subLabel}>Boş saatleri görmek için hizmet seçin.</p>
          ) : effectiveStaffId === '' ? null : slots.isPending ? (
            <Skeleton variant="block" height="96px" />
          ) : slots.isError ? (
            <div className={styles.actions}>
              <p className={styles.error}>
                {errorMessage(slots.error instanceof ApiProblem ? slots.error.code : undefined)}
              </p>
              <Button size="sm" onClick={() => void slots.refetch()}>
                Tekrar dene
              </Button>
            </div>
          ) : slots.data.slots.length === 0 && slots.data.unavailableTimes.length === 0 ? (
            <p className={styles.subLabel}>{`Bu gün ${staffName} için boş saat yok.`}</p>
          ) : (
            <SlotGrid
              result={slots.data}
              legend={`${formatDate(date, 'panel')} · ${staffName} için boş saatler`}
              value={time}
              onChange={(value) => {
                setTime(value);
                setCustomTime(false);
                setProblem(null);
              }}
            />
          )}
          {showCustomTime ? (
            <TextField
              className={styles.customTime}
              label="Saat"
              type="time"
              step={300}
              hint="Listede olmayan saat"
              value={time ?? ''}
              onChange={(event) => {
                setTime(event.target.value === '' ? null : event.target.value);
                setProblem(null);
              }}
            />
          ) : serviceIds.length > 0 ? (
            <div>
              <Button
                size="sm"
                variant="ghost"
                onClick={() => {
                  setCustomTime(true);
                }}
              >
                Listede olmayan saat
              </Button>
            </div>
          ) : null}
          {showErrors && errors.time ? <p className={styles.error}>{errors.time}</p> : null}
        </div>

        <div className={styles.row}>
          <Select
            label="Tekrar"
            value={channel === 'WalkIn' ? '0' : String(repeat)}
            disabled={channel === 'WalkIn'}
            options={repeatOptions.map((o) => ({ value: String(o.value), label: o.label }))}
            hint={
              channel === 'WalkIn'
                ? 'Kapıdan gelen randevu tekrarlanmaz.'
                : 'Her hafta seçilirse ilk 3 ayın randevuları oluşturulur, sonrası otomatik eklenir.'
            }
            onChange={(event) => {
              setRepeat(parseRepeat(event.target.value));
            }}
          />
          <TextField
            label="Not (isteğe bağlı)"
            placeholder="Yalnız ekip görür"
            hint="Sağlık bilgisi yazmayın."
            maxLength={1000}
            autoComplete="off"
            value={note}
            onChange={(event) => {
              setNote(event.target.value);
            }}
          />
        </div>

        <Switch
          label="Müşteriye SMS gönder"
          description={
            notify
              ? 'Randevu bilgisi ve iptal bağlantısı'
              : 'Onay mesajı gitmez; hatırlatmalar yine gider.'
          }
          checked={notify}
          onChange={(event) => {
            setNotify(event.target.checked);
          }}
        />
      </Modal>
      {confirm ? (
        <OutsideHoursDialog
          code={confirm.code}
          confirmLabel="Yine de ekle"
          consequence="Yine de eklerseniz randevu bu saate yazılır."
          onCancel={() => {
            setConfirm(null);
          }}
          onConfirm={() => {
            const body = { ...confirm.body, allowOutsideWorkingHours: true };
            setConfirm(null);
            void send(body);
          }}
        />
      ) : null}
    </>
  );
}

function localTimeOf(instantUtc: string, timeZone: string): string {
  return formatClock(localMinutesOfDay(instantUtc, timeZone));
}

function ProblemBanner({
  problem,
  timeZone,
  onPick,
  onDismiss,
}: {
  problem: Problem;
  timeZone: string;
  onPick: (instantUtc: string) => void;
  onDismiss: () => void;
}) {
  switch (problem.kind) {
    case 'slot': {
      const times = problem.alternatives.map((alt) => ({ alt, label: localTimeOf(alt, timeZone) }));
      return (
        <Banner
          tone="hata"
          title={problem.time ? `${problem.time} artık boş değil.` : 'Bu saat az önce doldu.'}
          action={
            times.length > 0 ? (
              <span className={styles.actions}>
                {times.slice(0, 2).map(({ alt, label }) => (
                  <Button
                    key={alt}
                    size="sm"
                    onClick={() => {
                      onPick(alt);
                    }}
                  >
                    {`${label} seç`}
                  </Button>
                ))}
              </span>
            ) : undefined
          }
        >
          {times.length > 0
            ? `Siz seçerken başka bir randevu alındı. En yakın boş saatler: ${times.map((t) => t.label).join(', ')}.`
            : 'Siz seçerken başka bir randevu alındı. Başka bir saat seçin.'}
        </Banner>
      );
    }

    case 'readOnly':
      return (
        <Banner tone="hata" title="Yeni randevu eklenemez.">
          {errorMessage('subscription_read_only')}
        </Banner>
      );
    case 'existingPhone':
      return (
        <Banner tone="uyari" title="Bu numara zaten kayıtlı.">
          Kayıtlı müşteri seçildi. Bilgileri kontrol edip randevuyu oluşturun.
        </Banner>
      );
    case 'error':
      return (
        <Banner
          tone="hata"
          title="Randevu oluşturulamadı."
          action={
            <Button size="sm" onClick={onDismiss}>
              Kapat
            </Button>
          }
        >
          {errorMessage(problem.code)}
        </Banner>
      );
  }
}
