import { CategoryChips, ServiceItem } from '@omvila/ui-web/booking';
import { useState } from 'react';
import type { Catalog } from '../booking-api';
import { maxServices } from '../selection';
import styles from './steps.module.css';

interface ServicesStepProps {
  catalog: Catalog;
  selected: readonly string[];
  onChange: (serviceIds: string[]) => void;
  /** Marka ayarı: kapalıysa fiyat gösterilmez (katalog da fiyatı null döner). */
  showPrices: boolean;
}

/** Hizmet seçimi (Main.png; başlık akışta, katalog gelmeden çizilir): kategori çipleri, kategoriye göre gruplu, çoklu seçim (en çok 5). */
export function ServicesStep({ catalog, selected, onChange, showPrices }: ServicesStepProps) {
  const [category, setCategory] = useState<string | null>(null);
  const categories = catalog.categories.filter((item) =>
    catalog.services.some((service) => service.categoryId === item.id),
  );
  const groups = categories
    .filter((item) => category === null || item.id === category)
    .map((item) => ({
      ...item,
      services: catalog.services.filter((service) => service.categoryId === item.id),
    }));
  const full = selected.length >= maxServices;

  return (
    <div className={styles.stack}>
      {categories.length > 1 ? (
        <CategoryChips categories={categories} value={category} onChange={setCategory} />
      ) : null}
      {groups.map((group) => (
        <section key={group.id} className={styles.list} aria-labelledby={`kategori-${group.id}`}>
          <h3 id={`kategori-${group.id}`} className={styles.groupTitle}>
            {group.name}
          </h3>
          <div className={styles.grid}>
            {group.services.map((service) => {
              const checked = selected.includes(service.id);
              return (
                <ServiceItem
                  key={service.id}
                  name={service.name}
                  durationMinutes={service.durationMinutes}
                  price={showPrices ? (service.price ?? null) : null}
                  priceDisplay={service.priceDisplay}
                  description={service.description ?? null}
                  depositHint={service.hasDeposit ? (service.depositHint ?? null) : null}
                  checked={checked}
                  disabled={!checked && full}
                  onCheckedChange={(next) => {
                    onChange(
                      next ? [...selected, service.id] : selected.filter((id) => id !== service.id),
                    );
                  }}
                />
              );
            })}
          </div>
        </section>
      ))}
      {full ? (
        <p className={styles.note} role="status">
          Bir randevuda en çok {maxServices} hizmet seçebilirsiniz.
        </p>
      ) : null}
    </div>
  );
}
