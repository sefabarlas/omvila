import { AppointmentTicket, BookingButton } from '@omvila/ui-web/booking';
import { formatMoney } from '@omvila/ui-web/format';
import { displayStatus } from '@omvila/ui-web/status';
import type { Clock as ClockSource } from '@omvila/ui-web/time';
import {
  CalendarCheck,
  CalendarClock,
  Clock,
  Hourglass,
  MapPin,
  Phone,
  RefreshCw,
  X,
} from 'lucide-react';
import { useEffect, useRef } from 'react';
import { LinkButton } from '@/components/LinkButton';
import { StateBanner } from '@/components/StateBanner';
import { messageFor } from '@/booking/problem';
import {
  cancellationText,
  isRequestCancellation,
  responseSentence,
  responseWindow,
} from '@/lib/approval';
import { directionsUrl } from '@/lib/directions';
import type { PublicAppointment } from './appointment-api';
import { durationMinutes, isActive, planText, staffNames, whenLabel } from './details';
import styles from './appointment.module.css';

/** Randevum üstündeki işlem sonucu. */
export type Notice =
  | { kind: 'rescheduled' }
  | { kind: 'cancelled' }
  | { kind: 'cutoff' }
  | { kind: 'conflict' }
  | { kind: 'error'; error: unknown };

interface AppointmentViewProps {
  appointment: PublicAppointment;
  notice: Notice | null;
  /** İptal sayfası açıkken arka plan etkileşimsiz. */
  inert: boolean;
  clock: ClockSource;
  onReschedule: () => void;
  onCancel: () => void;
}

function CallSalon({ phone }: { phone: string | null | undefined }) {
  return phone ? (
    <LinkButton href={`tel:${phone}`} icon={<Phone size={18} />} block>
      Salonu ara
    </LinkButton>
  ) : null;
}

function NoticeBanner({ notice, appointment }: { notice: Notice; appointment: PublicAppointment }) {
  switch (notice.kind) {
    case 'rescheduled':
      return (
        <StateBanner
          tone="basari"
          icon={<CalendarCheck size={20} />}
          title="Saatiniz değiştirildi."
          description={`Yeni randevu saatiniz ${whenLabel(appointment)}.`}
        />
      );
    case 'cancelled':
      // İptal kartı (aşağıda) sonucu gösterir; ayrı şerit yok.
      return null;
    case 'cutoff':
      return (
        <StateBanner
          tone="uyari"
          icon={<Clock size={20} />}
          title="Online değişiklik süresi geçti."
          description="Randevunuzu değiştirmek veya iptal etmek için salonu arayın."
        />
      );
    case 'conflict':
      return (
        <StateBanner
          tone="uyari"
          icon={<RefreshCw size={20} />}
          title="Randevu bu arada değişti."
          description="Güncel bilgileri gösteriyoruz. Gerekirse işlemi yeniden yapın."
        />
      );
    case 'error':
      return (
        <StateBanner
          tone="hata"
          icon={<X size={20} />}
          title="İşlem tamamlanamadı."
          description={messageFor(notice.error)}
        />
      );
  }
}

/**
 * Randevum (Randevum.png): başlık bloğu, yol tarifi, kalemler ve tutar, "Plan değişti mi?" kartı. Onay bekleyen
 * talepte "Randevunuz · Onay bekliyor" ve kalan yanıt süresi; salonun onaylayamadığı veya zamanında yanıtlamadığı
 * talep "İptal" olarak gösterilir ("reddedildi" yazılmaz, kriter 3). Takvime ekle
 * (dosya indirme), kapora satırları (Dilim 4) ve uygulama tanıtımı (Dilim 5) yok. Süre geçmişse butonların yerine
 * "Salonu ara" (durum-makinesi.md § Müşteri sınırları, kriter 10).
 */
export function AppointmentView({
  appointment,
  notice,
  inert,
  clock,
  onReschedule,
  onCancel,
}: AppointmentViewProps) {
  const { branch } = appointment;
  const cancelled = appointment.status === 'Cancelled';
  // Kapora bekleyen talep (DepositPending) Dilim 4'te kendi kartını alır; burada yalnız onay bekleyen talep.
  const pending = appointment.displayStatus === 'PendingApproval';
  const active = isActive(appointment);
  const directions = cancelled ? null : directionsUrl(branch);
  const status = displayStatus(appointment.displayStatus) ?? 'onay';
  const cancelledRef = useRef<HTMLElement>(null);

  useEffect(() => {
    if (notice?.kind === 'cancelled') cancelledRef.current?.focus();
  }, [notice]);

  return (
    <main className={styles.page} inert={inert}>
      <AppointmentTicket
        headingLevel={1}
        salonName={appointment.brand.name}
        logoUrl={appointment.brand.logoUrl ?? null}
        branchAddress={`${branch.addressLine}, ${branch.district}`}
        status={status}
        startUtc={appointment.startUtc}
        timeZone={appointment.timeZone}
        serviceNames={appointment.items.map((item) => item.serviceName)}
        staffName={staffNames(appointment)}
        durationMinutes={durationMinutes(appointment)}
        className={styles.ticket ?? ''}
        actions={
          directions ? (
            <LinkButton href={directions} icon={<MapPin size={18} />} block>
              Yol tarifi
            </LinkButton>
          ) : undefined
        }
      />
      <div className={styles.body}>
        {notice ? <NoticeBanner notice={notice} appointment={appointment} /> : null}

        {cancelled ? (
          <section
            ref={cancelledRef}
            className={styles.card}
            aria-labelledby="iptal-baslik"
            tabIndex={-1}
            {...(notice?.kind === 'cancelled' ? { role: 'status' } : {})}
          >
            <h2 id="iptal-baslik" className={styles.cardTitle}>
              {isRequestCancellation(appointment.cancellation)
                ? 'Talebiniz iptal edildi'
                : 'Randevunuz iptal edildi'}
            </h2>
            <p className={styles.text}>
              {cancellationText(appointment.brand.name, appointment.cancellation)}
            </p>
            <LinkButton href={`/${encodeURIComponent(appointment.brand.slug)}`} block>
              Yeni randevu al
            </LinkButton>
          </section>
        ) : null}

        {pending ? (
          <section className={styles.pending} aria-labelledby="onay-baslik">
            <span className={styles.pendingIcon} aria-hidden="true">
              <Hourglass size={20} />
            </span>
            <div className={styles.pendingBody}>
              <h2 id="onay-baslik" className={styles.cardTitle}>
                Salon onayı bekleniyor
              </h2>
              <p className={styles.muted}>
                {responseSentence(
                  appointment.brand.name,
                  responseWindow(appointment.holdExpiresUtc, clock),
                )}{' '}
                Onaylandığında SMS ile bildireceğiz; yanıt gelmezse talep iptal olur.
              </p>
            </div>
          </section>
        ) : null}

        <section className={styles.card} aria-label="Hizmetler ve tutar">
          <ul className={styles.rows}>
            {appointment.items.map((item, index) => (
              <li key={`${item.serviceName}-${String(index)}`} className={styles.row}>
                <span className={styles.rowLabel}>{item.serviceName}</span>
                <span className={styles.rowValue}>
                  {item.paidFromPackage ? 'Paketten' : formatMoney(item.price)}
                </span>
              </li>
            ))}
            {cancelled ? null : (
              <li className={styles.row}>
                <span className={styles.rowLabel}>Salonda ödenecek</span>
                <span className={styles.rowValue}>{formatMoney(appointment.amountDue)}</span>
              </li>
            )}
          </ul>
        </section>

        {active ? (
          <section className={styles.card} aria-labelledby="plan-baslik">
            <h2 id="plan-baslik" className={styles.cardTitle}>
              Plan değişti mi?
            </h2>
            <p className={styles.text}>{planText(appointment)}</p>
            <div className={styles.planActions}>
              {appointment.canReschedule ? (
                <BookingButton
                  variant="secondary"
                  size="md"
                  block
                  icon={<CalendarClock size={18} />}
                  onClick={onReschedule}
                >
                  Saati değiştir
                </BookingButton>
              ) : null}
              {appointment.canCancel ? (
                <BookingButton variant="danger" size="md" block onClick={onCancel}>
                  Randevuyu iptal et
                </BookingButton>
              ) : null}
              {!appointment.canCancel || !appointment.canReschedule ? (
                <CallSalon phone={branch.phone} />
              ) : null}
            </div>
          </section>
        ) : null}

        <p className={styles.footnote}>Bu sayfa yalnızca bu bağlantıyı açan kişiye görünür.</p>
      </div>
    </main>
  );
}
