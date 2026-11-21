import { Link2 } from 'lucide-react';
import { StatePage } from '@/components/StatePage';

/**
 * "Bağlantı geçersiz" deseni (Durumlar.png). Tasarımdaki "Salon sayfasına dön" eylemi yok: 404'te kodun
 * hangi salona ait olduğu bilinmez (T-52, ayrımsız 404).
 */
export function AppointmentMissing() {
  return (
    <StatePage
      icon={<Link2 size={24} />}
      title="Bu randevu bulunamadı"
      description="Bağlantının süresi dolmuş ya da randevu iptal edilmiş olabilir."
    />
  );
}
