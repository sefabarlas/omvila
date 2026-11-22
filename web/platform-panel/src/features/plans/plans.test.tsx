import { cleanup, screen, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it } from 'vitest';
import { sessionFixture } from '@/test/fake-api';
import { renderApp } from '@/test/render-app';

// Kural: platform-paneli/PPlanlar: fiyat ve sınırlar veriden (boş fiyat "belirlenmedi", boş sınır "sınırsız"; yer
// tutucu yazılmaz); düzenleme Yönetici ve Finans'ta (If-Match, 412), Destek görür ve buton kilitli (KK4).
afterEach(cleanup);

const finance = { session: sessionFixture('PlatformFinance') };

describe('planlar ve fiyat', () => {
  it('plan kartları, ortak kurallar ve kontör paketleri', async () => {
    renderApp('/planlar');
    const plans = await screen.findByRole('list', { name: 'Planlar' });
    const [solo = plans, salon = plans, plus = plans] = within(plans).getAllByRole('listitem', {
      name: /./,
    });
    expect(within(solo).getByText('96 marka')).toBeTruthy();
    expect(within(solo).getByText('Fiyat belirlenmedi')).toBeTruthy();
    expect(within(solo).getByText('1 personel')).toBeTruthy();
    expect(within(solo).getByText('Online randevu ve hatırlatma')).toBeTruthy();
    expect(within(salon).getByText('1.000 ₺ / ay')).toBeTruthy();
    expect(within(salon).getByText('1 şube dahil, ek şube 250 ₺ / ay')).toBeTruthy();
    expect(within(salon).getByText('Kapıda sıra ve bekleme listesi')).toBeTruthy();
    expect(within(plus).getByText('Sınırsız şube')).toBeTruthy();
    expect(within(plus).getByText('Öncelikli destek')).toBeTruthy();
    expect(screen.getByText('7 gün hatırlatma, sonra salt okunur')).toBeTruthy();
    const rules = screen.getByRole('region', { name: 'Ortak kurallar' });
    expect(within(rules).getByText('Belirlenmedi')).toBeTruthy();
    const packages = await screen.findByRole('table', { name: 'Kontör paketleri' });
    expect(within(packages).getByText('20.000')).toBeTruthy();
    expect(within(packages).getAllByText('Belirlenmedi')).toHaveLength(3);
    expect(document.body.textContent).not.toMatch(/\[[A-ZİÖÜÇŞĞ ]+\]/);
  });

  it('Destek rolünde düzenleme kilitli', async () => {
    renderApp('/planlar');
    await screen.findByRole('list', { name: 'Planlar' });
    for (const name of ['Planı düzenle', 'Paketleri düzenle']) {
      const button = screen.getByRole('button', { name });
      expect((button as HTMLButtonElement).disabled).toBe(true);
      expect(
        document.getElementById(button.getAttribute('aria-describedby') ?? '')?.textContent,
      ).toBe('Yönetici veya Finans yetkisi ister');
    }
  });

  it('Finans planın fiyatını kaydeder (If-Match)', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/planlar', { ...finance, calls });
    await screen.findByRole('list', { name: 'Planlar' });
    await userEvent.click(screen.getByRole('button', { name: 'Planı düzenle' }));
    const dialog = await screen.findByRole('dialog', { name: 'Planı düzenle' });
    const tekil = within(dialog).getByRole('group', { name: 'Tekil' });
    await userEvent.type(within(tekil).getByLabelText('Aylık fiyat (₺)'), 'abc');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Planları kaydet' }));
    expect(await within(tekil).findByText('Tutarı 1.250 veya 850,50 gibi yazın.')).toBeTruthy();
    await userEvent.clear(within(tekil).getByLabelText('Aylık fiyat (₺)'));
    await userEvent.type(within(tekil).getByLabelText('Aylık fiyat (₺)'), '450');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Planları kaydet' }));
    expect(await screen.findByText('Planlar kaydedildi')).toBeTruthy();
    expect(calls['PUT /api/v1/admin/plans If-Match']?.[0]).toBe('W/"301"');
    const body = calls['/api/v1/admin/plans']?.find(
      (c) => c && typeof c === 'object' && 'plans' in c,
    ) as {
      plans: {
        code: string;
        monthlyPrice: unknown;
        staffLimit: unknown;
        includedBranchCount: unknown;
      }[];
    };
    expect(body.plans[0]).toMatchObject({
      code: 'solo',
      monthlyPrice: { amountMinor: 45000, currency: 'TRY' },
      staffLimit: 1,
    });
    expect(body.plans[2]?.includedBranchCount).toBeNull();
    expect(await screen.findByText('450 ₺ / ay')).toBeTruthy();
  });

  it('plan kaydında 412 çakışması', async () => {
    renderApp('/planlar', { ...finance, platform: { plansConflict: true } });
    await screen.findByRole('list', { name: 'Planlar' });
    await userEvent.click(screen.getByRole('button', { name: 'Planı düzenle' }));
    const dialog = await screen.findByRole('dialog', { name: 'Planı düzenle' });
    await userEvent.click(within(dialog).getByRole('button', { name: 'Planları kaydet' }));
    expect(await within(dialog).findByText('Kayıt çakışması.')).toBeTruthy();
    expect(within(dialog).getByRole('button', { name: 'Yenile' })).toBeTruthy();
  });

  it('Finans kontör paketinin fiyatını kaydeder', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/planlar', { ...finance, calls });
    await screen.findByRole('table', { name: 'Kontör paketleri' });
    await userEvent.click(screen.getByRole('button', { name: 'Paketleri düzenle' }));
    const dialog = await screen.findByRole('dialog', { name: 'Kontör paketlerini düzenle' });
    const starter = within(dialog).getByRole('group', { name: 'Başlangıç' });
    await userEvent.type(within(starter).getByLabelText('Fiyat (₺)'), '150');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Paketleri kaydet' }));
    expect(await screen.findByText('Kontör paketleri kaydedildi')).toBeTruthy();
    expect(calls['PUT /api/v1/admin/credit-packages If-Match']?.[0]).toBe('W/"302"');
    const table = screen.getByRole('table', { name: 'Kontör paketleri' });
    expect(await within(table).findByText('150 ₺')).toBeTruthy();
  });
});
