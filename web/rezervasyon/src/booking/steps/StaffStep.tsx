import { StaffPicker, SwitchCard } from '@omvila/ui-web/booking';
import type { StaffOption } from '@omvila/ui-web/booking';
import { formatMoney } from '@omvila/ui-web/format';
import type { Catalog } from '../booking-api';
import { effectiveSameStaff, staffFor } from '../selection';
import type { Selection } from '../selection';
import styles from './steps.module.css';

interface StaffStepProps {
  catalog: Catalog;
  selection: Selection;
  showPrices: boolean;
  onChange: (staff: string[], sameStaff: boolean) => void;
}

/** Boş dize seçilmemiş demektir. */
const chosen = (value: string | undefined) => (value === undefined || value === '' ? null : value);

function options(catalog: Catalog, serviceIds: readonly string[], showPrices: boolean) {
  return staffFor(catalog, serviceIds).map((member): StaffOption => {
    const extra = showPrices
      ? (member.surcharges ?? [])
          .filter((item) => serviceIds.includes(item.serviceId))
          .reduce((sum, item) => sum + item.amount.amountMinor, 0)
      : 0;
    return {
      id: member.id,
      name: member.displayName,
      title: member.title ?? null,
      photoUrl: member.photoUrl ?? null,
      priceNote: extra > 0 ? `+${formatMoney({ amountMinor: extra, currency: 'TRY' })}` : null,
    };
  });
}

/**
 * Personel seçimi (Personel.png). İlk seçenek "Fark etmez" (allowAnyStaffOption). ≥ 2 hizmette "aynı kişiden"
 * anahtarı; kapalıyken her hizmete ayrı kişi seçilir (S-06).
 */
export function StaffStep({ catalog, selection, showPrices, onChange }: StaffStepProps) {
  const same = effectiveSameStaff(selection, catalog);
  const names = selection.serviceIds
    .map((id) => catalog.services.find((service) => service.id === id)?.name)
    .filter((name): name is string => Boolean(name));
  const anyStaff = catalog.allowAnyStaffOption ? {} : (false as const);

  const switchCard =
    selection.serviceIds.length >= 2 ? (
      <SwitchCard
        label={
          selection.serviceIds.length === 2
            ? 'İki hizmet de aynı kişiden'
            : 'Tüm hizmetler aynı kişiden'
        }
        description={same ? names.join(', ') : 'Kapatırsanız her hizmete ayrı kişi seçersiniz'}
        checked={same}
        onCheckedChange={(checked) => {
          onChange(checked ? [] : selection.serviceIds.map(() => ''), checked);
        }}
      />
    ) : null;

  if (same) {
    const staff = options(catalog, selection.serviceIds, showPrices);
    return (
      <div className={styles.stack}>
        {switchCard}
        {staff.length === 0 && anyStaff === false ? (
          <p className={styles.text} role="status">
            {selection.serviceIds.length > 1
              ? 'Seçtiğiniz hizmetleri aynı kişi vermiyor. Anahtarı kapatıp her hizmet için ayrı kişi seçin.'
              : 'Bu hizmet için online seçilebilen personel yok.'}
          </p>
        ) : (
          <StaffPicker
            staff={staff}
            value={chosen(selection.staff[0])}
            anyStaff={anyStaff}
            onChange={(id) => {
              onChange([id], true);
            }}
          />
        )}
        <p className={styles.note}>Yalnızca seçtiğiniz hizmetleri veren personel listelenir.</p>
      </div>
    );
  }

  return (
    <div className={styles.stack}>
      {switchCard}
      {selection.serviceIds.map((serviceId, index) => {
        const name = names[index] ?? '';
        return (
          <section
            key={serviceId}
            className={styles.list}
            aria-labelledby={`personel-${serviceId}`}
          >
            <h3 id={`personel-${serviceId}`} className={styles.groupTitle}>
              {name}
            </h3>
            <StaffPicker
              label={`${name} için personel`}
              staff={options(catalog, [serviceId], showPrices)}
              value={chosen(selection.staff[index])}
              anyStaff={anyStaff}
              onChange={(id) => {
                const next = selection.serviceIds.map((_, i) => selection.staff[i] ?? '');
                next[index] = id;
                onChange(next, false);
              }}
            />
          </section>
        );
      })}
    </div>
  );
}
