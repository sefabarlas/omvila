import { Button, useToast } from '@omvila/ui-web/panel';
import { Link2 } from 'lucide-react';
import { useSession } from '@/app/session';
import { useRecordOnboardingEvent } from './api';
import { displayUrl } from './OnboardingChecklist';
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
  const toast = useToast();
  const { can } = useSession();
  const record = useRecordOnboardingEvent();

  const copy = async () => {
    try {
      await navigator.clipboard.writeText(bookingUrl);
      toast.show({ title: 'Bağlantı kopyalandı' });
      if (can('settings.manage')) {
        record.mutate('BookingLinkCopied');
      }
    } catch {
      toast.show({
        tone: 'hata',
        title: 'Bağlantı kopyalanamadı',
        message: 'Bağlantıyı seçip elle kopyalayın.',
      });
    }
  };

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
        <Button variant="primary" size="sm" onClick={() => void copy()}>
          Kopyala
        </Button>
      </div>
    </section>
  );
}
