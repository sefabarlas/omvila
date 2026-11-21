import type { Metadata } from 'next';
import { AppointmentMissing } from '@/appointment/AppointmentMissing';
import { privateRobots } from '@/lib/metadata';

export const metadata: Metadata = { title: 'Bu randevu bulunamadı', robots: privateRobots };

/** Biçimi bozuk kod (sunucuda 404). Bilinmeyen kodu istemci aynı bileşenle gösterir (AppointmentManager). */
export default function AppointmentNotFound() {
  return <AppointmentMissing />;
}
