import styles from './Segmented.module.css';

export interface SegmentedProps<T extends string | number> {
  /** Ekran okuyucu için grup adı (görünür ad genelde satır başlığındadır). */
  legend: string;
  name: string;
  options: readonly { value: T; label: string }[];
  value: T;
  onChange: (value: T) => void;
  disabled?: boolean;
  describedBy?: string;
  /** Form dışında duruyorsa bağlı olduğu form. */
  form?: string;
}

/** Az seçenekli tek seçim (radyo grubu); ok tuşlarıyla gezilir. */
export function Segmented<T extends string | number>({
  legend,
  name,
  options,
  value,
  onChange,
  disabled = false,
  describedBy,
  form,
}: SegmentedProps<T>) {
  return (
    <fieldset className={styles.group} disabled={disabled} aria-describedby={describedBy}>
      <legend className={styles.legend}>{legend}</legend>
      {options.map((option) => (
        <label key={String(option.value)} className={styles.option}>
          <input
            type="radio"
            className={styles.input}
            name={name}
            form={form}
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
