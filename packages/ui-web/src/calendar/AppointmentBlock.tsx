import { Repeat } from 'lucide-react';
import type { ButtonHTMLAttributes, CSSProperties, Ref } from 'react';
import { formatTime } from '../format';
import { classNames } from '../panel/Button';
import { statusLabels } from '../status';
import { COMPACT_BELOW_MINUTES } from './geometry';
import type { CalendarBlock } from './types';
import styles from './AppointmentBlock.module.css';

export interface AppointmentBlockProps extends Omit<
  ButtonHTMLAttributes<HTMLButtonElement>,
  'children' | 'style'
> {
  block: CalendarBlock;
  timeZone: string;
  /** Izgara içindeki yer (saat cinsinden) ve çakışma şeridi; verilmezse akışta durur (sürükleme hayaleti). */
  position?: { top: number; height: number; lane?: number; lanes?: number };
  /** Sürüklenirken kaynağı soluklaştırır. */
  dragging?: boolean;
  buttonRef?: Ref<HTMLButtonElement>;
  style?: CSSProperties;
}

/** Ekran okuyucu adı: "Selin Öztürk, Boya ve fön, 10:30–12:30, Geldi". */
export function blockAccessibleName(block: CalendarBlock, timeZone: string): string {
  const time = `${formatTime(block.startUtc, timeZone)}–${formatTime(block.endUtc, timeZone)}`;
  const parts = [block.customerName, block.serviceName, time, statusLabels[block.status]];
  if (block.note) {
    parts.push(block.note);
  }

  if (block.isRecurring) {
    parts.push('tekrarlayan');
  }

  return parts.join(', ');
}

function durationMinutes(block: CalendarBlock): number {
  return Math.round((Date.parse(block.endUtc) - Date.parse(block.startUtc)) / 60_000);
}

/**
 * Takvim bloğu (AppointmentBlock README): zemin ve üst şerit durum rengini taşır; 45 dakikadan kısa blok tek
 * satır; bekleyen (onay, kapora) kesik çizgili; iptal üstü çizili; bekleme dilimi taralı "Bekleme · müsait";
 * `note` durumun yanında küçük işarettir ("Havale bildirildi").
 */
export function AppointmentBlock({
  block,
  timeZone,
  position,
  dragging = false,
  buttonRef,
  className,
  style,
  type = 'button',
  ...rest
}: AppointmentBlockProps) {
  const total = durationMinutes(block);
  const compact = total < COMPACT_BELOW_MINUTES;
  const start = Date.parse(block.startUtc);
  const gaps = total > 0 ? (block.segments ?? []).filter((segment) => !segment.staffBusy) : [];
  const showStatus = block.status !== 'onaylandi';
  const positionStyle = position
    ? ({
        '--blok-ust': position.top,
        '--blok-boy': position.height,
        '--blok-serit': position.lane ?? 0,
        '--blok-serit-sayisi': position.lanes ?? 1,
      } as CSSProperties)
    : undefined;

  return (
    <button
      {...rest}
      ref={buttonRef}
      type={type}
      aria-label={blockAccessibleName(block, timeZone)}
      className={classNames(
        styles.block,
        styles[block.status],
        position && styles.placed,
        compact && styles.compact,
        dragging && styles.dragging,
        className,
      )}
      style={{ ...positionStyle, ...style }}
      data-status={block.status}
    >
      <span className={styles.top} aria-hidden="true">
        {compact ? <span className={styles.dot} /> : null}
        <strong className={styles.customer}>{block.customerName}</strong>
        {block.isRecurring ? <Repeat className={styles.repeat} size={12} strokeWidth={2} /> : null}
        {compact && block.note ? <span className={styles.note}>{block.note}</span> : null}
        <span className={styles.time}>{formatTime(block.startUtc, timeZone)}</span>
      </span>
      {compact ? null : (
        <>
          <span className={styles.service} aria-hidden="true">
            {block.serviceName}
          </span>
          {showStatus ? (
            <span className={styles.status} aria-hidden="true">
              <span className={styles.dot} />
              {statusLabels[block.status]}
              {block.note ? <span className={styles.note}>{block.note}</span> : null}
            </span>
          ) : null}
        </>
      )}
      {gaps.map((gap) => {
        const offset = Math.max(0, (Date.parse(gap.startUtc) - start) / 60_000);
        const size = Math.min(
          total - offset,
          (Date.parse(gap.endUtc) - Date.parse(gap.startUtc)) / 60_000,
        );
        if (size <= 0) {
          return null;
        }

        return (
          <span
            key={gap.startUtc}
            className={styles.gap}
            data-testid="bekleme-seridi"
            aria-hidden="true"
            style={{ '--bekleme-ust': offset / 60, '--bekleme-boy': size / 60 } as CSSProperties}
          >
            <span>Bekleme · müsait</span>
          </span>
        );
      })}
    </button>
  );
}
