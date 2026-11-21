import { formatDuration, formatMoney, formatTime } from '@omvila/ui-web/format';
import { Banner, Button } from '@omvila/ui-web/panel';
import { Banknote, BellRing, Check } from 'lucide-react';
import approvalStyles from '@/features/approvals/Approvals.module.css';
import type { AppointmentDetail } from '@/features/calendar/api';
import calendarStyles from '@/features/calendar/Calendar.module.css';
import { locativeTime } from '@/shared/turkish';
import { depositCheckHint, transferReportedTitle } from './deposit-view';
import styles from './Deposits.module.css';

interface PendingDepositSectionProps {
  detail: AppointmentDetail;
  /** Havale süresinden kalan dakika (0 = doldu); tutma süresi yoksa null. */
  minutesLeft: number | null;
  /** Hatırlat ve Kaporasız onayla görünür mü (süre dolmadı, izin var). */
  showActions: boolean;
  canMark: boolean;
  canApprove: boolean;
  busy: boolean;
  reminding: boolean;
  waiving: boolean;
  onRemind: () => void;
  onWaive: () => void;
}

/**
 * Kapora bekleyen randevunun kapora bölümü (isletme-uygulamasi/KaporaOnay içeriği, panel yan panelinde): tutar,
 * açıklama kodu, müşteri bildirimi, kalan süre; "Hatırlat" (bir kez; sonra "Hatırlatıldı · 11:40") ve "Kaporasız onayla".
 * "Kapora geldi" yan panelin alt butonudur.
 */
export function PendingDepositSection({
  detail,
  minutesLeft,
  showActions,
  canMark,
  canApprove,
  busy,
  reminding,
  waiving,
  onRemind,
  onWaive,
}: PendingDepositSectionProps) {
  const deposit = detail.deposit;
  if (!deposit) {
    return null;
  }

  const tz = detail.timeZone;
  const reportedUtc = deposit.status === 'CustomerReported' ? deposit.customerReportedUtc : null;
  const remindedUtc = deposit.remindedUtc ?? null;
  return (
    <section className={approvalStyles.section} aria-labelledby="kapora-bolumu">
      <h3 id="kapora-bolumu" className={approvalStyles.sectionTitle}>
        Kapora
      </h3>
      {deposit.status === 'CustomerReported' ? (
        <Banner tone="uyari" title={transferReportedTitle}>
          {reportedUtc
            ? `Müşteri havaleyi ${locativeTime(formatTime(reportedUtc, tz))} yaptığını bildirdi.`
            : 'Müşteri havaleyi yaptığını bildirdi.'}
        </Banner>
      ) : null}
      <dl className={approvalStyles.rows}>
        <div className={approvalStyles.row}>
          <dt>Tutar</dt>
          <dd>{formatMoney(deposit.required)}</dd>
        </div>
        {deposit.transferCode ? (
          <div className={approvalStyles.row}>
            <dt>Açıklama kodu</dt>
            <dd>{deposit.transferCode}</dd>
          </div>
        ) : null}
        <div className={approvalStyles.row}>
          <dt>Müşteri bildirimi</dt>
          <dd>{reportedUtc ? `“Havaleyi yaptım” · ${formatTime(reportedUtc, tz)}` : 'Yok'}</dd>
        </div>
        {minutesLeft !== null ? (
          <div className={approvalStyles.row}>
            <dt>Kalan süre</dt>
            <dd>{minutesLeft > 0 ? formatDuration(minutesLeft) : 'Süre doldu'}</dd>
          </div>
        ) : null}
      </dl>
      <p className={calendarStyles.note}>{depositCheckHint}</p>
      {showActions && (canMark || canApprove) ? (
        <div className={styles.actions}>
          {canMark ? (
            <Button
              size="sm"
              icon={<BellRing size={14} aria-hidden="true" />}
              disabled={remindedUtc !== null || busy}
              loading={reminding}
              loadingLabel="Gönderiliyor…"
              onClick={onRemind}
            >
              {remindedUtc ? `Hatırlatıldı · ${formatTime(remindedUtc, tz)}` : 'Hatırlat'}
            </Button>
          ) : null}
          {canApprove ? (
            <Button
              size="sm"
              icon={<Check size={14} aria-hidden="true" />}
              disabled={busy}
              loading={waiving}
              loadingLabel="Onaylanıyor…"
              onClick={onWaive}
            >
              Kaporasız onayla
            </Button>
          ) : null}
        </div>
      ) : null}
    </section>
  );
}

interface DepositSummaryProps {
  detail: AppointmentDetail;
  canMark: boolean;
  onRefund: () => void;
}

/**
 * İşaretlenmiş kaporanın satırı (onaylı, tamamlanmış veya iptal randevu yan paneli): "Kapora 150 ₺ · havale ile
 * alındı" ve "Salon iade etti" eylemi; iade sonrası "Salon iade etti · 150 ₺"; kaporasız onaylanmışsa bilgi satırı.
 */
export function DepositSummary({ detail, canMark, onRefund }: DepositSummaryProps) {
  const deposit = detail.deposit;
  if (!deposit) {
    return null;
  }

  const received = `Kapora ${formatMoney(deposit.required)} · havale ile alındı`;
  const icon = (
    <span className={calendarStyles.actionIcon} aria-hidden="true">
      <Banknote size={16} />
    </span>
  );

  if (deposit.status === 'Received') {
    return canMark ? (
      <li>
        <button type="button" className={calendarStyles.action} onClick={onRefund}>
          {icon}
          <span className={calendarStyles.actionText}>
            <strong>{received}</strong>
            <small>Salon iade etti olarak işaretleyin</small>
          </span>
        </button>
      </li>
    ) : (
      <li className={calendarStyles.action}>
        {icon}
        <span className={calendarStyles.actionText}>
          <strong>{received}</strong>
          {deposit.transferCode ? <small>Açıklama kodu {deposit.transferCode}</small> : null}
        </span>
      </li>
    );
  }

  if (deposit.status === 'Refunded') {
    return (
      <li className={calendarStyles.action}>
        {icon}
        <span className={calendarStyles.actionText}>
          <strong>
            {`Salon iade etti · ${formatMoney(deposit.refundedAmount ?? deposit.required)}`}
          </strong>
          <small>{received}</small>
        </span>
      </li>
    );
  }

  if (deposit.status === 'Waived') {
    return (
      <li className={calendarStyles.action}>
        {icon}
        <span className={calendarStyles.actionText}>
          <strong>Kaporasız onaylandı</strong>
          <small>{`İstenen kapora ${formatMoney(deposit.required)} alınmadı`}</small>
        </span>
      </li>
    );
  }

  return null;
}
