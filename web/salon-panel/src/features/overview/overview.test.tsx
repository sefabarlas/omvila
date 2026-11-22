import { cleanup, screen, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it, vi } from 'vitest';
import { sessionFixture } from '../../test/fake-api';
import { renderApp } from '../../test/render-app';

// Kural: dilim-01 kabul kriteri 1 (kurulumda 1/5), § Alan kuralları (adımlar), salon-paneli/KayitKurulum ve GenelBakisBos
// (selamlama, bloklar 0/—, bağlantı paylaşma), urun-kurallari.md (sahte sayı yok).
afterEach(() => {
  cleanup();
  localStorage.clear();
});

describe('kurulum', () => {
  it('1/5 tamamlandı; yapılan adım üstü çizili, diğerleri ekranlarına bağlı', async () => {
    renderApp('/');
    expect(await screen.findByText('1 / 5 tamamlandı')).toBeTruthy();
    expect(screen.getByRole('heading', { name: 'İlk randevunuza 4 adım' })).toBeTruthy();
    const steps = within(
      screen.getByRole('region', { name: 'İlk randevunuza 4 adım' }),
    ).getAllByRole('listitem');
    expect(steps).toHaveLength(5);
    const [done, hours] = steps;
    if (!done || !hours) {
      throw new Error('adımlar eksik');
    }
    expect(within(done).queryByRole('link')).toBeNull();
    expect(
      within(hours)
        .getByRole('link', { name: /Çalışma saatlerinizi girin/ })
        .getAttribute('href'),
    ).toBe('/ayarlar/sube');
    expect(await screen.findByText('omvila.com/kuafor-elif')).toBeTruthy();
    expect(screen.getByText('Merkez şubesi · 2 dk')).toBeTruthy();
  });

  it('"Sonra yaparım" ilk hafta görünümüne geçer ve tarayıcıda hatırlanır', async () => {
    renderApp('/');
    await userEvent.click(await screen.findByRole('button', { name: 'Sonra yaparım' }));
    expect(await screen.findByRole('heading', { level: 1, name: 'İyi günler, Elif' })).toBeTruthy();
    cleanup();
    renderApp('/');
    expect(await screen.findByRole('heading', { level: 1, name: 'İyi günler, Elif' })).toBeTruthy();
  });
});

describe('ilk hafta', () => {
  it('selamlama, tarih ve şube; bloklar 0 ve —; bağlantı kopyalanınca olay bildirilir', async () => {
    const calls: Record<string, unknown[]> = {};
    const writeText = vi.fn(() => Promise.resolve());
    Object.defineProperty(navigator, 'clipboard', { value: { writeText }, configurable: true });
    renderApp('/', {
      session: sessionFixture({ brand: { ...sessionFixture().brand, status: 'Active' } }),
      doneSteps: ['BusinessCreated', 'WorkingHours', 'Services', 'Staff', 'ShareBookingLink'],
      // Bugün randevu ve onay bekleyen talep yok: GenelBakisBos (dolu hâl approvals/approval.test.tsx).
      calendar: { empty: true },
      calls,
    });

    expect(await screen.findByRole('heading', { level: 1, name: 'İyi günler, Elif' })).toBeTruthy();
    expect(screen.getByText('Çarşamba, 25 Kasım · Merkez')).toBeTruthy();
    // Takvim ve kuyruk yüklenince boş gün: bağlantı kartı ve ilk hafta blokları.
    expect(await screen.findByText('Kurulum tamam')).toBeTruthy();
    const summary = screen.getByRole('group', { name: 'Özet' });
    expect(within(summary).getByText('talep yok')).toBeTruthy();
    expect(within(summary).getByText('—')).toBeTruthy();

    await userEvent.click(screen.getByRole('button', { name: 'Kopyala' }));
    expect(writeText).toHaveBeenCalledWith('https://omvila.com/kuafor-elif');
    expect(await screen.findByText('Bağlantı kopyalandı')).toBeTruthy();
    expect(calls['/api/v1/manage/onboarding/events']?.[0]).toEqual({ event: 'BookingLinkCopied' });
  });

  it('ayar izni olmayan şube müdürü kopyalayınca olay bildirmez', async () => {
    const calls: Record<string, unknown[]> = {};
    Object.defineProperty(navigator, 'clipboard', {
      value: { writeText: () => Promise.resolve() },
      configurable: true,
    });
    renderApp('/', {
      session: sessionFixture({
        permissions: ['report.view', 'calendar.view', 'customer.view'],
        membership: { role: 'BranchManager', branchIds: [], staffId: null },
        brand: { ...sessionFixture().brand, status: 'Active' },
      }),
      calendar: { empty: true },
      calls,
    });
    await userEvent.click(await screen.findByRole('button', { name: 'Kopyala' }));
    await screen.findByText('Bağlantı kopyalandı');
    expect(calls['/api/v1/manage/onboarding/events']).toBeUndefined();
  });
});

// Kural: dilim-06 KK6, salon-paneli/GenelBakis ("BU HAFTA DOLULUK %74 · geçen hafta %68", "Bu hafta doluluk · personel
// bazında"); manageGetOverview (T-120). "Hedefin %72'si" sözleşmede yok (T-116).
describe('bu hafta doluluk', () => {
  const active = sessionFixture({ brand: { ...sessionFixture().brand, status: 'Active' } });

  it('bento ciro ve doluluk; personel satırları doluluğa göre', async () => {
    renderApp('/', { session: active });
    const summary = await screen.findByRole('group', { name: 'Özet' });
    expect(await within(summary).findByText('%74')).toBeTruthy();
    expect(within(summary).getByText('geçen hafta %68')).toBeTruthy();
    expect(within(summary).getByText('3.340 ₺')).toBeTruthy();
    expect(within(summary).queryByText(/hedefin/)).toBeNull();

    const card = screen.getByRole('region', { name: 'Bu hafta doluluk' });
    const meters = within(card).getAllByRole('meter');
    expect(meters.map((m) => m.getAttribute('aria-label'))).toEqual([
      'Elif Kaya doluluk',
      'Mert Demir doluluk',
      'Can Yücel doluluk',
      'Zeynep Aksoy doluluk',
    ]);
    expect(meters[0]?.getAttribute('aria-valuenow')).toBe('86');
    expect(within(card).getByText('%61')).toBeTruthy();
  });

  it('çalışma dakikası yoksa "—" ve açıklama', async () => {
    renderApp('/', { session: active, reports: { noWorkingMinutes: true } });
    const card = await screen.findByRole('region', { name: 'Bu hafta doluluk' });
    expect(
      await within(card).findByText('Bu hafta çalışma saati tanımlı personel yok.'),
    ).toBeTruthy();
    const summary = screen.getByRole('group', { name: 'Özet' });
    expect(within(summary).getByText('çalışma saati yok')).toBeTruthy();
  });
});
