import { cleanup, screen, waitFor, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it } from 'vitest';
import { sessionFixture, staffPermissions } from '../../test/fake-api';
import type { FakeApiOptions } from '../../test/fake-api';
import { walkInIds } from '../../test/fake-queues';
import { renderApp } from '../../test/render-app';

// Kural: dilim-05 KK4 (numaralar şube ve gün bazında 1'den; "Başlat" → Kapıdan kanallı Geldi randevusu), T-91
// (tahmini bekleme sunucudan, telefonsuz bilet), roller-yetkiler.md (walkin.manage personelde de var).
// Saat: Çarşamba 25 Kasım 2026, 12:00 İstanbul (sıranın generatedUtc'si de bu an).
afterEach(cleanup);

const path = '/takvim/kapida-sira';
const haircut = '0193a8f0-0000-7000-8000-0000000000f1';
const elif = '0193a8f0-0000-7000-8000-0000000000c1';

function openQueue(options: FakeApiOptions = {}) {
  const calls: Record<string, unknown[]> = {};
  const result = renderApp(path, { calls, withServices: true, ...options });
  return { ...result, calls };
}

const rowTexts = (table: HTMLElement) =>
  within(table)
    .getAllByRole('row')
    .slice(1)
    .map((row) => row.textContent);

describe('kapıda sıra', () => {
  it('özet, numara sırasıyla tablo; ilk satır randevuya çevrilir, diğerleri sıradan çıkar', async () => {
    openQueue();
    expect(await screen.findByRole('heading', { level: 1, name: 'Kapıda sıra' })).toBeTruthy();
    const table = await screen.findByRole('table', { name: 'Sıradakiler' });
    expect(screen.getByText('Merkez · bugün · numaralar her gün 1’den başlar')).toBeTruthy();
    const nav = screen.getByRole('navigation', { name: 'Ana menü' });
    expect(within(nav).getByRole('link', { name: 'Takvim' }).getAttribute('aria-current')).toBe(
      'page',
    );

    const kpis = screen.getByRole('group', { name: 'Sıra özeti' });
    expect(within(kpis).getByText('3')).toBeTruthy();
    expect(within(kpis).getByText('en eskisi 12 dk')).toBeTruthy();
    expect(within(kpis).getByText('~35 dk')).toBeTruthy();
    expect(within(kpis).getByText('7')).toBeTruthy();
    expect(within(kpis).getByText('randevuya çevrildi')).toBeTruthy();

    expect(rowTexts(table)).toEqual([
      '8Okan ErSaç kesimiMert12 dkRandevuya çevir',
      '9Murat Çelik—Fark etmez8 dkSıradan çıkar',
      '10Emre YavuzSaç kesimiMert2 dkSıradan çıkar',
    ]);
    expect(within(table).getByText('8').className).toContain('numberFirst');
    expect(within(table).getByText('9').className).not.toContain('numberFirst');
    expect(
      screen.getByText('Randevuya çevrilen kişi takvime “Kapıdan” kanalıyla eklenir.'),
    ).toBeTruthy();
  });

  it('"Randevuya çevir": If-Match ile gider, bilet sıradan düşer', async () => {
    const { calls } = openQueue();
    await userEvent.click(await screen.findByRole('button', { name: 'Randevuya çevir: Okan Er' }));
    expect(await screen.findByText('Randevuya çevrildi')).toBeTruthy();
    expect(screen.getByText('Okan Er takvime “Kapıdan” kanalıyla eklendi.')).toBeTruthy();
    expect(calls[`POST /api/v1/manage/walk-in-tickets/${walkInIds.okan}/start If-Match`]).toEqual([
      'W/"1"',
    ]);
    const table = screen.getByRole('table', { name: 'Sıradakiler' });
    await waitFor(() => {
      expect(rowTexts(table)[0]).toBe('9Murat Çelik—Fark etmez8 dkRandevuya çevir');
    });
  });

  it('personel "Fark etmez" ve hizmetsiz bilet: önce hizmet ve personel seçilir', async () => {
    const { calls } = openQueue();
    await userEvent.click(await screen.findByRole('button', { name: 'Randevuya çevir: Okan Er' }));
    await userEvent.click(
      await screen.findByRole('button', { name: 'Randevuya çevir: Murat Çelik' }),
    );
    const dialog = await screen.findByRole('dialog', { name: 'Randevuya çevir' });
    expect(within(dialog).getByText('No 9 · Murat Çelik · şimdi başlar')).toBeTruthy();
    await userEvent.selectOptions(within(dialog).getByLabelText('Hizmet'), haircut);
    await userEvent.selectOptions(within(dialog).getByLabelText('Personel'), elif);
    await userEvent.click(within(dialog).getByRole('button', { name: 'Randevuya çevir' }));
    await waitFor(() => {
      expect(calls[`/api/v1/manage/walk-in-tickets/${walkInIds.murat}/start`]).toEqual([
        { serviceId: haircut, staffId: elif },
      ]);
    });
    await waitFor(() => {
      expect(screen.queryByRole('dialog', { name: 'Randevuya çevir' })).toBeNull();
    });
  });

  it('"Randevuya çevir" kayıt çakışması: şerit ve Yenile', async () => {
    openQueue({ queues: { startError: 'concurrency_conflict' } });
    await userEvent.click(await screen.findByRole('button', { name: 'Randevuya çevir: Okan Er' }));
    expect(await screen.findByText('Kayıt çakışması')).toBeTruthy();
    expect(screen.getByRole('button', { name: 'Yenile' })).toBeTruthy();
  });

  it('"Sıradan çıkar": If-Match ile gider, satır kalkar', async () => {
    const { calls } = openQueue();
    await userEvent.click(await screen.findByRole('button', { name: 'Sıradan çıkar: Emre Yavuz' }));
    expect(await screen.findByText('Emre Yavuz sıradan çıkarıldı')).toBeTruthy();
    expect(calls[`POST /api/v1/manage/walk-in-tickets/${walkInIds.emre}/leave If-Match`]).toEqual([
      'W/"1"',
    ]);
    await waitFor(() => {
      expect(screen.queryByRole('button', { name: 'Sıradan çıkar: Emre Yavuz' })).toBeNull();
    });
  });

  it('"Sıraya ekle": ad, telefonsuz, hizmet, fark etmez; dönen numara yazar', async () => {
    const { calls } = openQueue();
    await screen.findByRole('table', { name: 'Sıradakiler' });
    await userEvent.click(screen.getByRole('button', { name: 'Sıraya ekle' }));
    const dialog = await screen.findByRole('dialog', { name: 'Sıraya ekle' });
    expect(within(dialog).getByText('Sıradaki numara: No 11')).toBeTruthy();
    expect(within(dialog).getByText('Tahmini bekleme ~35 dk · önünde 3 kişi var.')).toBeTruthy();
    expect(
      within(dialog).getByRole('button', { name: 'Fark etmez' }).getAttribute('aria-pressed'),
    ).toBe('true');
    await userEvent.type(within(dialog).getByLabelText('Ad'), 'Burak');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Saç kesimi' }));
    await userEvent.click(within(dialog).getByRole('button', { name: 'Sıraya ekle' }));
    expect(await screen.findByText('Sıraya eklendi · No 11')).toBeTruthy();
    expect(calls['/api/v1/manage/walk-in-tickets']).toEqual([
      {
        branchId: '0193a8f0-0000-7000-8000-0000000000b1',
        fullName: 'Burak',
        serviceId: haircut,
        preferredStaffId: null,
      },
    ]);
    expect(calls['/api/v1/manage/walk-in-tickets Idempotency-Key']).toHaveLength(1);
  });

  it('"Sıraya ekle" doğrulama: ad zorunlu, telefon girildiyse geçerli olmalı', async () => {
    const { calls } = openQueue();
    await screen.findByRole('table', { name: 'Sıradakiler' });
    await userEvent.click(screen.getByRole('button', { name: 'Sıraya ekle' }));
    const dialog = await screen.findByRole('dialog', { name: 'Sıraya ekle' });
    await userEvent.type(within(dialog).getByLabelText('Telefon (isteğe bağlı)'), '0532 12');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Sıraya ekle' }));
    expect(within(dialog).getByText('Ad en az 2 karakter olmalı.')).toBeTruthy();
    expect(within(dialog).getByText('Cep telefonu 10 haneli olmalı: 0532 123 45 67')).toBeTruthy();
    expect(calls['/api/v1/manage/walk-in-tickets']).toBeUndefined();
  });

  it('boş sıra: KapidaSiraBos', async () => {
    openQueue({ queues: { walkInsEmpty: true } });
    expect(await screen.findByRole('heading', { name: 'Sırada kimse yok' })).toBeTruthy();
    expect(screen.getByText('Merkez · bugün')).toBeTruthy();
    expect(screen.getAllByRole('button', { name: 'Sıraya ekle' })).toHaveLength(2);
  });

  it('personel kapıda sırayı görür; menüde "Kapıda sıra" seçili', async () => {
    renderApp(path, {
      withServices: true,
      session: sessionFixture({
        permissions: staffPermissions,
        membership: { role: 'Staff', branchIds: [], staffId: elif },
      }),
    });
    expect(await screen.findByRole('table', { name: 'Sıradakiler' })).toBeTruthy();
    const nav = screen.getByRole('navigation', { name: 'Ana menü' });
    expect(
      within(nav).getByRole('link', { name: 'Kapıda sıra' }).getAttribute('aria-current'),
    ).toBe('page');
  });
});

describe('takvim ve Genel bakışta kapıda sıra', () => {
  it('takvimdeki "Kapıda sıra" geçişi sıra sayfasını açar', async () => {
    const { router } = renderApp('/takvim');
    await userEvent.click(await screen.findByRole('button', { name: 'Kapıda sıra' }));
    expect(await screen.findByRole('heading', { level: 1, name: 'Kapıda sıra' })).toBeTruthy();
    expect(router.state.location.pathname).toBe(path);
  });

  it('Genel bakış "Kapıda sırada" bloğu sıradaki kişi sayısını gösterir', async () => {
    renderApp('/', {
      session: sessionFixture({ brand: { ...sessionFixture().brand, status: 'Active' } }),
    });
    await screen.findByRole('region', { name: 'Gün akışı' });
    const bento = screen.getByRole('group', { name: 'Özet' });
    expect(within(bento).getByText('Kapıda sırada')).toBeTruthy();
    expect(await within(bento).findByText('~35 dk bekleme')).toBeTruthy();
    expect(within(bento).getByText('3')).toBeTruthy();
  });

  it('izni olmayan kullanıcıda blok ve geçiş yok', async () => {
    renderApp('/takvim', {
      session: sessionFixture({
        permissions: ['calendar.view', 'calendar.view.all-staff', 'report.view'],
      }),
    });
    await screen.findByRole('region', { name: 'Günlük takvim' });
    expect(screen.queryByText('Kapıda sırada')).toBeNull();
    expect(screen.queryByRole('button', { name: 'Kapıda sıra' })).toBeNull();
  });
});
