import { formatMoney, formatTime } from '@omvila/ui-web/format';
import type { AppointmentDetail } from '@/features/calendar/api';
import styles from './Checkout.module.css';
import { methodLabels } from './checkout';

/**
 * Randevunun tahsilatları (AppointmentDetail.payments; sunucu yalnız payment.collect yetkisinde döner): kapora
 * havalesi, salonda alınanlar, paketten düşülen kalemlerin 0 ₺ kaydı. Eskiden yeniye.
 */
export function AppointmentPayments({ detail }: { detail: AppointmentDetail }) {
  const payments = detail.payments ?? [];
  if (payments.length === 0) {
    return null;
  }

  return (
    <section className={styles.paidList} aria-labelledby="randevu-odemeleri">
      <h3 id="randevu-odemeleri" className={styles.paidTitle}>
        Ödemeler
      </h3>
      <ul className={styles.paidItems}>
        {payments.map((payment) => (
          <li key={payment.id} className={styles.paidRow}>
            <span>
              {[
                payment.isDeposit ? 'Kapora' : null,
                methodLabels[payment.method],
                formatTime(payment.paidUtc, detail.timeZone),
                payment.refundedAmount
                  ? `${formatMoney(payment.refundedAmount)} iade edildi`
                  : null,
              ]
                .filter(Boolean)
                .join(' · ')}
            </span>
            <strong>{formatMoney(payment.amount)}</strong>
          </li>
        ))}
      </ul>
    </section>
  );
}
