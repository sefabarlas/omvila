import type { StoryDefault } from '@ladle/react';
import { Bell, Plus, Trash2 } from 'lucide-react';
import { Button } from './Button';
import { IconButton } from './IconButton';
import { previewRow } from './preview';

export default { title: 'Panel/Button' } satisfies StoryDefault;

export const Varyantlar = () => (
  <div style={previewRow}>
    <Button variant="primary" icon={<Plus size={16} />}>
      Randevu ekle
    </Button>
    <Button>Düzenle</Button>
    <Button variant="ghost">Vazgeç</Button>
    <Button variant="danger" icon={<Trash2 size={16} />}>
      Randevuyu iptal et
    </Button>
    <Button disabled>Kaydet</Button>
    <Button variant="primary" loading loadingLabel="Kaydediliyor…">
      Kaydet
    </Button>
  </div>
);

export const Boyutlar = () => (
  <div style={previewRow}>
    <Button size="sm">Geldi</Button>
    <Button>Kaydet</Button>
    <Button size="lg" variant="primary">
      Değişiklikleri kaydet
    </Button>
    <IconButton label="Bildirimler" icon={<Bell size={18} />} />
    <IconButton label="Bildirimler" size="sm" icon={<Bell size={16} />} />
  </div>
);
