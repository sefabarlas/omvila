import { Check } from 'lucide-react';
import { useId } from 'react';
import { formatDuration } from '../format/date';
import { formatMoney, formatPrice } from '../format/money';
import type { MoneyValue, PriceDisplay } from '../format/money';
import { cx } from './internal';
import shared from './shared.module.css';

export interface ServiceItemProps {
  /** "Saç kesimi". */
  name: string;
  durationMinutes: number;
  /** showPrices kapalıysa null: fiyat gösterilmez. */
  price?: MoneyValue | null;
  /** StartingFrom: "2.800 ₺'den". */
  priceDisplay?: PriceDisplay;
  /** Alt satır açıklaması: "Uzunluğa göre değişir". */
  description?: string | null;
  /** Bilgi amaçlı kapora notu: "Kapora 300 ₺". */
  depositHint?: MoneyValue | null;
  checked: boolean;
  onCheckedChange: (checked: boolean) => void;
  disabled?: boolean;
  className?: string;
}

/** Hizmetin alt satırı: "3 sa · Kapora 300 ₺", "1 sa 30 dk · Uzunluğa göre değişir". */
export function serviceMeta(
  durationMinutes: number,
  description?: string | null,
  depositHint?: MoneyValue | null,
): string {
  return [
    formatDuration(durationMinutes),
    description,
    depositHint ? `Kapora ${formatMoney(depositHint)}` : null,
  ]
    .filter(Boolean)
    .join(' · ');
}

/**
 * Seçilebilir hizmet kartı (components/ServiceItem). Birden fazla hizmet seçilebildiği için onay kutusu
 * semantiği: kartın tamamı etikettir, Boşluk tuşu seçer. Ad erişilebilir ad, süre ve fiyat açıklamadır.
 */
export function ServiceItem({
  name,
  durationMinutes,
  price,
  priceDisplay = 'Fixed',
  description,
  depositHint,
  checked,
  onCheckedChange,
  disabled = false,
  className,
}: ServiceItemProps) {
  const id = useId();
  const nameId = `${id}-ad`;
  const metaId = `${id}-bilgi`;
  return (
    <label
      className={cx(
        shared.card,
        checked && shared.cardSelected,
        disabled && shared.cardDisabled,
        className,
      )}
    >
      <input
        type="checkbox"
        className={shared.visuallyHidden}
        checked={checked}
        disabled={disabled}
        aria-labelledby={nameId}
        aria-describedby={metaId}
        onChange={(event) => {
          onCheckedChange(event.target.checked);
        }}
      />
      <span className={shared.cardText}>
        <span id={nameId} className={shared.cardTitle}>
          {name}
        </span>
        <span className={shared.cardMeta}>
          <span id={metaId}>
            {serviceMeta(durationMinutes, description, depositHint)}
            {price ? (
              <span className={shared.visuallyHidden}>, {formatPrice(price, priceDisplay)}</span>
            ) : null}
          </span>
        </span>
      </span>
      {price ? (
        <span className={shared.price} aria-hidden="true">
          {formatPrice(price, priceDisplay)}
        </span>
      ) : null}
      <span className={cx(shared.check, checked && shared.checkOn)} aria-hidden="true">
        {checked ? <Check size={16} strokeWidth={2.5} /> : null}
      </span>
    </label>
  );
}
