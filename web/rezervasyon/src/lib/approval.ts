import type { components } from '@omvila/api-client-ts';
import type { Clock } from '@omvila/ui-web/time';

// Onay bekleyen talep (Dilim 3, A1/A4/A5): kalan yanıt süresi ve iptal nedeninin müşteri metni. Süre yanıttaki
// `holdExpiresUtc`'den hesaplanır (sabit "2 saat" yok); müşteriye hiçbir yerde "reddedildi" denmez.

type Cancellation = NonNullable<components['schemas']['PublicAppointment']['cancellation']>;

/**
 * Kalan yanıt süresi, okunur biçimde: "2 saat", "1 saat 35 dakika", "45 dakika". Süre yoksa veya dolmuşsa null
 * (metin süresiz cümleye döner). Dakikaya yuvarlanır: oluşturma yanıtı birkaç saniye sonra çizildiğinde
 * 120 dakikalık tutma "2 saat" kalır.
 */
export function responseWindow(holdExpiresUtc: string | null | undefined, clock: Clock) {
  if (!holdExpiresUtc) return null;
  const minutes = Math.round((new Date(holdExpiresUtc).getTime() - clock.now().getTime()) / 60_000);
  if (minutes <= 0) return null;
  const hours = Math.floor(minutes / 60);
  const rest = minutes % 60;
  if (hours === 0) return `${String(rest)} dakika`;
  return rest === 0 ? `${String(hours)} saat` : `${String(hours)} saat ${String(rest)} dakika`;
}

/** "Kuaför Elif en geç 2 saat içinde yanıtlar." (Talep.png); süre bilinmiyorsa süresiz. */
export function responseSentence(salonName: string, window: string | null): string {
  return window
    ? `${salonName} en geç ${window} içinde yanıtlar.`
    : `${salonName} talebinizi inceleyip yanıtlar.`;
}

/** Onay bekleyen talebin iptali mi (salon onaylayamadı veya zamanında yanıtlanmadı). */
export function isRequestCancellation(cancellation: Cancellation | null | undefined): boolean {
  return cancellation?.reason === 'DeclinedBySalon' || cancellation?.reason === 'ApprovalExpired';
}

/**
 * İptal kartının açıklaması. Dil bildirim şablonlarıyla aynı (bildirimler.md: `appointment.declined`,
 * `appointment.approval_expired`): "onaylayamadı" / "zamanında yanıtlayamadı", durum her zaman "İptal".
 */
export function cancellationText(salonName: string, cancellation: Cancellation | null | undefined) {
  switch (cancellation?.reason) {
    case 'DeclinedBySalon':
      return `${salonName} bu talebi onaylayamadı. Başka bir saat seçerek yeni randevu alabilirsiniz.`;
    case 'ApprovalExpired':
      return `${salonName} talebinizi zamanında yanıtlayamadı. Başka bir saat seçerek yeni randevu alabilirsiniz.`;
    default:
      return cancellation?.cancelledBy === 'Customer'
        ? 'İptal salona bildirildi. Yeni randevu için salon sayfasını açabilirsiniz.'
        : 'Yeni randevu için salon sayfasını açabilir veya salonu arayabilirsiniz.';
  }
}
