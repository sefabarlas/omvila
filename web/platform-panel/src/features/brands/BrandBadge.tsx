import { Badge } from '@omvila/ui-web/panel';
import type { BadgeTone } from '@omvila/ui-web/panel';

/** Marka durumu rozeti ("Deneme · 9 gün", "Ödeme gecikti", "Kontör bitti"). */
export function BrandBadge({ label, tone }: { label: string; tone: BadgeTone }) {
  return <Badge tone={tone}>{label}</Badge>;
}
