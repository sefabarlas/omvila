import { expect, test } from '@playwright/test';
import type { Page } from '@playwright/test';
import type { PublicAppointment } from '../src/appointment/appointment-api';
import { ids, nowUtc } from '../src/test/booking-fixtures';
import { accessCode } from '../src/test/appointment-fixtures';
import { bookingApi, pendingRequest } from './support';

// Kritik akış 3, müşteri tarafı (test-stratejisi.md; dilim-03 kriter 1, 2, 3, 4): onay gerektiren salonda akış 2'nin
// aynısı → "Talebiniz alındı" → SMS bağlantısında (/r/{kod}) "Onay bekliyor" → salonun kararı (onay, ret) veya yanıt
// süresinin dolması → müşterinin gördüğü durum. Salonun kararı panel e2e'sinde (web/salon-panel/e2e/onay.e2e.ts)
// denenir; burada sunucunun karardan sonraki yanıtı verilir. Müşteri SMS'leri (sahte sağlayıcı günlüğü) API
// entegrasyon testlerindedir (ApprovalTests; acik-sorular.md T-72). "Şimdi" 24 Kasım 2026 10:00; talep 14:30, yanıt
// süresi 12:00'de doluyor.
const api = 'http://localhost:4011/api/v1/public';

type Decision = 'pending' | 'approved' | 'declined' | 'expired';

/** Talebin SMS bağlantısındaki hâli; salonun kararına veya süre dolumuna göre. */
function linkAppointment(decision: Decision): PublicAppointment {
  const request = pendingRequest();
  switch (decision) {
    case 'pending':
      return request;
    case 'approved':
      return {
        ...request,
        status: 'Confirmed',
        displayStatus: 'Confirmed',
        holdExpiresUtc: null,
        version: '2',
      };
    case 'declined':
    case 'expired':
      return {
        ...request,
        status: 'Cancelled',
        displayStatus: 'Cancelled',
        holdExpiresUtc: null,
        canCancel: false,
        canReschedule: false,
        cancellation: {
          cancelledUtc: decision === 'declined' ? '2026-11-24T07:20:00Z' : '2026-11-24T09:01:00Z',
          cancelledBy: decision === 'declined' ? 'Salon' : 'System',
          reason: decision === 'declined' ? 'DeclinedBySalon' : 'ApprovalExpired',
        },
        version: '2',
      };
  }
}

/** Onaylı salonun akış uçları + SMS bağlantısı; `decide` sonraki okumaların hâlini değiştirir. */
async function approvalSalon(page: Page) {
  const calls = await bookingApi(page, { requiresApproval: true });
  let decision: Decision = 'pending';
  await page.route(`${api}/appointments/${accessCode}`, async (route) => {
    if (route.request().method() === 'OPTIONS') {
      await route.fallback();
      return;
    }

    const current = linkAppointment(decision);
    await route.fulfill({
      status: 200,
      contentType: 'application/json',
      headers: {
        'Access-Control-Allow-Origin': 'http://localhost:3100',
        'Access-Control-Allow-Credentials': 'true',
        'Access-Control-Expose-Headers': 'ETag',
        ETag: `W/"${current.version}"`,
      },
      body: JSON.stringify(current),
    });
  });
  return {
    calls,
    decide: (next: Decision) => {
      decision = next;
    },
  };
}

/** Hizmet → personel → saat → telefon → kod → onay → "Talebiniz alındı". */
async function requestAppointment(page: Page) {
  const next = () => page.getByRole('button', { name: 'Devam et' }).click();
  await page.goto('/kuafor-elif');
  await expect(page.getByRole('heading', { level: 1, name: 'Kuaför Elif' })).toBeVisible();
  await page.getByText('Saç kesimi', { exact: true }).click();
  await page.getByText('Fön', { exact: true }).click();
  await next();
  await page.getByRole('radio', { name: /Elif Kaya/ }).click();
  await next();
  await page.getByRole('radio', { name: '14:30' }).click();
  await next();
  await page.getByLabel('Ad soyad').fill('Ayşe Yılmaz');
  await page.getByLabel('Telefon numaranız').fill('532 123 45 67');
  await page.getByText(/okudum; randevu için/).click();
  await page.getByRole('button', { name: 'Doğrulama kodu gönder' }).click();
  await page.getByRole('textbox', { name: 'Doğrulama kodu, hane 1' }).fill('482193');
  await expect(page.getByRole('heading', { name: 'Son kontrol' })).toBeVisible();
  await page.getByRole('button', { name: 'Randevuyu oluştur' }).click();

  await expect(page.getByRole('heading', { name: 'Talebiniz alındı' })).toBeVisible();
  await expect(page.getByText('Kuaför Elif en geç 2 saat içinde yanıtlar.')).toBeVisible();
  await expect(
    page.getByRole('region', { name: 'Talep özeti' }).getByText('Onay bekliyor'),
  ).toBeVisible();
  await expect(page.getByRole('heading', { name: 'Randevunuz onaylandı' })).toHaveCount(0);
}

/** SMS bağlantısı: talep "Onay bekliyor", salonun yanıt süresiyle. */
async function openPendingLink(page: Page) {
  await page.goto(`/r/${accessCode}`);
  await expect(page.getByText('Randevunuz · Onay bekliyor')).toBeVisible();
  await expect(page.getByRole('heading', { name: 'Salon onayı bekleniyor' })).toBeVisible();
  await expect(page.getByText('Kuaför Elif en geç 2 saat içinde yanıtlar.')).toBeVisible();
}

const noRejectedWording = async (page: Page) => {
  await expect(page.locator('body')).not.toContainText(/reddedil|reddetti/i);
};

test.describe('Kritik akış 3: onaylı salonda talep → salonun kararı → müşterinin gördüğü durum', () => {
  test('onay: talep "Onaylandı" olur (kriter 1, 2)', async ({ page }) => {
    const errors: string[] = [];
    page.on('pageerror', (error) => errors.push(error.message));
    const salon = await approvalSalon(page);
    await requestAppointment(page);
    expect(salon.calls.create).toHaveLength(1);
    expect(salon.calls.create[0]).toMatchObject({
      items: [
        { serviceId: ids.sac, staffId: ids.elif },
        { serviceId: ids.fon, staffId: ids.elif },
      ],
    });
    await openPendingLink(page);

    salon.decide('approved');
    await page.reload();
    await expect(page.getByText('Randevunuz · Onaylandı')).toBeVisible();
    await expect(page.getByRole('heading', { name: 'Salon onayı bekleniyor' })).toHaveCount(0);
    await expect(page.getByRole('heading', { name: 'Plan değişti mi?' })).toBeVisible();
    expect(errors).toEqual([]);
  });

  test('ret: müşteri "İptal" görür, "reddedildi" geçmez (kriter 3)', async ({ page }) => {
    const errors: string[] = [];
    page.on('pageerror', (error) => errors.push(error.message));
    const salon = await approvalSalon(page);
    await requestAppointment(page);
    await openPendingLink(page);

    salon.decide('declined');
    await page.reload();
    await expect(page.getByText('Randevunuz · İptal')).toBeVisible();
    await expect(page.getByRole('heading', { name: 'Talebiniz iptal edildi' })).toBeVisible();
    await expect(
      page.getByText(
        'Kuaför Elif bu talebi onaylayamadı. Başka bir saat seçerek yeni randevu alabilirsiniz.',
      ),
    ).toBeVisible();
    await expect(page.getByRole('link', { name: 'Yeni randevu al' })).toHaveAttribute(
      'href',
      '/kuafor-elif',
    );
    await expect(page.getByRole('button', { name: 'Randevuyu iptal et' })).toHaveCount(0);
    await noRejectedWording(page);
    expect(errors).toEqual([]);
  });

  test('süre dolumu: 120 dk yanıtsız talep iptal olur (saat ileri, kriter 4)', async ({ page }) => {
    const errors: string[] = [];
    page.on('pageerror', (error) => errors.push(error.message));
    const salon = await approvalSalon(page);
    await requestAppointment(page);
    await openPendingLink(page);

    // 121 dk sonra: sistem işi talebi ApprovalExpired ile iptal etmiş.
    await page.clock.setFixedTime(new Date(Date.parse(nowUtc) + 121 * 60_000));
    salon.decide('expired');
    await page.reload();
    await expect(page.getByText('Randevunuz · İptal')).toBeVisible();
    await expect(page.getByRole('heading', { name: 'Talebiniz iptal edildi' })).toBeVisible();
    await expect(
      page.getByText(
        'Kuaför Elif talebinizi zamanında yanıtlayamadı. Başka bir saat seçerek yeni randevu alabilirsiniz.',
      ),
    ).toBeVisible();
    await noRejectedWording(page);
    expect(errors).toEqual([]);
  });
});
