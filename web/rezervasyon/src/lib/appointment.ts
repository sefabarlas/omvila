import type { components } from '@omvila/api-client-ts';
import { cache } from 'react';
import { createApi } from './api';

export type PublicAppointment = components['schemas']['PublicAppointment'];

// Sözleşmedeki accessCode deseni; uymayan kod API'ye gitmeden "bulunamadı" sayılır.
const accessCodePattern = /^[A-Za-z0-9]{8,10}$/;

/** SMS bağlantısındaki randevu; bilinmeyen, süresi dolmuş veya bozuk kod `undefined` (T-52: ayrımsız 404). */
export const loadAppointment = cache(
  async (code: string): Promise<PublicAppointment | undefined> => {
    if (!accessCodePattern.test(code)) return undefined;
    const { data, response } = await createApi().GET('/api/v1/public/appointments/{accessCode}', {
      params: { path: { accessCode: code } },
      cache: 'no-store',
    });
    if (response.status === 404) return undefined;
    if (!response.ok || !data)
      throw new Error(`publicGetAppointmentByCode ${String(response.status)}`);
    return data;
  },
);
