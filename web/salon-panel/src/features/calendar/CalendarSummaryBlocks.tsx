import { formatDuration, formatMoney } from '@omvila/ui-web/format';
import { Bento, BentoBlock } from '@omvila/ui-web/panel';
import type { CashDay } from '@/features/cash/api';
import type { CalendarView } from './api';
import { inLabel, summarize } from './calendar-view';

interface CalendarSummaryBlocksProps {
  /** Bugünün takvimi; yüklenmediyse bloklar "—" ve 0 gösterir. */
  view: CalendarView | undefined;
  now: Date;
  /** Onay bekleyenler yalnız onaylayabilene (appointment.approve). */
  showPending: boolean;
  /** Günün tahsilatı (kasa, manageGetCashDay); kasayı göremeyen kullanıcıda yok. */
  revenue?: CashDay['total'] | undefined;
}

/**
 * Özet blokları (salon-paneli/Main): sıradaki, onay bekleyen, bugün. Bugün bloğu kasayı görebilen kullanıcıda günün
 * tahsilatını ("9.840 ₺", manageGetCashDay) ve randevu sayısını, diğerlerinde randevu sayısını gösterir (sahte sayı
 * yok; acik-sorular.md T-97). "Kapıda sırada" ayrı PR'da. "Onay bekliyor" kapora bekleyenleri saymaz (T-84 (3)).
 */
export function CalendarSummaryBlocks({
  view,
  now,
  showPending,
  revenue,
}: CalendarSummaryBlocksProps) {
  const summary = summarize(view, now);
  const { next, pending, today } = summary;
  return (
    <Bento label="Bugünün özeti">
      <BentoBlock
        tone="marka"
        label={next ? `Sıradaki · ${inLabel(next.inMinutes)}` : 'Sıradaki'}
        value={next?.time ?? '—'}
        sub={next?.line ?? 'Bugün sırada randevu yok'}
      />
      {showPending ? (
        <BentoBlock
          tone="vurgu"
          label="Onay bekliyor"
          value={String(pending.count)}
          sub={
            pending.expiresInMinutes !== null
              ? `ilki ${formatDuration(pending.expiresInMinutes)} içinde iptal olur`
              : 'Bekleyen talep yok'
          }
        />
      ) : null}
      {revenue ? (
        <BentoBlock
          label="Bugün"
          value={formatMoney(revenue.amount)}
          sub={`${String(today.appointments)} randevu · ${String(today.completed)} tamamlandı`}
        />
      ) : (
        <BentoBlock
          label="Bugün"
          value={String(today.appointments)}
          sub={`randevu · ${String(today.completed)} tamamlandı`}
        />
      )}
    </Bento>
  );
}
