import { Banner } from '@omvila/ui-web/panel';
import { branchToday, daysBetween } from '@omvila/ui-web/time';
import type { Clock } from '@omvila/ui-web/time';
import type { ManageSession } from './api';
import { useClock } from './clock';

const trialBannerDays = 7;

interface SubscriptionBannerProps {
  subscription: ManageSession['subscription'];
  timeZone: string;
  /** Deneme ve ödeme şeritleri yalnız işletmeyi yönetenlere (ayar yetkisi); salt okunur şeridi herkese. */
  manages: boolean;
}

/**
 * Abonelik şeridi (durum-makinesi.md § Abonelik; salon-paneli/Durumlar). Ödeme ve plan sayfaları Dilim 6'da geldiği
 * için şeritlerde eylem butonu yoktur.
 */
export function SubscriptionBanner({ subscription, timeZone, manages }: SubscriptionBannerProps) {
  const clock = useClock();
  if (!manages && subscription.status !== 'ReadOnly') {
    return null;
  }

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
      // Son haftada şerit (T-40); öncesinde kalan gün kurulum ekranının bağlamında yazar (KayitKurulum).
      if (days === null || days > trialBannerDays) {
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
          Rezervasyon sayfanız şu an randevu almıyor. Takviminizi ve müşterilerinizi görebilirsiniz;
          yeni randevu eklenemez.
        </Banner>
      );
    default:
      return null;
  }
}
