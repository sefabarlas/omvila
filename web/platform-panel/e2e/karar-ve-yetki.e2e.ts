import { expect, test } from '@playwright/test';
import {
  elifNoticeId,
  noticeDetailBody,
  noticeDetailDemo,
  queueDemo,
  screenInstant,
} from './approvals-demo';
import { brandDetailDemo, demoBrandId } from './platform-demo';
import { api, asAdmin, json, signedIn } from './support';

// Dilim 6 KK3 ve KK4. KK3: tutarı veya kodu uyuşmayan hareket seçilince "Kontrol edin" uyarısı nedeni yazar;
// "Bulunamadı, markaya yaz" bildirimi NotFound yapar (If-Match, not isteğe bağlı). KK4: Destek rolünde "Kontör ekle"
// ve "Planı değiştir" kilitli ve pasif, ipucu "Yönetici yetkisi ister"; Yönetici kontör ekler (neden denetime yazılır).
// API tarafı (403, denetim kaydı) backend entegrasyon testlerindedir.
const preflight = {
  status: 204,
  headers: {
    'Access-Control-Allow-Origin': '*',
    'Access-Control-Allow-Headers': '*',
    'Access-Control-Allow-Methods': 'POST',
  },
};
const note = 'Havaleyi hesabımızda göremedik. Dekontu kontrol edip yeniden bildirin.';

test('uyuşmayan harekette "Kontrol edin" nedeni; "Bulunamadı, markaya yaz" → NotFound (KK3)', async ({
  page,
}) => {
  const captured: { body?: unknown; ifMatch?: string | undefined } = {};
  await signedIn(page);
  await queueDemo(page);
  await noticeDetailDemo(page);
  await page.route(`${api}/payment-notices/${elifNoticeId}/not-found`, async (route) => {
    if (route.request().method() === 'OPTIONS') {
      await route.fulfill(preflight);
      return;
    }

    captured.body = route.request().postDataJSON();
    captured.ifMatch = route.request().headers()['if-match'];
    await route.fulfill(
      json({
        ...noticeDetailBody(),
        status: 'NotFound',
        candidates: [],
        resolution: {
          resolvedUtc: screenInstant,
          resolvedByName: 'Ayşe Demir',
          matchedBankTransactionId: null,
          message: note,
        },
        version: 'W/"7011"',
      }),
    );
  });

  await page.goto(`/onay/havale/${elifNoticeId}`);
  await expect(
    page.getByRole('heading', { level: 1, name: 'Kuaför Elif · havale bildirimi' }),
  ).toBeVisible();

  // Kod yok, tutar tutuyor.
  await page.getByRole('radio', { name: /KAAN ÖZTÜRK/ }).check();
  const warning = page.getByRole('status').filter({ hasText: 'Kontrol edin:' });
  await expect(warning).toContainText('açıklama kodu yok; tutar ve gönderen adıyla eşleştirin.');
  // Kod da tutar da tutmuyor.
  await page.getByRole('radio', { name: /SPA MODA/ }).check();
  await expect(warning).toContainText('açıklama kodu yok ve tutar farklı');
  // Uyarıya rağmen eşleştirme operatöre açık (T-116 (3)).
  await expect(page.getByRole('button', { name: 'Seçili hareketle eşleştir' })).toBeEnabled();

  await page.getByRole('button', { name: 'Bulunamadı, markaya yaz' }).click();
  const dialog = page.getByRole('dialog', { name: 'Bulunamadı, markaya yaz' });
  await dialog.getByLabel('Markaya not').fill(note);
  await dialog.getByRole('button', { name: 'Markaya yaz' }).click();

  await expect(page.getByText('Kuaför Elif havalenin bulunamadığını görecek.')).toBeVisible();
  expect(captured.body).toEqual({ message: note });
  expect(captured.ifMatch).toBe('W/"7001"');
});

test('Destek: "Kontör ekle" ve "Planı değiştir" kilitli, nedeni ipucunda (KK4)', async ({
  page,
}) => {
  await signedIn(page);
  await brandDetailDemo(page);
  let writes = 0;
  page.on('request', (request) => {
    const path = new URL(request.url()).pathname;
    if (request.method() !== 'GET' && path.includes(`/brands/${demoBrandId}/`)) {
      writes += 1;
    }
  });
  await page.goto(`/markalar/${demoBrandId}`);
  await expect(page.getByRole('heading', { level: 1, name: 'Kuaför Elif' })).toBeVisible();

  const hints = page.getByRole('tooltip', { name: 'Yönetici yetkisi ister', includeHidden: true });
  for (const [index, name] of ['Kontör ekle', 'Planı değiştir'].entries()) {
    const button = page.getByRole('button', { name });
    await expect(button).toBeDisabled();
    await button.hover({ force: true });
    await expect(hints.nth(index)).toBeVisible();
    await page.mouse.move(0, 0);
  }

  await expect(page.getByText(/yönetici yetkisi ister/)).toBeVisible();
  await expect(page.getByRole('dialog')).toHaveCount(0);
  expect(writes).toBe(0);
});

test('Yönetici kontör ekler: adet ve neden gider (KK4)', async ({ page }) => {
  await signedIn(page);
  await asAdmin(page);
  await brandDetailDemo(page);
  let body: unknown;
  await page.route(`${api}/brands/${demoBrandId}/credits`, async (route) => {
    if (route.request().method() === 'POST') {
      body = route.request().postDataJSON();
    }

    // Yanıt Prism'den (istek gövdesi sözleşmeye göre doğrulanır).
    await route.fallback();
  });
  await page.goto(`/markalar/${demoBrandId}`);
  await page.getByRole('button', { name: 'Kontör ekle' }).click();
  const dialog = page.getByRole('dialog', { name: 'Kontör ekle' });
  await dialog.getByLabel('Kontör').fill('500');
  await dialog.getByLabel('Neden').fill('Destek talebi #4821');
  await dialog.getByRole('button', { name: 'Kontörü ekle' }).click();

  await expect(page.getByText('Kontör eklendi')).toBeVisible();
  await expect(dialog).toBeHidden();
  expect(body).toEqual({ credits: 500, reason: 'Destek talebi #4821' });
});
