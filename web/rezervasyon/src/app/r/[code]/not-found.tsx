import { Link2 } from 'lucide-react';
import type { Metadata } from 'next';
import { StatePage } from '@/components/StatePage';
import { privateRobots } from '@/lib/metadata';

export const metadata: Metadata = { title: 'Bu randevu bulunamadı', robots: privateRobots };

/**
 * "Bağlantı geçersiz" deseni (Durumlar.png). Tasarımdaki "Salon sayfasına dön" eylemi yok: 404'te kodun
 * hangi salona ait olduğu bilinmez (T-52, ayrımsız 404).
 */
export default function AppointmentNotFound() {
  return (
    <StatePage
      icon={<Link2 size={24} />}
      title="Bu randevu bulunamadı"
      description="Bağlantının süresi dolmuş ya da randevu iptal edilmiş olabilir."
    />
  );
}
