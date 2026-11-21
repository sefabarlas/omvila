import { useRef } from 'react';
import type { ClipboardEvent, KeyboardEvent } from 'react';
import styles from './fields.module.css';

interface CodeInputProps {
  label: string;
  value: string;
  onChange: (value: string) => void;
  length?: number;
  invalid?: boolean;
  disabled?: boolean;
  /** Hata metninin kimliği (aria-describedby). */
  describedBy?: string | undefined;
}

/**
 * SMS kodu (rezervasyon-sayfasi/Kod): hane başına kutu; yazınca sonrakine geçer, geri silme öncekine döner,
 * yapıştırma ve SMS otomatik doldurması (`one-time-code`) haneleri doldurur. Panel CodeInput'unun mobil ölçekli
 * karşılığı (panel CSS'i rezervasyon sayfasında yüklenmez).
 */
export function CodeInput({
  label,
  value,
  onChange,
  length = 6,
  invalid = false,
  disabled = false,
  describedBy,
}: CodeInputProps) {
  const refs = useRef<(HTMLInputElement | null)[]>([]);
  const digits = Array.from({ length }, (_, i) => value[i] ?? '');

  const fill = (text: string, from: number) => {
    const incoming = text.replace(/\D/g, '');
    if (!incoming) return;
    const next = digits.slice();
    for (let i = 0; i < incoming.length && from + i < length; i++) {
      next[from + i] = incoming[i] ?? '';
    }
    onChange(next.join(''));
    refs.current[Math.min(from + incoming.length, length - 1)]?.focus();
  };

  const set = (index: number, digit: string) => {
    const next = digits.slice();
    next[index] = digit;
    onChange(next.join(''));
  };

  const onKeyDown = (event: KeyboardEvent<HTMLInputElement>, index: number) => {
    if (event.key === 'Backspace' && !digits[index] && index > 0) {
      event.preventDefault();
      set(index - 1, '');
      refs.current[index - 1]?.focus();
    } else if (event.key === 'ArrowLeft' && index > 0) {
      refs.current[index - 1]?.focus();
    } else if (event.key === 'ArrowRight' && index < length - 1) {
      refs.current[index + 1]?.focus();
    }
  };

  return (
    <fieldset className={styles.codeGroup} aria-describedby={describedBy}>
      <legend className={styles.visuallyHidden}>{label}</legend>
      <div className={styles.codeBoxes}>
        {digits.map((digit, index) => (
          <input
            // Haneler sabit konumludur; dizin anahtar olarak güvenli.
            key={index}
            ref={(element) => {
              refs.current[index] = element;
            }}
            className={styles.codeBox}
            inputMode="numeric"
            pattern="[0-9]*"
            autoComplete={index === 0 ? 'one-time-code' : 'off'}
            // eslint-disable-next-line jsx-a11y/no-autofocus -- kod ekranının tek görevi
            autoFocus={index === 0}
            aria-label={`${label}, hane ${String(index + 1)}`}
            aria-invalid={invalid || undefined}
            disabled={disabled}
            value={digit}
            onChange={(event) => {
              const text = event.target.value;
              if (text.length > 1) {
                fill(text, index);
                return;
              }
              const d = text.replace(/\D/g, '');
              set(index, d);
              if (d && index < length - 1) refs.current[index + 1]?.focus();
            }}
            onKeyDown={(event) => {
              onKeyDown(event, index);
            }}
            onPaste={(event: ClipboardEvent<HTMLInputElement>) => {
              event.preventDefault();
              fill(event.clipboardData.getData('text'), index);
            }}
          />
        ))}
      </div>
    </fieldset>
  );
}
