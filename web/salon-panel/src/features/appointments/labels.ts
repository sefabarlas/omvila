import type { components } from '@omvila/api-client-ts';
import { formatDate } from '@omvila/ui-web/format';

type Channel = components['schemas']['AppointmentChannel'];
export type PanelChannel = Exclude<Channel, 'Online'>;

/** Kanal etiketi (yan panel başlığı, "Nasıl geldi?"). */
export const channelLabels: Record<Channel, string> = {
  Online: 'Online',
  Panel: 'Panelden',
  Phone: 'Telefonla',
  WalkIn: 'Kapıdan',
};

/** "Nasıl geldi?" seçenekleri (ManageAppointmentCreate.channel); tasarımdaki varsayılan Telefonla. */
export const channelOptions: readonly { value: PanelChannel; label: string }[] = [
  { value: 'Phone', label: channelLabels.Phone },
  { value: 'Panel', label: channelLabels.Panel },
  { value: 'WalkIn', label: channelLabels.WalkIn },
];

/** Tekrar: 0 = tekrarlanmasın, diğerleri RecurrenceRequest.intervalWeeks. */
export type RepeatWeeks = 0 | 1 | 2 | 4;

export const repeatOptions: readonly { value: RepeatWeeks; label: string }[] = [
  { value: 0, label: 'Tekrarlanmasın' },
  { value: 1, label: 'Her hafta' },
  { value: 2, label: '2 haftada bir' },
  { value: 4, label: '4 haftada bir' },
];

export function parseRepeat(value: string): RepeatWeeks {
  return value === '1' || value === '2' || value === '4' ? (Number(value) as RepeatWeeks) : 0;
}

/** Atlanan tekrarlar: "3 tarih dolu olduğu için atlandı: 23 Aralık, 20 Ocak, 17 Şubat". */
export function skippedMessage(dates: readonly string[]): string {
  return `${String(dates.length)} tarih dolu olduğu için atlandı: ${dates
    .map((date) => formatDate(date, 'dayMonth'))
    .join(', ')}.`;
}
