import { cleanup, screen, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it } from 'vitest';
import { overviewFixture } from '@/test/fake-api';
import { renderApp } from '@/test/render-app';

// Kural: platform-paneli/Main (KK4): dört özet bloğu, aktivasyon hunisi, dikkat isteyen markalar; boş ve hata hâlleri.
afterEach(cleanup);

describe('genel bakış', () => {
  it('özet blokları, huni ve dikkat isteyen markalar', async () => {
    renderApp('/');
    expect(await screen.findByText('Bugün · 20 Kasım')).toBeTruthy();
    const summary = await screen.findByRole('group', { name: 'Platform özeti' });
    expect(within(summary).getByText('412')).toBeTruthy();
    expect(within(summary).getByText('+18 bu ay')).toBeTruthy();
    expect(within(summary).getByText('5 havale bildirimi · 2 SMS başlığı')).toBeTruthy();
    expect(within(summary).getByText("9'u bu hafta bitiyor")).toBeTruthy();
    expect(within(summary).getByText("3'ü salt okunura geçmek üzere")).toBeTruthy();

    const funnel = screen.getByRole('region', { name: 'Aktivasyon hunisi · son 30 gün' });
    expect(within(funnel).getByText('Çalışma saatlerini girdi')).toBeTruthy();
    expect(within(funnel).getByText('71')).toBeTruthy();

    const attention = screen.getByRole('region', { name: 'Dikkat isteyen markalar' });
    expect(
      within(attention).getByRole('link', {
        name: 'Berber Kaan, Ödeme gecikti · 6 gün, 1 şube, bu ay 0 randevu',
      }),
    ).toBeTruthy();
    expect(within(attention).getByText('Kontör bitti')).toBeTruthy();
  });

  it('dikkat isteyen markaya tıklayınca marka detayı', async () => {
    const { router } = renderApp('/');
    const attention = await screen.findByRole('region', { name: 'Dikkat isteyen markalar' });
    await userEvent.click(within(attention).getByRole('link', { name: /^Nail Studio Moda/ }));
    expect(router.state.location.pathname).toBe('/markalar/0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa202');
  });

  it('boş hâller: sahte sayı yok', async () => {
    renderApp('/', {
      overview: {
        ...overviewFixture,
        activeBrands: { count: 0, newThisMonth: 0 },
        trialing: { count: 0, endingThisWeek: 0 },
        pastDue: { count: 0, readOnlySoon: 0 },
        funnel: { ...overviewFixture.funnel, signedUp: 0, addedServices: 0 },
        attentionBrands: [],
      },
    });
    expect(await screen.findByText('Bu ay yeni kayıt yok')).toBeTruthy();
    expect(screen.getByText('Bu dönemde yeni kayıt yok.')).toBeTruthy();
    expect(screen.getByRole('heading', { name: 'Dikkat isteyen marka yok' })).toBeTruthy();
  });

  it('hata: tekrar dene şeridi', async () => {
    renderApp('/', { failures: { '/api/v1/admin/overview': 500 } });
    const alert = await screen.findByRole('alert');
    expect(within(alert).getByText('Bilgiler yüklenemedi.')).toBeTruthy();
    expect(within(alert).getByRole('button', { name: 'Tekrar dene' })).toBeTruthy();
  });

  it('üst bar araması Markalar listesini arar', async () => {
    const { router } = renderApp('/');
    await userEvent.type(
      await screen.findByRole('searchbox', { name: 'Marka ara' }),
      'elif{Enter}',
    );
    expect(await screen.findByRole('heading', { level: 1, name: 'Markalar' })).toBeTruthy();
    expect(router.state.location.search).toEqual({ q: 'elif' });
  });
});
