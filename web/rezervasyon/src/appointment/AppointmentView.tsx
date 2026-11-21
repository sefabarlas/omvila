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
import { awaitingTransfer, receivedDeposit } from '@/lib/deposit';
import { directionsUrl } from '@/lib/directions';
import { DepositNotReceived } from '@/deposit/DepositNotReceived';
import { DepositTransfer } from '@/deposit/DepositTransfer';
import type { Loaded, PublicAppointment } from './appointment-api';
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
  /** "Havaleyi yaptım" sonrası güncel randevu ve ETag. */
  onDepositReported: (loaded: Loaded) => void;
  /** Kapora süresi sayfa açıkken doldu: randevu yeniden okunur. */
  onRefresh: () => void;
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
 * talep "İptal" olarak gösterilir ("reddedildi" yazılmaz, kriter 3). Kapora bekleniyorsa Havale ekranı (Dilim 4,
 * bildirimden sonra "Salon onayı bekleniyor"); kapora alındıysa satırı ve "Salonda ödenecek" (toplam − kapora);
 * süresinde gelmediyse Durumlar.png "Kapora gelmedi". Takvime ekle (dosya indirme) ve uygulama tanıtımı (Dilim 5)
 * yok. Süre geçmişse butonların yerine "Salonu ara" (durum-makinesi.md § Müşteri sınırları, kriter 10).
 */
export function AppointmentView({
  appointment,
  notice,
  inert,
  clock,
  onReschedule,
  onCancel,
  onDepositReported,
  onRefresh,
}: AppointmentViewProps) {
  const { branch } = appointment;
  const cancelled = appointment.status === 'Cancelled';
  // Kapora bekleyen talep (DepositPending) Havale ekranında; burada yalnız onay bekleyen talep.
  const pending = appointment.displayStatus === 'PendingApproval';
  const active = isActive(appointment);
  const directions = cancelled ? null : directionsUrl(branch);
  const status = displayStatus(appointment.displayStatus) ?? 'onay';
  const cancelledRef = useRef<HTMLElement>(null);

  useEffect(() => {
    if (notice?.kind === 'cancelled') cancelledRef.current?.focus();
  }, [notice]);

  const deposit = receivedDeposit(appointment);
  const depositNotReceived = appointment.cancellation?.reason === 'DepositNotReceived';
  const planCard = active ? (
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
  ) : null;
  const footnote = (
    <p className={styles.footnote}>Bu sayfa yalnızca bu bağlantıyı açan kişiye görünür.</p>
  );

  if (awaitingTransfer(appointment)) {
    return (
      <main className={styles.page} inert={inert}>
        <div className={styles.body}>
          {notice ? <NoticeBanner notice={notice} appointment={appointment} /> : null}
          <DepositTransfer
            appointment={appointment}
            clock={clock}
            headingLevel={1}
            onReported={onDepositReported}
            onStale={onRefresh}
          >
            {planCard}
            {footnote}
          </DepositTransfer>
        </div>
      </main>
    );
  }

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
            {depositNotReceived ? (
              <DepositNotReceived phone={branch.phone} />
            ) : (
              <p className={styles.text}>
                {cancellationText(appointment.brand.name, appointment.cancellation)}
                {appointment.deposit?.status === 'Received' &&
                appointment.cancellation?.cancelledBy === 'Salon'
                  ? ' Kapora iadesi için salon sizinle iletişime geçecek.'
                  : null}
              </p>
            )}
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
            {deposit?.status === 'Received' ? (
              <li className={styles.row}>
                <span className={styles.rowLabel}>Kapora alındı (havale ile)</span>
                <span className={styles.rowValue}>−{formatMoney(deposit.required)}</span>
              </li>
            ) : null}
            {deposit?.status === 'Refunded' ? (
              <li className={styles.row}>
                <span className={styles.rowLabel}>Kapora · Salon iade etti</span>
                <span className={styles.rowValue}>
                  {formatMoney(deposit.refundedAmount ?? deposit.required)}
                </span>
              </li>
            ) : null}
            {cancelled ? null : (
              <li className={styles.row}>
                <span className={styles.rowLabel}>Salonda ödenecek</span>
                <span className={styles.rowValue}>{formatMoney(appointment.amountDue)}</span>
              </li>
            )}
          </ul>
        </section>

        {planCard}

        {footnote}
      </div>
    </main>
  );
}
