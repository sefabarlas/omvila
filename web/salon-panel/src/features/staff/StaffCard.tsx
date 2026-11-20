import { Avatar, Badge } from '@omvila/ui-web/panel';
import type { CSSProperties } from 'react';
import type { StaffSummary } from './api';
import styles from './Staff.module.css';

/**
 * Personel kartı (salon-paneli/Personel). Bu hafta, doluluk ve prim randevu verisiyle (Dilim 2+) dolar; şimdilik
 * "—" (sahte sayı yok). Takvim rengi avatar halkasında.
 */
export function StaffCard({
  person,
  selected,
  onSelect,
}: {
  person: StaffSummary;
  selected: boolean;
  onSelect: () => void;
}) {
  const name = person.displayName ?? person.fullName;
  const ring = { '--halka': person.calendarColor } as CSSProperties;
  return (
    <button type="button" className={styles.card} aria-pressed={selected} onClick={onSelect}>
      <span className={styles.cardHead}>
        <span className={styles.ring} style={ring}>
          <Avatar name={name} src={person.photoUrl ?? null} />
        </span>
        <span className={styles.personText}>
          <span className={styles.name}>{name}</span>
          {person.title ? <span className={styles.title}>{person.title}</span> : null}
        </span>
      </span>
      <span className={styles.metrics}>
        {['Bu hafta', 'Doluluk', 'Bu ay prim'].map((label) => (
          <span key={label}>
            <span className={styles.metricLabel}>{label}</span>
            <span className={styles.metricValue}>—</span>
          </span>
        ))}
      </span>
      <span className={styles.badges}>
        {person.membershipStatus === 'Invited' ? (
          <Badge tone="bilgi">Davet gönderildi</Badge>
        ) : null}
        {person.isBookableOnline ? (
          <Badge tone="basari">Online randevuya açık</Badge>
        ) : (
          <Badge>Yalnızca panelden</Badge>
        )}
      </span>
    </button>
  );
}
