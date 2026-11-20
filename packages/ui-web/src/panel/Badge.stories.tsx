import type { StoryDefault } from '@ladle/react';
import { Check } from 'lucide-react';
import { statusKeys } from '../status';
import { Badge } from './Badge';
import type { BadgeTone } from './Badge';
import { previewRow } from './preview';
import { StatusBadge } from './StatusBadge';

export default { title: 'Panel/Badge' } satisfies StoryDefault;

const tones: [BadgeTone, string][] = [
  ['notr', 'Pasif'],
  ['marka', 'Paket'],
  ['vurgu', 'Yeni'],
  ['basari', 'Ödendi'],
  ['uyari', 'Az kaldı'],
  ['hata', 'Borçlu'],
  ['bilgi', 'VIP'],
];

export const Tonlar = () => (
  <div style={previewRow}>
    {tones.map(([tone, label]) => (
      <Badge
        key={tone}
        tone={tone}
        icon={tone === 'basari' ? <Check size={12} strokeWidth={3} /> : undefined}
      >
        {label}
      </Badge>
    ))}
  </div>
);

export const Durumlar = () => (
  <div style={previewRow}>
    {statusKeys.map((status) => (
      <StatusBadge key={status} status={status} />
    ))}
  </div>
);
