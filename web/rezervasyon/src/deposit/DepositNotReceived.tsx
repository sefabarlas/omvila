import { Phone, TimerOff } from 'lucide-react';
import { LinkButton } from '@/components/LinkButton';
import { StateBanner } from '@/components/StateBanner';

/**
 * Durumlar.png "Kapora gelmedi" (A5, `DepositNotReceived`): süre doldu, randevu iptal edildi. Süre yanıtta
 * olmadığı için metinde yazılmaz; salonun randevuyu yeniden açması v1 kapsamı dışında olduğundan vaat edilmez.
 */
export function DepositNotReceived({ phone }: { phone: string | null | undefined }) {
  return (
    <StateBanner
      tone="uyari"
      icon={<TimerOff size={20} />}
      title="Kapora süresinde gelmedi, randevu iptal edildi."
      description="Havaleyi yaptıysanız dekontla salonu arayın."
    >
      {phone ? (
        <LinkButton href={`tel:${phone}`} icon={<Phone size={18} />}>
          Salonu ara
        </LinkButton>
      ) : null}
    </StateBanner>
  );
}
