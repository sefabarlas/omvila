import { XCircle } from 'lucide-react';
import { useId, useRef } from 'react';
import type { ClipboardEvent, KeyboardEvent } from 'react';
import styles from './CodeInput.module.css';

export interface CodeInputProps {
  label: string;
  value: string;
  onChange: (value: string) => void;
  /** Hane sayısı (OTP 6). */
  length?: number;
  error?: string | undefined;
  disabled?: boolean;
  /** İlk haneye açılışta odaklan (kod ekranının tek görevi). */
  focusOnMount?: boolean;
}

/**
 * Tek kullanımlık kod: hane başına kutu, yazınca sonrakine geçer, geri silme öncekine döner, yapıştırma dağıtılır.
 * İlk kutu `one-time-code` otomatik doldurmasını alır (SMS'ten gelen kod).
 */
export function CodeInput({
  label,
  value,
  onChange,
  length = 6,
  error,
  disabled = false,
  focusOnMount = false,
}: CodeInputProps) {
  const id = useId();
  const refs = useRef<(HTMLInputElement | null)[]>([]);
  const digits = Array.from({ length }, (_, i) => value[i] ?? '');

  const set = (index: number, digit: string) => {
    const next = digits.slice();
    next[index] = digit;
    onChange(next.join('').slice(0, length));
  };

  const fill = (text: string, from: number) => {
    const incoming = text.replace(/\D/g, '');
    if (!incoming) {
      return;
    }

    const next = digits.slice();
    for (let i = 0; i < incoming.length && from + i < length; i++) {
      next[from + i] = incoming[i] ?? '';
    }

    onChange(next.join(''));
    refs.current[Math.min(from + incoming.length, length - 1)]?.focus();
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
    <fieldset className={styles.group} aria-describedby={error ? `${id}-error` : undefined}>
      <legend className={styles.label}>{label}</legend>
      <div className={styles.boxes}>
        {digits.map((digit, index) => (
          <input
            // Haneler sabit konumludur; sıra değişmez (dizin anahtar olarak güvenli).
            key={index}
            ref={(el) => {
              refs.current[index] = el;
            }}
            className={styles.box}
            inputMode="numeric"
            autoComplete={index === 0 ? 'one-time-code' : 'off'}
            // eslint-disable-next-line jsx-a11y/no-autofocus -- kod ekranının tek görevi
            autoFocus={focusOnMount && index === 0}
            aria-label={`${label}, hane ${String(index + 1)}`}
            aria-invalid={error ? true : undefined}
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
              if (d && index < length - 1) {
                refs.current[index + 1]?.focus();
              }
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
      {error ? (
        <p id={`${id}-error`} className={styles.hint}>
          <XCircle size={14} aria-hidden="true" />
          {error}
        </p>
      ) : null}
    </fieldset>
  );
}
