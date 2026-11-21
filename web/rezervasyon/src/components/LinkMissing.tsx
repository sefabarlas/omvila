import { Link2 } from 'lucide-react';
import { StatePage } from './StatePage';

interface LinkMissingProps {
  /** "Bu teklif bulunamadı", "Bu bağlantı geçersiz". */
  title: string;
}

/**
 * SMS bağlantısının kodu bilinmiyor, süresi dolmuş veya biçimi bozuk ("Bağlantı geçersiz" deseni, Durumlar.png).
 * Kodun hangi salona ait olduğu bilinmez (ayrımsız 404); eylem yok.
 */
export function LinkMissing({ title }: LinkMissingProps) {
  return (
    <StatePage
      icon={<Link2 size={24} />}
      title={title}
      description="Bağlantı hatalı ya da artık geçerli değil. Güncel bağlantıyı salondan isteyebilirsiniz."
    />
  );
}
