import { cleanup, screen, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it } from 'vitest';
import { sessionFixture, staffPermissions } from '../../test/fake-api';
import { renderApp } from '../../test/render-app';

// Kural: dilim-01 kapsamı (Ayarlar: işletme bilgileri, randevu kuralları yalnız kayıt, rezervasyon sayfası),
// salon-paneli/AyarlarIsletme, Ayarlar, AyarlarRezervasyon; api-kurallari.md (If-Match, 412 "Kayıt çakışması · Yenile",
// 409 slug_unavailable); sureler-ve-ayarlar.md (seçenekler, kapora IBAN olmadan istenemez); roller-yetkiler.md.
afterEach(cleanup);

const save = () => userEvent.click(screen.getByRole('button', { name: 'Değişiklikleri kaydet' }));

describe('İşletme bilgileri', () => {
  it('Ayarlar ilk bölümü açar; yalnız yapılmış bölümler menüde; alanlar dolu', async () => {
    const { router } = renderApp('/ayarlar');
    expect(await screen.findByRole('textbox', { name: 'İşletme adı' })).toBeTruthy();
    expect(router.state.location.pathname).toBe('/ayarlar/isletme');
    const nav = screen.getByRole('navigation', { name: 'Ayar bölümleri' });
    expect(
      within(nav)
        .getAllByRole('link')
        .map((link) => link.textContent),
    ).toEqual([
      'İşletme bilgileri',
      'Şubeler ve saatler',
      'Randevu kuralları',
      'Rezervasyon sayfası',
      'Bildirimler ve SMS',
      'Kapora ve IBAN',
      'KVKK ve veri',
    ]);
    expect(
      within(nav).getByRole('link', { name: 'İşletme bilgileri' }).getAttribute('aria-current'),
    ).toBe('page');
    expect(screen.getByRole<HTMLInputElement>('textbox', { name: 'İşletme adı' }).value).toBe(
      'Kuaför Elif',
    );
    expect(screen.getByRole<HTMLInputElement>('textbox', { name: 'İşletme telefonu' }).value).toBe(
      '0216 345 12 12',
    );
    expect(screen.getByRole<HTMLSelectElement>('combobox', { name: 'Kategori' }).value).toBe(
      'hair_salon',
    );
  });

  it('kaydet: sürümle (If-Match) gönderilir, boş alan null olur, onay bildirimi', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/ayarlar/isletme', { calls });
    const name = await screen.findByRole('textbox', { name: 'İşletme adı' });
    await userEvent.clear(name);
    await userEvent.type(name, 'Kuaför Elif Kadıköy');
    await userEvent.type(screen.getByRole('textbox', { name: 'E-posta' }), 'merhaba@ornek.com');
    await save();
    expect(await screen.findByText('Değişiklikler kaydedildi')).toBeTruthy();
    expect(calls['PATCH /api/v1/manage/brand If-Match']).toEqual(['7']);
    expect(calls['/api/v1/manage/brand']?.[0]).toEqual({
      name: 'Kuaför Elif Kadıköy',
      industryCode: 'hair_salon',
      shortDescription: null,
      phone: '0216 345 12 12',
      email: 'merhaba@ornek.com',
      instagram: '@kuaforelif',
      website: null,
      legalName: null,
      taxOffice: null,
      taxNumber: null,
    });
  });

  it('kısa ad ve hatalı e-posta gönderilmez', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/ayarlar/isletme', { calls });
    const name = await screen.findByRole('textbox', { name: 'İşletme adı' });
    await userEvent.clear(name);
    await userEvent.type(name, 'K');
    await userEvent.type(screen.getByRole('textbox', { name: 'E-posta' }), 'merhaba');
    await save();
    expect(await screen.findByText('İşletme adı en az 2 karakter olmalı.')).toBeTruthy();
    expect(screen.getByText('E-posta adresini kontrol edin: ad@alan.com')).toBeTruthy();
    expect(calls['/api/v1/manage/brand']).toBeUndefined();
  });

  it('sunucu telefonu reddederse alanın altında yazar', async () => {
    renderApp('/ayarlar/isletme', { saveError: 'validation_failed:phone' });
    await screen.findByRole('textbox', { name: 'İşletme adı' });
    await save();
    expect(await screen.findByText('Telefon numarasını kontrol edin: 0216 345 12 12')).toBeTruthy();
  });

  it('412: "Kayıt çakışması" şeridi, Yenile ile kalkar', async () => {
    renderApp('/ayarlar/isletme', { saveError: 'concurrency_conflict' });
    await screen.findByRole('textbox', { name: 'İşletme adı' });
    await save();
    expect(await screen.findByText('Kayıt çakışması')).toBeTruthy();
    await userEvent.click(screen.getByRole('button', { name: 'Yenile' }));
    expect(screen.queryByText('Kayıt çakışması')).toBeNull();
  });

  it('ayar izni olmayan personel 403 görür', async () => {
    renderApp('/ayarlar/isletme', {
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

describe('Randevu kuralları', () => {
  it('onay kapalıyken yanıt süresi pasif; açıp kaydedince ekranda olmayan alanlar korunur', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/ayarlar/randevu-kurallari', { calls, depositAvailable: true });
    const timeout = await screen.findByRole<HTMLSelectElement>('combobox', {
      name: 'Yanıt süresi',
    });
    expect(timeout.disabled).toBe(true);
    expect(timeout.selectedOptions[0]?.textContent).toBe('2 saat (önerilen)');
    await userEvent.click(screen.getByRole('switch', { name: 'Randevular onayımla kesinleşsin' }));
    expect(timeout.disabled).toBe(false);
    await userEvent.selectOptions(timeout, '240');
    await userEvent.selectOptions(
      screen.getByRole('combobox', { name: 'Ücretsiz iptal süresi' }),
      '48',
    );
    await save();
    expect(await screen.findByText('Değişiklikler kaydedildi')).toBeTruthy();
    expect(calls['PUT /api/v1/manage/brand/settings If-Match']).toEqual(['3']);
    const body = calls['/api/v1/manage/brand/settings']?.[0] as Record<string, unknown>;
    expect(body).toMatchObject({
      requiresApproval: true,
      approvalTimeoutMinutes: 240,
      cancellationCutoffHours: 48,
      minNoticeMinutes: 60,
      reminderOffsetsMinutes: [1440, 120],
      depositPolicy: 'RiskyCustomersOnly',
    });
    expect(body).not.toHaveProperty('depositAvailable');
    expect(body).not.toHaveProperty('version');
  });

  it('IBAN yoksa kapora seçenekleri pasif ve nedeni yazar; kayıtlı değer gönderilir', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/ayarlar/randevu-kurallari', { calls });
    expect(await screen.findByText('Kapora IBAN’ı tanımlı değil.')).toBeTruthy();
    expect(
      screen.getByRole<HTMLSelectElement>('combobox', { name: 'Kapora istenecek durumlar' })
        .disabled,
    ).toBe(true);
    await save();
    await screen.findByText('Değişiklikler kaydedildi');
    expect(calls['/api/v1/manage/brand/settings']?.[0]).toMatchObject({
      depositPolicy: 'RiskyCustomersOnly',
      riskyCustomerNoShowThreshold: 2,
      approvalTimeoutMinutes: 120,
    });
  });
});

describe('Rezervasyon sayfası', () => {
  it('bağlantı adı, kapak rengi ve fiyat; kaydedince yeni bağlantı üstte', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/ayarlar/rezervasyon', { calls });
    const slug = await screen.findByRole('textbox', { name: 'Bağlantı adı' });
    expect(screen.getAllByText('omvila.com/kuafor-elif').length).toBeGreaterThan(0);
    expect(screen.getByRole<HTMLInputElement>('radio', { name: 'Petrol' }).checked).toBe(true);
    await userEvent.clear(slug);
    await userEvent.type(slug, 'elif-kadikoy');
    await userEvent.click(screen.getByRole('radio', { name: 'Mercan' }));
    await userEvent.click(screen.getByRole('switch', { name: 'Fiyatları göster' }));
    await save();
    expect(await screen.findByText('Değişiklikler kaydedildi')).toBeTruthy();
    expect(calls['/api/v1/manage/brand/booking-page']?.[0]).toEqual({
      slug: 'elif-kadikoy',
      coverColor: 'mercan',
      showReviews: true,
      showPrices: false,
      preAppointmentNote: null,
    });
    expect((await screen.findAllByText('omvila.com/elif-kadikoy')).length).toBeGreaterThan(0);
  });

  it('geçersiz bağlantı adı gönderilmez; alınmış ad alanın altında yazar', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/ayarlar/rezervasyon', { calls, saveError: 'slug_unavailable' });
    const slug = await screen.findByRole('textbox', { name: 'Bağlantı adı' });
    await userEvent.clear(slug);
    await userEvent.type(slug, 'Elif K');
    await save();
    expect(
      await screen.findByText('Küçük harf, rakam ve tire kullanın; 3–60 karakter.'),
    ).toBeTruthy();
    expect(calls['/api/v1/manage/brand/booking-page']).toBeUndefined();
    await userEvent.clear(slug);
    await userEvent.type(slug, 'panel');
    await save();
    expect(await screen.findByText('Bu bağlantı adı kullanılamıyor.')).toBeTruthy();
  });
});
