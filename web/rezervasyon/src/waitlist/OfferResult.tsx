import type { Clock } from '@omvila/ui-web/time';
import { DepositStep } from '@/booking/steps/DepositStep';
import { DoneStep } from '@/booking/steps/DoneStep';
import { RequestStep } from '@/booking/steps/RequestStep';
import type { PublicAppointment } from './waitlist-api';

interface OfferResultProps {
  appointment: PublicAppointment;
  clock: Clock;
  onChange: (appointment: PublicAppointment) => void;
}

/**
 * Teklif kabulünün sonucu (W4), rezervasyon akışının sonuç ekranlarıyla aynı: kapora isteyen randevu Havale, onay
 * bekleyen Talep, onaylı Başarı. Maskeli numara bu bağlantıda yok; metin "telefonunuza" der.
 */
export function OfferResult({ appointment, clock, onChange }: OfferResultProps) {
  if (
    appointment.displayStatus === 'DepositPending' ||
    appointment.cancellation?.reason === 'DepositNotReceived'
  ) {
    return <DepositStep appointment={appointment} clock={clock} onChange={onChange} />;
  }
  return appointment.displayStatus === 'PendingApproval' ? (
    <RequestStep appointment={appointment} phoneMasked="" showPrices clock={clock} />
  ) : (
    <DoneStep appointment={appointment} phoneMasked="" />
  );
}
