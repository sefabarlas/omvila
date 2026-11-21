import { useId } from 'react';
import type { ReactNode } from 'react';
import { cx, useRovingRadio } from './internal';
import styles from './TimeSlotGrid.module.css';

export interface TimeSlot {
  /** Şubenin yerel saati, "14:30". */
  time: string;
  /** false: dolu, üstü çizili ve devre dışı (availability/slots unavailableTimes). */
  available: boolean;
}

export type SlotPeriod = 'sabah' | 'ogleden-sonra' | 'aksam';

export const slotPeriodLabels: Record<SlotPeriod, string> = {
  sabah: 'Sabah',
  'ogleden-sonra': 'Öğleden sonra',
  aksam: 'Akşam',
};

/** Gösterim grubu: 12:00 öncesi Sabah, 17:00 öncesi Öğleden sonra, sonrası Akşam. */
export function slotPeriod(time: string): SlotPeriod {
  if (time < '12:00') {
    return 'sabah';
  }

  return time < '17:00' ? 'ogleden-sonra' : 'aksam';
}

export interface TimeSlotGridProps {
  slots: readonly TimeSlot[];
  value: string | null;
  onChange: (time: string) => void;
  /** true: Sabah / Öğleden sonra / Akşam başlıklarıyla; false: tek ızgara (Ertele). */
  grouped?: boolean;
  /** Grup başlıklarının düzeyi. */
  headingLevel?: 2 | 3;
  /** Radyo grubunun ekran okuyucu adı. */
  label?: string;
  /** Uygun saat yoksa gösterilir (FullDayNotice). */
  empty?: ReactNode;
  className?: string;
}

/**
 * Saat ızgarası (components/TimeSlotGrid): 4 kolon, tek radyo grubu, oklarla uygun saatler arasında gezinme.
 * Dolu saatler devre dışı ve üstü çizili; ekran okuyucu "dolu" okur.
 */
export function TimeSlotGrid({
  slots,
  value,
  onChange,
  grouped = true,
  headingLevel = 2,
  label = 'Saat',
  empty,
  className,
}: TimeSlotGridProps) {
  const id = useId();
  const { itemProps } = useRovingRadio(
    slots.map((slot) => ({ value: slot.time, disabled: !slot.available })),
    value,
    onChange,
  );

  if (!slots.some((slot) => slot.available)) {
    return empty ? <div className={className}>{empty}</div> : null;
  }

  const Heading = headingLevel === 2 ? 'h2' : 'h3';
  const renderSlot = (slot: TimeSlot) => (
    <button
      key={slot.time}
      type="button"
      className={cx(styles.slot, slot.time === value && styles.selected)}
      aria-label={slot.available ? undefined : `${slot.time}, dolu`}
      {...itemProps({ value: slot.time, disabled: !slot.available })}
    >
      {slot.time}
    </button>
  );

  if (!grouped) {
    return (
      <div className={cx(styles.grid, className)} role="radiogroup" aria-label={label}>
        {slots.map(renderSlot)}
      </div>
    );
  }

  const groups = (Object.keys(slotPeriodLabels) as SlotPeriod[])
    .map((period) => ({ period, items: slots.filter((slot) => slotPeriod(slot.time) === period) }))
    .filter((group) => group.items.length > 0);

  return (
    <div className={cx(styles.groups, className)} role="radiogroup" aria-label={label}>
      {groups.map(({ period, items }) => (
        <div key={period} className={styles.group} role="group" aria-labelledby={`${id}-${period}`}>
          <Heading id={`${id}-${period}`} className={styles.heading}>
            {slotPeriodLabels[period]}
          </Heading>
          <div className={styles.grid}>{items.map(renderSlot)}</div>
        </div>
      ))}
    </div>
  );
}
