import { cleanup, screen, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it } from 'vitest';
import { brandId, sessionFixture } from '@/test/fake-api';
import { renderApp } from '@/test/render-app';

// Kural: platform-paneli/PMarkalar ve PMarkaDetay (KK4, KK5): süzgeç, arama, imleç; Özet; "Bu görüntüleme denetim
// kaydına yazıldı"; telefon maskeli; Destek rolünde kontör ve plan butonları pasif ("Yönetici yetkisi ister");
// Yönetici kontör ekler ve planı If-Match ile değiştirir (plan_limit_exceeded, 412).
afterEach(cleanup);

const admin = { session: sessionFixture('PlatformAdmin') };

describe('markalar', () => {
  it('tablo, sayılar ve durum rozetleri', async () => {
    renderApp('/markalar');
    const table = await screen.findByRole('table', { name: 'Markalar' });
    expect(screen.getByText('412 aktif · 64 denemede')).toBeTruthy();
    expect(within(table).getByText('Deneme · 9 gün')).toBeTruthy();
    expect(within(table).getByText('Ödeme gecikti')).toBeTruthy();
    expect(within(table).getByText('Salt okunur')).toBeTruthy();
    expect(within(table).getByText('Berber · Ankara')).toBeTruthy();
    expect(screen.queryByRole('button', { name: 'Dışa aktar' })).toBeNull();
  });

  it('süzgeç çipi sözleşmedeki filtreyi gönderir', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/markalar', { calls });
    await screen.findByRole('table', { name: 'Markalar' });
    await userEvent.click(screen.getByRole('radio', { name: 'Kapora açık' }));
    const table = await screen.findByRole('table', { name: 'Markalar' });
    expect(await within(table).findByText('Kuaför Elif')).toBeTruthy();
    expect(within(table).queryByText('Berber Kaan')).toBeNull();
    expect(calls['/api/v1/admin/brands']?.at(-1)).toEqual({
      filter: 'DepositEnabled',
      limit: '25',
    });
  });

  it('arama sonucu yoksa boş hâl', async () => {
    renderApp('/markalar?q=yok');
    expect(await screen.findByRole('heading', { name: 'Aramaya uyan marka yok' })).toBeTruthy();
  });

  it('imleçle sayfalama: Daha fazla göster', async () => {
    renderApp('/markalar', { paged: true });
    const table = await screen.findByRole('table', { name: 'Markalar' });
    expect(within(table).queryByText('Saç Atölyesi')).toBeNull();
    await userEvent.click(screen.getByRole('button', { name: 'Daha fazla göster' }));
    expect(await within(table).findByText('Saç Atölyesi')).toBeTruthy();
    expect(screen.queryByRole('button', { name: 'Daha fazla göster' })).toBeNull();
  });
});

describe('marka detayı', () => {
  it('özet kartları, marka bilgileri, kontör hareketleri ve denetim notu (Destek)', async () => {
    renderApp(`/markalar/${brandId}`);
    expect(await screen.findByRole('heading', { level: 1, name: 'Kuaför Elif' })).toBeTruthy();
    expect(screen.getByText('Kuaför · İstanbul · 2 şube · kayıt 11 Kasım')).toBeTruthy();
    const summary = await screen.findByRole('group', { name: 'Marka özeti' });
    expect(within(summary).getByText('Salon planı · 9 gün kaldı')).toBeTruthy();
    expect(within(summary).getByText('SMS · tahmini 26 gün yeter')).toBeTruthy();
    expect(within(summary).getByText('18 Kasım bildirildi · eşleştirme bekliyor')).toBeTruthy();

    const facts = screen.getByRole('region', { name: 'Marka bilgileri' });
    expect(within(facts).getByText('Elif Kaya · 0532 *** ** 05')).toBeTruthy();
    expect(within(facts).getByText('omvila.com/kuafor-elif')).toBeTruthy();
    expect(within(facts).getByText('3 personelde bağlı (tek yönlü)')).toBeTruthy();
    expect(within(facts).getByText('OMVILA (varsayılan) · başvuru yok')).toBeTruthy();

    const credits = screen.getByRole('table', { name: 'Kontör hareketleri' });
    expect(within(credits).getByText('Paket satın alındı · 1.000 SMS')).toBeTruthy();
    expect(within(credits).getByText('+1.000')).toBeTruthy();
    expect(within(credits).getByText('−46')).toBeTruthy();

    expect(screen.getByText('Bu görüntüleme denetim kaydına yazıldı.')).toBeTruthy();
    expect(screen.getByText(/yönetici yetkisi ister/)).toBeTruthy();
  });

  it('Destek rolünde kontör ve plan butonları pasif, nedeni yazar', async () => {
    renderApp(`/markalar/${brandId}`);
    const grant = await screen.findByRole('button', { name: 'Kontör ekle' });
    const plan = screen.getByRole('button', { name: 'Planı değiştir' });
    for (const button of [grant, plan]) {
      expect((button as HTMLButtonElement).disabled).toBe(true);
      const describedBy = button.getAttribute('aria-describedby') ?? '';
      expect(document.getElementById(describedBy)?.textContent).toBe('Yönetici yetkisi ister');
    }
  });

  it('sekmeler: Kontör defteri ve boş fatura listesi', async () => {
    const { router } = renderApp(`/markalar/${brandId}`, admin);
    await userEvent.click(await screen.findByRole('tab', { name: 'Kontör' }));
    expect(router.state.location.search).toEqual({ sekme: 'kontor' });
    const panel = await screen.findByRole('tabpanel');
    expect(await within(panel).findByText('Deneme hediyesi')).toBeTruthy();
    await userEvent.click(screen.getByRole('tab', { name: 'Abonelik ve fatura' }));
    expect(await screen.findByRole('heading', { name: 'Henüz fatura yok' })).toBeTruthy();
  });

  it('Yönetici kontör ekler; neden ve miktar gönderilir', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp(`/markalar/${brandId}`, { ...admin, calls });
    await userEvent.click(await screen.findByRole('button', { name: 'Kontör ekle' }));
    const dialog = await screen.findByRole('dialog', { name: 'Kontör ekle' });
    await userEvent.click(within(dialog).getByRole('button', { name: 'Kontörü ekle' }));
    expect(await within(dialog).findByText('Nedeni en az 3 harfle yazın.')).toBeTruthy();
    await userEvent.type(within(dialog).getByLabelText('Kontör'), '500');
    await userEvent.type(within(dialog).getByLabelText('Neden'), 'Destek talebi #4821');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Kontörü ekle' }));
    expect(await screen.findByText('+500 kontör yazıldı. Bakiye 1.740.')).toBeTruthy();
    expect(calls[`/api/v1/admin/brands/${brandId}/credits`]?.[0]).toEqual({
      credits: 500,
      reason: 'Destek talebi #4821',
    });
  });

  it('Yönetici planı değiştirir (If-Match)', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp(`/markalar/${brandId}`, { ...admin, calls });
    await userEvent.click(await screen.findByRole('button', { name: 'Planı değiştir' }));
    const dialog = await screen.findByRole('dialog', { name: 'Planı değiştir' });
    await within(dialog).findByRole('option', { name: 'Salon Plus' });
    await userEvent.selectOptions(within(dialog).getByLabelText('Plan'), 'salon_plus');
    await userEvent.type(within(dialog).getByLabelText('Neden'), 'Şube sayısı arttı');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Planı değiştir' }));
    expect(await screen.findByText('Kuaför Elif artık Salon Plus planında.')).toBeTruthy();
    expect(calls[`/api/v1/admin/brands/${brandId}/plan`]?.[0]).toEqual({
      planCode: 'salon_plus',
      billingCycle: 'Monthly',
      reason: 'Şube sayısı arttı',
    });
    expect(calls[`POST /api/v1/admin/brands/${brandId}/plan If-Match`]?.[0]).toBe('W/"5120"');
  });

  it('plan sınırı aşılırsa nedeni yazar', async () => {
    renderApp(`/markalar/${brandId}`, { ...admin, planError: 'plan_limit_exceeded' });
    await userEvent.click(await screen.findByRole('button', { name: 'Planı değiştir' }));
    const dialog = await screen.findByRole('dialog', { name: 'Planı değiştir' });
    await within(dialog).findByRole('option', { name: 'Tekil' });
    await userEvent.selectOptions(within(dialog).getByLabelText('Plan'), 'solo');
    await userEvent.type(within(dialog).getByLabelText('Neden'), 'Küçülme');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Planı değiştir' }));
    expect(await within(dialog).findByText('6 etkin personel, plan sınırı 1.')).toBeTruthy();
  });
});
