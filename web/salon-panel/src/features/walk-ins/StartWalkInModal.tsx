import { Button, Modal, Select } from '@omvila/ui-web/panel';
import { branchToday } from '@omvila/ui-web/time';
import { useQuery } from '@tanstack/react-query';
import { useState } from 'react';
import { useApi } from '@/app/api-context';
import { useClock } from '@/app/clock';
import { defaultTimeZone } from '@/app/config';
import { useSession } from '@/app/session';
import { calendarQuery } from '@/features/calendar/api';
import { servicesQuery } from '@/features/services/api';
import styles from '@/shared/ListPage.module.css';
import type { WalkInStartRequest, WalkInTicket } from './api';

/**
 * "Randevuya çevir" öncesi eksik seçim: bilette personel "Fark etmez" veya hizmet yoksa sözleşme gövdede ister.
 * Tasarımı yok; en yakın desen (seçim penceresi, T-98). Personel rolünde varsayılan kendisi (yalnız kendine açabilir).
 */
export function StartWalkInModal({
  branchId,
  ticket,
  pending,
  onClose,
  onStart,
}: {
  branchId: string;
  ticket: WalkInTicket;
  pending: boolean;
  onClose: () => void;
  onStart: (body: WalkInStartRequest) => void;
}) {
  const api = useApi();
  const clock = useClock();
  const { session } = useSession();
  const today = branchToday(clock, defaultTimeZone);
  const services = (useQuery(servicesQuery(api, branchId)).data?.items ?? []).filter(
    (s) => s.isActive,
  );
  const staff =
    useQuery(calendarQuery(api, { branchId, from: today, to: today })).data?.staff ?? [];
  const [serviceId, setServiceId] = useState(ticket.serviceId ?? '');
  const [staffId, setStaffId] = useState(
    ticket.preferredStaffId ?? session.membership.staffId ?? '',
  );
  const service = services.find((s) => s.id === serviceId) ?? services[0];
  const offering = service ? staff.filter((m) => service.staffIds.includes(m.staffId)) : staff;
  const chosenStaff = offering.find((m) => m.staffId === staffId) ?? offering[0];

  const submit = () => {
    if (!service || !chosenStaff) {
      return;
    }

    onStart({
      ...(ticket.serviceId ? {} : { serviceId: service.id }),
      ...(ticket.preferredStaffId ? {} : { staffId: chosenStaff.staffId }),
    });
  };

  return (
    <Modal
      open
      onClose={onClose}
      title="Randevuya çevir"
      description={`No ${String(ticket.queueNumber)} · ${ticket.customerName} · şimdi başlar`}
      footer={
        <>
          <Button onClick={onClose}>Vazgeç</Button>
          <Button
            variant="primary"
            loading={pending}
            loadingLabel="Çevriliyor…"
            disabled={!service || !chosenStaff}
            onClick={submit}
          >
            Randevuya çevir
          </Button>
        </>
      }
    >
      <div className={styles.form}>
        {ticket.serviceId ? null : (
          <Select
            label="Hizmet"
            value={service?.id ?? ''}
            options={
              services.length > 0
                ? services.map((s) => ({ value: s.id, label: s.name }))
                : [{ value: '', label: 'Hizmet yok' }]
            }
            onChange={(event) => {
              setServiceId(event.target.value);
            }}
          />
        )}
        {ticket.preferredStaffId ? null : (
          <Select
            label="Personel"
            value={chosenStaff?.staffId ?? ''}
            options={
              offering.length > 0
                ? offering.map((m) => ({ value: m.staffId, label: m.fullName }))
                : [{ value: '', label: 'Personel yok' }]
            }
            onChange={(event) => {
              setStaffId(event.target.value);
            }}
          />
        )}
      </div>
    </Modal>
  );
}
