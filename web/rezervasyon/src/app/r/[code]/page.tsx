import type { Metadata } from 'next';
import { notFound } from 'next/navigation';
import { actionParam, parseAction } from '@/appointment/action';
import { isAccessCode } from '@/appointment/appointment-api';
import { AppointmentManager } from '@/appointment/AppointmentManager';
import { privateRobots } from '@/lib/metadata';

interface AppointmentPageProps {
  params: Promise<{ code: string }>;
  searchParams?: Promise<Record<string, string | string[] | undefined>>;
}

export const metadata: Metadata = { title: 'Randevunuz', robots: privateRobots };

/**
 * SMS bağlantısı (omvila.com/r/{kod}). İstemcide çizilir (web/rezervasyon/AGENTS.md § Rotalar): randevu ve ETag
 * tarayıcıdan okunur. Biçimi bozuk kod API'ye gitmeden 404; bilinmeyen kod istemcide "Bu randevu bulunamadı".
 */
export default async function AppointmentPage({ params, searchParams }: AppointmentPageProps) {
  const { code } = await params;
  if (!isAccessCode(code)) notFound();
  const value = (await searchParams)?.[actionParam];
  return (
    <AppointmentManager
      code={code}
      initialAction={parseAction(typeof value === 'string' ? value : null)}
    />
  );
}
