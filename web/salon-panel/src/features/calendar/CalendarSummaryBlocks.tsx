import { formatDuration } from '@omvila/ui-web/format';
import { Bento, BentoBlock } from '@omvila/ui-web/panel';
import type { CalendarView } from './api';
import { inLabel, summarize } from './calendar-view';

interface CalendarSummaryBlocksProps {
  /** Bugünün takvimi; yüklenmediyse bloklar "—" ve 0 gösterir. */
  view: CalendarView | undefined;
  now: Date;
  /** Onay bekleyenler yalnız onaylayabilene (appointment.approve). */
  showPending: boolean;
}

/**
 * Özet blokları (salon-paneli/Main): sıradaki, onay bekleyen, bugün. "Kapıda sırada" Dilim 5'te, ciro ("9.840 ₺")
 * kasa ile Dilim 5'te gelir; o zamana kadar Bugün bloğu randevu sayısını gösterir (sahte sayı yok).
 */
export function CalendarSummaryBlocks({ view, now, showPending }: CalendarSummaryBlocksProps) {
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
      <BentoBlock
        label="Bugün"
        value={String(today.appointments)}
        sub={`randevu · ${String(today.completed)} tamamlandı`}
      />
    </Bento>
  );
}
