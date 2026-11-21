import { cleanup, screen, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it } from 'vitest';
import { ownerPermissions, sessionFixture } from '../../test/fake-api';
import { renderApp } from '../../test/render-app';

// Kural: dilim-04 kabul kriteri 1 (IBAN yokken kapora pasif, "IBAN ekle" deseni; geçersiz IBAN validation_failed),
// kapsam (alıcı, banka, IBAN TR + mod-97, önek, havale süresi 1/2/4 sa, "Müşteri ne görür?"); salon-paneli/AyarlarOdeme,
// Ayarlar (kapora özeti), Durumlar ("Kapora istenemiyor"); sözleşme manageGetBankAccount / manageUpdateBankAccount
// (tanımsızken W/"0", iban ilk kayıtta zorunlu, sonra isteğe bağlı); roller-yetkiler.md (settings.payment yalnız Owner);
// V1 (havale süresi yalnız Kapora ve IBAN'da düzenlenir).
afterEach(cleanup);

const path = '/api/v1/manage/brand/bank-account';
const validIban = 'TR29 0006 4000 0011 2345 6789 01';
const save = () => userEvent.click(screen.getByRole('button', { name: 'Kaydet' }));
const field = (name: string) => screen.getByRole<HTMLInputElement>('textbox', { name });
const preview = () => screen.getByRole('region', { name: 'Müşteri ne görür?' });
const managerWithoutPayment = () =>
  sessionFixture({ permissions: ownerPermissions.filter((p) => p !== 'settings.payment') });

describe('Kapora ve IBAN', () => {
  it('IBAN tanımsız: bilgi kutusu, varsayılanlar, menüde seçili; IBAN olmadan kaydedilmez', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/ayarlar/kapora', { calls });
    expect(await screen.findByText('OMVILA ödeme almaz.')).toBeTruthy();
    const nav = screen.getByRole('navigation', { name: 'Ayar bölümleri' });
    expect(
      within(nav).getByRole('link', { name: 'Kapora ve IBAN' }).getAttribute('aria-current'),
    ).toBe('page');
    expect(field('IBAN').value).toBe('');
    expect(field('Açıklama kodu öneki').value).toBe('OMV');
    expect(screen.getByRole<HTMLSelectElement>('combobox', { name: 'Havale süresi' }).value).toBe(
      '120',
    );
    expect(within(preview()).getByText('OMV-7K2X')).toBeTruthy();
    await userEvent.type(field('Alıcı adı'), 'Elif Kaya');
    await userEvent.type(field('Banka'), 'Ziraat Bankası');
    await save();
    expect(await screen.findByText('IBAN girin: TR ile başlayan 26 karakter.')).toBeTruthy();
    expect(calls[path]).toBeUndefined();
  });

  it('geçersiz IBAN alanın altında yazar ve alanla ilişkilidir; geçerli IBAN W/"0" ile gönderilir', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/ayarlar/kapora', { calls });
    await userEvent.type(await screen.findByRole('textbox', { name: 'Alıcı adı' }), 'Elif Kaya');
    await userEvent.type(field('Banka'), 'Ziraat Bankası');
    const iban = field('IBAN');
    await userEvent.type(iban, 'TR12 0006 4000 0011 2345 6789 01');
    await save();
    const message = 'IBAN’ı kontrol edin: TR ile başlayan 26 karakter olmalı.';
    expect(await screen.findByText(message)).toBeTruthy();
    expect(iban.getAttribute('aria-invalid')).toBe('true');
    expect(document.getElementById(iban.getAttribute('aria-describedby') ?? '')?.textContent).toBe(
      message,
    );
    expect(calls[path]).toBeUndefined();

    await userEvent.clear(iban);
    await userEvent.type(iban, validIban.toLowerCase());
    await userEvent.selectOptions(screen.getByRole('combobox', { name: 'Havale süresi' }), '240');
    expect(within(preview()).getByText('TR29 … 6789 01')).toBeTruthy();
    expect(within(preview()).getByText('Elif Kaya')).toBeTruthy();
    await save();
    expect(await screen.findByText('Kapora hesabı kaydedildi')).toBeTruthy();
    expect(calls[`PUT ${path} If-Match`]).toEqual(['W/"0"']);
    expect(calls[path]?.[0]).toEqual({
      accountHolderName: 'Elif Kaya',
      bankName: 'Ziraat Bankası',
      iban: 'TR290006400000112345678901',
      transferCodePrefix: 'OMV',
      transferWindowMinutes: 240,
    });
  });

  it('IBAN tanımlı: maskeli değer gösterilir; IBAN değişmezse gönderilmez; önek büyük harfe çevrilir', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/ayarlar/kapora', { calls, depositAvailable: true });
    const iban = await screen.findByRole<HTMLInputElement>('textbox', { name: 'IBAN' });
    expect(iban.value).toBe('TR29 •••• 6789 01');
    expect(within(preview()).getByText('TR29 … 6789 01')).toBeTruthy();
    const prefix = field('Açıklama kodu öneki');
    await userEvent.clear(prefix);
    await userEvent.type(prefix, 'elf');
    expect(prefix.value).toBe('ELF');
    expect(within(preview()).getByText('ELF-7K2X')).toBeTruthy();
    await save();
    expect(await screen.findByText('Kapora hesabı kaydedildi')).toBeTruthy();
    expect(calls[`PUT ${path} If-Match`]).toEqual(['W/"5521"']);
    expect(calls[path]?.[0]).toEqual({
      accountHolderName: 'Elif Kaya',
      bankName: 'Ziraat Bankası',
      transferCodePrefix: 'ELF',
      transferWindowMinutes: 120,
    });
  });

  it('kısa önek gönderilmez; sunucu IBAN’ı reddederse alanın altında yazar', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/ayarlar/kapora', {
      calls,
      depositAvailable: true,
      saveError: 'validation_failed:iban',
    });
    const prefix = await screen.findByRole('textbox', { name: 'Açıklama kodu öneki' });
    await userEvent.clear(prefix);
    await userEvent.type(prefix, 'O');
    await save();
    expect(await screen.findByText('Önek 2–5 Latin harfi olmalı: OMV')).toBeTruthy();
    expect(calls[path]).toBeUndefined();
    await userEvent.type(prefix, 'MV');
    const iban = field('IBAN');
    await userEvent.clear(iban);
    await userEvent.type(iban, validIban);
    await save();
    expect(
      await screen.findByText('IBAN’ı kontrol edin: TR ile başlayan 26 karakter olmalı.'),
    ).toBeTruthy();
    expect(calls[path]).toHaveLength(1);
  });

  it('settings.payment izni yoksa menüde yok, adres 403', async () => {
    renderApp('/ayarlar/randevu-kurallari', { session: managerWithoutPayment() });
    const nav = await screen.findByRole('navigation', { name: 'Ayar bölümleri' });
    expect(within(nav).queryByRole('link', { name: 'Kapora ve IBAN' })).toBeNull();
    // IBAN ekle bağlantısı da yok; desen yine görünür.
    expect(await screen.findByText('Kapora IBAN’ı tanımlı değil.')).toBeTruthy();
    expect(screen.queryByRole('button', { name: 'IBAN ekle' })).toBeNull();
    cleanup();
    renderApp('/ayarlar/kapora', { session: managerWithoutPayment() });
    expect(
      await screen.findByRole('heading', { level: 1, name: 'Bu sayfayı görme yetkiniz yok' }),
    ).toBeTruthy();
  });
});

describe('Randevu kuralları kapora bölümü', () => {
  it('IBAN yokken "Kapora istenemiyor" deseni; IBAN ekle Kapora ve IBAN’ı açar', async () => {
    const { router } = renderApp('/ayarlar/randevu-kurallari');
    expect(await screen.findByText('Kapora IBAN’ı tanımlı değil.')).toBeTruthy();
    expect(
      screen.getByText('IBAN eklenene kadar kapora gerektiren hizmetler kaporasız alınır.'),
    ).toBeTruthy();
    expect(screen.queryByText('Havale süresi')).toBeNull();
    await userEvent.click(screen.getByRole('button', { name: 'IBAN ekle' }));
    expect(await screen.findByText('OMVILA ödeme almaz.')).toBeTruthy();
    expect(router.state.location.pathname).toBe('/ayarlar/kapora');
  });

  it('IBAN tanımlıyken özet, etkin seçenekler ve yalnız gösterilen havale süresi', async () => {
    renderApp('/ayarlar/randevu-kurallari', { depositAvailable: true });
    expect(
      await screen.findByText('Kapora IBAN’ı tanımlı · Elif Kaya · TR29 •••• 6789 01'),
    ).toBeTruthy();
    expect(
      screen.getByRole<HTMLSelectElement>('combobox', { name: 'Kapora istenecek durumlar' })
        .disabled,
    ).toBe(false);
    expect(screen.queryByRole('combobox', { name: 'Havale süresi' })).toBeNull();
    expect(screen.getByText('2 saat')).toBeTruthy();
    expect(
      screen
        .getByRole('link', { name: 'Havale süresini Kapora ve IBAN’da değiştirin' })
        .getAttribute('href'),
    ).toBe('/ayarlar/kapora');
  });
});

describe('Hizmet formu kapora alanı', () => {
  it('IBAN tanımlıyken etkin', async () => {
    renderApp('/hizmetler', { withServices: true, depositAvailable: true });
    await screen.findByRole('table', { name: 'Hizmetler' });
    await userEvent.click(screen.getByRole('button', { name: 'Hizmet ekle' }));
    const dialog = await screen.findByRole('dialog', { name: 'Hizmet ekle' });
    expect(
      within(dialog).getByRole<HTMLSelectElement>('combobox', { name: 'Kapora' }).disabled,
    ).toBe(false);
    expect(within(dialog).queryByText('Kapora için IBAN gerekir.')).toBeNull();
  });
});
