import { parseClock } from '@omvila/ui-web/time';
import type { SlotSearchResult } from './api';
import styles from './NewAppointment.module.css';

type Period = 'Morning' | 'Afternoon' | 'Evening';

const periodLabels: Record<Period, string> = {
  Morning: 'Sabah',
  Afternoon: 'Öğleden sonra',
  Evening: 'Akşam',
};

/** Sözleşmedeki sınırlar: Sabah < 12:00 ≤ Öğleden sonra < 17:00 ≤ Akşam (dolu saatler için). */
function periodOf(localTime: string): Period {
  const minutes = parseClock(localTime);
  return minutes < 12 * 60 ? 'Morning' : minutes < 17 * 60 ? 'Afternoon' : 'Evening';
}

interface Cell {
  time: string;
  taken: boolean;
}

/** Boş ve dolu saatleri dönemlere ayırır; dolu saat üstü çizili gösterilir (unavailableTimes). */
export function groupSlots(result: SlotSearchResult): { period: Period; cells: Cell[] }[] {
  const cells: (Cell & { period: Period })[] = [
    ...result.slots.map((slot) => ({ time: slot.localTime, taken: false, period: slot.period })),
    ...result.unavailableTimes
      .filter((time) => !result.slots.some((slot) => slot.localTime === time))
      .map((time) => ({ time, taken: true, period: periodOf(time) })),
  ].sort((a, b) => parseClock(a.time) - parseClock(b.time));
  return (['Morning', 'Afternoon', 'Evening'] as const)
    .map((period) => ({ period, cells: cells.filter((cell) => cell.period === period) }))
    .filter((group) => group.cells.length > 0);
}

interface SlotGridProps {
  result: SlotSearchResult;
  /** Grup adı (radyo grubu): "Çarşamba, 25 Kasım · Elif Kaya için boş saatler". */
  legend: string;
  value: string | null;
  onChange: (time: string) => void;
}

/**
 * Panel boş saat ızgarası (salon-paneli/YeniRandevu): dönem başlıkları, seçilebilir saatler radyo düğmesidir, dolu
 * saatler pasif ve üstü çizili. Rezervasyon sayfasının TimeSlotGrid'i mobil tokenlarla çizildiği için kullanılmaz.
 */
export function SlotGrid({ result, legend, value, onChange }: SlotGridProps) {
  const groups = groupSlots(result);
  return (
    <div role="radiogroup" aria-label={legend} className={styles.section}>
      {groups.map((group) => (
        <div key={group.period} className={styles.section}>
          <p className={styles.subLabel}>{periodLabels[group.period]}</p>
          <div className={styles.slots}>
            {group.cells.map((cell) => (
              <label
                key={cell.time}
                className={[styles.slot, cell.taken ? styles.slotTaken : ''].join(' ')}
              >
                <input
                  type="radio"
                  name="yeni-randevu-saat"
                  className={styles.slotInput}
                  value={cell.time}
                  disabled={cell.taken}
                  checked={!cell.taken && value === cell.time}
                  aria-label={cell.taken ? `${cell.time}, dolu` : cell.time}
                  onChange={() => {
                    onChange(cell.time);
                  }}
                />
                {cell.time}
              </label>
            ))}
          </div>
        </div>
      ))}
    </div>
  );
}
