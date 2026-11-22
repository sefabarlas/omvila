import { expect, test } from '@playwright/test';
import { captureMatch, elifNoticeId, noticeDetailDemo, queueDemo } from './approvals-demo';
import { signedIn } from './support';

// Kritik akış (KK2): Destek onay kuyruğundan havale bildirimini açar, "Kod tutuyor" hareketi seçili gelir, uyuşmayan
// hareket "Kontrol edin" uyarısı verir; "Kod tutuyor" hareketle If-Match ile eşleştirilir ve kuyruğa dönülür.
test('havale bildirimi "Kod tutuyor" hareketle eşleştirilir', async ({ page }) => {
  const captured: { body?: unknown; ifMatch?: string | undefined } = {};
  await signedIn(page);
  await queueDemo(page);
  await noticeDetailDemo(page);
  await captureMatch(page, captured);

  await page.goto('/');
  await page
    .getByRole('navigation', { name: 'Ana menü' })
    .getByRole('link', { name: /Onay kuyrukları/ })
    .click();
  await expect(page.getByRole('heading', { level: 1, name: 'Onay kuyrukları' })).toBeVisible();
  const elif = page.getByRole('listitem').filter({ hasText: 'Kuaför Elif' });
  await elif.getByRole('button', { name: 'İncele' }).click();

  await expect(
    page.getByRole('heading', { level: 1, name: 'Kuaför Elif · havale bildirimi' }),
  ).toBeVisible();
  await expect(page).toHaveURL(new RegExp(`/onay/havale/${elifNoticeId}$`));
  await expect(page.getByRole('radio', { name: /ELİF KAYA/ })).toBeChecked();
  await page.getByRole('radio', { name: /SPA MODA/ }).check();
  await expect(page.getByText('Kontrol edin:')).toBeVisible();
  await page.getByRole('radio', { name: /ELİF KAYA/ }).check();
  await expect(page.getByText('Kontrol edin:')).toHaveCount(0);

  await page.getByRole('button', { name: 'Seçili hareketle eşleştir' }).click();
  await expect(page.getByText('Kuaför Elif markasına bildirim gitti.')).toBeVisible();
  await expect(page.getByRole('heading', { level: 1, name: 'Onay kuyrukları' })).toBeVisible();
  expect(captured.body).toEqual({ bankTransactionId: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa401' });
  expect(captured.ifMatch).toBe('W/"7001"');
});
