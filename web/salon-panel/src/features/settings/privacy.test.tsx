import { cleanup, screen, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it } from 'vitest';
import { ownerPermissions, sessionFixture } from '../../test/fake-api';
import { renderApp } from '../../test/render-app';

// Kural: dilim-06 kapsam (Ayarlar → KVKK ve veri); sözleşme manageGetPrivacySettings / manageUpdatePrivacySettings
// (Custom'da metin zorunlu, saklama 3/5/10 yıl, If-Match), manageRequestDataExport (202 Queued, maskeli adres,
// Idempotency-Key); roller-yetkiler.md (settings.manage, customer.export yalnız Owner); salon-paneli/AyarlarKvkk.
afterEach(cleanup);

const path = '/api/v1/manage/brand/privacy';
const save = () => userEvent.click(screen.getByRole('button', { name: 'Değişiklikleri kaydet' }));

describe('KVKK ve veri: aydınlatma metni, veri saklama, dışa aktarma', () => {
  it('standart metin, önizleme ve saklama süresi kaydı', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/ayarlar/kvkk', { calls });
    expect(await screen.findByText('OMVILA standart aydınlatma metni')).toBeTruthy();
    expect(screen.getByText(/^Son güncelleme 1 Kasım 2026 · /)).toBeTruthy();
    await userEvent.click(screen.getByRole('button', { name: 'Önizle' }));
    const preview = await screen.findByRole('dialog', { name: 'OMVILA standart aydınlatma metni' });
    expect(within(preview).getByText(/^Kuaför Elif olarak kişisel verilerinizi/)).toBeTruthy();
    await userEvent.keyboard('{Escape}');
    const retention = screen.getByRole<HTMLSelectElement>('combobox', {
      name: 'İşlem kayıtlarını saklama süresi',
    });
    expect(retention.value).toBe('3');
    await userEvent.selectOptions(retention, '5');
    await save();
    expect(await screen.findByText('KVKK ayarları kaydedildi')).toBeTruthy();
    expect(calls[`PUT ${path} If-Match`]).toEqual(['W/"4410"']);
    expect(calls[path]?.[0]).toEqual({ noticeMode: 'Standard', transactionRetentionYears: 5 });
  });

  it('"Kendi metnimi kullan": boş metin kaydedilmez, yazılan metin gönderilir', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/ayarlar/kvkk', { calls });
    await userEvent.click(await screen.findByRole('button', { name: 'Kendi metnimi kullan' }));
    expect(screen.getByText('Kendi aydınlatma metniniz')).toBeTruthy();
    await save();
    expect(
      await screen.findByText('Aydınlatma metninizi yazın veya standart metni kullanın.'),
    ).toBeTruthy();
    expect(calls[path]).toBeUndefined();
    await userEvent.type(
      screen.getByRole('textbox', { name: 'Aydınlatma metniniz' }),
      'Salonumuzun metni.',
    );
    await save();
    expect(await screen.findByText('KVKK ayarları kaydedildi')).toBeTruthy();
    expect(calls[path]?.[0]).toEqual({
      noticeMode: 'Custom',
      customNoticeText: 'Salonumuzun metni.',
      transactionRetentionYears: 3,
    });
    expect(screen.getByRole('button', { name: 'Standart metni kullan' })).toBeTruthy();
  });

  it('dışa aktarma isteği: e-posta sorulur, durum ve maskeli adres satırda', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/ayarlar/kvkk', { calls });
    await userEvent.click(await screen.findByRole('button', { name: 'Dışa aktarma iste' }));
    const dialog = await screen.findByRole('dialog', { name: 'Tüm verileri dışa aktar' });
    await userEvent.type(within(dialog).getByRole('textbox', { name: 'E-posta' }), 'elif@');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Dışa aktarma iste' }));
    expect(
      await within(dialog).findByText('Geçerli bir e-posta adresi girin: ornek@alanadi.com'),
    ).toBeTruthy();
    await userEvent.type(within(dialog).getByRole('textbox', { name: 'E-posta' }), 'ornek.com');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Dışa aktarma iste' }));
    expect(await screen.findByText('Dışa aktarma isteğiniz alındı')).toBeTruthy();
    expect(screen.getByText('Hazırlanıyor')).toBeTruthy();
    expect(screen.getByText(/el\*\*\*@ornek\.com adresine gönderilecek/)).toBeTruthy();
    const exportPath = '/api/v1/manage/brand/data-exports';
    expect(calls[exportPath]).toEqual([{ email: 'elif@ornek.com' }]);
    expect(calls[`${exportPath} Idempotency-Key`]).toHaveLength(1);
    // KVKK formu dışa aktarma penceresinden gönderilmez.
    expect(calls[path]).toBeUndefined();
  });

  it('customer.export yoksa dışa aktarma satırı görünmez; aydınlatma metni ve saklama kalır', async () => {
    renderApp('/ayarlar/kvkk', {
      session: sessionFixture({
        permissions: ownerPermissions.filter((p) => p !== 'customer.export'),
      }),
    });
    expect(await screen.findByText('OMVILA standart aydınlatma metni')).toBeTruthy();
    expect(screen.getByRole('heading', { name: 'Silme talepleri' })).toBeTruthy();
    expect(screen.queryByRole('button', { name: 'Dışa aktarma iste' })).toBeNull();
  });

  it('ayarlar yüklenemezse yeniden deneme', async () => {
    renderApp('/ayarlar/kvkk', { access: { privacyFails: true } });
    expect(await screen.findByRole('heading', { name: 'KVKK ayarları yüklenemedi' })).toBeTruthy();
    expect(screen.getByRole('button', { name: 'Tekrar dene' })).toBeTruthy();
  });
});
