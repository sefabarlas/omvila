import { CalendarOff, Phone } from 'lucide-react';
import type { PublicBrand } from '@/lib/brand';
import { LinkButton } from './LinkButton';
import { StateBanner } from './StateBanner';

/**
 * "Online randevu kapalı" deseni (Durumlar.png): `bookingEnabled=false` (salt okunur abonelik veya kapalı).
 * Telefonu olan her şube için arama bağlantısı; tek şubede "Salonu ara".
 */
export function BookingDisabled({ brand }: { brand: PublicBrand }) {
  const callable = brand.branches.filter((branch) => branch.phone);
  return (
    <StateBanner
      tone="uyari"
      icon={<CalendarOff size={20} />}
      title={`${brand.name} şu an online randevu almıyor.`}
      description="Randevu için salonu arayabilirsiniz."
    >
      {callable.map((branch) => (
        <LinkButton key={branch.id} href={`tel:${branch.phone ?? ''}`} icon={<Phone size={18} />}>
          {callable.length === 1 ? 'Salonu ara' : `${branch.name} şubesini ara`}
        </LinkButton>
      ))}
    </StateBanner>
  );
}
