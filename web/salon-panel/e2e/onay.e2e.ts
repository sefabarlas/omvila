import { expect, test } from '@playwright/test';
import type { Page } from '@playwright/test';
import {
  demoCalendar,
  demoDepositQueueItem,
  demoNow,
  demoStaff,
  overviewDemo,
} from './calendar-demo';
import { sessionAs } from './support';

// Kritik akış 3, panel tarafı (test-stratejisi.md; dilim-03 kriter 2–5): onaylı salonda online talep "Dikkat
// isteyenler"de görünür → "İncele" yan paneli açar (kalan süre) → Onayla veya Reddet; yanıt süresi dolduysa karar
// butonları kalkar. Müşteri tarafı (Talep, Randevum) web/rezervasyon/e2e/onay.e2e.ts; müşteri SMS'i (sahte sağlayıcı
// günlüğü) uygulamalar arası gerçek backend olmadan denenemediği için API entegrasyon testlerinde
// (ApprovalTests: Requested/Approved/Declined/ApprovalExpired SMS'leri; acik-sorular.md T-72).
// Saat takvim demosundaki gibi Çarşamba 25 Kasım 2026 12:10 İstanbul (09:10Z); talep Pınar Yalçın 16:00, yanıt süresi
// 13:45'te doluyor (1 sa 35 dk). Yan panel görüntüsü (RandevuPanel, açık ve koyu) e2e-results/ekranlar altına yazılır.
const cors = {
  'Access-Control-Allow-Origin': 'http://localhost:5180',
  'Access-Control-Allow-Credentials': 'true',
};
const json = (body: unknown, status = 200) => ({
  status,
  contentType: 'application/json',
  headers: cors,
  body: JSON.stringify(body),
});

const zeynep = demoStaff[2]?.staffId ?? '';
// demoCalendar'ın ilk gününde 12. satır (Zeynep Aksoy 16:00, PendingApproval).
const requestId = '0193a8f0-0000-7000-8000-000000000011';
const holdExpiresUtc = '2026-11-25T10:45:00Z';
const nowUtc = new Date(demoNow).toISOString();

type State = 'PendingApproval' | 'Confirmed' | 'DeclinedBySalon';
type Outcome = 'ok' | 'hold_expired';

interface DecisionCall {
  action: 'approve' | 'decline';
  ifMatch: string | undefined;
}

function requestDetail(state: State) {
  const cancelled = state === 'DeclinedBySalon';
  const status = cancelled ? 'Cancelled' : state;
  return {
    id: requestId,
    reference: 'A-20481',
    branchId: '0193a8f0-0000-7000-8000-0000000000b1',
    status,
    displayStatus: status,
    channel: 'Online',
    startUtc: '2026-11-25T13:00:00Z',
    endUtc: '2026-11-25T14:00:00Z',
    timeZone: 'Europe/Istanbul',
    holdExpiresUtc: state === 'PendingApproval' ? holdExpiresUtc : null,
    customer: {
      brandCustomerId: '0193a8f0-0000-7000-8000-0000000000d9',
      fullName: 'Pınar Yalçın',
      phone: '+905550000123',
      visitCount: 0,
      isNew: true,
    },
    items: [
      {
        id: '0193a8f0-0000-7000-8000-0000000000e9',
        serviceId: '0193a8f0-0000-7000-8000-0000000000f9',
        serviceName: 'Cilt bakımı',
        staffId: zeynep,
        staffName: 'Zeynep Aksoy',
        startUtc: '2026-11-25T13:00:00Z',
        endUtc: '2026-11-25T14:00:00Z',
        price: { amountMinor: 90000, currency: 'TRY' },
        status: cancelled ? 'Cancelled' : 'Active',
        segments: [],
      },
    ],
    deposit: null,
    total: { amountMinor: 90000, currency: 'TRY' },
    amountDue: { amountMinor: 90000, currency: 'TRY' },
    customerNote: 'Hassas cildim var, kokusuz ürün tercih ederim.',
    isRecurring: false,
    noShowAtUtc: null,
    cancellation: cancelled
      ? { cancelledUtc: nowUtc, cancelledBy: 'Salon', reason: 'DeclinedBySalon', note: null }
      : null,
    history: [
      {
        kind: 'Created',
        occurredUtc: '2026-11-25T08:45:00Z',
        actorName: null,
        detail: 'Online talep oluşturuldu, SMS ile doğrulandı',
      },
      ...(state === 'Confirmed'
        ? [{ kind: 'Approved', occurredUtc: nowUtc, actorName: 'Elif Kaya', detail: null }]
        : []),
      ...(cancelled
        ? [{ kind: 'Declined', occurredUtc: nowUtc, actorName: 'Elif Kaya', detail: null }]
        : []),
    ],
    version: state === 'PendingApproval' ? '7' : '8',
  };
}

const queue = (state: State) => ({
  generatedUtc: nowUtc,
  totalCount: state === 'PendingApproval' ? 1 : 0,
  earliestHoldExpiresUtc: state === 'PendingApproval' ? holdExpiresUtc : null,
  items:
    state === 'PendingApproval'
      ? [
          {
            appointmentId: requestId,
            reference: 'A-20481',
            branchId: '0193a8f0-0000-7000-8000-0000000000b1',
            branchName: 'Kadıköy',
            timeZone: 'Europe/Istanbul',
            customerName: 'Pınar Yalçın',
            isNewCustomer: true,
            serviceSummary: 'Cilt bakımı',
            staffName: 'Zeynep Aksoy',
            startUtc: '2026-11-25T13:00:00Z',
            endUtc: '2026-11-25T14:00:00Z',
            holdExpiresUtc,
            createdUtc: '2026-11-25T08:45:00Z',
            channel: 'Online',
          },
        ]
      : [],
});

/**
 * Takvim demosu + onay kuyruğu, talep ayrıntısı ve karar uçları. Başarılı karar sonraki okumalara (kuyruk, ayrıntı,
 * takvim) yansır; `hold_expired`ta sunucu 409 döner (tutma süresi dolmuş, otomatik iptal işi henüz çalışmamış).
 */
async function approvalDemo(page: Page, outcome: Outcome = 'ok'): Promise<DecisionCall[]> {
  await overviewDemo(page);
  const calls: DecisionCall[] = [];
  let state: State = 'PendingApproval';
  await page.route(/\/api\/v1\/manage\/calendar\?/, async (route) => {
    if (route.request().method() === 'OPTIONS') {
      await route.fallback();
      return;
    }

    const view = demoCalendar(new URL(route.request().url()));
    view.appointments = view.appointments
      .filter((item) => item.appointmentId !== requestId || state !== 'DeclinedBySalon')
      .map((item) => {
        if (item.appointmentId !== requestId) {
          return item;
        }

        return state === 'Confirmed'
          ? { ...item, status: 'Confirmed', displayStatus: 'Confirmed', holdExpiresUtc: null }
          : { ...item, holdExpiresUtc };
      });
    await route.fulfill(json(view));
  });
  await page.route(/\/api\/v1\/manage\/approval-queue/, (route) =>
    route.request().method() === 'OPTIONS' ? route.fallback() : route.fulfill(json(queue(state))),
  );
  await page.route(`**/api/v1/manage/appointments/${requestId}`, (route) =>
    route.request().method() === 'OPTIONS'
      ? route.fallback()
      : route.fulfill(json(requestDetail(state))),
  );
  await page.route(
    new RegExp(`/api/v1/manage/appointments/${requestId}/(approve|decline)$`),
    async (route) => {
      const request = route.request();
      if (request.method() === 'OPTIONS') {
        await route.fallback();
        return;
      }

      const action = request.url().endsWith('/approve') ? 'approve' : 'decline';
      calls.push({ action, ifMatch: (await request.headerValue('If-Match')) ?? undefined });
      if (outcome === 'hold_expired') {
        await route.fulfill(
          json(
            { type: 'about:blank', title: 'hold_expired', status: 409, code: 'hold_expired' },
            409,
          ),
        );
        return;
      }

      state = action === 'approve' ? 'Confirmed' : 'DeclinedBySalon';
      await route.fulfill(json(requestDetail(state)));
    },
  );
  return calls;
}

/** Genel bakış → "Dikkat isteyenler" → "İncele" → onay bekleyen talebin yan paneli. */
async function openRequest(page: Page) {
  await page.goto('/');
  const attention = page.getByRole('region', { name: 'Dikkat isteyenler' });
  await expect(attention.getByText('1 online talep onay bekliyor')).toBeVisible();
  await expect(attention.getByText('Pınar Yalçın 16:00')).toBeVisible();
  await expect(page.getByText('ilki 1 sa 35 dk içinde iptal olur')).toBeVisible();
  await attention.getByRole('button', { name: 'İncele: Pınar Yalçın' }).click();
  const drawer = page.getByRole('dialog', { name: 'Randevu · #A-20481 · Online' });
  await expect(drawer.getByText('Pınar Yalçın')).toBeVisible();
  return drawer;
}

test.describe('Kritik akış 3: panelde onay bekleyen talep', () => {
  test('Onayla: talep onaylanır, kuyruktan düşer (kriter 2)', async ({ page }) => {
    const errors: string[] = [];
    page.on('pageerror', (error) => errors.push(error.message));
    const calls = await approvalDemo(page);
    const drawer = await openRequest(page);

    await expect(drawer.getByText('1 sa 35 dk içinde yanıtlayın.')).toBeVisible();
    await expect(drawer.getByText('Hassas cildim var, kokusuz ürün tercih ederim.')).toBeVisible();
    await drawer.getByRole('button', { name: 'Onayla' }).click();

    await expect(page.getByText('Randevu onaylandı')).toBeVisible();
    await expect(page.getByText('Müşteriye onay bildirimi gider.')).toBeVisible();
    expect(calls).toEqual([{ action: 'approve', ifMatch: '7' }]);
    await expect(drawer.getByRole('button', { name: 'Onayla' })).toHaveCount(0);
    await expect(drawer.getByRole('button', { name: 'Reddet' })).toHaveCount(0);
    await expect(drawer.getByRole('region', { name: 'Randevu durumu' })).toContainText('Onaylandı');

    // Kuyruk tazelenir: "Dikkat isteyenler" boşalır, bento 0.
    await page.keyboard.press('Escape');
    await expect(page.getByText('Dikkat isteyen bir şey yok')).toBeVisible();
    await expect(page.getByText('Bekleyen talep yok')).toBeVisible();
    expect(errors).toEqual([]);
  });

  test('Reddet: talep iptal edilir, takvimden düşer (kriter 3)', async ({ page }) => {
    const errors: string[] = [];
    page.on('pageerror', (error) => errors.push(error.message));
    const calls = await approvalDemo(page);
    const drawer = await openRequest(page);

    await drawer.getByRole('button', { name: 'Reddet' }).click();
    await expect(page.getByText('Talep iptal edildi')).toBeVisible();
    await expect(page.getByText('Müşteriye iptal bildirimi gider.')).toBeVisible();
    expect(calls).toEqual([{ action: 'decline', ifMatch: '7' }]);
    await expect(drawer.getByRole('button', { name: 'Onayla' })).toHaveCount(0);
    await expect(drawer.getByRole('region', { name: 'Randevu durumu' })).toContainText('İptal');
    await expect(drawer.getByText('25 Kasım tarihinde iptal edildi.')).toBeVisible();

    await page.keyboard.press('Escape');
    await expect(page.getByText('Dikkat isteyen bir şey yok')).toBeVisible();
    await page.goto('/takvim');
    const calendar = page.getByRole('region', { name: 'Günlük takvim' });
    await expect(calendar.getByRole('button', { name: /^Deniz Acar/ })).toBeVisible();
    await expect(calendar.getByRole('button', { name: /^Pınar Yalçın/ })).toHaveCount(0);
    expect(errors).toEqual([]);
  });

  test('süre dolumu: yanıt süresi geçmiş talepte karar butonları yok (kriter 4)', async ({
    page,
  }) => {
    const calls = await approvalDemo(page);
    // Saat ileri: 13:46 İstanbul, tutma 13:45'te doldu; otomatik iptal işi henüz çalışmadı (kuyrukta duruyor).
    await page.clock.setFixedTime('2026-11-25T13:46:00+03:00');
    await page.goto('/');
    await page
      .getByRole('region', { name: 'Dikkat isteyenler' })
      .getByRole('button', { name: 'İncele: Pınar Yalçın' })
      .click();
    const drawer = page.getByRole('dialog', { name: 'Randevu · #A-20481 · Online' });
    await expect(drawer.getByText('Yanıt süresi doldu')).toBeVisible();
    await expect(
      drawer.getByText('Talep birazdan otomatik iptal edilir ve müşteriye bildirim gider.'),
    ).toBeVisible();
    await expect(drawer.getByRole('button', { name: 'Onayla' })).toHaveCount(0);
    await expect(drawer.getByRole('button', { name: 'Reddet' })).toHaveCount(0);
    expect(calls).toEqual([]);
  });

  test('süre dolumu: sunucu 409 hold_expired dönerse "Yanıt süresi doldu" (kriter 4)', async ({
    page,
  }) => {
    const calls = await approvalDemo(page, 'hold_expired');
    const drawer = await openRequest(page);
    await drawer.getByRole('button', { name: 'Onayla' }).click();

    await expect(drawer.getByText('Yanıt süresi doldu')).toBeVisible();
    await expect(drawer.getByRole('button', { name: 'Onayla' })).toHaveCount(0);
    await expect(drawer.getByRole('button', { name: 'Reddet' })).toHaveCount(0);
    await expect(page.getByText('Randevu onaylandı')).toHaveCount(0);
    expect(calls).toEqual([{ action: 'approve', ifMatch: '7' }]);
  });

  test('personel rolü onay butonlarını ve kuyruğu görmez (kriter 5)', async ({ page }) => {
    const calls = await approvalDemo(page);
    await sessionAs(page, {
      permissions: ['calendar.view', 'appointment.checkin', 'customer.view'],
      membership: { role: 'Staff', branchIds: [], staffId: zeynep },
    });
    let queueRequested = false;
    page.on('request', (request) => {
      if (request.url().includes('/approval-queue') && request.method() === 'GET') {
        queueRequested = true;
      }
    });
    await page.goto('/takvim');
    await page
      .getByRole('region', { name: 'Günlük takvim' })
      .getByRole('button', { name: /^Pınar Yalçın/ })
      .click();
    const drawer = page.getByRole('dialog', { name: 'Randevu · #A-20481 · Online' });
    await expect(
      drawer.getByText('Onay bekleyen talepleri yalnızca yönetici ve işletme sahibi onaylar.'),
    ).toBeVisible();
    await expect(drawer.getByRole('button', { name: 'Onayla' })).toHaveCount(0);
    await expect(drawer.getByRole('button', { name: 'Reddet' })).toHaveCount(0);
    expect(queueRequested).toBe(false);
    expect(calls).toEqual([]);
  });
});

for (const theme of ['light', 'dark'] as const) {
  test(`RandevuPanel (${theme === 'light' ? 'açık' : 'koyu'})`, async ({ page }) => {
    const errors: string[] = [];
    page.on('pageerror', (error) => errors.push(error.message));
    await page.emulateMedia({ colorScheme: theme });
    await page.addInitScript((value) => {
      try {
        localStorage.setItem('omvila.tema', value);
      } catch {
        // Depolama kapalıysa sistem teması kullanılır.
      }
    }, theme);
    await approvalDemo(page);
    await page.goto('/takvim');
    await page
      .getByRole('region', { name: 'Günlük takvim' })
      .getByRole('button', { name: /^Pınar Yalçın/ })
      .click();
    const drawer = page.getByRole('dialog', { name: 'Randevu · #A-20481 · Online' });
    await expect(drawer.getByText('1 sa 35 dk içinde yanıtlayın.')).toBeVisible();
    await page.waitForFunction(() =>
      document.getAnimations().every((animation) => animation.playState !== 'running'),
    );
    await page.screenshot({
      path: `e2e-results/ekranlar/RandevuPanel${theme === 'dark' ? 'Koyu' : ''}.png`,
      // Pencere görüntüsü: tam sayfada sabit yan panel ve kenar menü kayar (sayfa üstündeki şeritler yüzünden).
      fullPage: false,
    });
    expect(errors).toEqual([]);
  });
}

// ───────── Kapora (dilim-04 KK4, KK5, KK7) ─────────
// Gizem Arslan 14:30 Fön · Mert Demir, kapora 150 ₺ (OMV-7K2X), havale süresi 13:45'te doluyor. "Kapora geldi"
// (manageMarkDepositReceived) ve "Kaporasız onayla" (manageApproveAppointment, waiveDeposit) sonraki okumalara yansır.
const depositId = demoDepositQueueItem.appointmentId;
const depositHold = demoDepositQueueItem.holdExpiresUtc;

type DepositState = 'Awaiting' | 'CustomerReported' | 'Received' | 'Waived';

interface DepositCall {
  action: string;
  ifMatch: string | undefined;
  idempotencyKey: string | undefined;
  body: unknown;
}

function depositDetail(state: DepositState) {
  const pending = state === 'Awaiting' || state === 'CustomerReported';
  const reported = state !== 'Awaiting' && state !== 'Waived';
  return {
    id: depositId,
    reference: 'A-20512',
    branchId: '0193a8f0-0000-7000-8000-0000000000b1',
    status: pending ? 'PendingApproval' : 'Confirmed',
    displayStatus: pending ? 'DepositPending' : 'Confirmed',
    channel: 'Online',
    startUtc: demoDepositQueueItem.startUtc,
    endUtc: demoDepositQueueItem.endUtc,
    timeZone: 'Europe/Istanbul',
    holdExpiresUtc: pending ? depositHold : null,
    customer: {
      brandCustomerId: '0193a8f0-0000-7000-8000-0000000000da',
      fullName: 'Gizem Arslan',
      phone: '+905550000148',
      visitCount: 3,
      isNew: false,
    },
    items: [
      {
        id: '0193a8f0-0000-7000-8000-0000000000ea',
        serviceId: '0193a8f0-0000-7000-8000-0000000000fa',
        serviceName: 'Fön',
        staffId: demoStaff[1]?.staffId ?? '',
        staffName: 'Mert Demir',
        startUtc: demoDepositQueueItem.startUtc,
        endUtc: demoDepositQueueItem.endUtc,
        price: { amountMinor: 45000, currency: 'TRY' },
        status: 'Active',
        segments: [],
      },
    ],
    deposit: {
      required: { amountMinor: 15000, currency: 'TRY' },
      status: state,
      transferCode: 'OMV-7K2X',
      customerReportedUtc: reported ? '2026-11-25T08:52:00Z' : null,
      receivedUtc: state === 'Received' ? nowUtc : null,
      remindedUtc: null,
      refundedUtc: null,
    },
    total: { amountMinor: 45000, currency: 'TRY' },
    amountDue: { amountMinor: state === 'Received' ? 30000 : 45000, currency: 'TRY' },
    customerNote: null,
    isRecurring: false,
    noShowAtUtc: null,
    cancellation: null,
    history: [
      {
        kind: 'Created',
        occurredUtc: '2026-11-25T08:40:00Z',
        actorName: null,
        detail: 'Online talep oluşturuldu, SMS ile doğrulandı',
      },
      ...(reported
        ? [
            {
              kind: 'DepositReported',
              occurredUtc: '2026-11-25T08:52:00Z',
              actorName: 'Gizem Arslan',
              detail: null,
            },
          ]
        : []),
      ...(state === 'Received'
        ? [{ kind: 'DepositReceived', occurredUtc: nowUtc, actorName: 'Elif Kaya', detail: null }]
        : []),
      ...(state === 'Waived'
        ? [{ kind: 'DepositWaived', occurredUtc: nowUtc, actorName: 'Elif Kaya', detail: null }]
        : []),
    ],
    version: pending ? '4' : '5',
  };
}

/** Genel bakış ve takvim demosu + kaporalı randevunun ayrıntısı, kuyruğu ve kapora uçları. */
async function depositDemo(page: Page, initial: DepositState = 'Awaiting'): Promise<DepositCall[]> {
  await overviewDemo(page);
  const calls: DepositCall[] = [];
  let state: DepositState = initial;
  const pending = () => state === 'Awaiting' || state === 'CustomerReported';
  await page.route(/\/api\/v1\/manage\/calendar\?/, async (route) => {
    if (route.request().method() === 'OPTIONS') {
      await route.fallback();
      return;
    }

    const view = demoCalendar(new URL(route.request().url()));
    view.appointments = view.appointments.map((item) =>
      item.appointmentId !== depositId
        ? item
        : pending()
          ? { ...item, depositStatus: state, holdExpiresUtc: depositHold }
          : {
              ...item,
              status: 'Confirmed',
              displayStatus: 'Confirmed',
              depositStatus: state,
              holdExpiresUtc: null,
            },
    );
    await route.fulfill(json(view));
  });
  await page.route(/\/api\/v1\/manage\/approval-queue/, (route) =>
    route.request().method() === 'OPTIONS'
      ? route.fallback()
      : route.fulfill(
          json({
            generatedUtc: nowUtc,
            totalCount: pending() ? 1 : 0,
            earliestHoldExpiresUtc: pending() ? depositHold : null,
            items: pending() ? [{ ...demoDepositQueueItem, depositStatus: state }] : [],
          }),
        ),
  );
  await page.route(`**/api/v1/manage/appointments/${depositId}`, (route) =>
    route.request().method() === 'OPTIONS'
      ? route.fallback()
      : route.fulfill(json(depositDetail(state))),
  );
  await page.route(
    new RegExp(`/api/v1/manage/appointments/${depositId}/(approve|deposit/mark-received)$`),
    async (route) => {
      const request = route.request();
      if (request.method() === 'OPTIONS') {
        await route.fallback();
        return;
      }

      const action = request.url().endsWith('/approve') ? 'approve' : 'mark-received';
      const body = request.postData() ? (request.postDataJSON() as unknown) : null;
      calls.push({
        action,
        ifMatch: (await request.headerValue('If-Match')) ?? undefined,
        idempotencyKey: (await request.headerValue('Idempotency-Key')) ?? undefined,
        body,
      });
      state = action === 'approve' ? 'Waived' : 'Received';
      await route.fulfill(json(depositDetail(state)));
    },
  );
  return calls;
}

async function openDeposit(page: Page) {
  await page.goto('/takvim');
  await page
    .getByRole('region', { name: 'Günlük takvim' })
    .getByRole('button', { name: /^Gizem Arslan, Fön, 14:30–15:00, Kapora bekleniyor/ })
    .click();
  const drawer = page.getByRole('dialog', { name: 'Randevu · #A-20512 · Online' });
  await expect(drawer.getByRole('region', { name: 'Kapora' })).toBeVisible();
  return drawer;
}

test.describe('Kapora: panelde kapora bekleyen randevu', () => {
  test('Kapora geldi: randevu onaylanır, If-Match ve Idempotency-Key gider (KK5)', async ({
    page,
  }) => {
    const errors: string[] = [];
    page.on('pageerror', (error) => errors.push(error.message));
    const calls = await depositDemo(page);
    const drawer = await openDeposit(page);
    await expect(drawer.getByText('OMV-7K2X')).toBeVisible();
    await expect(drawer.getByRole('button', { name: 'Onayla', exact: true })).toHaveCount(0);

    await drawer.getByRole('button', { name: 'Kapora geldi' }).click();
    await expect(page.getByText('Kapora geldi olarak işaretlendi')).toBeVisible();
    expect(calls).toHaveLength(1);
    expect(calls[0]).toMatchObject({ action: 'mark-received', ifMatch: '4', body: null });
    expect(calls[0]?.idempotencyKey).toMatch(/^[0-9a-f-]{36}$/);
    await expect(drawer.getByRole('region', { name: 'Randevu durumu' })).toContainText('Onaylandı');
    await expect(drawer.getByText('Kapora 150 ₺ · havale ile alındı')).toBeVisible();
    await page.keyboard.press('Escape');
    await expect(
      page
        .getByRole('region', { name: 'Günlük takvim' })
        .getByRole('button', { name: /^Gizem Arslan, Fön, 14:30–15:00, Onaylandı/ }),
    ).toBeVisible();
    expect(errors).toEqual([]);
  });

  test('Kaporasız onayla: waiveDeposit = true gider (KK7)', async ({ page }) => {
    const calls = await depositDemo(page);
    const drawer = await openDeposit(page);
    await drawer.getByRole('button', { name: 'Kaporasız onayla' }).click();
    await expect(page.getByText('Randevu kaporasız onaylandı')).toBeVisible();
    expect(calls.map(({ action, ifMatch, body }) => ({ action, ifMatch, body }))).toEqual([
      { action: 'approve', ifMatch: '4', body: { waiveDeposit: true } },
    ]);
    await expect(drawer.getByRole('region', { name: 'Randevu durumu' })).toContainText('Onaylandı');
    await expect(drawer.getByText('Kaporasız onaylandı').first()).toBeVisible();
  });

  test('Genel bakış: havale bildirilen satırdan "Kapora geldi" (KK4, KK5)', async ({ page }) => {
    const calls = await depositDemo(page, 'CustomerReported');
    await page.goto('/');
    const attention = page.getByRole('region', { name: 'Dikkat isteyenler' });
    await expect(attention.getByText('Gizem Arslan kapora bekliyor')).toBeVisible();
    await expect(
      attention.getByText('14:30 · Havale bildirildi · kontrol edin · 1 sa 35 dk kaldı'),
    ).toBeVisible();
    await attention.getByRole('button', { name: 'Kapora geldi: Gizem Arslan' }).click();
    await expect(page.getByText('Kapora geldi olarak işaretlendi')).toBeVisible();
    expect(calls.map((call) => [call.action, call.ifMatch])).toEqual([['mark-received', '4']]);
    await expect(page.getByText('Dikkat isteyen bir şey yok')).toBeVisible();
  });
});

const depositScreens: { name: string; state: DepositState; ready: string }[] = [
  {
    name: 'RandevuPanelKapora',
    state: 'Awaiting',
    ready: 'Banka uygulamanızda açıklama kodunu arayın.',
  },
  {
    name: 'RandevuPanelHavale',
    state: 'CustomerReported',
    ready: 'Havale bildirildi · kontrol edin',
  },
  {
    name: 'RandevuPanelKaporaAlindi',
    state: 'Received',
    ready: 'Kapora 150 ₺ · havale ile alındı',
  },
];

for (const screen of depositScreens) {
  for (const theme of ['light', 'dark'] as const) {
    test(`${screen.name} (${theme === 'light' ? 'açık' : 'koyu'})`, async ({ page }) => {
      const errors: string[] = [];
      page.on('pageerror', (error) => errors.push(error.message));
      await page.emulateMedia({ colorScheme: theme });
      await page.addInitScript((value) => {
        try {
          localStorage.setItem('omvila.tema', value);
        } catch {
          // Depolama kapalıysa sistem teması kullanılır.
        }
      }, theme);
      await depositDemo(page, screen.state);
      await page.goto('/takvim');
      await page
        .getByRole('region', { name: 'Günlük takvim' })
        .getByRole('button', { name: /^Gizem Arslan/ })
        .click();
      const drawer = page.getByRole('dialog', { name: 'Randevu · #A-20512 · Online' });
      await expect(drawer.getByText(screen.ready, { exact: false })).toBeVisible();
      await page.waitForFunction(() =>
        document.getAnimations().every((animation) => animation.playState !== 'running'),
      );
      await page.screenshot({
        path: `e2e-results/ekranlar/${screen.name}${theme === 'dark' ? 'Koyu' : ''}.png`,
        fullPage: false,
      });
      expect(errors).toEqual([]);
    });
  }
}
