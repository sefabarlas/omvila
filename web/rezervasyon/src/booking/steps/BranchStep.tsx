import { branchOpenStatus } from '@omvila/ui-web/booking';
import type { Clock } from '@omvila/ui-web/time';
import { ChevronRight } from 'lucide-react';
import type { PublicBrand } from '@/lib/brand';
import styles from './steps.module.css';

interface BranchStepProps {
  brand: PublicBrand;
  clock: Clock;
  onSelect: (branchId: string) => void;
}

/** Şube seçimi (Sube.png); yalnız markanın birden fazla şubesi varsa. */
export function BranchStep({ brand, clock, onSelect }: BranchStepProps) {
  return (
    <div className={styles.stack}>
      <h2 className={styles.titleMedium}>Hangi şubeye gelmek istersiniz?</h2>
      <div className={styles.list}>
        {brand.branches.map((branch, index) => {
          const status = branchOpenStatus(branch, clock);
          return (
            <button
              key={branch.id}
              type="button"
              className={styles.branch}
              onClick={() => {
                onSelect(branch.id);
              }}
            >
              <span
                className={[styles.branchSwatch, index % 2 === 1 && styles.branchSwatchAlt]
                  .filter(Boolean)
                  .join(' ')}
                aria-hidden="true"
              />
              <span className={styles.branchText}>
                <span className={styles.branchName}>{branch.name}</span>
                <span className={styles.note}>
                  {branch.addressLine}, {branch.district}
                </span>
                <span
                  className={[styles.pill, status.open && styles.pillOpen]
                    .filter(Boolean)
                    .join(' ')}
                >
                  {status.label}
                </span>
              </span>
              <ChevronRight className={styles.chevron} size={20} aria-hidden="true" />
            </button>
          );
        })}
      </div>
      <p className={styles.note}>Fiyatlar ve personel şubeye göre değişebilir.</p>
    </div>
  );
}
