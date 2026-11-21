import type { CSSProperties } from 'react';
import { Avatar } from '../panel/Avatar';
import { TimeGrid } from './TimeGrid';
import type { GridColumn } from './TimeGrid';
import type { CalendarCommonProps, CalendarStaff } from './types';
import styles from './Calendar.module.css';

export interface CalendarGridProps extends CalendarCommonProps {
  /** Şubenin yerel takvim günü: "2026-11-25". */
  date: string;
  /** Her personel bir kolon (sıra korunur). */
  staff: readonly CalendarStaff[];
}

/** Personel kolon başlığı: takvim rengi yalnız avatar halkasında (panel README § Takvim). */
export function StaffColumnHeader({ staff }: { staff: CalendarStaff }) {
  return (
    <>
      <span
        className={styles.staffRing}
        style={{ '--personel-rengi': staff.color } as CSSProperties}
      >
        <Avatar name={staff.name} src={staff.photoUrl ?? null} size="sm" />
      </span>
      <span className={styles.staffText}>
        <strong>{staff.name}</strong>
        {staff.subtitle ? <small>{staff.subtitle}</small> : null}
      </span>
    </>
  );
}

/**
 * Günlük takvim (DayCalendar): her personel bir kolon, 15 dakikalık ızgara, şu anki saat mercan çizgi ve
 * etiketiyle, boş alana tıklama yeni randevu, bloğa tıklama detay, isteğe bağlı sürükle-bırak.
 */
export function CalendarGrid({ date, staff, label, ...common }: CalendarGridProps) {
  const columns: GridColumn[] = staff.map((member) => ({
    key: member.id,
    staffId: member.id,
    date,
    name: member.name,
    header: <StaffColumnHeader staff={member} />,
  }));

  return <TimeGrid {...common} label={label ?? 'Günlük takvim'} columns={columns} showNowLabel />;
}
