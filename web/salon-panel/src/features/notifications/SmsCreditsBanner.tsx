import { Banner, Button } from '@omvila/ui-web/panel';
import { useNavigate } from '@tanstack/react-router';
import { useState } from 'react';
import type { ManageSession } from '@/app/api';
import { useSession } from '@/app/session';
import { CreditPurchaseModal } from '@/features/subscription/CreditPurchaseModal';
import { dativeCount } from '@/shared/turkish';
import { smsCreditsRoute } from './notification-view';

export interface SmsCreditsBannerProps {
  /** Oturumdaki kontör özeti; yalnız settings.manage izni olana döner (diğerlerinde alan yoktur). */
  smsCredits: ManageSession['smsCredits'];
}

/**
 * "Kontör az" şeridi (salon-paneli/Durumlar). Eşik sunucudadır (LowCreditThreshold): şerit yalnız `isLow` iken çıkar,
 * istemci sayıyı eşikle karşılaştırmaz. "Kontör al" işletme sahibinde (subscription.manage) kontör satın alma penceresini
 * açar (T-125); kontör alamayan yöneticide Ayarlar → Bildirimler ve SMS'i açar (T-69).
 */
export function SmsCreditsBanner({ smsCredits }: SmsCreditsBannerProps) {
  const navigate = useNavigate();
  const { can } = useSession();
  const [buying, setBuying] = useState(false);
  if (!smsCredits?.isLow) {
    return null;
  }

  const empty = smsCredits.balance === 0;
  return (
    <Banner
      tone="uyari"
      title={
        empty ? 'SMS kontörünüz bitti.' : `SMS kontörünüz ${dativeCount(smsCredits.balance)} düştü.`
      }
      action={
        <>
          <Button
            variant="primary"
            size="sm"
            onClick={() => {
              if (can('subscription.manage')) {
                setBuying(true);
              } else {
                void navigate({ to: smsCreditsRoute });
              }
            }}
          >
            Kontör al
          </Button>
          <CreditPurchaseModal
            open={buying}
            onClose={() => {
              setBuying(false);
            }}
          />
        </>
      }
    >
      {empty
        ? "Hatırlatma ve onay SMS'leri gitmiyor; uygulama bildirimleri devam ediyor."
        : "Bitince hatırlatma ve onay SMS'leri gitmez; uygulama bildirimleri devam eder."}
    </Banner>
  );
}
