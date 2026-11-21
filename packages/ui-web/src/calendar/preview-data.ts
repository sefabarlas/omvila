// Önizleme ve test verisi: salon-paneli/Main.png (gün) ve HaftaTakvim.png (hafta). Kurgusal kişiler.
import type { StatusKey } from '../status';
import { localDateTimeToUtc } from '../time';
import type { CalendarBlock, CalendarStaff } from './types';

export const previewTimeZone = 'Europe/Istanbul';
export const previewDate = '2025-11-25';
/** Salı, 25 Kasım 2025, 12:10 İstanbul. Yıl, tasarımdaki gün adlarıyla (Salı 25, Pzt 24) uyuşsun diye 2025. */
export const previewNow = new Date('2025-11-25T09:10:00Z');

interface Row {
  staffId: string;
  date?: string;
  start: string;
  end: string;
  customer: string;
  service: string;
  status: StatusKey;
  recurring?: boolean;
  gap?: [string, string];
}

let sequence = 0;

function toBlock(row: Row): CalendarBlock {
  sequence += 1;
  const date = row.date ?? previewDate;
  const utc = (time: string) => localDateTimeToUtc(date, time, previewTimeZone);
  const block: CalendarBlock = {
    appointmentId: `randevu-${String(sequence)}`,
    itemId: `kalem-${String(sequence)}`,
    staffId: row.staffId,
    startUtc: utc(row.start),
    endUtc: utc(row.end),
    status: row.status,
    customerName: row.customer,
    serviceName: row.service,
    isRecurring: row.recurring ?? false,
  };
  if (row.gap) {
    block.segments = [
      { startUtc: utc(row.start), endUtc: utc(row.gap[0]), staffBusy: true },
      { startUtc: utc(row.gap[0]), endUtc: utc(row.gap[1]), staffBusy: false },
      { startUtc: utc(row.gap[1]), endUtc: utc(row.end), staffBusy: true },
    ];
  }

  return block;
}

const dayRows: Row[] = [
  {
    staffId: 'elif',
    start: '09:30',
    end: '10:15',
    customer: 'Ayşe Yılmaz',
    service: 'Saç kesimi',
    status: 'tamamlandi',
  },
  {
    staffId: 'elif',
    start: '10:30',
    end: '12:30',
    customer: 'Selin Öztürk',
    service: 'Boya ve fön',
    status: 'geldi',
    gap: ['10:45', '11:30'],
  },
  {
    staffId: 'elif',
    start: '13:00',
    end: '13:45',
    customer: 'Derya Şahin',
    service: 'Saç kesimi',
    status: 'onaylandi',
    recurring: true,
  },
  {
    staffId: 'elif',
    start: '15:30',
    end: '16:30',
    customer: 'Buse Kaya',
    service: 'Keratin',
    status: 'onaylandi',
  },
  {
    staffId: 'elif',
    start: '17:00',
    end: '17:45',
    customer: 'Nur Ateş',
    service: 'Fön',
    status: 'onaylandi',
  },
  {
    staffId: 'mert',
    start: '09:00',
    end: '10:00',
    customer: 'Can Demir',
    service: 'Sakal ve kesim',
    status: 'tamamlandi',
  },
  {
    staffId: 'mert',
    start: '11:00',
    end: '11:30',
    customer: 'Okan Er',
    service: 'Saç kesimi',
    status: 'geldi',
  },
  {
    staffId: 'mert',
    start: '14:30',
    end: '15:00',
    customer: 'Gizem Arslan',
    service: 'Fön',
    status: 'kapora',
  },
  {
    staffId: 'mert',
    start: '16:00',
    end: '16:45',
    customer: 'Kerem Aydın',
    service: 'Saç kesimi',
    status: 'onaylandi',
  },
  {
    staffId: 'zeynep',
    start: '10:00',
    end: '11:30',
    customer: 'Melis Tunç',
    service: 'Manikür ve pedikür',
    status: 'tamamlandi',
  },
  {
    staffId: 'zeynep',
    start: '12:30',
    end: '13:30',
    customer: 'Ece Koç',
    service: 'Kaş ve kirpik',
    status: 'gelmedi',
  },
  {
    staffId: 'zeynep',
    start: '16:00',
    end: '17:00',
    customer: 'Pınar Yalçın',
    service: 'Cilt bakımı',
    status: 'onay',
  },
  {
    staffId: 'can',
    start: '09:30',
    end: '11:00',
    customer: 'Hande Er',
    service: 'Dip boya',
    status: 'tamamlandi',
  },
  {
    staffId: 'can',
    start: '12:00',
    end: '12:45',
    customer: 'Deniz Acar',
    service: 'Saç kesimi',
    status: 'onay',
  },
  {
    staffId: 'can',
    start: '14:00',
    end: '15:30',
    customer: 'Aslı Kurt',
    service: 'Balyaj',
    status: 'onaylandi',
  },
  {
    staffId: 'can',
    start: '17:30',
    end: '18:00',
    customer: 'Tuna Oral',
    service: 'Saç kesimi',
    status: 'iptal',
  },
];

export const previewBlocks: CalendarBlock[] = dayRows.map(toBlock);

const staffBase = [
  { id: 'elif', name: 'Elif Kaya', color: '#14857A' },
  { id: 'mert', name: 'Mert Demir', color: '#D85E40' },
  { id: 'zeynep', name: 'Zeynep Aksoy', color: '#9C8566' },
  { id: 'can', name: 'Can Yücel', color: '#2F6BB8' },
];

/** Alt satır veriden: iptaller hariç randevu sayısı, bekleyen varsa ek. */
export const previewStaff: CalendarStaff[] = staffBase.map((member) => {
  const own = previewBlocks.filter((b) => b.staffId === member.id && b.status !== 'iptal');
  const pending = own.some((b) => b.status === 'onay' || b.status === 'kapora');
  return {
    ...member,
    subtitle: `${String(own.length)} randevu${pending ? ' · bekleyen var' : ''}`,
  };
});

export const previewWeekDays = [
  '2025-11-24',
  '2025-11-25',
  '2025-11-26',
  '2025-11-27',
  '2025-11-28',
  '2025-11-29',
];

const weekRows: Row[] = [
  {
    staffId: 'elif',
    date: '2025-11-25',
    start: '09:30',
    end: '10:15',
    customer: 'Ayşe Yılmaz',
    service: 'Saç kesimi',
    status: 'tamamlandi',
  },
  {
    staffId: 'elif',
    date: '2025-11-25',
    start: '10:30',
    end: '12:00',
    customer: 'Selin Öztürk',
    service: 'Boya ve fön',
    status: 'geldi',
  },
  {
    staffId: 'elif',
    date: '2025-11-25',
    start: '13:00',
    end: '13:45',
    customer: 'Derya Şahin',
    service: 'Saç kesimi',
    status: 'onaylandi',
  },
  {
    staffId: 'elif',
    date: '2025-11-25',
    start: '15:30',
    end: '16:30',
    customer: 'Buse Kaya',
    service: 'Keratin',
    status: 'onaylandi',
  },
  {
    staffId: 'elif',
    date: '2025-11-26',
    start: '10:00',
    end: '10:45',
    customer: 'Kerem Aydın',
    service: 'Saç kesimi',
    status: 'onaylandi',
  },
  {
    staffId: 'elif',
    date: '2025-11-26',
    start: '14:00',
    end: '14:45',
    customer: 'Derya Şahin',
    service: 'Saç kesimi',
    status: 'onaylandi',
  },
  {
    staffId: 'elif',
    date: '2025-11-26',
    start: '16:00',
    end: '17:30',
    customer: 'Hande Er',
    service: 'Dip boya',
    status: 'kapora',
  },
  {
    staffId: 'elif',
    date: '2025-11-27',
    start: '11:00',
    end: '11:30',
    customer: 'Nur Ateş',
    service: 'Fön',
    status: 'onay',
  },
  {
    staffId: 'elif',
    date: '2025-11-27',
    start: '13:00',
    end: '14:30',
    customer: 'Aslı Kurt',
    service: 'Balyaj',
    status: 'onaylandi',
  },
  {
    staffId: 'elif',
    date: '2025-11-28',
    start: '10:00',
    end: '11:00',
    customer: 'Melis Tunç',
    service: 'Manikür',
    status: 'onaylandi',
  },
  {
    staffId: 'elif',
    date: '2025-11-28',
    start: '15:00',
    end: '16:00',
    customer: 'Tuğba Er',
    service: 'Keratin',
    status: 'onaylandi',
  },
  {
    staffId: 'elif',
    date: '2025-11-29',
    start: '09:00',
    end: '10:00',
    customer: 'Ece Koç',
    service: 'Kaş ve kirpik',
    status: 'onaylandi',
  },
  {
    staffId: 'elif',
    date: '2025-11-29',
    start: '11:00',
    end: '12:30',
    customer: 'Buse Kaya',
    service: 'Boya',
    status: 'onaylandi',
  },
  {
    staffId: 'elif',
    date: '2025-11-29',
    start: '14:00',
    end: '14:45',
    customer: 'Kaan Polat',
    service: 'Saç kesimi',
    status: 'onaylandi',
  },
];

export const previewWeekBlocks: CalendarBlock[] = weekRows.map(toBlock);
