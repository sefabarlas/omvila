import { formatMoney } from '@omvila/ui-web/format';
import type { DepositRule, Service } from './api';

/** "450 ₺"; "En az" gösterimde "1.800 ₺'den". */
export function formatServicePrice(service: Pick<Service, 'price' | 'priceDisplay'>): string {
  const amount = formatMoney(service.price);
  return service.priceDisplay === 'StartingFrom' ? `${amount}'den` : amount;
}

/**
 * Kapora kolonu: "Yok", "Genel ayar", "Her zaman · 300 ₺", "Her zaman · %30". IBAN yoksa kapora istenemez, "Yok".
 */
export function depositLabel(deposit: DepositRule, depositAvailable: boolean): string {
  if (!depositAvailable || deposit.mode === 'Never') {
    return 'Yok';
  }

  if (deposit.mode === 'Inherit') {
    return 'Genel ayar';
  }

  if (deposit.amount) {
    return `Her zaman · ${formatMoney(deposit.amount)}`;
  }

  return deposit.percent !== undefined ? `Her zaman · %${String(deposit.percent)}` : 'Her zaman';
}
