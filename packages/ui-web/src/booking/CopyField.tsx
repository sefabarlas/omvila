import { Check, Copy } from 'lucide-react';
import { useState } from 'react';
import type { ReactNode } from 'react';
import { BookingButton } from './BookingButton';
import { cx } from './internal';
import shared from './shared.module.css';
import styles from './CopyField.module.css';

export interface CopyFieldProps {
  /** "Alıcı", "IBAN", "Açıklama kodu", "Tutar". */
  label: string;
  /** Panoya kopyalanacak değer (IBAN boşluksuz). */
  value: string;
  /** Ekranda gösterilen değer (formatIban ile gruplanmış IBAN); verilmezse value. */
  displayValue?: ReactNode;
  /**
   * Kopyalama uygulamada yapılır (navigator.clipboard). false dönerse veya hata atarsa "Kopyalandı" gösterilmez.
   */
  onCopy: (value: string) => unknown;
  copyLabel?: string;
  copiedLabel?: string;
  className?: string;
}

/**
 * Kopyalanabilir alan (rezervasyon-sayfasi/Havale): etiket, değer, "Kopyala" butonu. Başarılı kopyalamada buton
 * "Kopyalandı" olur ve ekran okuyucuya bildirilir; değer değişince veya odak çıkınca eski hâline döner.
 */
export function CopyField({
  label,
  value,
  displayValue,
  onCopy,
  copyLabel = 'Kopyala',
  copiedLabel = 'Kopyalandı',
  className,
}: CopyFieldProps) {
  const [copiedValue, setCopiedValue] = useState<string | null>(null);
  const copied = copiedValue === value;

  const handleCopy = async () => {
    try {
      const result: unknown = await Promise.resolve(onCopy(value));
      setCopiedValue(result === false ? null : value);
    } catch {
      setCopiedValue(null);
    }
  };

  return (
    <div className={cx(styles.field, className)}>
      <div className={styles.text}>
        <span className={styles.label}>{label}</span>
        <span className={styles.value}>{displayValue ?? value}</span>
      </div>
      <BookingButton
        variant="secondary"
        size="sm"
        icon={copied ? <Check size={16} /> : <Copy size={16} />}
        onClick={() => {
          void handleCopy();
        }}
        onBlur={() => {
          setCopiedValue(null);
        }}
      >
        {copied ? copiedLabel : copyLabel}
        <span className={shared.visuallyHidden}>{` ${label}`}</span>
      </BookingButton>
      <span className={shared.visuallyHidden} role="status">
        {copied ? `${label} ${copiedLabel.toLocaleLowerCase('tr-TR')}` : ''}
      </span>
    </div>
  );
}

export interface CopyListProps {
  children: ReactNode;
  className?: string;
}

/** CopyField'ları çizgiyle ayrılmış tek kartta toplar (Havale ekranı). */
export function CopyList({ children, className }: CopyListProps) {
  return <div className={cx(styles.list, className)}>{children}</div>;
}
