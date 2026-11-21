import { Check, Users } from 'lucide-react';
import type { ReactNode } from 'react';
import { BookingAvatar } from './BookingAvatar';
import { cx, useRovingRadio } from './internal';
import shared from './shared.module.css';
import styles from './StaffPicker.module.css';

/** "Fark etmez" seçeneğinin değeri. */
export const ANY_STAFF = 'any';

export interface StaffOption {
  id: string;
  /** "Elif Kaya". */
  name: string;
  /** "Kurucu · son randevunuz onunla". */
  title?: string | null;
  photoUrl?: string | null;
  /** Personel ek ücreti: "+50 ₺". */
  priceNote?: string | null;
}

export interface StaffPickerProps {
  staff: readonly StaffOption[];
  /** Seçili personel kimliği veya ANY_STAFF; seçim yoksa null. */
  value: string | null;
  onChange: (staffId: string) => void;
  /**
   * "Fark etmez" seçeneği (her zaman ilk sırada). Marka ayarı (allowAnyStaffOption) kapalıysa false.
   * description varsayılanı "En erken uygun saat".
   */
  anyStaff?: { label?: string; description?: string } | false;
  /** Radyo grubunun ekran okuyucu adı. */
  label?: string;
  /** Listenin altı: "İki hizmet de aynı kişiden" anahtarı (SwitchCard). */
  children?: ReactNode;
  className?: string;
}

interface Row extends StaffOption {
  any?: boolean;
}

/** Personel seçimi (components/StaffPicker): tek seçim, radyo grubu, oklarla gezinme. */
export function StaffPicker({
  staff,
  value,
  onChange,
  anyStaff = {},
  label = 'Personel',
  children,
  className,
}: StaffPickerProps) {
  const rows: Row[] = [
    ...(anyStaff === false
      ? []
      : [
          {
            id: ANY_STAFF,
            name: anyStaff.label ?? 'Fark etmez',
            title: anyStaff.description ?? 'En erken uygun saat',
            any: true,
          },
        ]),
    ...staff,
  ];
  const { itemProps } = useRovingRadio(
    rows.map((row) => ({ value: row.id })),
    value,
    onChange,
  );

  return (
    <div className={cx(styles.root, className)}>
      <div className={styles.list} role="radiogroup" aria-label={label}>
        {rows.map((row) => {
          const selected = row.id === value;
          return (
            <button
              key={row.id}
              type="button"
              className={cx(shared.card, selected && shared.cardSelected)}
              {...itemProps({ value: row.id })}
            >
              {row.any ? (
                <span className={styles.anyIcon} aria-hidden="true">
                  <Users size={22} />
                </span>
              ) : (
                <BookingAvatar name={row.name} src={row.photoUrl ?? null} decorative />
              )}
              <span className={shared.cardText}>
                <span className={shared.cardTitle}>{row.name}</span>
                {row.title ? <span className={shared.cardMeta}>{row.title}</span> : null}
              </span>
              {row.priceNote ? (
                <span className={cx(shared.price, styles.note)}>{row.priceNote}</span>
              ) : null}
              <span className={cx(shared.check, selected && shared.checkOn)} aria-hidden="true">
                {selected ? <Check size={16} strokeWidth={2.5} /> : null}
              </span>
            </button>
          );
        })}
      </div>
      {children}
    </div>
  );
}
