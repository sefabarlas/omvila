import { cleanup, screen, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it } from 'vitest';
import { branchId, secondBranchId, sessionFixture, staffPermissions } from '../../test/fake-api';
import { renderApp } from '../../test/render-app';

// Kural: dilim-01 kabul kriteri 7 (aşamalı "Boya" 45 + 40 + 35 = 120 dk; IBAN yokken kapora pasif), 8 (hazır listeden
// seçilenler eklenir), 9 (Nişantaşı şube fiyatı 550 ₺), salon-paneli/Hizmetler, HizmetlerBos, HizmetEkle, KategoriEkle.
afterEach(cleanup);

const twoBranches = sessionFixture({
  branches: [
    { id: branchId, name: 'Kadıköy' },
    { id: secondBranchId, name: 'Nişantaşı' },
  ],
});

describe('boş hizmet listesi', () => {
  it('hazır liste önerisi; seçilenler fiyatlarıyla eklenir (kabul kriteri 8)', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/hizmetler', { calls });
    expect(await screen.findByRole('heading', { name: 'İlk hizmetinizi ekleyin' })).toBeTruthy();
    expect(screen.getByText('Merkez fiyatları')).toBeTruthy();
    expect(await screen.findByText('3 hizmet; fiyatları siz girersiniz.')).toBeTruthy();
    await userEvent.click(screen.getByRole('button', { name: 'Hazır listeden başla' }));
    const dialog = await screen.findByRole('dialog', { name: 'Kuaför hazır listesi' });
    await userEvent.click(within(dialog).getByRole('checkbox', { name: 'Boya' }));
    await userEvent.type(within(dialog).getByRole('textbox', { name: 'Saç kesimi fiyatı' }), '450');
    await userEvent.click(within(dialog).getByRole('button', { name: '2 hizmeti ekle' }));
    expect(await screen.findByText('2 hizmet eklendi')).toBeTruthy();
    expect(calls['/api/v1/manage/services/from-templates']?.[0]).toEqual({
      items: [
        {
          templateId: '0193a8f0-0000-7000-8000-000000000a01',
          price: { amountMinor: 45000, currency: 'TRY' },
        },
        { templateId: '0193a8f0-0000-7000-8000-000000000a02' },
      ],
    });
  });

  it('ayar izni olmayan personel 403 görür', async () => {
    renderApp('/hizmetler', {
      session: sessionFixture({
        permissions: staffPermissions,
        membership: { role: 'Staff', branchIds: [], staffId: null },
      }),
    });
    expect(
      await screen.findByRole('heading', { level: 1, name: 'Bu sayfayı görme yetkiniz yok' }),
    ).toBeTruthy();
  });
});

describe('hizmet listesi', () => {
  it('kategoriler, süre, fiyat, personel ve kapora; online açılınca genel kayıt yazılır', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/hizmetler', { calls, withServices: true });
    const table = await screen.findByRole('table', { name: 'Hizmetler' });
    expect(screen.getByText('2 hizmet · 2 kategori · Merkez fiyatları')).toBeTruthy();
    expect(screen.getByRole('button', { name: /Saç\s*2/ }).getAttribute('aria-pressed')).toBe(
      'true',
    );
    const [first, second] = within(table).getAllByRole('row').slice(1);
    if (!first || !second) {
      throw new Error('satırlar eksik');
    }

    expect(first.textContent).toContain('45 dk');
    expect(first.textContent).toContain('450 ₺');
    expect(within(first).getByLabelText('Elif Kaya, Mert Demir, Can Yücel')).toBeTruthy();
    expect(second.textContent).toContain("1.800 ₺'den");
    // IBAN yokken kapora istenemez.
    expect(second.textContent).toContain('Yok');
    await userEvent.click(screen.getByRole('switch', { name: 'Keratin online randevuya açık' }));
    const path = '/api/v1/manage/services/0193a8f0-0000-7000-8000-0000000000f2';
    await screen.findByRole('table', { name: 'Hizmetler' });
    await expect.poll(() => calls[path]?.length).toBe(1);
    expect(calls[`PUT ${path} If-Match`]).toEqual(['2']);
    expect(calls[path]?.[0]).toMatchObject({
      isOnlineBookable: true,
      price: { amountMinor: 180000, currency: 'TRY' },
    });
  });

  it('aşamalı hizmet: süre aşama toplamı 120 dk; IBAN yokken kapora pasif (kabul kriteri 7)', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/hizmetler', { calls, withServices: true });
    await screen.findByRole('table', { name: 'Hizmetler' });
    await userEvent.click(screen.getByRole('button', { name: 'Hizmet ekle' }));
    const dialog = await screen.findByRole('dialog', { name: 'Hizmet ekle' });
    expect(
      within(dialog).getByRole<HTMLSelectElement>('combobox', { name: 'Kapora' }).disabled,
    ).toBe(true);
    await userEvent.type(within(dialog).getByRole('textbox', { name: 'Hizmet adı' }), 'Boya');
    await userEvent.selectOptions(
      within(dialog).getByRole('combobox', { name: 'Kategori' }),
      'Renk',
    );
    await userEvent.type(within(dialog).getByRole('textbox', { name: 'Fiyat' }), '900');
    const addStage = async (name: string, minutes: string, waiting = false) => {
      await userEvent.click(within(dialog).getByRole('button', { name: 'Aşama' }));
      await userEvent.type(within(dialog).getByRole('textbox', { name: 'Aşama adı' }), name);
      await userEvent.type(within(dialog).getByRole('textbox', { name: 'Aşama süresi' }), minutes);
      if (waiting) {
        await userEvent.click(
          within(dialog).getByRole('checkbox', { name: 'Bekleme (personel müsait)' }),
        );
      }

      await userEvent.click(within(dialog).getByRole('button', { name: 'Aşamayı ekle' }));
    };
    await addStage('Uygulama', '45');
    await addStage('Bekleme', '40', true);
    await addStage('Yıkama ve fön', '35');
    expect(within(dialog).getByRole<HTMLInputElement>('textbox', { name: 'Süre' }).value).toBe(
      '120',
    );
    await userEvent.click(within(dialog).getByRole('button', { name: 'Hizmeti ekle' }));
    expect(await screen.findByText('Boya eklendi')).toBeTruthy();
    expect(calls['/api/v1/manage/services']?.[0]).toMatchObject({
      name: 'Boya',
      categoryId: '0193a8f0-0000-7000-8000-0000000000e2',
      durationMinutes: 120,
      price: { amountMinor: 90000, currency: 'TRY' },
      priceDisplay: 'Fixed',
      deposit: { mode: 'Inherit' },
      isOnlineBookable: true,
      stages: [
        { name: 'Uygulama', durationMinutes: 45, staffBusy: true },
        { name: 'Bekleme', durationMinutes: 40, staffBusy: false },
        { name: 'Yıkama ve fön', durationMinutes: 35, staffBusy: true },
      ],
      staffIds: [
        '0193a8f0-0000-7000-8000-0000000000c1',
        '0193a8f0-0000-7000-8000-0000000000c2',
        '0193a8f0-0000-7000-8000-0000000000c3',
      ],
    });
  });

  it('tek personelli salonda "Kimler veriyor?" dizi olarak gönderilir', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/hizmetler', { calls, withServices: true, onlyOwner: true });
    await screen.findByRole('table', { name: 'Hizmetler' });
    await userEvent.click(screen.getByRole('button', { name: 'Hizmet ekle' }));
    const dialog = await screen.findByRole('dialog', { name: 'Hizmet ekle' });
    await userEvent.type(within(dialog).getByRole('textbox', { name: 'Hizmet adı' }), 'Fön');
    await userEvent.type(within(dialog).getByRole('textbox', { name: 'Süre' }), '30');
    await userEvent.type(within(dialog).getByRole('textbox', { name: 'Fiyat' }), '250');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Hizmeti ekle' }));
    expect(await screen.findByText('Fön eklendi')).toBeTruthy();
    expect(calls['/api/v1/manage/services']?.[0]).toMatchObject({
      staffIds: ['0193a8f0-0000-7000-8000-0000000000c1'],
    });
  });

  it('eksik ad, süre ve fiyat gönderilmez', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/hizmetler', { calls, withServices: true });
    await screen.findByRole('table', { name: 'Hizmetler' });
    await userEvent.click(screen.getByRole('button', { name: 'Hizmet ekle' }));
    const dialog = await screen.findByRole('dialog', { name: 'Hizmet ekle' });
    await userEvent.click(within(dialog).getByRole('button', { name: 'Hizmeti ekle' }));
    expect(await within(dialog).findByText('Hizmet adını girin.')).toBeTruthy();
    expect(within(dialog).getByText('Süre 5 ile 720 dakika arasında olmalı.')).toBeTruthy();
    expect(within(dialog).getByText('Fiyatı girin: 450 veya 85,50 gibi.')).toBeTruthy();
    expect(calls['/api/v1/manage/services']).toBeUndefined();
  });

  it('Nişantaşı şube fiyatı 550 ₺ görünür ve değiştirilir; genel fiyat korunur (kabul kriteri 9)', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/hizmetler', { calls, withServices: true, session: twoBranches });
    await screen.findByRole('table', { name: 'Hizmetler' });
    expect(screen.getByText('Fiyatlar Kadıköy şubesi için.')).toBeTruthy();
    await userEvent.selectOptions(screen.getByLabelText('Şube'), secondBranchId);
    expect(await screen.findByText('550 ₺')).toBeTruthy();
    await userEvent.click(screen.getByRole('button', { name: 'Saç kesimi düzenle' }));
    const dialog = await screen.findByRole('dialog', { name: 'Hizmeti düzenle' });
    const branchPrice = within(dialog).getByRole<HTMLInputElement>('textbox', {
      name: 'Nişantaşı şubesinde fiyat',
    });
    expect(branchPrice.value).toBe('550');
    expect(
      within(dialog).getByRole<HTMLInputElement>('textbox', { name: 'Genel fiyat' }).value,
    ).toBe('450');
    await userEvent.clear(branchPrice);
    await userEvent.type(branchPrice, '600');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Hizmeti kaydet' }));
    expect(await screen.findByText('Hizmet kaydedildi')).toBeTruthy();
    const service = '/api/v1/manage/services/0193a8f0-0000-7000-8000-0000000000f1';
    expect(calls[service]?.[0]).toMatchObject({ price: { amountMinor: 45000, currency: 'TRY' } });
    expect(calls[`${service}/branch-prices/${secondBranchId}`]?.[0]).toEqual({
      price: { amountMinor: 60000, currency: 'TRY' },
      isAvailable: true,
    });
  });

  it('412: "Kayıt çakışması · Yenile" şeridi (kabul kriteri 15)', async () => {
    renderApp('/hizmetler', { withServices: true, saveError: 'concurrency_conflict' });
    await screen.findByRole('table', { name: 'Hizmetler' });
    await userEvent.click(screen.getByRole('button', { name: 'Saç kesimi düzenle' }));
    const dialog = await screen.findByRole('dialog', { name: 'Hizmeti düzenle' });
    await userEvent.click(within(dialog).getByRole('button', { name: 'Hizmeti kaydet' }));
    expect(await within(dialog).findByText('Kayıt çakışması')).toBeTruthy();
    await userEvent.click(within(dialog).getByRole('button', { name: 'Yenile' }));
    expect(within(dialog).queryByText('Kayıt çakışması')).toBeNull();
  });

  it('kategori ekle: aynı ad reddedilir, yeni kategori eklenir', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/hizmetler', { calls, withServices: true });
    await screen.findByRole('table', { name: 'Hizmetler' });
    await userEvent.click(screen.getByRole('button', { name: 'Kategori ekle' }));
    const dialog = await screen.findByRole('dialog', { name: 'Kategori ekle' });
    const name = within(dialog).getByRole('textbox', { name: 'Kategori adı' });
    await userEvent.type(name, 'Saç');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Kategoriyi ekle' }));
    expect(await within(dialog).findByText('Bu adda bir kategori zaten var.')).toBeTruthy();
    await userEvent.clear(name);
    await userEvent.type(name, 'Makyaj');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Kategoriyi ekle' }));
    expect(await screen.findByText('Makyaj kategorisi eklendi')).toBeTruthy();
    expect(calls['/api/v1/manage/service-categories']?.[1]).toEqual({ name: 'Makyaj' });
  });
});
