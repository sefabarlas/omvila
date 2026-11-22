import styles from './FilterChips.module.css';

export interface FilterChipsProps<T extends string> {
  legend: string;
  name: string;
  options: readonly { value: T; label: string }[];
  value: T;
  onChange: (value: T) => void;
}

/**
 * Liste filtre çipleri (salon-paneli/Musteriler: Tümü, Düzenli…; platform-paneli/PMarkalar): tek seçim, radyo grubu;
 * ok tuşlarıyla gezilir.
 */
export function FilterChips<T extends string>({
  legend,
  name,
  options,
  value,
  onChange,
}: FilterChipsProps<T>) {
  return (
    <fieldset className={styles.chips}>
      <legend className={styles.visuallyHidden}>{legend}</legend>
      {options.map((option) => (
        <label key={option.value} className={styles.chip}>
          <input
            type="radio"
            className={styles.chipInput}
            name={name}
            checked={option.value === value}
            onChange={() => {
              onChange(option.value);
            }}
          />
          {option.label}
        </label>
      ))}
    </fieldset>
  );
}
