import { expect, test } from '@playwright/test';
import type { Page } from '@playwright/test';
import { calendarDemo, demoAppointment, demoCalendar, demoStaff } from './calendar-demo';
import { newAppointmentDemo } from './new-appointment-demo';
import { captureBody } from './support';

// Kritik akış 7 (test-stratejisi.md): panel takvimi; yeni randevu, sürükle-bırak, çakışma uyarısı (409 slot_taken),
// eşzamanlı düzenleme çakışması (412). Takvim tasarımdaki veridir (calendar-demo.ts, Çarşamba 25 Kasım 2026); taşıma
// ucu test tarafından yanıtlanır ve başarılı taşıma sonraki takvim okumalarına yansır. Sürükleme ve uyarı anlarının
// görüntüleri (TakvimSurukle, Durumlar) e2e-results/ekranlar altına yazılır.
// Sürükleme sırasında tam sayfa görüntü alınmaz: görüntü için pencere büyütülünce sürükleme iptal olur.
const shot = (page: Page, name: string, fullPage = true) =>
  page.screenshot({ path: `e2e-results/ekranlar/${name}.png`, fullPage });
const cors = {
  'Access-Control-Allow-Origin': 'http://localhost:5180',
  'Access-Control-Allow-Credentials': 'true',
};
const elif = demoStaff[0]?.staffId ?? '';
const zeynep = demoStaff[2]?.staffId ?? '';

type Outcome = 'ok' | 'slot_taken' | 'concurrency_conflict';

interface MoveCall {
  ifMatch: string | undefined;
  body: { startUtc: string; items?: { itemId: string; staffId: string }[]; scope?: string };
}

/** Takvim + taşıma ucu. `ok`ta taşınan randevu sonraki takvim yanıtlarında yeni yerindedir. */
async function calendarWithMoves(page: Page, outcome: Outcome): Promise<MoveCall[]> {
  await calendarDemo(page);
  const calls: MoveCall[] = [];
  const moved = new Map<string, { startUtc: string; staffId: string | undefined }>();
  await page.route(/\/api\/v1\/manage\/calendar\?/, async (route) => {
    if (route.request().method() === 'OPTIONS') {
      await route.fallback();
      return;
    }

    const view = demoCalendar(new URL(route.request().url()));
    view.appointments = view.appointments.map((item) => {
      const move = moved.get(item.appointmentId);
      if (!move) {
        return item;
      }

      const length = Date.parse(item.endUtc) - Date.parse(item.startUtc);
      return {
        ...item,
        startUtc: move.startUtc,
        endUtc: new Date(Date.parse(move.startUtc) + length).toISOString(),
        staffId: move.staffId ?? item.staffId,
      };
    });
    await route.fulfill({
      contentType: 'application/json',
      headers: cors,
      body: JSON.stringify(view),
    });
  });
  await page.route(/\/api\/v1\/manage\/appointments\/[^/]+\/reschedule$/, async (route) => {
    const request = route.request();
    if (request.method() !== 'POST') {
      await route.fallback();
      return;
    }

    const body = request.postDataJSON() as MoveCall['body'];
    calls.push({ ifMatch: (await request.headerValue('If-Match')) ?? undefined, body });
    const appointmentId = new URL(request.url()).pathname.split('/').at(-2) ?? '';
    if (outcome === 'ok') {
      moved.set(appointmentId, { startUtc: body.startUtc, staffId: body.items?.[0]?.staffId });
      await route.fulfill({
        contentType: 'application/json',
        headers: { ...cors, ETag: '"4"' },
        body: JSON.stringify({
          ...demoAppointment({ id: appointmentId }),
          startUtc: body.startUtc,
          version: '4',
        }),
      });
      return;
    }

    const status = outcome === 'slot_taken' ? 409 : 412;
    await route.fulfill({
      status,
      contentType: 'application/problem+json',
      headers: cors,
      body: JSON.stringify({ type: 'about:blank', title: outcome, status, code: outcome }),
    });
  });
  return calls;
}

const column = (page: Page, staffId: string) => page.locator(`[data-column="${staffId}"]`);
const derya = (page: Page, staffId: string) =>
  column(page, staffId).getByRole('button', { name: /^Derya Şahin/ });

/** Derya Şahin'i (Elif, 13:00) fareyle Zeynep Aksoy kolonuna `hours` saat kaydırarak bırakır. */
async function dragDeryaToZeynep(page: Page, hours: number, beforeDrop?: () => Promise<void>) {
  const block = derya(page, elif);
  await expect(block).toBeVisible();
  const from = await block.boundingBox();
  const to = await column(page, zeynep).boundingBox();
  if (!from || !to) {
    throw new Error('Takvim ölçülemedi.');
  }

  const x = from.x + from.width / 2;
  const y = from.y + Math.min(from.height / 2, 16);
  await page.mouse.move(x, y);
  await page.mouse.down();
  // Sürükleme 4 px sonra başlar (PointerSensor); 1 saat = 64 px.
  await page.mouse.move(x + 10, y + 10, { steps: 4 });
  await page.mouse.move(to.x + to.width / 2, y + hours * 64, { steps: 12 });
  await beforeDrop?.();
  await page.mouse.up();
}

async function openCalendar(page: Page) {
  // Sürüklenen blok pencerenin alt %20'sine girerse dnd-kit sayfayı kaydırır; takvimin tamamı pencereye sığar.
  await page.setViewportSize({ width: 1440, height: 1200 });
  await page.goto('/takvim');
  await expect(page.getByRole('heading', { level: 1, name: 'Takvim' })).toBeVisible();
  await expect(page.getByRole('region', { name: 'Günlük takvim' })).toBeVisible();
}

test.describe('Kritik akış 7: panel takvimi', () => {
  test('yeni randevu: "Randevu ekle" ile müşteri, hizmet ve saat seçilip oluşturulur', async ({
    page,
  }) => {
    const errors: string[] = [];
    page.on('pageerror', (error) => errors.push(error.message));
    await newAppointmentDemo(page);
    await openCalendar(page);

    await page.getByRole('button', { name: 'Randevu ekle', exact: true }).click();
    const dialog = page.getByRole('dialog', { name: 'Yeni randevu' });
    await dialog.getByRole('searchbox', { name: 'Müşteri ara' }).fill('Derya');
    await dialog.getByRole('button', { name: /Derya Şahin · 0532 760 21 18/ }).click();
    await dialog.getByRole('button', { name: 'Saç kesimi · 45 dk' }).click();
    await dialog.getByRole('radio', { name: '14:30' }).check();
    await expect(dialog.getByText('Çar 25 Kas · 14:30–15:15')).toBeVisible();

    const create = await captureBody(page, '/appointments', 'POST');
    await dialog.getByRole('button', { name: 'Randevu oluştur' }).click();
    expect(await create()).toMatchObject({
      branchId: expect.any(String),
      startUtc: '2026-11-25T11:30:00.000Z',
      items: [{ serviceId: '0193a8f0-0000-7000-8000-0000000000f1' }],
    });
    await expect(page.getByText('Randevu oluşturuldu')).toBeVisible();
    await expect(dialog).toBeHidden();
    expect(errors).toEqual([]);
  });

  test('sürükle-bırak: randevu başka personele ve saate taşınır (If-Match, tekrarlayanda yalnız bu)', async ({
    page,
  }) => {
    const calls = await calendarWithMoves(page, 'ok');
    await openCalendar(page);

    await dragDeryaToZeynep(page, 1, async () => {
      await expect(
        page.getByText("Zeynep Aksoy'a taşı · 14:00–14:45 · müşteriye SMS gider").first(),
      ).toBeAttached();
      await shot(page, 'TakvimSurukle', false);
    });

    await expect(derya(page, zeynep)).toBeVisible();
    await expect(derya(page, elif)).toHaveCount(0);
    expect(calls).toEqual([
      {
        ifMatch: '3',
        body: {
          startUtc: '2026-11-25T11:00:00.000Z',
          items: [{ itemId: expect.any(String) as unknown, staffId: zeynep }],
          scope: 'ThisOnly',
        },
      },
    ]);
  });

  test('dolu saate bırakma: blok eski yerine döner, "Saat dolu" uyarısı', async ({ page }) => {
    const calls = await calendarWithMoves(page, 'slot_taken');
    await openCalendar(page);

    // Zeynep'in 12:30–13:30 arası dolu (Ece Koç); bırakılan saat 12:30.
    await dragDeryaToZeynep(page, -0.5);

    await expect(page.getByText('Saat dolu: 12:30 artık boş değil.')).toBeVisible();
    await expect(page.getByText('Randevu eski yerine döndü.', { exact: false })).toBeVisible();
    await expect(derya(page, elif)).toBeVisible();
    await expect(derya(page, zeynep)).toHaveCount(0);
    expect(calls).toHaveLength(1);
    await shot(page, 'DurumSaatDolu');
  });

  test('eşzamanlı düzenleme (412): "Kayıt çakışması · Yenile"', async ({ page }) => {
    await calendarWithMoves(page, 'concurrency_conflict');
    await openCalendar(page);

    await dragDeryaToZeynep(page, 1);

    await expect(page.getByText('Kayıt çakışması')).toBeVisible();
    await expect(derya(page, elif)).toBeVisible();
    await shot(page, 'DurumKayitCakismasi');
    await page.getByRole('button', { name: 'Yenile' }).click();
    await expect(page.getByText('Kayıt çakışması')).toHaveCount(0);
  });
});
