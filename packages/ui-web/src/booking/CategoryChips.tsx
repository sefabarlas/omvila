import { Check } from 'lucide-react';
import { cx } from './internal';
import styles from './CategoryChips.module.css';

export interface CategoryOption {
  id: string;
  name: string;
}

export interface CategoryChipsProps {
  categories: readonly CategoryOption[];
  /** Seçili kategori; null = "Tümü". */
  value: string | null;
  onChange: (categoryId: string | null) => void;
  allLabel?: string;
  /** Grubun ekran okuyucu adı. */
  label?: string;
  className?: string;
}

/** Hizmet kategorisi filtresi (components/Chip): tek seçim, seçili çipte onay işareti, aria-pressed. */
export function CategoryChips({
  categories,
  value,
  onChange,
  allLabel = 'Tümü',
  label = 'Kategori',
  className,
}: CategoryChipsProps) {
  const options: { id: string | null; name: string }[] = [
    { id: null, name: allLabel },
    ...categories,
  ];
  return (
    <div className={cx(styles.chips, className)} role="group" aria-label={label}>
      {options.map((option) => {
        const selected = option.id === value;
        return (
          <button
            key={option.id ?? 'tumu'}
            type="button"
            className={cx(styles.chip, selected && styles.selected)}
            aria-pressed={selected}
            onClick={() => {
              onChange(option.id);
            }}
          >
            {selected ? <Check size={16} strokeWidth={2.25} aria-hidden="true" /> : null}
            {option.name}
          </button>
        );
      })}
    </div>
  );
}
