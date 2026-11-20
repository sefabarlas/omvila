// İşletme türleri (IndustryCode); kayıt ve işletme bilgileri ekranı.
export const industries = [
  { value: 'hair_salon', label: 'Kuaför' },
  { value: 'barber', label: 'Berber' },
  { value: 'beauty', label: 'Güzellik salonu' },
  { value: 'nail', label: 'Tırnak' },
  { value: 'spa', label: 'Spa ve masaj' },
  { value: 'dietitian', label: 'Diyetisyen' },
  { value: 'other', label: 'Diğer' },
] as const;

export type IndustryCode = (typeof industries)[number]['value'];
