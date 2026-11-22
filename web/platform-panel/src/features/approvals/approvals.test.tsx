import { cleanup, screen, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { sessionFixture } from '@/test/fake-api';
import type { FakeApiOptions } from '@/test/fake-api';
import { noticeIds, senderId, txIds } from '@/test/fake-platform';
import type { PlatformFakeOptions } from '@/test/fake-platform';
import { renderApp } from '@/test/render-app';

// Kural: platform-paneli/POnay, POnayBos, POnayDetay (KK2, KK3): bekleyenler en eskiden yeniye; "Kod tutuyor" adayı
// listeden eşleştirilir (If-Match); kod/tutar uyuşmazsa "Kontrol edin"; "Bulunamadı, markaya yaz" ve notlu ret;
// SMS gönderen adı onay/ret; 409 bank_transaction_already_matched ve 412 ayrı yazılır; Finans SMS sekmesini görmez.
const downloads: string[] = [];
beforeEach(() => {
  downloads.length = 0;
  URL.createObjectURL = () => 'blob:test';
  URL.revokeObjectURL = () => undefined;
  vi.spyOn(HTMLAnchorElement.prototype, 'click').mockImplementation(function (
    this: HTMLAnchorElement,
  ) {
    downloads.push(this.download);
  });
});
afterEach(() => {
  cleanup();
  vi.restoreAllMocks();
});

function open(path: string, platform: PlatformFakeOptions = {}, options: FakeApiOptions = {}) {
  const calls: Record<string, unknown[]> = {};
  const result = renderApp(path, { calls, platform, ...options });
  return { ...result, calls };
}

describe('onay kuyrukları', () => {
  it('bekleyenler, sekme sayıları, "Kod tutuyor" rozeti ve "Kontrol edin" uyarısı', async () => {
    open('/onay');
    const list = await screen.findByRole('list', { name: 'Bekleyen kayıtlar' });
    expect(screen.getByText('3 bekleyen · en eskisi 2 gün')).toBeTruthy();
    expect(screen.getByRole('tab', { name: /Havale bildirimi/ }).textContent).toContain('2');
    expect(screen.getByRole('tab', { name: /SMS başlığı/ }).textContent).toContain('1');
    const [elif = list, kaan = list] = within(list).getAllByRole('listitem');
    expect(within(elif).getByText('· Abonelik · Kasım · havale bildirildi')).toBeTruthy();
    expect(within(elif).getByText(/tutar ve açıklama tutuyor · dekont yüklendi/)).toBeTruthy();
    expect(within(elif).getByText('Bankada eşleşen hareket var')).toBeTruthy();
    expect(within(kaan).getByText(/gönderen “Kaan Ö.”/)).toBeTruthy();
    expect(within(kaan).getByText('Kontrol edin:')).toBeTruthy();
    expect(
      within(kaan).getByText('açıklama kodu yok; tutar ve gönderen adıyla eşleştirin.'),
    ).toBeTruthy();
    expect(within(kaan).queryByRole('button', { name: /eşleştir/ })).toBeNull();
    expect(screen.queryByText('NAILSTUDIO', { exact: false })).toBeNull();
  });

  it('boş kuyruk (POnayBos)', async () => {
    open('/onay', { emptyQueue: true });
    expect(await screen.findByRole('heading', { name: 'Bekleyen onay yok' })).toBeTruthy();
    expect(screen.getByText('Bekleyen yok')).toBeTruthy();
    expect(screen.getByRole('button', { name: 'Denetim kaydı' })).toBeTruthy();
  });

  it('"Kod tutuyor" adayı listeden eşleştirilir (If-Match)', async () => {
    const { calls } = open('/onay');
    await userEvent.click(
      await screen.findByRole('button', { name: 'Kuaför Elif havalesini eşleştir' }),
    );
    expect(await screen.findByText('Kuaför Elif markasına bildirim gitti.')).toBeTruthy();
    expect(calls[`/api/v1/admin/payment-notices/${noticeIds.elif}/match`]?.[0]).toEqual({
      bankTransactionId: txIds.elif,
    });
    expect(calls[`POST /api/v1/admin/payment-notices/${noticeIds.elif}/match If-Match`]?.[0]).toBe(
      'W/"7001"',
    );
    // Kuyruk tazelenir; eşleşen bildirim düşer.
    expect(await screen.findByText('2 bekleyen · en eskisi 1 gün')).toBeTruthy();
  });

  it('hareket başka bildirime bağlıysa (409) nedenini yazar', async () => {
    open('/onay', { matchError: 'bank_transaction_already_matched' });
    await userEvent.click(
      await screen.findByRole('button', { name: 'Kuaför Elif havalesini eşleştir' }),
    );
    expect(await screen.findByText('Hareket başka bildirime bağlı.')).toBeTruthy();
    expect(
      screen.getByText('Bu banka hareketi başka bir bildirimle eşleştirildi. Sayfayı yenileyin.'),
    ).toBeTruthy();
  });

  it('SMS gönderen adı onaylanır (If-Match)', async () => {
    const { calls } = open('/onay?sekme=sms');
    expect(await screen.findByText(/NAILSTUDIO · şu an OMVILA adıyla gidiyor/)).toBeTruthy();
    await userEvent.click(screen.getByRole('button', { name: 'NAILSTUDIO adını onayla' }));
    expect(
      await screen.findByText("Nail Studio Moda SMS'leri NAILSTUDIO adıyla gidecek."),
    ).toBeTruthy();
    expect(calls[`POST /api/v1/admin/sms-senders/${senderId}/approve If-Match`]?.[0]).toBe(
      'W/"7002"',
    );
  });

  it('SMS gönderen adı nedenle reddedilir', async () => {
    const { calls } = open('/onay?sekme=sms');
    await userEvent.click(await screen.findByRole('button', { name: 'NAILSTUDIO adını reddet' }));
    const dialog = await screen.findByRole('dialog', { name: 'SMS gönderen adını reddet' });
    await userEvent.click(within(dialog).getByRole('button', { name: 'Adı reddet' }));
    expect(await within(dialog).findByText('Nedeni en az 3 harfle yazın.')).toBeTruthy();
    await userEvent.type(within(dialog).getByLabelText('Neden'), 'Başlık unvanla uyuşmuyor.');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Adı reddet' }));
    expect(await screen.findByText('SMS gönderen adı reddedildi')).toBeTruthy();
    expect(calls[`/api/v1/admin/sms-senders/${senderId}/reject`]?.[0]).toEqual({
      reason: 'Başlık unvanla uyuşmuyor.',
    });
  });

  it('Finans rolü SMS başlığı sekmesini görmez', async () => {
    open('/onay', {}, { session: sessionFixture('PlatformFinance') });
    await screen.findByRole('list', { name: 'Bekleyen kayıtlar' });
    expect(screen.queryByRole('tab', { name: /SMS başlığı/ })).toBeNull();
  });

  it('uç 403 dönerse yetki sayfası', async () => {
    open('/onay', {}, { failures: { '/api/v1/admin/approval-queue': 403 } });
    expect(
      await screen.findByRole('heading', { level: 1, name: 'Bu sayfayı görme yetkiniz yok' }),
    ).toBeTruthy();
  });
});

describe('havale bildirimi eşleştirme (POnayDetay)', () => {
  const detail = `/onay/havale/${noticeIds.elif}`;

  it('bildirim, dekont ve adaylar; "Kod tutuyor" aday seçili gelir', async () => {
    open(detail);
    expect(
      await screen.findByRole('heading', { level: 1, name: 'Kuaför Elif · havale bildirimi' }),
    ).toBeTruthy();
    expect(screen.getByText('Abonelik · Kasım 2026 · 2 gündür bekliyor')).toBeTruthy();
    expect(screen.getByText('Salon planı · Kasım 2026')).toBeTruthy();
    expect(screen.getByText('18 Kas 14:20 · panelden')).toBeTruthy();
    expect(screen.getByText('11–21 Kasım')).toBeTruthy();
    const radios = screen.getAllByRole('radio');
    expect(radios).toHaveLength(3);
    expect(screen.getByRole<HTMLInputElement>('radio', { name: /ELİF KAYA/ }).checked).toBe(true);
    expect(screen.getByText('18 Kas 14:05 · açıklama: ABN-KE-1125')).toBeTruthy();
    expect(screen.getByText('Kod tutuyor')).toBeTruthy();
    expect(screen.queryByText('Kontrol edin:')).toBeNull();
    expect(
      screen.getByText(
        'Eşleştirince Kasım dönemi ödendi sayılır, fatura kesilir ve markaya bildirim gider. Eşleştirme denetim kaydına yazılır.',
      ),
    ).toBeTruthy();
  });

  it('uyuşmayan aday seçilince "Kontrol edin"; seçili hareketle eşleştirir ve kuyruğa döner', async () => {
    const { calls, router } = open(detail);
    await userEvent.click(await screen.findByRole('radio', { name: /SPA MODA/ }));
    expect(screen.getByText('Kontrol edin:')).toBeTruthy();
    expect(
      screen.getByText(
        'açıklama kodu yok ve tutar farklı; havalenin bu bildirime ait olduğundan emin olun.',
      ),
    ).toBeTruthy();
    await userEvent.click(screen.getByRole('button', { name: 'Seçili hareketle eşleştir' }));
    expect(await screen.findByText('Havale eşleştirildi')).toBeTruthy();
    expect(calls[`/api/v1/admin/payment-notices/${noticeIds.elif}/match`]?.[0]).toEqual({
      bankTransactionId: txIds.spa,
    });
    expect(router.state.location.pathname).toBe('/onay');
  });

  it('412 çakışmasında yenileme önerir', async () => {
    open(detail, { matchError: 'concurrency_conflict' });
    await userEvent.click(await screen.findByRole('button', { name: 'Seçili hareketle eşleştir' }));
    expect(await screen.findByText('Kayıt çakışması.')).toBeTruthy();
    expect(screen.getByRole('button', { name: 'Yenile' })).toBeTruthy();
  });

  it('"Bulunamadı, markaya yaz" isteğe bağlı notla NotFound yapar', async () => {
    const { calls } = open(detail);
    await userEvent.click(await screen.findByRole('button', { name: 'Bulunamadı, markaya yaz' }));
    const dialog = await screen.findByRole('dialog', { name: 'Bulunamadı, markaya yaz' });
    await userEvent.type(within(dialog).getByLabelText('Markaya not'), 'Havale görünmüyor.');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Markaya yaz' }));
    expect(await screen.findByText('Kuaför Elif havalenin bulunamadığını görecek.')).toBeTruthy();
    expect(calls[`/api/v1/admin/payment-notices/${noticeIds.elif}/not-found`]?.[0]).toEqual({
      message: 'Havale görünmüyor.',
    });
    expect(await screen.findByText('Bulunamadı olarak markaya yazıldı')).toBeTruthy();
  });

  it('ret notu zorunlu', async () => {
    const { calls } = open(detail);
    await userEvent.click(await screen.findByRole('button', { name: 'Reddet' }));
    const dialog = await screen.findByRole('dialog', { name: 'Havale bildirimini reddet' });
    await userEvent.click(within(dialog).getByRole('button', { name: 'Bildirimi reddet' }));
    expect(await within(dialog).findByText('Reddetme nedenini yazın.')).toBeTruthy();
    await userEvent.type(within(dialog).getByLabelText('Markaya not'), 'Yinelenen bildirim.');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Bildirimi reddet' }));
    expect(await screen.findByText('Bildirim reddedildi')).toBeTruthy();
    expect(calls[`/api/v1/admin/payment-notices/${noticeIds.elif}/reject`]?.[0]).toEqual({
      message: 'Yinelenen bildirim.',
    });
  });

  it('dekont indirilir', async () => {
    open(detail);
    await userEvent.click(await screen.findByRole('button', { name: 'Dekontu aç' }));
    await vi.waitFor(() => {
      expect(downloads).toEqual(['dekont-ziraat.pdf']);
    });
  });

  it('dekontsuz ve adayı uyuşmayan bildirim', async () => {
    open(`/onay/havale/${noticeIds.kaan}`);
    expect(await screen.findByText('Marka dekont yüklemedi.')).toBeTruthy();
    // "Kod tutuyor" aday yok: hiçbiri seçili değil, eşleştir pasif.
    expect(
      screen.getByRole<HTMLButtonElement>('button', { name: 'Seçili hareketle eşleştir' }).disabled,
    ).toBe(true);
  });
});
