import type { StoryDefault } from '@ladle/react';
import { Phone, Search } from 'lucide-react';
import { Checkbox } from './Checkbox';
import { previewStack } from './preview';
import { Select } from './Select';
import { Switch } from './Switch';
import { TextField } from './TextField';

export default { title: 'Panel/Form' } satisfies StoryDefault;

export const Alanlar = () => (
  <div style={previewStack}>
    <TextField label="Ad soyad" placeholder="Ayşe Yılmaz" />
    <TextField
      label="Telefon"
      icon={<Phone size={16} />}
      placeholder="0532 123 45 67"
      hint="SMS kodu bu numaraya gelir."
    />
    <TextField label="Telefon" defaultValue="0532 12" error="Telefon numarası 10 haneli olmalı." />
    <TextField label="Fiyat" suffix="₺" placeholder="450" />
    <TextField
      label="Ara"
      icon={<Search size={16} />}
      placeholder="Müşteri, telefon veya randevu"
    />
    <Select
      label="İşletme türü"
      options={['Kuaför', 'Berber', 'Güzellik salonu', 'Spa', 'Tırnak', 'Diyetisyen']}
    />
  </div>
);

export const Secimler = () => (
  <div style={previewStack}>
    <Checkbox label="Bu bilgisayarda 30 gün hatırla" defaultChecked />
    <Checkbox label="Kullanım koşullarını okudum" description="Aydınlatma metni dahil." />
    <Checkbox label="Devre dışı" disabled />
    <Switch label="Online randevu açık" defaultChecked />
    <Switch label="Müşteriye SMS gönder" description="Randevu oluşturulunca onay mesajı gider." />
  </div>
);
