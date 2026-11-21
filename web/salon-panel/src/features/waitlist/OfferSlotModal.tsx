import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { formatDate } from '@omvila/ui-web/format';
import { Banner, Button, Modal, Select, Skeleton, TextField, useToast } from '@omvila/ui-web/panel';
import { useQuery } from '@tanstack/react-query';
import { useState } from 'react';
import { useApi } from '@/app/api-context';
import { slotsQuery } from '@/features/appointments/api';
import appointmentStyles from '@/features/appointments/NewAppointment.module.css';
import { SlotGrid } from '@/features/appointments/SlotGrid';
import { calendarQuery } from '@/features/calendar/api';
import { servicesQuery } from '@/features/services/api';
import styles from '@/shared/ListPage.module.css';
import { useOfferSlot } from './api';
import type { WaitlistEntry } from './api';
import { preferenceLabel } from './labels';

interface OfferSlotModalProps {
  branchId: string;
  entry: WaitlistEntry;
  /** Şubede bugün (listenin "şimdi"sinden). */
  today: string;
  holdMinutes: number | undefined;
  onClose: () => void;
  /** 412: sayfada "Kayıt çakışması" şeridi. */
  onConflict: () => void;
}

/**
 * "Saat öner" (manageOfferSlot): bekleyenin hizmetleri için gün ve personel seçilir, boş saatler yeni randevudaki
 * ızgarayla (manageSearchSlots) gösterilir. Tercih edilen personel varsa ve "başka personel de olur" denmediyse yalnız
 * o personel. Saat teklif süresi boyunca tutulur; 409 slot_taken, insufficient_credits ve invalid_state_transition
 * pencerede yazılır.
 */
export function OfferSlotModal({
  branchId,
  entry,
  today,
  holdMinutes,
  onClose,
  onConflict,
}: OfferSlotModalProps) {
  const api = useApi();
  const toast = useToast();
  const offer = useOfferSlot();
  const firstDate = [...entry.dates].sort().find((d) => d >= today) ?? today;
  const [date, setDate] = useState(firstDate);
  const [staffId, setStaffId] = useState(entry.preferredStaffId ?? '');
  const [time, setTime] = useState<string | null>(null);
  const [problem, setProblem] = useState<string | null>(null);

  const services = useQuery(servicesQuery(api, branchId)).data?.items ?? [];
  const day = useQuery(calendarQuery(api, { branchId, from: date, to: date }));
  const wanted = services.filter((s) => entry.serviceIds.includes(s.id));
  const offering = (day.data?.staff ?? []).filter(
    (member) =>
      wanted.every((service) => service.staffIds.includes(member.staffId)) &&
      (entry.preferredStaffId === null ||
        entry.preferredStaffId === undefined ||
        entry.acceptOtherStaff ||
        member.staffId === entry.preferredStaffId),
  );
  const chosen = offering.find((m) => m.staffId === staffId) ?? offering[0];
  const effectiveStaffId = chosen?.staffId ?? '';
  const slots = useQuery({
    ...slotsQuery(api, branchId, date, entry.serviceIds, effectiveStaffId),
    enabled: effectiveStaffId !== '',
  });
  const slot = time ? slots.data?.slots.find((s) => s.localTime === time) : undefined;

  const submit = () => {
    if (!slot) {
      setProblem('Saat seçin.');
      return;
    }

    setProblem(null);
    offer.mutate(
      { entry, startUtc: slot.startUtc, staffId: effectiveStaffId },
      {
        onSuccess: () => {
          toast.show({
            tone: 'basari',
            title: 'Teklif gönderildi',
            message: `${entry.customer.fullName} · ${formatDate(date, 'panel')} ${slot.localTime}`,
          });
          onClose();
        },
        onError: (error) => {
          if (error instanceof ApiProblem && error.status === 412) {
            onConflict();
            return;
          }

          const code = error instanceof ApiProblem ? error.code : undefined;
          if (code === 'slot_taken') {
            setTime(null);
            void slots.refetch();
          }

          setProblem(
            code === 'insufficient_credits'
              ? 'SMS kontörünüz az. Teklif müşteriye ulaşamayacağı için gönderilmedi; kontör yükleyip tekrar deneyin.'
              : errorMessage(code),
          );
        },
      },
    );
  };

  return (
    <Modal
      open
      onClose={onClose}
      title="Saat öner"
      description={`${entry.customer.fullName} · ${entry.serviceNames.join(', ')} · ${preferenceLabel(entry.dates, entry.timeWindows)}`}
      size="lg"
      footer={
        <>
          <Button onClick={onClose}>Vazgeç</Button>
          <Button
            variant="primary"
            loading={offer.isPending}
            loadingLabel="Gönderiliyor…"
            onClick={submit}
          >
            Teklifi gönder
          </Button>
        </>
      }
    >
      <div className={styles.form}>
        {problem ? <Banner tone="hata" title={problem} /> : null}
        <div className={appointmentStyles.row}>
          <TextField
            label="Gün"
            type="date"
            value={date}
            min={today || undefined}
            onChange={(event) => {
              if (/^\d{4}-\d{2}-\d{2}$/.test(event.target.value)) {
                setDate(event.target.value);
                setTime(null);
                setProblem(null);
              }
            }}
          />
          <Select
            label="Personel"
            value={effectiveStaffId}
            disabled={offering.length <= 1}
            options={
              offering.length > 0
                ? offering.map((m) => ({ value: m.staffId, label: m.fullName }))
                : [{ value: '', label: day.isPending ? 'Yükleniyor…' : 'Personel yok' }]
            }
            onChange={(event) => {
              setStaffId(event.target.value);
              setTime(null);
            }}
          />
        </div>
        {effectiveStaffId === '' ? (
          day.isPending ? (
            <Skeleton variant="block" height="96px" />
          ) : (
            <p className={appointmentStyles.subLabel}>
              Bu gün bu hizmetleri veren müsait personel yok.
            </p>
          )
        ) : slots.isPending ? (
          <Skeleton variant="block" height="96px" />
        ) : slots.isError ? (
          <p className={appointmentStyles.error}>
            {errorMessage(slots.error instanceof ApiProblem ? slots.error.code : undefined)}
          </p>
        ) : slots.data.slots.length === 0 ? (
          <p className={appointmentStyles.subLabel}>
            {`Bu gün ${chosen?.fullName ?? ''} için boş saat yok.`}
          </p>
        ) : (
          <SlotGrid
            result={slots.data}
            legend={`${formatDate(date, 'panel')} · ${chosen?.fullName ?? ''} için boş saatler`}
            value={time}
            onChange={(value) => {
              setTime(value);
              setProblem(null);
            }}
          />
        )}
        {holdMinutes !== undefined ? (
          <p className={styles.note}>
            {`Saat ${String(holdMinutes)} dakika ${entry.customer.fullName} için tutulur; yanıt gelmezse saat listede sıradakine geçer.`}
          </p>
        ) : null}
      </div>
    </Modal>
  );
}
