import type { Metadata } from 'next';
import { notFound } from 'next/navigation';
import { loadAppointment } from '@/lib/appointment';
import { privateRobots } from '@/lib/metadata';
import styles from './page.module.css';

interface AppointmentPageProps {
  params: Promise<{ code: string }>;
}

// 404 belgesi istemcide çizilir ve sayfanın metadatasını kullanır; başlık not-found.tsx ile aynı olmalı.
export async function generateMetadata({ params }: AppointmentPageProps): Promise<Metadata> {
  const appointment = await loadAppointment((await params).code);
  return {
    title: appointment ? appointment.brand.name : 'Bu randevu bulunamadı',
    robots: privateRobots,
  };
}

/**
 * SMS bağlantısı (omvila.com/r/{kod}). Geçici iskelet: yalnız kodu doğrular. Randevum, saat değiştir ve
 * iptal ekranları sonraki PR'da gelir.
 */
export default async function AppointmentPage({ params }: AppointmentPageProps) {
  const appointment = await loadAppointment((await params).code);
  if (!appointment) notFound();

  return (
    <main className={styles.page}>
      <h1 className={styles.title}>{appointment.brand.name}</h1>
    </main>
  );
}
