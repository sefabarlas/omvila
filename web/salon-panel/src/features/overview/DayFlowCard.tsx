import { formatTime } from '@omvila/ui-web/format';
import { Button, EmptyState, Skeleton, StatusBadge } from '@omvila/ui-web/panel';
import { displayStatus } from '@omvila/ui-web/status';
import type { StatusKey } from '@omvila/ui-web/status';
import { toBranchDate } from '@omvila/ui-web/time';
import { Link } from '@tanstack/react-router';
import { CalendarCheck } from 'lucide-react';
import type { CalendarView } from '@/features/calendar/api';
import styles from './Overview.module.css';

interface FlowRow {
  appointmentId: string;
  time: string;
  status: StatusKey;
  customerName: string;
  detail: string;
}

/** Günün (view.from) randevuları saat sırasıyla; çok kalemli randevu tek satır (ilk kalemin saati), iptaller yok. */
export function dayFlowRows(view: CalendarView): FlowRow[] {
  const staffFirstName = new Map(
    view.staff.map((member) => [member.staffId, member.fullName.split(' ')[0] ?? '']),
  );
  const grouped = new Map<string, CalendarView['appointments']>();
  for (const item of view.appointments) {
    if (
      item.status === 'Cancelled' ||
      toBranchDate(new Date(item.startUtc), view.timeZone) !== view.from
    ) {
      continue;
    }

    grouped.set(item.appointmentId, [...(grouped.get(item.appointmentId) ?? []), item]);
  }

  return [...grouped.values()]
    .map((items) => [...items].sort((a, b) => a.startUtc.localeCompare(b.startUtc)))
    .sort((a, b) => (a[0]?.startUtc ?? '').localeCompare(b[0]?.startUtc ?? ''))
    .flatMap((items) => {
      const first = items[0];
      const status = first ? displayStatus(first.displayStatus) : null;
      if (!first || !status) {
        return [];
      }

      const services = [...new Set(items.map((item) => item.serviceName))].join(' + ');
      const staff = [
        ...new Set(
          items.map((item) => (item.staffId ? staffFirstName.get(item.staffId) : undefined)),
        ),
      ].filter((name): name is string => !!name);
      return [
        {
          appointmentId: first.appointmentId,
          time: formatTime(first.startUtc, view.timeZone),
          status,
          customerName: first.customerName,
          detail: [services, ...staff].join(' · '),
        },
      ];
    });
}

interface DayFlowCardProps {
  view: CalendarView | undefined;
  isError: boolean;
  onRetry: () => void;
  onOpen: (appointmentId: string) => void;
}

/** "Gün akışı" (salon-paneli/GenelBakis): saat, durum, müşteri · hizmet · personel; satır yan paneli açar. */
export function DayFlowCard({ view, isError, onRetry, onOpen }: DayFlowCardProps) {
  const rows = view ? dayFlowRows(view) : [];
  return (
    <section className={styles.card} aria-labelledby="gun-akisi">
      <div className={styles.cardHead}>
        <h2 id="gun-akisi" className={styles.listTitle}>
          Gün akışı
        </h2>
        <Link to="/takvim" className={styles.cardLink}>
          Takvime git
        </Link>
      </div>
      {isError ? (
        <div className={styles.cardError} role="alert">
          <p>Bugünün randevuları yüklenemedi.</p>
          <Button size="sm" onClick={onRetry}>
            Tekrar dene
          </Button>
        </div>
      ) : !view ? (
        <Skeleton variant="block" height="240px" />
      ) : rows.length === 0 ? (
        <EmptyState
          icon={<CalendarCheck size={24} />}
          title="Bugün randevu yok"
          description="Gelen randevular burada saat sırasıyla görünür."
        />
      ) : (
        <ul className={styles.flow}>
          {rows.map((row) => (
            <li key={row.appointmentId}>
              <button
                type="button"
                className={styles.flowRow}
                onClick={() => {
                  onOpen(row.appointmentId);
                }}
              >
                <span className={styles.flowTime}>{row.time}</span>
                <span
                  className={[styles.flowDot, styles[row.status]].filter(Boolean).join(' ')}
                  aria-hidden="true"
                />
                <span className={styles.flowText}>
                  <strong>{row.customerName}</strong>
                  <span className={styles.flowDetail}>{` · ${row.detail}`}</span>
                </span>
                <StatusBadge status={row.status} />
              </button>
            </li>
          ))}
        </ul>
      )}
    </section>
  );
}
