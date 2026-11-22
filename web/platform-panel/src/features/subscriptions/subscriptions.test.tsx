import { cleanup, screen, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it } from 'vitest';
import { renderApp } from '@/test/render-app';

// Kural: platform-paneli/PAbonelikler: üst sayılar süzgeçten bağımsız; durum rozeti ("Ödeme gecikti · 6. gün",
// "Aktif · kontör bitti"); "Sonraki adım" türden ve günden üretilir ("Yarın salt okunur"); süzgeç çipi filter gönderir.
afterEach(cleanup);

describe('abonelikler', () => {
  it('üst sayılar, rozetler ve sonraki adım', async () => {
    renderApp('/abonelikler');
    const table = await screen.findByRole('table', { name: 'Abonelikler' });
    expect(screen.getByText('Kasım 2026')).toBeTruthy();
    const summary = screen.getByRole('group', { name: 'Abonelik özeti' });
    expect(within(summary).getByText('348')).toBeTruthy();
    expect(within(summary).getByText("9'u bu hafta bitiyor")).toBeTruthy();
    expect(within(table).getByText('Ödeme gecikti · 6. gün')).toBeTruthy();
    expect(within(table).getByText('Yarın salt okunur')).toBeTruthy();
    expect(within(table).getByText('Ödeme gelince açılır')).toBeTruthy();
    expect(within(table).getByText('29 Kas deneme biter')).toBeTruthy();
    expect(within(table).getByText('Aktif · kontör bitti')).toBeTruthy();
    expect(within(table).getByText('SMS gönderilmiyor')).toBeTruthy();
    expect(within(table).getByText('1.240')).toBeTruthy();
    expect(screen.queryByRole('button', { name: 'Dışa aktar' })).toBeNull();
  });

  it('süzgeç çipi filtreyi gönderir; satır marka detayını açar', async () => {
    const calls: Record<string, unknown[]> = {};
    const { router } = renderApp('/abonelikler', { calls });
    await screen.findByRole('table', { name: 'Abonelikler' });
    await userEvent.click(screen.getByRole('radio', { name: 'Kontörü azalan' }));
    const table = await screen.findByRole('table', { name: 'Abonelikler' });
    await within(table).findByText('Nail Studio Moda');
    expect(within(table).queryByText('Berber Kaan')).toBeNull();
    expect(calls['/api/v1/admin/subscriptions']?.at(-1)).toEqual({
      filter: 'CreditsLow',
      limit: '25',
    });
    await userEvent.click(within(table).getByText('Nail Studio Moda'));
    expect(router.state.location.pathname).toBe('/markalar/0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa202');
  });

  it('süzgece uyan yoksa boş hâl', async () => {
    renderApp('/abonelikler?filtre=Cancelled');
    expect(
      await screen.findByRole('heading', { name: 'Bu süzgece uyan abonelik yok' }),
    ).toBeTruthy();
  });

  it('hata olursa yeniden dene', async () => {
    renderApp('/abonelikler', { failures: { '/api/v1/admin/subscriptions': 500 } });
    expect(await screen.findByText('Bilgiler yüklenemedi.')).toBeTruthy();
    expect(screen.getByRole('button', { name: 'Tekrar dene' })).toBeTruthy();
  });
});
