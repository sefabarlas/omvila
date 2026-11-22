import type { Page } from '@playwright/test';
import { api, json, respond } from './support';

// Tasarımdaki veri (platform-paneli/POnay, POnayDetay, PAbonelikler, PDenetim, PPlanlar): Prism örnekleri bir iki
// satırlık; ekran karşılaştırması tasarımla aynı satırlarla yapılır. Bildirimler ekran gününden 2 gün önce gelmiş
// sayılır ("2 gündür bekliyor"). Fiyatlar S-14 çözülene kadar boş olabilir; burada eşleşme kuralını göstermek için
// beklenen tutar dolu verilir. Ekran anı sabittir (20 Kasım 2026 10:00 İstanbul; tarayıcı saati fixedScreenClock ile).
export const screenInstant = '2026-11-20T07:00:00.000Z';
const screenMs = Date.parse(screenInstant);

/** Tarayıcının "şimdi"sini ekran anına sabitler ("2 gündür bekliyor", "Yarın salt okunur" tasarımla aynı). */
export async function fixedScreenClock(page: Page) {
  await page.clock.setFixedTime(screenMs);
}

export const elifNoticeId = '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f9b01';
const money = (amountMinor: number) => ({ amountMinor, currency: 'TRY' });
const daysAgo = (days: number, hour = 11) => {
  const d = new Date(screenMs);
  d.setUTCDate(d.getUTCDate() - days);
  d.setUTCHours(hour, 20, 0, 0);
  return d.toISOString();
};
const dateAgo = (days: number) => daysAgo(days).slice(0, 10);

const notice = (
  id: string,
  brandName: string,
  submittedUtc: string,
  patch: Record<string, unknown>,
) => ({
  type: 'PaymentNotice',
  id,
  brandId: `0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f${id.slice(-4)}`,
  brandName,
  submittedUtc,
  paymentNotice: {
    purpose: 'Subscription',
    periodStart: '2026-11-01',
    credits: null,
    expectedAmount: money(100000),
    senderName: null,
    hasReceipt: false,
    version: 'W/"7001"',
    ...patch,
  },
  smsSender: null,
});

export function queueDemoBody() {
  return {
    paymentNoticeCount: 3,
    smsSenderCount: 1,
    oldestSubmittedUtc: daysAgo(2),
    items: [
      notice(elifNoticeId, 'Kuaför Elif', daysAgo(2), {
        transferCode: 'ABN-KE-1125',
        hasReceipt: true,
        candidate: {
          bankTransactionId: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa401',
          verdict: 'CodeMatches',
          codeFound: true,
          amountMatches: true,
        },
      }),
      notice('0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f9b02', 'Spa Moda', daysAgo(1), {
        purpose: 'Credits',
        periodStart: null,
        credits: 5000,
        expectedAmount: money(50000),
        transferCode: 'KNT-SM-0412',
        candidate: {
          bankTransactionId: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa403',
          verdict: 'CodeMatches',
          codeFound: true,
          amountMatches: true,
        },
      }),
      notice('0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f9b03', 'Berber Kaan', daysAgo(1, 8), {
        transferCode: 'ABN-BK-1125',
        senderName: 'Kaan Ö.',
        candidate: {
          bankTransactionId: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa402',
          verdict: 'CheckRequired',
          codeFound: false,
          amountMatches: true,
        },
      }),
      {
        type: 'SmsSender',
        id: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa501',
        brandId: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa202',
        brandName: 'Nail Studio Moda',
        submittedUtc: daysAgo(0, 6),
        paymentNotice: null,
        smsSender: {
          requestedName: 'NAILSTUDIO',
          currentEffectiveName: 'OMVILA',
          version: 'W/"7002"',
        },
      },
    ],
  };
}

export async function queueDemo(page: Page) {
  await fixedScreenClock(page);
  await respond(page, '/approval-queue*', queueDemoBody());
}

export async function emptyQueueDemo(page: Page) {
  await fixedScreenClock(page);
  await respond(page, '/approval-queue*', {
    paymentNoticeCount: 0,
    smsSenderCount: 0,
    oldestSubmittedUtc: null,
    items: [],
  });
}

const candidate = (
  n: string,
  senderName: string,
  description: string | null,
  days: number,
  hour: number,
  amountMinor: number,
  codeFound: boolean,
  amountMatches: boolean,
) => ({
  bankTransactionId: `0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa40${n}`,
  valueDate: dateAgo(days),
  receivedUtc: daysAgo(days, hour),
  amount: money(amountMinor),
  senderName,
  description,
  codeFound,
  amountMatches,
  verdict: codeFound && amountMatches ? 'CodeMatches' : 'CheckRequired',
});

export function noticeDetailBody() {
  return {
    id: elifNoticeId,
    brandId: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5001',
    brandName: 'Kuaför Elif',
    purpose: 'Subscription',
    planName: 'Salon',
    periodStart: '2026-11-01',
    periodEnd: '2026-11-30',
    credits: null,
    expectedAmount: money(100000),
    transferCode: 'ABN-KE-1125',
    senderName: 'Elif Kaya',
    notifiedUtc: daysAgo(2),
    channel: 'Panel',
    status: 'Pending',
    receipt: {
      fileName: 'Dekont · Ziraat Bankası',
      contentType: 'application/pdf',
      sizeBytes: 182400,
      uploadedUtc: daysAgo(2),
    },
    resolution: null,
    candidateWindow: { from: dateAgo(4), to: dateAgo(2) },
    candidates: [
      candidate('1', 'ELİF KAYA', 'ABN-KE-1125', 2, 11, 100000, true, true),
      candidate('2', 'KAAN ÖZTÜRK', null, 2, 8, 100000, false, true),
      candidate('3', 'SPA MODA GÜZ. LTD', 'KNT-SM-0412', 3, 13, 50000, false, false),
    ],
    version: 'W/"7001"',
  };
}

export async function noticeDetailDemo(page: Page) {
  await fixedScreenClock(page);
  await respond(page, `/payment-notices/${elifNoticeId}`, noticeDetailBody());
}

const subscription = (n: number, brandName: string, patch: Record<string, unknown>) => ({
  brandId: `0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa20${String(n)}`,
  brandName,
  planName: 'Salon',
  status: 'Active',
  pastDueDays: null,
  trialDaysLeft: null,
  creditBalance: 500,
  creditsLow: false,
  nextStep: { type: 'Renews', date: '2026-12-01' },
  ...patch,
});

export async function subscriptionsDemo(page: Page) {
  await fixedScreenClock(page);
  await respond(page, '/subscriptions*', {
    counts: { active: 348, trialing: 64, trialEndingThisWeek: 9, pastDue: 11, readOnly: 3 },
    items: [
      subscription(1, 'Berber Kaan', {
        status: 'PastDue',
        pastDueDays: 6,
        creditBalance: 210,
        nextStep: { type: 'ReadOnlyOn', date: dateAgo(-1) },
      }),
      subscription(2, 'Saç Atölyesi', {
        status: 'ReadOnly',
        creditBalance: 0,
        nextStep: { type: 'ReactivatesOnPayment', date: null },
      }),
      subscription(3, 'Kuaför Elif', {
        status: 'Trialing',
        trialDaysLeft: 7,
        creditBalance: 1240,
        nextStep: { type: 'TrialEnds', date: '2026-11-29' },
      }),
      subscription(4, 'Dyt. Ece Arslan', {
        planName: 'Tekil',
        status: 'Trialing',
        trialDaysLeft: 0,
        creditBalance: 320,
        nextStep: { type: 'TrialEnds', date: '2026-12-02' },
      }),
      subscription(5, 'Spa Moda', { planName: 'Salon Plus', creditBalance: 4800 }),
      subscription(6, 'Nail Studio Moda', {
        creditBalance: 0,
        creditsLow: true,
        nextStep: { type: 'SmsStopped', date: null },
      }),
    ],
    nextCursor: null,
  });
}

const person = (name: string, role: string) => ({
  type: 'PlatformUser',
  id: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa101',
  name,
  platformRole: role,
});
const system = { type: 'System', id: null, name: null, platformRole: null };
const brandRef = (n: number, name: string) => ({
  id: `0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa20${String(n)}`,
  name,
});
const minutesAgo = (minutes: number) => new Date(screenMs - minutes * 60_000).toISOString();

export async function auditDemo(page: Page) {
  await fixedScreenClock(page);
  await respond(page, '/audit-log*', {
    items: [
      {
        id: 6,
        occurredUtc: minutesAgo(10),
        actor: person('Ayşe Demir', 'PlatformSupport'),
        action: 'View',
        entityType: 'Brand',
        entityId: null,
        brand: brandRef(1, 'Kuaför Elif'),
        changes: { section: 'Summary' },
      },
      {
        id: 5,
        occurredUtc: minutesAgo(24),
        actor: person('Ayşe Demir', 'PlatformSupport'),
        action: 'Match',
        entityType: 'PaymentNotice',
        entityId: null,
        brand: brandRef(2, 'Spa Moda'),
        changes: { purpose: 'Credits', credits: 5000, transferCode: 'KNT-SM-0412' },
      },
      {
        id: 4,
        occurredUtc: minutesAgo(51),
        actor: person('Kerem Öz', 'PlatformAdmin'),
        action: 'Create',
        entityType: 'CreditTransaction',
        entityId: null,
        brand: brandRef(3, 'Nail Studio Moda'),
        changes: { credits: 500, reason: 'destek talebi #4821' },
      },
      {
        id: 3,
        occurredUtc: minutesAgo(140),
        actor: system,
        action: 'Update',
        entityType: 'Subscription',
        entityId: null,
        brand: brandRef(4, 'Saç Atölyesi'),
        changes: { status: 'ReadOnly', previousStatus: 'PastDue' },
      },
      {
        id: 2,
        occurredUtc: minutesAgo(225),
        actor: person('Kerem Öz', 'PlatformAdmin'),
        action: 'Approve',
        entityType: 'BrandSmsSender',
        entityId: null,
        brand: brandRef(5, 'Berber Kaan'),
        changes: { name: 'BERBERKAAN' },
      },
      {
        id: 1,
        occurredUtc: minutesAgo(480),
        actor: system,
        action: 'Anonymize',
        entityType: 'BrandCustomer',
        entityId: null,
        brand: brandRef(1, 'Kuaför Elif'),
        changes: { count: 1 },
      },
    ],
    nextCursor: null,
  });
}

const features = (on: boolean[]) => ({
  walkInQueue: on[0] ?? false,
  waitlist: on[1] ?? false,
  packages: on[2] ?? false,
  crossBranchReports: on[3] ?? false,
  prioritySupport: on[4] ?? false,
});

export async function plansDemo(page: Page) {
  await fixedScreenClock(page);
  const plan = (
    code: string,
    name: string,
    description: string,
    brandCount: number,
    patch: Record<string, unknown>,
  ) => ({
    code,
    name,
    description,
    monthlyPrice: null,
    yearlyPrice: null,
    includedBranchCount: 1,
    extraBranchPrice: null,
    staffLimit: null,
    includedSmsCredits: null,
    features: features([]),
    isPublic: true,
    isActive: true,
    brandCount,
    ...patch,
  });
  await respond(page, '/plans', {
    plans: [
      plan('solo', 'Tekil', 'Tek başına çalışanlar', 96, { staffLimit: 1 }),
      plan('salon', 'Salon', 'Küçük ve orta salonlar', 241, {
        features: features([true, true, true]),
      }),
      plan('salon_plus', 'Salon Plus', 'Çok şubeli işletmeler', 75, {
        includedBranchCount: null,
        features: features([true, true, true, true, true]),
      }),
    ],
    rules: { trialDays: 14, pastDueGraceDays: 7, trialCredits: null },
    version: 'W/"301"',
  });
}

export async function bankDemo(page: Page) {
  await fixedScreenClock(page);
  await respond(page, '/bank-transactions*', {
    items: [
      {
        id: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa401',
        valueDate: dateAgo(2),
        receivedUtc: daysAgo(2, 11),
        amount: money(100000),
        senderName: 'ELİF KAYA',
        description: 'ABN-KE-1125',
        bankReference: '20261118-001204',
        source: 'Import',
        matchedNoticeId: null,
        matchedBrandName: null,
      },
      {
        id: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa402',
        valueDate: dateAgo(2),
        receivedUtc: daysAgo(2, 8),
        amount: money(100000),
        senderName: 'KAAN ÖZTÜRK',
        description: null,
        bankReference: '20261118-000731',
        source: 'Import',
        matchedNoticeId: null,
        matchedBrandName: null,
      },
      {
        id: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa403',
        valueDate: dateAgo(3),
        receivedUtc: null,
        amount: money(50000),
        senderName: 'SPA MODA GÜZ. LTD',
        description: 'KNT-SM-0412',
        bankReference: '20261117-000913',
        source: 'Manual',
        matchedNoticeId: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f9b02',
        matchedBrandName: 'Spa Moda',
      },
    ],
    nextCursor: null,
  });
}

/** Eşleştirme ucu: gövdeyi ve If-Match'i yakalar, sonuçlanmış bildirimi döner. */
export async function captureMatch(
  page: Page,
  captured: { body?: unknown; ifMatch?: string | undefined },
) {
  await page.route(`${api}/payment-notices/${elifNoticeId}/match`, async (route) => {
    if (route.request().method() === 'OPTIONS') {
      await route.fulfill({
        status: 204,
        headers: {
          'Access-Control-Allow-Origin': '*',
          'Access-Control-Allow-Headers': '*',
          'Access-Control-Allow-Methods': 'POST',
        },
      });
      return;
    }

    captured.body = route.request().postDataJSON();
    captured.ifMatch = route.request().headers()['if-match'];
    await route.fulfill(
      json({
        ...noticeDetailBody(),
        status: 'Matched',
        candidates: [],
        resolution: {
          resolvedUtc: screenInstant,
          resolvedByName: 'Ayşe Demir',
          matchedBankTransactionId: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa401',
          message: null,
        },
        version: 'W/"7010"',
      }),
    );
  });
}
