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
  /** Kapıda sıra (walkin.manage): verilmezse blok yok; sıra yüklenmediyse "—". */
  walkIns?: WalkInSummary | undefined;
  /** Günün tahsilatı (kasa, manageGetCashDay); kasayı göremeyen kullanıcıda yok. */
  revenue?: CashDay['total'] | undefined;
}

export interface WalkInSummary {
  waitingCount: number | null;
  estimatedWaitMinutesForNew: number | null;
}

/** "Kapıda sırada · 3 · ~35 dk bekleme" (salon-paneli/Main, GenelBakis); sayı manageListWalkIns'ten (T-91 (6)). */
export function WalkInBlock({ summary }: { summary: WalkInSummary }) {
  const { waitingCount, estimatedWaitMinutesForNew: estimate } = summary;
  return (
    <BentoBlock
      tone="kum"
      label="Kapıda sırada"
      value={waitingCount === null ? '—' : String(waitingCount)}
      sub={
        waitingCount === null
          ? 'sıra yüklenmedi'
          : waitingCount === 0
            ? 'sırada kimse yok'
            : estimate !== null
              ? `~${formatDuration(estimate)} bekleme`
              : 'müsait personel yok'
      }
    />
  );
}

/**
 * Özet blokları (salon-paneli/Main): sıradaki, onay bekleyen, kapıda sırada, bugün. Bugün bloğu kasayı görebilen
 * kullanıcıda günün tahsilatını ("9.840 ₺", manageGetCashDay) ve randevu sayısını, diğerlerinde randevu sayısını
 * gösterir (sahte sayı yok; acik-sorular.md T-97). "Kapıda sırada" manageListWalkIns'ten (T-98). "Onay bekliyor" kapora
 * bekleyenleri saymaz (T-84 (3)).
 */
export function CalendarSummaryBlocks({
  view,
  now,
  showPending,
  walkIns,
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
      {walkIns ? <WalkInBlock summary={walkIns} /> : null}
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
