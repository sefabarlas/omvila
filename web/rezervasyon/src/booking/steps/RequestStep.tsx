import { BookingStatusBadge } from '@omvila/ui-web/booking';
import {
  formatDate,
  formatDuration,
  formatMoney,
  formatTime,
  initials,
} from '@omvila/ui-web/format';
import { toBranchDate } from '@omvila/ui-web/time';
import type { Clock } from '@omvila/ui-web/time';
import { Clock as ClockIcon, Info } from 'lucide-react';
import { responseSentence, responseWindow } from '@/lib/approval';
import type { PublicAppointment } from '../booking-api';
import styles from './RequestStep.module.css';
import shared from './steps.module.css';

interface RequestStepProps {
  appointment: PublicAppointment;
  phoneMasked: string;
  /** Marka fiyat göstermiyorsa tutar satırı yazılmaz. */
  showPrices: boolean;
  clock: Clock;
}

/**
 * Talep (Talep.png): onay gerektiren salonda oluşturulan randevu (A1). Durum "Onay bekliyor"; "onaylandı" yazılmaz
 * (S-03). Kalan yanıt süresi `holdExpiresUtc`'den; salon yanıtlamazsa talep iptal olur ve müşteriye SMS gider.
 */
export function RequestStep({ appointment, phoneMasked, showPrices, clock }: RequestStepProps) {
  const { brand, timeZone, startUtc } = appointment;
  const minutes = Math.round(
    (new Date(appointment.endUtc).getTime() - new Date(startUtc).getTime()) / 60_000,
  );
  const staff = [...new Set(appointment.items.map((item) => item.staffName).filter(Boolean))];
  const details = [appointment.items.map((item) => item.serviceName).join(', '), ...staff].join(
    ' · ',
  );
  const window = responseWindow(appointment.holdExpiresUtc, clock);

  return (
    <div className={shared.stack}>
      <span className={styles.icon} aria-hidden="true">
        <ClockIcon size={32} strokeWidth={2} />
      </span>
      <div className={shared.center} role="status">
        <h2 className={shared.titleMedium}>Talebiniz alındı</h2>
        <p className={styles.lead}>
          {responseSentence(brand.name, window)} Onaylandığında {phoneMasked} numarasına SMS
          göndereceğiz.
        </p>
      </div>

      <section className={styles.card} aria-label="Talep özeti">
        <div className={styles.head}>
          <span className={styles.logo} aria-hidden="true">
            {brand.logoUrl ? <img src={brand.logoUrl} alt="" /> : initials(brand.name)}
          </span>
          <span className={styles.headText}>
            <strong className={styles.salon}>{brand.name}</strong>
            <span className={styles.details}>{details}</span>
          </span>
          <BookingStatusBadge status="onay" />
        </div>
        <dl className={styles.facts}>
          <div>
            <dt>Tarih</dt>
            <dd>{formatDate(toBranchDate(new Date(startUtc), timeZone), 'short')}</dd>
          </div>
          <div>
            <dt>Saat</dt>
            <dd>{formatTime(startUtc, timeZone)}</dd>
          </div>
          <div>
            <dt>Süre</dt>
            <dd>{formatDuration(minutes)}</dd>
          </div>
        </dl>
        <div className={styles.foot}>
          {showPrices ? (
            <strong className={styles.total}>{formatMoney(appointment.total)}</strong>
          ) : null}
          <span className={styles.footNote}>Salon onaylayınca SMS ile bildireceğiz</span>
        </div>
      </section>

      <div className={shared.infoBox}>
        <Info size={20} aria-hidden="true" className={styles.infoIcon} />
        <p>
          Saat sizin için ayrıldı. Salon yanıt vermezse talep düşer ve size haber veririz; başka bir
          saat seçebilirsiniz.
        </p>
      </div>

      <a className={styles.back} href={`/${encodeURIComponent(brand.slug)}`}>
        Salon sayfasına dön
      </a>
    </div>
  );
}
