import {
  DndContext,
  DragOverlay,
  KeyboardSensor,
  PointerSensor,
  pointerWithin,
  rectIntersection,
  useDraggable,
  useDroppable,
  useSensor,
  useSensors,
} from '@dnd-kit/core';
import type {
  Announcements,
  CollisionDetection,
  DragEndEvent,
  DragMoveEvent,
  DragStartEvent,
  KeyboardCoordinateGetter,
} from '@dnd-kit/core';
import { useMemo, useRef, useState } from 'react';
import type { CSSProperties, KeyboardEvent, ReactNode } from 'react';
import { classNames } from '../panel/Button';
import { formatClock, localMinutesOfDay, toBranchDate } from '../time';
import { AppointmentBlock, blockAccessibleName } from './AppointmentBlock';
import {
  SLOT_MINUTES,
  assignLanes,
  blockLayout,
  computeDropTarget,
  gridRange,
  hourLabels,
  minutesToPx,
  slotStartUtc,
} from './geometry';
import type { GridRange } from './geometry';
import type { CalendarBlock, CalendarCommonProps, CalendarDropTarget } from './types';
import styles from './TimeGrid.module.css';

/** Izgara kolonu: gün görünümünde personel, hafta görünümünde gün. */
export interface GridColumn {
  key: string;
  staffId: string;
  date: string;
  /** Boş alan butonlarının ve varsayılan ipucunun adı: "Elif Kaya" veya "Salı, 25 Kasım". */
  name: string;
  header: ReactNode;
}

export interface TimeGridProps extends CalendarCommonProps {
  columns: readonly GridColumn[];
  /** Şu anki saat etiketi saat oluğunda (gün görünümü). */
  showNowLabel?: boolean;
  /** Başlık satırı ek sınıfı. */
  headClassName?: string;
}

interface ColumnData {
  staffId: string;
  date: string;
  name: string;
}

interface DragData {
  block: CalendarBlock;
  column: ColumnData;
}

interface ActiveTarget extends CalendarDropTarget {
  columnKey: string;
  columnName: string;
}

const collisionDetection: CollisionDetection = (args) => {
  const byPointer = pointerWithin(args);
  return byPointer.length > 0 ? byPointer : rectIntersection(args);
};

/** Klavyeyle taşıma: ↑/↓ 15 dakika, ←/→ komşu kolon (dnd-kit klavye sensörü). */
export const calendarKeyboardCoordinates: KeyboardCoordinateGetter = (
  event,
  { currentCoordinates, context },
) => {
  const step = minutesToPx(SLOT_MINUTES);
  switch (event.code) {
    case 'ArrowDown':
      event.preventDefault();
      return { ...currentCoordinates, y: currentCoordinates.y + step };
    case 'ArrowUp':
      event.preventDefault();
      return { ...currentCoordinates, y: currentCoordinates.y - step };
    case 'ArrowLeft':
    case 'ArrowRight': {
      event.preventDefault();
      const rect = context.collisionRect;
      if (!rect) {
        return undefined;
      }

      const centerX = rect.left + rect.width / 2;
      const columns = [...context.droppableRects.values()].sort((a, b) => a.left - b.left);
      const index = columns.findIndex((c) => centerX >= c.left && centerX < c.left + c.width);
      const next = columns[index + (event.code === 'ArrowRight' ? 1 : -1)];
      if (index === -1 || !next) {
        return undefined;
      }

      return {
        ...currentCoordinates,
        x: currentCoordinates.x + next.left + next.width / 2 - centerX,
      };
    }
    default:
      return undefined;
  }
};

const screenReaderInstructions = {
  draggable:
    'Randevuyu açmak için Enter tuşuna basın. Taşımak için boşluk tuşuna basın; yukarı ve aşağı ok 15 dakika, sağ ve sol ok kolon değiştirir. Boşluk veya Enter ile bırakın, Escape ile vazgeçin.',
};

function defaultDescribe(target: ActiveTarget): string {
  return `${target.columnName} · ${target.startTime}–${target.endTime}`;
}

export function TimeGrid({
  columns,
  timeZone,
  openTime,
  closeTime,
  blocks,
  unavailable = [],
  now,
  onSlotClick,
  onBlockClick,
  onMove,
  describeDrop,
  label = 'Takvim',
  className,
  showNowLabel = false,
  headClassName,
}: TimeGridProps) {
  const range = useMemo(() => gridRange(openTime, closeTime), [openTime, closeTime]);
  const labels = hourLabels(range);
  const hours = (range.endMinutes - range.startMinutes) / 60;
  const slotCount = (range.endMinutes - range.startMinutes) / SLOT_MINUTES;
  const rootRef = useRef<HTMLDivElement>(null);
  const [rovingSlot, setRovingSlot] = useState<{ column: number; slot: number }>({
    column: 0,
    slot: 0,
  });
  const [active, setActive] = useState<DragData | null>(null);
  const [target, setTarget] = useState<ActiveTarget | null>(null);
  const targetRef = useRef<ActiveTarget | null>(null);
  const justDragged = useRef(false);

  const sensors = useSensors(
    useSensor(PointerSensor, { activationConstraint: { distance: 4 } }),
    useSensor(KeyboardSensor, {
      coordinateGetter: calendarKeyboardCoordinates,
      keyboardCodes: { start: ['Space'], cancel: ['Escape'], end: ['Space', 'Enter'] },
    }),
  );

  const nowDate = now ? toBranchDate(now, timeZone) : null;
  const nowMinutes = now ? localMinutesOfDay(now, timeZone) : null;
  const nowVisible =
    nowMinutes !== null && nowMinutes >= range.startMinutes && nowMinutes <= range.endMinutes;
  const nowTop = nowMinutes !== null ? (nowMinutes - range.startMinutes) / 60 : 0;
  const nowColumnIndex = columns.findIndex((c) => c.date === nowDate);

  const describe = (t: ActiveTarget): string =>
    describeDrop ? describeDrop(t) : defaultDescribe(t);

  const resolveTarget = (event: DragMoveEvent | DragEndEvent): ActiveTarget | null => {
    const data = event.active.data.current as DragData | undefined;
    const overColumn = columns.find((c) => c.key === event.over?.id);
    if (!data || !overColumn) {
      return null;
    }

    const measured = event.over?.rect.height ?? 0;
    const hourHeightPx = measured > 0 ? measured / hours : undefined;
    const drop = computeDropTarget({
      block: data.block,
      from: data.column,
      to: { staffId: overColumn.staffId, date: overColumn.date },
      deltaY: event.delta.y,
      timeZone,
      range,
      ...(hourHeightPx ? { hourHeightPx } : {}),
    });
    return { ...drop, columnKey: overColumn.key, columnName: overColumn.name };
  };

  const updateTarget = (next: ActiveTarget | null) => {
    targetRef.current = next;
    setTarget(next);
  };

  const handleDragStart = (event: DragStartEvent) => {
    setActive((event.active.data.current as DragData | undefined) ?? null);
  };

  const handleDragMove = (event: DragMoveEvent) => {
    updateTarget(resolveTarget(event));
  };

  const finish = () => {
    setActive(null);
    updateTarget(null);
    justDragged.current = true;
    setTimeout(() => {
      justDragged.current = false;
    }, 0);
  };

  const handleDragEnd = (event: DragEndEvent) => {
    const drop = resolveTarget(event);
    finish();
    if (drop && onMove && (drop.staffChanged || drop.timeChanged)) {
      onMove({
        appointmentId: drop.appointmentId,
        itemId: drop.itemId,
        staffId: drop.staffId,
        startUtc: drop.startUtc,
      });
    }
  };

  const announcements: Announcements = {
    onDragStart: ({ active: a }) => {
      const data = a.data.current as DragData | undefined;
      return data ? `${data.block.customerName} taşınıyor.` : undefined;
    },
    onDragMove: () => (targetRef.current ? describe(targetRef.current) : undefined),
    onDragOver: ({ over }) =>
      over ? undefined : 'Bu konuma bırakılamaz. Escape ile vazgeçebilirsiniz.',
    onDragEnd: () =>
      targetRef.current ? `Bırakıldı: ${describe(targetRef.current)}.` : 'Taşıma iptal edildi.',
    onDragCancel: () => 'Taşıma iptal edildi. Randevu yerinde kaldı.',
  };

  const focusSlot = (column: number, slot: number) => {
    const c = Math.min(Math.max(column, 0), columns.length - 1);
    const s = Math.min(Math.max(slot, 0), slotCount - 1);
    setRovingSlot({ column: c, slot: s });
    rootRef.current
      ?.querySelector<HTMLButtonElement>(`[data-slot="${String(c)}-${String(s)}"]`)
      ?.focus();
  };

  const onSlotKeyDown = (event: KeyboardEvent, column: number, slot: number) => {
    const moves: Record<string, [number, number]> = {
      ArrowDown: [column, slot + 1],
      ArrowUp: [column, slot - 1],
      ArrowRight: [column + 1, slot],
      ArrowLeft: [column - 1, slot],
      Home: [column, 0],
      End: [column, slotCount - 1],
    };
    const move = moves[event.key];
    if (move) {
      event.preventDefault();
      focusSlot(move[0], move[1]);
    }
  };

  const gridStyle = { '--kolon-sayisi': columns.length, '--izgara-saat': hours } as CSSProperties;

  return (
    <section
      className={classNames(styles.calendar, className)}
      aria-label={label}
      style={gridStyle}
    >
      <DndContext
        sensors={sensors}
        collisionDetection={collisionDetection}
        onDragStart={handleDragStart}
        onDragMove={handleDragMove}
        onDragEnd={handleDragEnd}
        onDragCancel={finish}
        accessibility={{ announcements, screenReaderInstructions }}
      >
        <div className={styles.scroll} ref={rootRef}>
          <div className={classNames(styles.head, headClassName)}>
            <div className={styles.gutterHead} />
            {columns.map((column) => (
              <div key={column.key} className={styles.colHead}>
                {column.header}
              </div>
            ))}
          </div>
          <div className={styles.body}>
            <div className={styles.gutter} aria-hidden="true">
              {labels.map((hour) => (
                <div key={hour} className={styles.hour}>
                  <span>{hour}</span>
                </div>
              ))}
              {showNowLabel && nowVisible && nowColumnIndex !== -1 ? (
                <span
                  className={styles.nowLabel}
                  style={{ '--simdi-ust': nowTop } as CSSProperties}
                >
                  {formatClock(nowMinutes)}
                </span>
              ) : null}
            </div>
            {columns.map((column, columnIndex) => (
              <Column
                key={column.key}
                column={column}
                columnIndex={columnIndex}
                range={range}
                slotCount={slotCount}
                timeZone={timeZone}
                blocks={blocks}
                unavailable={unavailable}
                rovingSlot={rovingSlot}
                nowTop={nowVisible && column.date === nowDate ? nowTop : null}
                nowFirst={columnIndex === nowColumnIndex}
                draggable={Boolean(onMove)}
                activeItemId={active?.block.itemId ?? null}
                target={target?.columnKey === column.key ? target : null}
                hint={target?.columnKey === column.key ? describe(target) : null}
                onSlotClick={
                  onSlotClick
                    ? (slot) => {
                        setRovingSlot({ column: columnIndex, slot });
                        onSlotClick({
                          staffId: column.staffId,
                          date: column.date,
                          startUtc: slotStartUtc(
                            column.date,
                            range.startMinutes + slot * SLOT_MINUTES,
                            timeZone,
                          ),
                        });
                      }
                    : null
                }
                onSlotKeyDown={onSlotKeyDown}
                onBlockClick={(block) => {
                  if (!justDragged.current) {
                    onBlockClick?.(block.appointmentId, block);
                  }
                }}
              />
            ))}
          </div>
        </div>
        <DragOverlay dropAnimation={null}>
          {active ? (
            <div className={styles.ghost}>
              <strong>{active.block.customerName}</strong>
              <span> · {active.block.serviceName}</span>
            </div>
          ) : null}
        </DragOverlay>
      </DndContext>
    </section>
  );
}

interface ColumnProps {
  column: GridColumn;
  columnIndex: number;
  range: GridRange;
  slotCount: number;
  timeZone: string;
  blocks: readonly CalendarBlock[];
  unavailable: NonNullable<CalendarCommonProps['unavailable']>;
  rovingSlot: { column: number; slot: number };
  nowTop: number | null;
  nowFirst: boolean;
  draggable: boolean;
  activeItemId: string | null;
  target: ActiveTarget | null;
  hint: string | null;
  /** null: salt okunur ızgara; boş dilimler buton değil düz hücredir. */
  onSlotClick: ((slot: number) => void) | null;
  onSlotKeyDown: (event: KeyboardEvent, column: number, slot: number) => void;
  onBlockClick: (block: CalendarBlock) => void;
}

function Column({
  column,
  columnIndex,
  range,
  slotCount,
  timeZone,
  blocks,
  unavailable,
  rovingSlot,
  nowTop,
  nowFirst,
  draggable,
  activeItemId,
  target,
  hint,
  onSlotClick,
  onSlotKeyDown,
  onBlockClick,
}: ColumnProps) {
  const data: ColumnData = { staffId: column.staffId, date: column.date, name: column.name };
  const { setNodeRef, isOver } = useDroppable({ id: column.key, data });

  const placed = blocks
    .filter((block) => block.staffId === column.staffId)
    .map((block) => ({ block, layout: blockLayout(block, column.date, timeZone, range) }))
    .filter(
      (item): item is { block: CalendarBlock; layout: NonNullable<typeof item.layout> } =>
        item.layout !== null,
    );
  const lanes = assignLanes(placed.map((item) => item.layout));
  const shaded = unavailable
    .filter((r) => r.staffId === column.staffId)
    .map((r) => ({ range: r, layout: blockLayout(r, column.date, timeZone, range) }));

  return (
    <div
      ref={setNodeRef}
      className={classNames(styles.column, isOver && styles.columnOver)}
      data-column={column.key}
    >
      {shaded.map(({ range: r, layout }) =>
        layout ? (
          <div
            key={`${r.startUtc}-${r.endUtc}`}
            className={styles.unavailable}
            style={{ '--blok-ust': layout.top, '--blok-boy': layout.height } as CSSProperties}
          >
            {r.label ? <span className={styles.unavailableLabel}>{r.label}</span> : null}
          </div>
        ) : null,
      )}
      {Array.from({ length: slotCount }, (_, slot) => {
        const time = formatClock(range.startMinutes + slot * SLOT_MINUTES);
        const roving = rovingSlot.column === columnIndex && rovingSlot.slot === slot;
        if (!onSlotClick) {
          return <div key={time} className={styles.cell} />;
        }
        return (
          <button
            key={time}
            type="button"
            className={styles.slot}
            data-slot={`${String(columnIndex)}-${String(slot)}`}
            tabIndex={roving ? 0 : -1}
            aria-label={`Randevu ekle: ${column.name}, ${time}`}
            onClick={() => {
              onSlotClick(slot);
            }}
            onKeyDown={(event) => {
              onSlotKeyDown(event, columnIndex, slot);
            }}
          />
        );
      })}
      {placed.map(({ block, layout }, index) => (
        <DraggableBlock
          key={block.itemId}
          block={block}
          column={data}
          timeZone={timeZone}
          position={{ ...layout, ...lanes[index] }}
          enabled={draggable && block.draggable !== false}
          dragging={activeItemId === block.itemId}
          onClick={onBlockClick}
        />
      ))}
      {target ? (
        <div
          className={styles.dropTarget}
          aria-hidden="true"
          style={
            {
              '--blok-ust':
                (localMinutesOfDay(target.startUtc, timeZone) - range.startMinutes) / 60,
              '--blok-boy': (Date.parse(target.endUtc) - Date.parse(target.startUtc)) / 3_600_000,
            } as CSSProperties
          }
        >
          {hint ? <span className={styles.hint}>{hint}</span> : null}
        </div>
      ) : null}
      {nowTop !== null ? (
        <div
          className={classNames(styles.now, nowFirst && styles.nowFirst)}
          style={{ '--simdi-ust': nowTop } as CSSProperties}
          aria-hidden="true"
        />
      ) : null}
    </div>
  );
}

interface DraggableBlockProps {
  block: CalendarBlock;
  column: ColumnData;
  timeZone: string;
  position: { top: number; height: number; lane?: number; lanes?: number };
  enabled: boolean;
  dragging: boolean;
  onClick: (block: CalendarBlock) => void;
}

function DraggableBlock({
  block,
  column,
  timeZone,
  position,
  enabled,
  dragging,
  onClick,
}: DraggableBlockProps) {
  const data: DragData = { block, column };
  const { setNodeRef, listeners, attributes } = useDraggable({
    id: block.itemId,
    data,
    disabled: !enabled,
    attributes: { roleDescription: 'taşınabilir randevu' },
  });
  const dragProps = enabled ? { ...attributes, ...listeners } : {};

  return (
    <AppointmentBlock
      {...dragProps}
      block={block}
      timeZone={timeZone}
      position={position}
      dragging={dragging}
      buttonRef={setNodeRef}
      aria-label={blockAccessibleName(block, timeZone)}
      onClick={() => {
        onClick(block);
      }}
    />
  );
}
