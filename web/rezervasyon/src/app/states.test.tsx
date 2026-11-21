import { render, screen } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import { json, mockApi, problem } from '@/test/api-mock';
import ErrorPage from './error';
import NotFound from './not-found';
import HomePage from './page';
import AppointmentNotFound from './r/[code]/not-found';
import AppointmentPage from './r/[code]/page';

vi.mock('next/navigation', () => ({
  notFound: () => {
    throw new Error('notFound');
  },
}));

describe('durum sayfaları', () => {
  it('kök adres salon keşfi yerine bağlantı ister', () => {
    render(<HomePage />);
    expect(
      screen.getByRole('heading', { level: 1, name: 'Salonun bağlantısını açın' }),
    ).toBeTruthy();
    expect(
      screen.getByText(
        'Randevu almak için salonunuzun paylaştığı bağlantıyı veya QR kodu kullanın.',
      ),
    ).toBeTruthy();
    expect(screen.queryByRole('searchbox')).toBeNull();
    expect(screen.queryByRole('link')).toBeNull();
  });

  it('geçersiz bağlantı', () => {
    render(<NotFound />);
    expect(screen.getByRole('heading', { level: 1, name: 'Bu bağlantı geçersiz' })).toBeTruthy();
  });

  it('bulunamayan randevu tasarımdaki metni gösterir', () => {
    render(<AppointmentNotFound />);
    expect(screen.getByRole('heading', { level: 1, name: 'Bu randevu bulunamadı' })).toBeTruthy();
    expect(
      screen.getByText('Bağlantının süresi dolmuş ya da randevu iptal edilmiş olabilir.'),
    ).toBeTruthy();
  });

  it('hata sayfası tekrar denemeyi sunar', () => {
    const reset = vi.fn();
    render(<ErrorPage error={new Error('x')} reset={reset} />);
    expect(screen.getByRole('heading', { level: 1, name: 'Sayfa şu an açılamadı' })).toBeTruthy();
    screen.getByRole('button', { name: 'Tekrar dene' }).click();
    expect(reset).toHaveBeenCalledOnce();
  });
});

describe('SMS bağlantısı (/r/[code])', () => {
  const props = (code: string) => ({ params: Promise.resolve({ code }) });

  it('404 bulunamadı sayfasına çıkar', async () => {
    mockApi(() => problem(404, 'not_found'));
    await expect(AppointmentPage(props('7Kx2Pq9a'))).rejects.toThrow('notFound');
  });

  it('biçimi bozuk kodu API’ye sormaz', async () => {
    const calls = mockApi(() => problem(404, 'not_found'));
    await expect(AppointmentPage(props('kisa'))).rejects.toThrow('notFound');
    expect(calls).toHaveLength(0);
  });

  it('geçerli kodda salon adını gösterir', async () => {
    mockApi(() => json(200, { brand: { name: 'Kuaför Elif', slug: 'kuafor-elif' } }));
    render(await AppointmentPage(props('7Kx2Pq9a')));
    expect(screen.getByRole('heading', { level: 1, name: 'Kuaför Elif' })).toBeTruthy();
  });
});
