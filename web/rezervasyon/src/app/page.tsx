import { Link2 } from 'lucide-react';
import type { Metadata } from 'next';
import { StatePage } from '@/components/StatePage';
import { privateRobots } from '@/lib/metadata';

export const metadata: Metadata = { robots: privateRobots };

/**
 * Kök adres. Salon keşfi yok (ürün kuralı 2): arama veya liste gösterilmez, müşteri salonun
 * paylaştığı bağlantı veya QR ile gelir.
 */
export default function HomePage() {
  return (
    <StatePage
      icon={<Link2 size={24} />}
      title="Salonun bağlantısını açın"
      description="Randevu almak için salonunuzun paylaştığı bağlantıyı veya QR kodu kullanın."
    />
  );
}
