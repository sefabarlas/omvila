import { Banner } from '@omvila/ui-web/panel';
import { branchToday, daysBetween } from '@omvila/ui-web/time';
import type { Clock } from '@omvila/ui-web/time';
import type { ManageSession } from './api';
import { useClock } from './clock';

interface SubscriptionBannerProps {
  subscription: ManageSession['subscription'];
  timeZone: string;
}

/**
 * Abonelik şeridi (durum-makinesi.md § Abonelik; salon-paneli/Durumlar). Ödeme ve plan sayfaları Dilim 6'da geldiği
 * için şeritlerde eylem butonu yoktur.
 */
export function SubscriptionBanner({ subscription, timeZone }: SubscriptionBannerProps) {
  const clock = useClock();
  const banner = content(subscription, daysLeft(subscription, clock, timeZone));
  return banner;
}

function daysLeft(
  subscription: ManageSession['subscription'],
  clock: Clock,
  timeZone: string,
): number | null {
  return subscription.trialEndsOn
    ? daysBetween(branchToday(clock, timeZone), subscription.trialEndsOn)
    : null;
}

function content(subscription: ManageSession['subscription'], days: number | null) {
  switch (subscription.status) {
    case 'Trialing': {
      if (days === null) {
        return null;
      }

      return (
        <Banner
          tone="bilgi"
          title={
            days > 0
              ? `Deneme sürenizin bitmesine ${String(days)} gün var`
              : 'Deneme süreniz bugün bitiyor'
          }
        />
      );
    }

    case 'PastDue':
      return (
        <Banner tone="uyari" title="Ödemeniz alınamadı.">
          Kartınızı güncelleyin ya da havale ile ödeyin; aksi hâlde panel salt okunur olur ve online
          randevu kapanır.
        </Banner>
      );
    case 'ReadOnly':
      return (
        <Banner tone="hata" title="Online randevu kapalı.">
          Rezervasyon sayfanız şu an randevu almıyor. Takvimi görebilir, elle randevu
          ekleyebilirsiniz.
        </Banner>
      );
    default:
      return null;
  }
}
