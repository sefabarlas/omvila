import { AppointmentTicket } from '@omvila/ui-web/booking';
import { displayStatus } from '@omvila/ui-web/status';
import { Check, MapPin, Phone } from 'lucide-react';
import { LinkButton } from '@/components/LinkButton';
import { directionsUrl } from '@/lib/directions';
import { sentTo } from '@/lib/sent-to';
import type { PublicAppointment } from '../booking-api';
import styles from './steps.module.css';

interface DoneStepProps {
  appointment: PublicAppointment;
  phoneMasked: string;
}

/** Erişim kodundan bu sitedeki randevu sayfası (manageUrl'nin kökü ortama göre değişir; yol aynıdır). */
const managePath = (accessCode: string) => `/r/${encodeURIComponent(accessCode)}`;

/**
 * Başarı (Basari.png). "Randevunuz onaylandı" yalnız gerçekten onaylı randevuda (S-03); onay bekleyen talep
 * RequestStep'te (Talep.png), kapora bekleyen randevu DepositStep'te (Havale.png). Takvime ekleme (dosya indirme) ve
 * uygulama tanıtımı bu dilimde yok.
 */
export function DoneStep({ appointment, phoneMasked }: DoneStepProps) {
  const confirmed = appointment.status === 'Confirmed';
  const durationMinutes = Math.round(
    (new Date(appointment.endUtc).getTime() - new Date(appointment.startUtc).getTime()) / 60_000,
  );
  const staff = [...new Set(appointment.items.map((item) => item.staffName).filter(Boolean))];
  const { branch } = appointment;
  const directions = directionsUrl(branch);

  return (
    <div className={styles.stack}>
      <span className={styles.doneIcon} aria-hidden="true">
        <Check size={36} strokeWidth={2.5} />
      </span>
      <div className={styles.center} role="status">
        <h2 className={styles.titleMedium}>
          {confirmed ? 'Randevunuz onaylandı' : 'Talebiniz alındı'}
        </h2>
        <p className={styles.note}>
          {confirmed
            ? `Onay mesajını ${sentTo(phoneMasked)} gönderdik.`
            : `Salon onaylayınca ${sentTo(phoneMasked)} SMS göndeririz.`}
        </p>
      </div>
      <AppointmentTicket
        headingLevel={2}
        salonName={appointment.brand.name}
        logoUrl={appointment.brand.logoUrl ?? null}
        branchAddress={`${branch.addressLine}, ${branch.district}`}
        status={displayStatus(appointment.displayStatus) ?? 'onay'}
        startUtc={appointment.startUtc}
        timeZone={appointment.timeZone}
        serviceNames={appointment.items.map((item) => item.serviceName)}
        staffName={staff.length > 0 ? staff.join(', ') : null}
        durationMinutes={durationMinutes}
      />
      <div className={styles.actions}>
        {directions ? (
          <LinkButton href={directions} icon={<MapPin size={18} />}>
            Yol tarifi
          </LinkButton>
        ) : null}
        {branch.phone ? (
          <LinkButton href={`tel:${branch.phone}`} icon={<Phone size={18} />}>
            Salonu ara
          </LinkButton>
        ) : null}
      </div>
      <p className={[styles.note, styles.center].join(' ')}>
        Randevunuzu SMS’teki bağlantıdan veya{' '}
        <a className={styles.linkButton} href={managePath(appointment.accessCode)}>
          randevu sayfanızdan
        </a>{' '}
        değiştirebilir ya da iptal edebilirsiniz.
      </p>
    </div>
  );
}
