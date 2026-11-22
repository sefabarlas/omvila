import { Button } from '@omvila/ui-web/panel';
import type { CSSProperties } from 'react';
import type { ManageOverview } from './api';
import styles from './Overview.module.css';

/**
 * "Bu hafta doluluk · personel bazında" (salon-paneli/GenelBakis): doluluğa göre azalan satırlar; nokta personelin
 * takvim rengi (veri), çubuk marka rengi. Çalışma dakikası olmayan personel listede yoktur (manageGetOverview).
 */
export function OccupancyCard({
  occupancy,
  isError,
  onRetry,
}: {
  occupancy: ManageOverview['weeklyOccupancy'] | undefined;
  isError: boolean;
  onRetry: () => void;
}) {
  return (
    <section className={styles.card} aria-labelledby="bu-hafta-doluluk">
      <div className={styles.cardHead}>
        <h2 id="bu-hafta-doluluk" className={styles.listTitle}>
          Bu hafta doluluk
        </h2>
        <span className={styles.progress}>personel bazında</span>
      </div>
      {isError ? (
        <div className={styles.cardError}>
          <p>Doluluk yüklenemedi.</p>
          <Button size="sm" onClick={onRetry}>
            Tekrar dene
          </Button>
        </div>
      ) : !occupancy ? null : occupancy.staff.length === 0 ? (
        <p className={styles.progress}>Bu hafta çalışma saati tanımlı personel yok.</p>
      ) : (
        <ul className={styles.occupancy}>
          {occupancy.staff.map((row) => (
            <li key={row.staffId} className={styles.occupancyRow}>
              <span className={styles.occupancyName}>
                <span
                  className={styles.occupancyDot}
                  style={{ '--personel-rengi': row.calendarColor } as CSSProperties}
                  aria-hidden="true"
                />
                {row.staffName}
              </span>
              <span className={styles.occupancyValue}>%{row.percent}</span>
              <span
                className={styles.occupancyTrack}
                role="meter"
                aria-label={`${row.staffName} doluluk`}
                aria-valuemin={0}
                aria-valuemax={100}
                aria-valuenow={row.percent}
              >
                <span
                  className={styles.occupancyBar}
                  style={{ width: `${String(row.percent)}%` }}
                />
              </span>
            </li>
          ))}
        </ul>
      )}
    </section>
  );
}
