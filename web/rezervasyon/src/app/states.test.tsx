import { render, screen } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import { mockApi, problem } from '@/test/api-mock';
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
  const props = (code: string, islem?: string) => ({
    params: Promise.resolve({ code }),
    searchParams: Promise.resolve(islem ? { islem } : {}),
  });

  it('biçimi bozuk kodu API’ye sormadan 404 verir', async () => {
    const calls = mockApi(() => problem(404, 'not_found'));
    await expect(AppointmentPage(props('kisa'))).rejects.toThrow('notFound');
    expect(calls).toHaveLength(0);
  });

  it('geçerli biçimde istemci bileşenine kodu ve alt ekranı verir (istemcide okunur)', async () => {
    const calls = mockApi(() => problem(404, 'not_found'));
    const element = await AppointmentPage(props('7Kx2Pq9a', 'iptal'));
    expect(element.props).toEqual({ code: '7Kx2Pq9a', initialAction: 'cancel' });
    expect(calls).toHaveLength(0);
  });
});
