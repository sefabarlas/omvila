import type { StoryDefault } from '@ladle/react';
import { Bell } from 'lucide-react';
import { useState } from 'react';
import { Avatar } from './Avatar';
import { IconButton } from './IconButton';
import { previewRow, previewStack } from './preview';
import { Tabs } from './Tabs';
import { Tooltip } from './Tooltip';

export default { title: 'Panel/Gezinme' } satisfies StoryDefault;

export const Sekmeler = () => {
  const [value, setValue] = useState('gelecek');
  return (
    <div style={previewStack}>
      <Tabs
        label="Randevular"
        value={value}
        onChange={setValue}
        items={[
          { value: 'gelecek', label: 'Gelecek', count: 2 },
          { value: 'gecmis', label: 'Geçmiş', count: 14 },
          { value: 'iptal', label: 'İptal' },
          { value: 'notlar', label: 'Notlar' },
        ]}
      />
    </div>
  );
};

export const Avatarlar = () => (
  <div style={previewRow}>
    <Avatar name="Elif Kaya" size="sm" />
    <Avatar name="Mert Demir" />
    <Avatar name="İlker Şahin" size="lg" />
    <Avatar name="Selin Öztürk" />
  </div>
);

export const Ipucu = () => (
  <div style={{ ...previewRow, paddingTop: 'var(--bosluk-12)' }}>
    <Tooltip content="Onay bekleyen 3 randevu">
      <IconButton label="Bildirimler" icon={<Bell size={18} />} />
    </Tooltip>
  </div>
);
