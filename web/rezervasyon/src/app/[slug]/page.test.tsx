import { render, screen } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import { brandFixture, json, mockApi, problem } from '@/test/api-mock';
import SalonPage, { generateMetadata } from './page';

vi.mock('next/navigation', () => ({
  notFound: () => {
    throw new Error('notFound');
  },
  permanentRedirect: (url: string) => {
    throw new Error(`permanentRedirect ${url}`);
  },
}));

const props = (slug: string) => ({ params: Promise.resolve({ slug }) });

describe('salon sayfası', () => {
  it('salon kartını gösterir ve isteği X-Client ile yapar', async () => {
    const calls = mockApi(() => json(200, brandFixture()));

    render(await SalonPage(props('kuafor-elif')));

    expect(screen.getByRole('heading', { level: 1, name: 'Kuaför Elif' })).toBeTruthy();
    expect(screen.getByText('Kuaför · Saç, bakım, makyaj')).toBeTruthy();
    expect(screen.getByText('Moda Cad. 42, Kadıköy · İstanbul')).toBeTruthy();
    expect(screen.queryByRole('status')).toBeNull();
    expect(calls[0]?.url).toBe('http://localhost:4010/api/v1/public/brands/kuafor-elif');
    expect(calls[0]?.headers.get('X-Client')).toBe('booking-page');
  });

  it('online randevu kapalıysa durumu ve arama bağlantısını gösterir', async () => {
    mockApi(() => json(200, brandFixture({ bookingEnabled: false })));

    render(await SalonPage(props('kuafor-elif')));

    const status = screen.getByRole('status');
    expect(status.textContent).toContain('Kuaför Elif şu an online randevu almıyor.');
    expect(status.textContent).toContain('Randevu için salonu arayabilirsiniz.');
    const call = screen.getByRole('link', { name: 'Salonu ara' });
    expect(call.getAttribute('href')).toBe('tel:+905321234567');
  });

  it('birden fazla şubede her şube için arama bağlantısı verir', async () => {
    const [branch] = brandFixture().branches;
    if (!branch) throw new Error('şube yok');
    mockApi(() =>
      json(
        200,
        brandFixture({
          bookingEnabled: false,
          branches: [
            branch,
            { ...branch, id: '0190a000-0000-7000-8000-000000000003', name: 'Moda', phone: null },
            { ...branch, id: '0190a000-0000-7000-8000-000000000004', name: 'Bostancı' },
          ],
        }),
      ),
    );

    render(await SalonPage(props('kuafor-elif')));

    expect(screen.getByRole('link', { name: 'Kadıköy şubesini ara' })).toBeTruthy();
    expect(screen.getByRole('link', { name: 'Bostancı şubesini ara' })).toBeTruthy();
    expect(screen.queryByRole('link', { name: 'Moda şubesini ara' })).toBeNull();
    expect(screen.getByText('3 şube')).toBeTruthy();
  });

  it('404 bulunamadı sayfasına çıkar', async () => {
    mockApi(() => problem(404, 'not_found'));
    await expect(SalonPage(props('yok-boyle-salon'))).rejects.toThrow('notFound');
  });

  it('sözleşme desenine uymayan adı API’ye sormadan bulunamadı sayar', async () => {
    const calls = mockApi(() => json(200, brandFixture()));
    await expect(SalonPage(props('Kuafor_Elif'))).rejects.toThrow('notFound');
    expect(calls).toHaveLength(0);
  });

  it('301 yeni bağlantı adına kalıcı yönlendirir', async () => {
    const calls = mockApi(
      () =>
        new Response(null, {
          status: 301,
          headers: { Location: '/api/v1/public/brands/elif-guzellik' },
        }),
    );
    await expect(SalonPage(props('kuafor-elif'))).rejects.toThrow(
      'permanentRedirect /elif-guzellik',
    );
    expect(calls[0]?.redirect).toBe('manual');
  });

  it('yönlendirme izlenip başka ad gelirse de yönlendirir', async () => {
    mockApi(() => json(200, brandFixture({ slug: 'elif-guzellik' })));
    await expect(SalonPage(props('kuafor-elif'))).rejects.toThrow(
      'permanentRedirect /elif-guzellik',
    );
  });

  it('5xx hatayı error.tsx’e bırakır', async () => {
    mockApi(() => problem(503, 'try_again'));
    await expect(SalonPage(props('kuafor-elif'))).rejects.toThrow('publicGetBrand 503');
  });
});

describe('salon sayfası metadatası', () => {
  it('Open Graph etiketlerini salondan üretir', async () => {
    mockApi(() => json(200, brandFixture({ logoUrl: 'https://cdn.example.test/logo.png' })));

    const metadata = await generateMetadata(props('kuafor-elif'));

    expect(metadata.title).toBe('Kuaför Elif · Online randevu');
    expect(metadata.description).toBe('Saç, bakım, makyaj');
    expect(metadata.alternates?.canonical).toBe('http://localhost:3000/kuafor-elif');
    expect(metadata.openGraph).toMatchObject({
      type: 'website',
      locale: 'tr_TR',
      siteName: 'OMVILA',
      title: 'Kuaför Elif · Online randevu',
      url: 'http://localhost:3000/kuafor-elif',
      images: [{ url: 'https://cdn.example.test/logo.png', alt: 'Kuaför Elif' }],
    });
  });

  it('açıklama ve logo yoksa varsayılan metni kullanır, görsel eklemez', async () => {
    mockApi(() => json(200, brandFixture({ shortDescription: null })));

    const metadata = await generateMetadata(props('kuafor-elif'));

    expect(metadata.description).toBe(
      'Kuaför Elif için hizmetinizi ve saatinizi seçin, üye olmadan randevu alın.',
    );
    expect(metadata.openGraph).not.toHaveProperty('images');
    expect(metadata.twitter).not.toHaveProperty('images');
  });

  it('bulunamayan salonda geçersiz bağlantı başlığı ve noindex döner', async () => {
    mockApi(() => problem(404, 'not_found'));
    expect(await generateMetadata(props('yok-boyle-salon'))).toEqual({
      title: 'Bu bağlantı geçersiz',
      robots: { index: false, follow: false },
    });
  });
});
