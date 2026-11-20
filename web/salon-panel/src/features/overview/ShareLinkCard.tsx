import { Button } from '@omvila/ui-web/panel';
import { Link2 } from 'lucide-react';
import { displayUrl, useCopyBookingLink } from '@/shared/booking-link';
import styles from './Overview.module.css';

/**
 * salon-paneli/GenelBakisBos: ilk hafta bağlantı paylaşma kutusu. Kopyalamak kurulumun "bağlantıyı paylaşın" adımını
 * tamamlar (yalnız ayar yetkisi olan bildirir). QR masa kartı ve hikâye görseli ayrı bir PR'da (T-40).
 */
export function ShareLinkCard({
  bookingUrl,
  setupComplete,
}: {
  bookingUrl: string;
  setupComplete: boolean;
}) {
  const copy = useCopyBookingLink();

  return (
    <section className={styles.share} aria-labelledby="paylas-baslik">
      <div>
        <p className={styles.shareLabel}>
          {setupComplete ? 'Kurulum tamam' : 'Rezervasyon bağlantınız'}
        </p>
        <h2 id="paylas-baslik" className={styles.shareTitle}>
          İlk online randevunuz için bağlantınızı paylaşın
        </h2>
        <p className={styles.shareText}>
          Müşterileriniz bu bağlantıdan uygulama indirmeden randevu alır. Randevular geldikçe bu
          ekran dolacak.
        </p>
      </div>
      <div className={styles.link}>
        <Link2 size={18} aria-hidden="true" />
        <span className={styles.linkText}>{displayUrl(bookingUrl)}</span>
        <Button variant="primary" size="sm" onClick={() => void copy(bookingUrl)}>
          Kopyala
        </Button>
      </div>
    </section>
  );
}
