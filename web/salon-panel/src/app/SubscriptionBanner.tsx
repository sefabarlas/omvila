import { Banner, Button } from '@omvila/ui-web/panel';
import { branchToday, daysBetween } from '@omvila/ui-web/time';
import { useNavigate } from '@tanstack/react-router';
import { CreditCard } from 'lucide-react';
import type { ManageSession } from './api';
import { useClock } from './clock';

const trialBannerDays = 7;
export const subscriptionRoute = '/ayarlar/abonelik';

type SubscriptionSummary = Pick<
  ManageSession['subscription'],
  'status' | 'trialEndsOn' | 'pastDueSince' | 'readOnlyOn'
>;

/** Salt okunur ve iptal: yeni randevu, online rezervasyon ve salonun başlattığı SMS kapalı (KK1, K5; T-118 (8)). */
export function blocksNewWork(status: ManageSession['subscription']['status']): boolean {
  return status === 'ReadOnly' || status === 'Cancelled';
}

interface SubscriptionBannerProps {
  subscription: SubscriptionSummary;
  timeZone: string;
  /** Deneme ve ödeme şeritleri yalnız işletmeyi yönetenlere (ayar yetkisi); salt okunur şeridi herkese. */
  manages: boolean;
  /** Ödeme eylemleri ("Havale ile öde", "Ödemeyi tamamla") yalnız subscription.manage (Owner). */
  canPay: boolean;
  /** Ayarlar → Abonelik ve fatura: deneme şeridi her gün görünür, ödeme sayfasına giden buton yoktur. */
  onSubscriptionPage?: boolean;
}

/**
 * Abonelik şeridi (durum-makinesi.md § Abonelik; salon-paneli/GenelBakisGecikti, TakvimSaltOkunur, Durumlar). Gün
 * sayıları sunucunun verdiği tarihlerden (pastDueSince, readOnlyOn) şubenin bugününe göre sayılır. Kart ödemesi
 * (T-06) olmadığı için "Kartı güncelle" yoktur; ödeme havaleyle Abonelik ve fatura sayfasından yapılır (T-125).
 */
export function SubscriptionBanner({
  subscription,
  timeZone,
  manages,
  canPay,
  onSubscriptionPage = false,
}: SubscriptionBannerProps) {
  const clock = useClock();
  const navigate = useNavigate();
  const { status } = subscription;
  if (!manages && !blocksNewWork(status)) {
    return null;
  }

  const today = branchToday(clock, timeZone);
  const payAction = (label: string) =>
    canPay && !onSubscriptionPage ? (
      <Button
        variant="primary"
        size="sm"
        icon={<CreditCard size={14} aria-hidden="true" />}
        onClick={() => void navigate({ to: subscriptionRoute })}
      >
        {label}
      </Button>
    ) : undefined;

  switch (status) {
    case 'Trialing': {
      const days = subscription.trialEndsOn ? daysBetween(today, subscription.trialEndsOn) : null;
      // Son haftada şerit (T-40); öncesinde kalan gün kurulum ekranının bağlamında yazar. Abonelik sayfasında her gün.
      if (days === null || (!onSubscriptionPage && days > trialBannerDays)) {
        return null;
      }

      return (
        <Banner
          tone="bilgi"
          title={
            days > 0
              ? `Deneme sürenizin bitmesine ${String(days)} gün var.`
              : 'Deneme süreniz bugün bitiyor.'
          }
        >
          {onSubscriptionPage
            ? 'Havale ile ödeyin; ödeme eşleşince abonelik deneme bitince kesintisiz devam eder. Ödeme yapılmazsa hatırlatma süresi başlar, sonra panel salt okunur olur ve online randevu kapanır.'
            : null}
        </Banner>
      );
    }

    case 'PastDue': {
      const day = subscription.pastDueSince
        ? daysBetween(subscription.pastDueSince, today) + 1
        : null;
      const left = subscription.readOnlyOn ? daysBetween(today, subscription.readOnlyOn) : null;
      const when =
        left === null
          ? 'Havale ile ödemezseniz'
          : left > 0
            ? `${String(left)} gün içinde havale ile ödemezseniz`
            : 'Bugün havale ile ödemezseniz';
      return (
        <Banner
          tone="uyari"
          title={
            day !== null && day > 0
              ? `Abonelik ödemesi alınamadı · ${String(day)}. gün`
              : 'Abonelik ödemesi alınamadı.'
          }
          action={payAction('Havale ile öde')}
        >
          {`${when} panel salt okunur olur ve online randevu kapanır.`}
        </Banner>
      );
    }

    case 'ReadOnly':
    case 'Cancelled': {
      const days = subscription.pastDueSince ? daysBetween(subscription.pastDueSince, today) : null;
      return (
        <Banner tone="hata" title="Panel salt okunur" action={payAction('Ödemeyi tamamla')}>
          {days !== null && days > 0
            ? `Abonelik ödemesi ${String(days)} gündür alınamadı. `
            : 'Abonelik ödemesi alınamadı. '}
          Randevularınız ve verileriniz duruyor; yeni randevu, online rezervasyon ve elle SMS
          kapalı. Var olan randevuların hatırlatmaları gider.
        </Banner>
      );
    }

    default:
      return null;
  }
}
