import { cleanup, screen, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it } from 'vitest';
import { sessionFixture, staffPermissions } from '../../test/fake-api';
import { renderApp } from '../../test/render-app';

// Kural: dilim-03 kapsamı (Ayarlar → Bildirimler ve SMS: kontör özeti, hatırlatma zamanları, değerlendirme daveti,
// şablon ezmeleri, gönderen adı başvurusu; kriter 11), salon-paneli/AyarlarBildirim; sözleşme manageGet/Update
// NotificationSettings (If-Match, 412, templates[i].customBody), manageGetSmsOverview, manageRequestSmsSender;
// roller-yetkiler.md (settings.manage).
afterEach(cleanup);

const save = () => userEvent.click(screen.getByRole('button', { name: 'Değişiklikleri kaydet' }));
const open = async (options: Parameters<typeof renderApp>[1] = {}) => {
  const result = renderApp('/ayarlar/bildirimler', options);
  await screen.findByRole('heading', { name: 'Hatırlatmalar' });
  return result;
};

describe('Bildirimler ve SMS', () => {
  it('menüde seçili; kontör, bu ay gönderilen ve gönderen adı sunucudan', async () => {
    await open();
    const nav = screen.getByRole('navigation', { name: 'Ayar bölümleri' });
    expect(
      within(nav).getByRole('link', { name: 'Bildirimler ve SMS' }).getAttribute('aria-current'),
    ).toBe('page');
    const credits = screen.getByRole('region', { name: 'SMS kontörü' });
    expect(within(credits).getByText('1.240')).toBeTruthy();
    expect(within(credits).getByText('bu tempoyla ~18 gün yeter')).toBeTruthy();
    const sent = screen.getByRole('region', { name: 'Bu ay gönderilen' });
    expect(within(sent).getByText('862')).toBeTruthy();
    expect(within(sent).getByText('SMS · 1.904 bildirim')).toBeTruthy();
    const sender = screen.getByRole('region', { name: 'Gönderen adı' });
    expect(within(sender).getByText('KUAFORELIF')).toBeTruthy();
    expect(within(sender).getByText('operatör onaylı')).toBeTruthy();
    expect(screen.getByText(/Bakiye 200 kontörün altına/)).toBeTruthy();
    // Kontör satın alma ve kart formu yok (ürün kuralı 1).
    expect(screen.queryByRole('button', { name: /Kontör al/ })).toBeNull();
  });

  it('tahmin yoksa alt satır yazılmaz (sahte sayı yok)', async () => {
    await open({ notificationSettings: { noEstimate: true } });
    const credits = screen.getByRole('region', { name: 'SMS kontörü' });
    expect(within(credits).queryByText(/gün yeter/)).toBeNull();
  });

  it('hatırlatma kaldır, ekle, daveti kapat: If-Match ile tam gövde; şablon gönderilmez', async () => {
    const calls: Record<string, unknown[]> = {};
    await open({ calls });
    const chips = screen.getByRole('list', { name: 'Randevu hatırlatma zamanları' });
    expect(within(chips).getByText('1 gün önce')).toBeTruthy();
    expect(within(chips).getByText('2 saat önce')).toBeTruthy();
    await userEvent.click(
      screen.getByRole('button', { name: '2 saat önce hatırlatmasını kaldır' }),
    );
    await userEvent.click(screen.getByRole('button', { name: 'Ekle' }));
    await userEvent.selectOptions(
      screen.getByRole('combobox', { name: 'Hatırlatma zamanı' }),
      '180',
    );
    expect(within(chips).getByText('3 saat önce')).toBeTruthy();
    await userEvent.click(screen.getByRole('switch', { name: 'Değerlendirme daveti' }));
    await save();
    expect(await screen.findByText('Değişiklikler kaydedildi')).toBeTruthy();
    expect(calls['PUT /api/v1/manage/brand/notification-settings If-Match']).toEqual(['W/"9120"']);
    expect(calls['/api/v1/manage/brand/notification-settings']?.[0]).toEqual({
      reminderOffsetsMinutes: [1440, 180],
      reviewInviteEnabled: false,
    });
  });

  it('en fazla üç hatırlatma: üçüncüden sonra "Ekle" yok', async () => {
    await open();
    await userEvent.click(screen.getByRole('button', { name: 'Ekle' }));
    await userEvent.selectOptions(
      screen.getByRole('combobox', { name: 'Hatırlatma zamanı' }),
      '60',
    );
    expect(screen.queryByRole('button', { name: 'Ekle' })).toBeNull();
  });

  it('şablon düzenle: zorunlu değişken denetlenir; değişen şablon gönderilir', async () => {
    const calls: Record<string, unknown[]> = {};
    await open({ calls });
    const reminder = screen.getByRole('button', { name: 'Hatırlatma şablonunu düzenle' });
    expect(screen.getAllByText('Sistem şablonu').length).toBeGreaterThan(0);
    await userEvent.click(reminder);
    const dialog = await screen.findByRole('dialog', { name: 'Hatırlatma' });
    const area = within(dialog).getByRole('textbox', { name: 'Salon metni' });
    await userEvent.clear(area);
    await userEvent.type(area, 'Yarın {{saat} randevunuz var.');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Metni kullan' }));
    expect(within(dialog).getByText('{bağlantı} değişkeni metinde olmalı.')).toBeTruthy();
    await userEvent.type(area, ' Değişiklik: {{bağlantı}');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Metni kullan' }));
    expect(screen.queryByRole('dialog')).toBeNull();
    expect(screen.getAllByText('Sizin metniniz')).toHaveLength(2);
    await save();
    expect(await screen.findByText('Değişiklikler kaydedildi')).toBeTruthy();
    expect(calls['/api/v1/manage/brand/notification-settings']?.[0]).toEqual({
      reminderOffsetsMinutes: [1440, 120],
      reviewInviteEnabled: true,
      templates: [
        {
          eventCode: 'appointment.reminder',
          variant: null,
          customBody: 'Yarın {saat} randevunuz var. Değişiklik: {bağlantı}',
        },
      ],
    });
  });

  it('sistem şablonuna dön: customBody null gönderilir', async () => {
    const calls: Record<string, unknown[]> = {};
    await open({ calls });
    await userEvent.click(
      screen.getByRole('button', { name: 'Randevu onaylandı şablonunu düzenle' }),
    );
    const dialog = await screen.findByRole('dialog', { name: 'Randevu onaylandı' });
    await userEvent.click(within(dialog).getByRole('button', { name: 'Sistem şablonuna dön' }));
    await save();
    await screen.findByText('Değişiklikler kaydedildi');
    expect(calls['/api/v1/manage/brand/notification-settings']?.[0]).toMatchObject({
      templates: [{ eventCode: 'appointment.confirmed', variant: null, customBody: null }],
    });
  });

  it('sunucu metni uzun bulursa satırın altında yazar', async () => {
    await open({ notificationSettings: { saveError: 'template_too_long' } });
    await userEvent.click(screen.getByRole('button', { name: 'Hatırlatma şablonunu düzenle' }));
    const dialog = await screen.findByRole('dialog', { name: 'Hatırlatma' });
    await userEvent.type(
      within(dialog).getByRole('textbox', { name: 'Salon metni' }),
      ' Teşekkürler',
    );
    await userEvent.click(within(dialog).getByRole('button', { name: 'Metni kullan' }));
    await save();
    expect(
      await screen.findByText("Metin en uzun değerlerle 1 SMS'i (155 karakter) aşıyor. Kısaltın."),
    ).toBeTruthy();
  });

  it('412: "Kayıt çakışması" şeridi, Yenile ile kalkar ve form sunucu değerine döner', async () => {
    await open({ notificationSettings: { saveError: 'concurrency_conflict' } });
    await userEvent.click(
      screen.getByRole('button', { name: '2 saat önce hatırlatmasını kaldır' }),
    );
    await save();
    expect(await screen.findByText('Kayıt çakışması')).toBeTruthy();
    await userEvent.click(screen.getByRole('button', { name: 'Yenile' }));
    expect(screen.queryByText('Kayıt çakışması')).toBeNull();
    expect(await screen.findByText('2 saat önce')).toBeTruthy();
  });

  it('gönderen adı başvurusu: giriş büyük Latin harfe çevrilir, durum "onay bekleniyor" olur', async () => {
    const calls: Record<string, unknown[]> = {};
    await open({ calls, notificationSettings: { senderStatus: 'None' } });
    const sender = screen.getByRole('region', { name: 'Gönderen adı' });
    expect(within(sender).getByText('OMVILA')).toBeTruthy();
    await userEvent.click(within(sender).getByRole('button', { name: 'Başvur' }));
    const dialog = await screen.findByRole('dialog', { name: 'Gönderen adı başvurusu' });
    await userEvent.type(
      within(dialog).getByRole('textbox', { name: 'Gönderen adı' }),
      'Kuaför Elif',
    );
    await userEvent.click(within(dialog).getByRole('button', { name: 'Başvur' }));
    expect(await screen.findByText('Başvurunuz alındı')).toBeTruthy();
    expect(calls['/api/v1/manage/brand/sms-sender']?.[0]).toEqual({ name: 'KUAFORELIF' });
    expect(calls['/api/v1/manage/brand/sms-sender Idempotency-Key']).toHaveLength(1);
    expect(within(sender).getByText('KUAFORELIF için operatör onayı bekleniyor')).toBeTruthy();
    expect(within(sender).getByText('OMVILA')).toBeTruthy();
    expect(within(sender).queryByRole('button')).toBeNull();
  });

  it('kısa gönderen adı gönderilmez', async () => {
    const calls: Record<string, unknown[]> = {};
    await open({ calls, notificationSettings: { senderStatus: 'Rejected' } });
    const sender = screen.getByRole('region', { name: 'Gönderen adı' });
    expect(within(sender).getByText('Ad, işletme unvanıyla eşleşmiyor.')).toBeTruthy();
    await userEvent.click(within(sender).getByRole('button', { name: 'Başvur' }));
    const dialog = await screen.findByRole('dialog', { name: 'Gönderen adı başvurusu' });
    await userEvent.type(within(dialog).getByRole('textbox', { name: 'Gönderen adı' }), 'ab');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Başvur' }));
    expect(
      await within(dialog).findByText(
        '3–11 karakter; harf ve rakam kullanın, en az bir harf olsun.',
      ),
    ).toBeTruthy();
    expect(calls['/api/v1/manage/brand/sms-sender']).toBeUndefined();
  });

  it('ayar izni olmayan personel 403 görür', async () => {
    renderApp('/ayarlar/bildirimler', {
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
