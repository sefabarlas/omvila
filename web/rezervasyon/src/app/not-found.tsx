import { Link2 } from 'lucide-react';
import type { Metadata } from 'next';
import { StatePage } from '@/components/StatePage';
import { notFoundTitle, privateRobots } from '@/lib/metadata';

export const metadata: Metadata = { title: notFoundTitle, robots: privateRobots };

/** Bilinmeyen adres veya salon bağlantısı (404). Salon keşfi olmadığı için yönlendirilecek liste yok. */
export default function NotFound() {
  return (
    <StatePage
      icon={<Link2 size={24} />}
      title={notFoundTitle}
      description="Bağlantı yanlış yazılmış ya da salon bağlantısını değiştirmiş olabilir. Güncel bağlantıyı salondan isteyebilirsiniz."
    />
  );
}
