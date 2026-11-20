import { cleanup, screen, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it } from 'vitest';
import { branchId, sessionFixture } from '../../test/fake-api';
import { renderApp } from '../../test/render-app';

// Kural: dilim-01 kabul kriteri 5 (Salı 10:00–13:00 ve 13:45–19:00; örtüşen aralık hatalı), 6 (1 Ocak tek gün,
// Ramazan Bayramı 3 gün), § Kapsam (Şube ekle); salon-paneli/AyarlarSube ve SubeEkle; api-kurallari.md (If-Match).
afterEach(cleanup);

const base = `/api/v1/manage/branches/${branchId}`;
const save = () => userEvent.click(screen.getByRole('button', { name: 'Değişiklikleri kaydet' }));
const time = (name: string) => screen.getByLabelText<HTMLInputElement>(name);

async function setTime(name: string, value: string) {
  const input = time(name);
  await userEvent.clear(input);
  await userEvent.type(input, value);
}

describe('Şubeler ve saatler', () => {
  it('seçili şubenin saatleri: açık günlerde aralıklar, kapalı günler, saat dilimi', async () => {
    renderApp('/ayarlar/sube');
    expect(await screen.findByRole('heading', { name: 'Merkez · çalışma saatleri' })).toBeTruthy();
    expect(screen.getByText('Merkez · Moda Cad. 42')).toBeTruthy();
    expect(screen.getByRole<HTMLInputElement>('switch', { name: 'Salı açık' }).checked).toBe(true);
    expect(time('Salı 1. aralık başlangıç').value).toBe('10:00');
    expect(screen.getByRole<HTMLInputElement>('switch', { name: 'Pazartesi açık' }).checked).toBe(
      false,
    );
    expect(screen.getByText('İstanbul (UTC+3)')).toBeTruthy();
    // Tek şubede şubeler arası geçiş süresi gösterilmez.
    expect(screen.queryByText('Şubeler arası geçiş süresi')).toBeNull();
  });

  it('Salı 10:00–13:00 ve 13:45–19:00 sürümle kaydedilir (kabul kriteri 5)', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/ayarlar/sube', { calls });
    await screen.findByRole('heading', { name: 'Merkez · çalışma saatleri' });
    await setTime('Salı 1. aralık bitiş', '13:00');
    await userEvent.click(screen.getByRole('button', { name: 'Aralık ekle: Salı' }));
    await setTime('Salı 2. aralık başlangıç', '13:45');
    await setTime('Salı 2. aralık bitiş', '19:00');
    await save();
    expect(await screen.findByText('Değişiklikler kaydedildi')).toBeTruthy();
    expect(calls[`PUT ${base}/working-hours If-Match`]).toEqual(['9']);
    expect(calls[`${base}/working-hours`]?.[0]).toEqual({
      days: [
        {
          dayOfWeek: 2,
          intervals: [
            { start: '10:00', end: '13:00' },
            { start: '13:45', end: '19:00' },
          ],
        },
        { dayOfWeek: 6, intervals: [{ start: '09:00', end: '19:00' }] },
      ],
    });
    // Saat aralığı ve geçiş süresi değişmediği için gönderilmez.
    expect(calls[base]).toBeUndefined();
  });

  it('örtüşen aralık kaydedilmez ve günün altında yazar', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/ayarlar/sube', { calls });
    await screen.findByRole('heading', { name: 'Merkez · çalışma saatleri' });
    await userEvent.click(screen.getByRole('button', { name: 'Aralık ekle: Salı' }));
    await setTime('Salı 2. aralık başlangıç', '13:00');
    await setTime('Salı 2. aralık bitiş', '19:00');
    await save();
    expect(await screen.findByText('Aralıklar örtüşmemeli.')).toBeTruthy();
    expect(calls[`${base}/working-hours`]).toBeUndefined();
  });

  it('açılan güne önceki açık günün saatleri gelir; saat aralığı şubeye kaydedilir', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/ayarlar/sube', { calls });
    await screen.findByRole('heading', { name: 'Merkez · çalışma saatleri' });
    await userEvent.click(screen.getByRole('switch', { name: 'Çarşamba açık' }));
    expect(time('Çarşamba 1. aralık bitiş').value).toBe('20:00');
    await userEvent.click(screen.getByRole('radio', { name: '10 dk' }));
    await save();
    await screen.findByText('Değişiklikler kaydedildi');
    expect(calls[`${base}/working-hours`]?.[0]).toMatchObject({
      days: [
        { dayOfWeek: 2 },
        { dayOfWeek: 3, intervals: [{ start: '10:00', end: '20:00' }] },
        { dayOfWeek: 6 },
      ],
    });
    expect(calls[`PATCH ${base} If-Match`]).toEqual(['5']);
    expect(calls[base]?.[0]).toEqual({ slotIntervalMinutes: 10 });
  });

  it('iki şubede geçiş süresi marka ayarına kaydedilir', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/ayarlar/sube', {
      calls,
      session: sessionFixture({
        branches: [
          { id: branchId, name: 'Merkez' },
          { id: '0193a8f0-0000-7000-8000-0000000000b2', name: 'Moda Sahil' },
        ],
      }),
    });
    await userEvent.selectOptions(
      await screen.findByRole('combobox', { name: 'Şubeler arası geçiş süresi' }),
      '45',
    );
    await save();
    await screen.findByText('Değişiklikler kaydedildi');
    expect(calls['/api/v1/manage/brand/settings']?.[0]).toMatchObject({
      defaultBranchTransferMinutes: 45,
      approvalTimeoutMinutes: 120,
    });
  });
});

describe('Özel günler', () => {
  it('tek gün ve çok günlü satırlar (kabul kriteri 6); satır kaldırılınca her gün silinir', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/ayarlar/sube', { calls });
    const card = await screen.findByRole('region', { name: 'Özel günler ve tatiller' });
    const rows = within(card).getAllByRole('listitem');
    expect(rows.map((row) => row.textContent)).toEqual([
      expect.stringMatching(/31 Aralık.*Erken kapanış 10:00–16:00.*Özel saat · Tek gün/),
      expect.stringMatching(/1 Ocak.*Yılbaşı.*Kapalı · Tek gün/),
      expect.stringMatching(/20–22 Mart.*Ramazan Bayramı.*Kapalı · 3 gün/),
    ]);
    await userEvent.click(
      within(card).getByRole('button', { name: '20–22 Mart özel gününü kaldır' }),
    );
    expect(await screen.findByText('Özel gün kaldırıldı')).toBeTruthy();
    expect(Object.keys(calls).filter((path) => path.startsWith(`${base}/overrides/`))).toHaveLength(
      3,
    );
  });

  it('kapalı gün aralığı eklenir', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/ayarlar/sube', { calls });
    await userEvent.click(await screen.findByRole('button', { name: 'Özel gün ekle' }));
    const dialog = screen.getByRole('dialog', { name: 'Özel gün ekle' });
    await userEvent.type(within(dialog).getByLabelText('Başlangıç tarihi'), '2027-03-20');
    await userEvent.type(within(dialog).getByLabelText('Bitiş tarihi'), '2027-03-22');
    await userEvent.type(within(dialog).getByRole('textbox', { name: 'Not' }), 'Ramazan Bayramı');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Özel günü ekle' }));
    expect(await screen.findByText('Özel gün eklendi')).toBeTruthy();
    expect(calls[`${base}/overrides`]?.[0]).toEqual({
      fromDate: '2027-03-20',
      toDate: '2027-03-22',
      kind: 'Closure',
      isClosed: true,
      intervals: [],
      note: 'Ramazan Bayramı',
    });
    expect(screen.queryByRole('dialog')).toBeNull();
  });

  it('özel saatte kapanış açılıştan sonra olmalı', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/ayarlar/sube', { calls });
    await userEvent.click(await screen.findByRole('button', { name: 'Özel gün ekle' }));
    const dialog = screen.getByRole('dialog', { name: 'Özel gün ekle' });
    await userEvent.selectOptions(
      within(dialog).getByRole('combobox', { name: 'Tür' }),
      'ShortDay',
    );
    await userEvent.type(within(dialog).getByLabelText('Tarih'), '2026-12-31');
    await userEvent.type(within(dialog).getByLabelText('Açılış'), '16:00');
    await userEvent.type(within(dialog).getByLabelText('Kapanış'), '10:00');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Özel günü ekle' }));
    expect(await within(dialog).findByText('Bitiş saati başlangıçtan sonra olmalı.')).toBeTruthy();
    expect(calls[`${base}/overrides`]).toBeUndefined();
  });
});

describe('Şube ekle', () => {
  it('saatler ve fiyatlar seçili şubeden kopyalanır', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/ayarlar/sube', { calls });
    await userEvent.click(await screen.findByRole('button', { name: 'Şube ekle' }));
    const dialog = screen.getByRole('dialog', { name: 'Şube ekle' });
    expect(
      within(dialog).getByRole<HTMLSelectElement>('combobox', { name: 'Çalışma saatleri' })
        .selectedOptions[0]?.textContent,
    ).toBe('İşletme saatleriyle aynı');
    await userEvent.type(within(dialog).getByRole('textbox', { name: 'Şube adı' }), 'Moda Sahil');
    await userEvent.type(within(dialog).getByRole('textbox', { name: 'Adres' }), 'Moda Cad. 120');
    await userEvent.type(within(dialog).getByRole('textbox', { name: 'İlçe' }), 'Kadıköy');
    await userEvent.type(within(dialog).getByRole('textbox', { name: 'İl' }), 'İstanbul');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Şubeyi ekle' }));
    expect(await screen.findByText('Moda Sahil şubesi eklendi')).toBeTruthy();
    expect(calls['/api/v1/manage/branches']?.[0]).toEqual({
      name: 'Moda Sahil',
      phone: null,
      addressLine: 'Moda Cad. 120',
      district: 'Kadıköy',
      city: 'İstanbul',
      copyWorkingHoursFromBranchId: branchId,
      copyServicePricesFromBranchId: branchId,
    });
  });

  it('eksik adres gönderilmez; plan sınırında nedeni yazar', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/ayarlar/sube', { calls, branchLimitReached: true });
    await userEvent.click(await screen.findByRole('button', { name: 'Şube ekle' }));
    const dialog = screen.getByRole('dialog', { name: 'Şube ekle' });
    await userEvent.type(within(dialog).getByRole('textbox', { name: 'Şube adı' }), 'Moda Sahil');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Şubeyi ekle' }));
    expect(await within(dialog).findByText('Adresi girin.')).toBeTruthy();
    expect(calls['/api/v1/manage/branches']).toBeUndefined();
    await userEvent.type(within(dialog).getByRole('textbox', { name: 'Adres' }), 'Moda Cad. 120');
    await userEvent.type(within(dialog).getByRole('textbox', { name: 'İlçe' }), 'Kadıköy');
    await userEvent.type(within(dialog).getByRole('textbox', { name: 'İl' }), 'İstanbul');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Şubeyi ekle' }));
    expect(await within(dialog).findByText('Planınızın şube sınırına ulaştınız')).toBeTruthy();
  });
});
