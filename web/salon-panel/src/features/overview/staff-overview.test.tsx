import { cleanup, screen, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it } from 'vitest';
import { sessionFixture, staffPermissions } from '../../test/fake-api';
import { renderApp } from '../../test/render-app';

// Kural: dilim-01 kabul kriteri 10 (personel girişi sonrası kısıtlı menü), roller-yetkiler.md (report.view yoksa
// personel özeti; ayarlar, hizmetler, personel kapalı), salon-paneli/PersonelRolu (sahte sayı yok).
afterEach(cleanup);

const deniz = sessionFixture({
  subscription: { status: 'Trialing', trialEndsOn: '2026-11-27' },
  user: {
    id: '0193a8f0-0000-7000-8000-000000000002',
    fullName: 'Deniz Kara',
    phoneMasked: '0538 *** ** 05',
  },
  permissions: staffPermissions,
  membership: { role: 'Staff', branchIds: [], staffId: '0193a8f0-0000-7000-8000-0000000000c9' },
  brand: { ...sessionFixture().brand, status: 'Active' },
});

describe('personel görünümü', () => {
  it('"Merhaba Deniz": kendi günü 0/—, rolün kapsamı ve kapalı alanlar', async () => {
    renderApp('/', { session: deniz });
    expect(await screen.findByRole('heading', { level: 1, name: 'Merhaba Deniz' })).toBeTruthy();
    expect(screen.getByText('Çarşamba, 25 Kasım · Merkez · personel görünümü')).toBeTruthy();
    const summary = screen.getByRole('group', { name: 'Özet' });
    expect(within(summary).getByText('Bu ay primim')).toBeTruthy();
    expect(within(summary).getAllByText('—').length).toBeGreaterThan(0);
    expect(screen.getByRole('heading', { name: 'Bugün randevunuz yok' })).toBeTruthy();
    const scope = screen.getByRole('region', { name: 'Bu rolde' });
    expect(within(scope).getByText('Kendi takvimini ve randevularını yönetir')).toBeTruthy();
    expect(within(scope).getByText('Kapıda sıraya müşteri alır')).toBeTruthy();
    expect(within(scope).getByText('Kendi primini ve puanını görür')).toBeTruthy();
    expect(within(scope).getByText('Kasa, raporlar, hizmet ve ayarlar kapalı')).toBeTruthy();
    expect(
      screen.getByText('Onay bekleyen talepleri yalnızca yönetici ve işletme sahibi onaylar.'),
    ).toBeTruthy();
    // Kurulum ekranı, bağlantı paylaşma ve deneme şeridi personelde yok.
    expect(screen.queryByText(/Deneme süren/)).toBeNull();
    expect(screen.queryByText(/İlk randevunuza/)).toBeNull();
    expect(screen.queryByRole('button', { name: 'Kopyala' })).toBeNull();
  });

  it('menü yalnız Takvimim; ayarlar, hizmetler ve personel adresleri 403', async () => {
    const { router } = renderApp('/', { session: deniz });
    const nav = await screen.findByRole('navigation', { name: 'Ana menü' });
    expect(
      within(nav)
        .getAllByRole('link')
        .map((a) => a.textContent),
    ).toEqual(['Takvimim']);
    expect(within(nav).getByRole('link', { name: 'Takvimim' }).getAttribute('aria-current')).toBe(
      'page',
    );
    for (const path of ['/ayarlar/isletme', '/hizmetler', '/personel'] as const) {
      await router.navigate({ to: path });
      expect(
        await screen.findByRole('heading', { level: 1, name: 'Bu sayfayı görme yetkiniz yok' }),
      ).toBeTruthy();
    }

    await userEvent.click(within(nav).getByRole('link', { name: 'Takvimim' }));
    expect(await screen.findByRole('heading', { level: 1, name: 'Merhaba Deniz' })).toBeTruthy();
  });
});
