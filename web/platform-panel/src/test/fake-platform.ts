import type { components } from '@omvila/api-client-ts';

// Sahte admin API'nin Dilim 6 PP2 uçları: onay kuyrukları, havale bildirimi, SMS adı, banka hareketleri, abonelikler,
// denetim kaydı, planlar ve kontör paketleri. Veriler platform-paneli/POnay, POnayDetay, PAbonelikler, PDenetim ve
// PPlanlar tasarımlarıyla aynı.
type Schemas = components['schemas'];

export const noticeIds = {
  elif: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f9b01',
  spa: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f9b02',
  kaan: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f9b03',
};
export const senderId = '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa501';
export const txIds = {
  elif: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa401',
  kaan: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa402',
  spa: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa403',
};

export interface PlatformFakeOptions {
  /** Kuyruk boş mu (POnayBos). */
  emptyQueue?: boolean;
  /** Eşleştirme yanıtı. */
  matchError?:
    'bank_transaction_already_matched' | 'concurrency_conflict' | 'invalid_state_transition';
  /** Elle hareket eklemede aynı referans. */
  duplicateReference?: boolean;
  /** Plan veya paket kaydında 412. */
  plansConflict?: boolean;
  /** CSV dosyası CSV değil (errors.file = invalid_format). */
  importInvalidFile?: boolean;
  /** Sonuçlanan bildirimler (kuyruktan düşer). */
  resolved?: Set<string>;
}

const money = (amountMinor: number) => ({ amountMinor, currency: 'TRY' as const });

const queueItems: Schemas['AdminApprovalQueueItem'][] = [
  {
    type: 'PaymentNotice',
    id: noticeIds.elif,
    brandId: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5001',
    brandName: 'Kuaför Elif',
    submittedUtc: '2026-11-18T11:20:00Z',
    paymentNotice: {
      purpose: 'Subscription',
      periodStart: '2026-11-01',
      credits: null,
      expectedAmount: money(100000),
      transferCode: 'ABN-KE-1125',
      senderName: null,
      hasReceipt: true,
      version: 'W/"7001"',
      candidate: {
        bankTransactionId: txIds.elif,
        verdict: 'CodeMatches',
        codeFound: true,
        amountMatches: true,
      },
    },
    smsSender: null,
  },
  {
    type: 'PaymentNotice',
    id: noticeIds.kaan,
    brandId: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa201',
    brandName: 'Berber Kaan',
    submittedUtc: '2026-11-19T09:00:00Z',
    paymentNotice: {
      purpose: 'Subscription',
      periodStart: '2026-11-01',
      credits: null,
      expectedAmount: money(100000),
      transferCode: 'ABN-BK-1125',
      senderName: 'Kaan Ö.',
      hasReceipt: false,
      version: 'W/"7003"',
      candidate: {
        bankTransactionId: txIds.kaan,
        verdict: 'CheckRequired',
        codeFound: false,
        amountMatches: true,
      },
    },
    smsSender: null,
  },
  {
    type: 'SmsSender',
    id: senderId,
    brandId: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa202',
    brandName: 'Nail Studio Moda',
    submittedUtc: '2026-11-19T15:00:00Z',
    paymentNotice: null,
    smsSender: { requestedName: 'NAILSTUDIO', currentEffectiveName: 'OMVILA', version: 'W/"7002"' },
  },
];

const candidate = (
  id: string,
  senderName: string,
  description: string | null,
  receivedUtc: string,
  amountMinor: number,
  codeFound: boolean,
  amountMatches: boolean,
): Schemas['BankTransactionCandidate'] => ({
  bankTransactionId: id,
  valueDate: receivedUtc.slice(0, 10),
  receivedUtc,
  amount: money(amountMinor),
  senderName,
  description,
  codeFound,
  amountMatches,
  verdict: codeFound && amountMatches ? 'CodeMatches' : 'CheckRequired',
});

const noticeDetail = (id: string): Schemas['AdminPaymentNoticeDetail'] => {
  const kaan = id === noticeIds.kaan;
  return {
    id,
    brandId: kaan ? '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa201' : '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5001',
    brandName: kaan ? 'Berber Kaan' : 'Kuaför Elif',
    purpose: 'Subscription',
    planName: 'Salon',
    periodStart: '2026-11-01',
    periodEnd: '2026-11-30',
    credits: null,
    expectedAmount: money(100000),
    transferCode: kaan ? 'ABN-BK-1125' : 'ABN-KE-1125',
    senderName: kaan ? 'Kaan Ö.' : 'Elif Kaya',
    notifiedUtc: '2026-11-18T11:20:00Z',
    channel: 'Panel',
    status: 'Pending',
    receipt: kaan
      ? null
      : {
          fileName: 'dekont-ziraat.pdf',
          contentType: 'application/pdf',
          sizeBytes: 182400,
          uploadedUtc: '2026-11-18T11:20:00Z',
        },
    resolution: null,
    candidateWindow: { from: '2026-11-11', to: '2026-11-21' },
    candidates: kaan
      ? [candidate(txIds.kaan, 'KAAN ÖZTÜRK', null, '2026-11-18T08:42:00Z', 100000, false, true)]
      : [
          candidate(
            txIds.elif,
            'ELİF KAYA',
            'ABN-KE-1125',
            '2026-11-18T11:05:00Z',
            100000,
            true,
            true,
          ),
          candidate(txIds.kaan, 'KAAN ÖZTÜRK', null, '2026-11-18T08:42:00Z', 100000, false, true),
          candidate(
            txIds.spa,
            'SPA MODA GÜZ. LTD',
            'KNT-SM-0412',
            '2026-11-17T13:10:00Z',
            50000,
            false,
            false,
          ),
        ],
    version: kaan ? 'W/"7003"' : 'W/"7001"',
  };
};

export const bankTransactionsFixture: Schemas['BankTransaction'][] = [
  {
    id: txIds.elif,
    valueDate: '2026-11-18',
    receivedUtc: '2026-11-18T11:05:00Z',
    amount: money(100000),
    senderName: 'ELİF KAYA',
    description: 'ABN-KE-1125',
    bankReference: '20261118-001204',
    source: 'Import',
    matchedNoticeId: null,
    matchedBrandName: null,
  },
  {
    id: txIds.spa,
    valueDate: '2026-11-17',
    receivedUtc: null,
    amount: money(50000),
    senderName: 'SPA MODA GÜZ. LTD',
    description: 'KNT-SM-0412',
    bankReference: '20261117-000913',
    source: 'Manual',
    matchedNoticeId: noticeIds.spa,
    matchedBrandName: 'Spa Moda',
  },
];

const subscription = (
  brandId: string,
  brandName: string,
  patch: Partial<Schemas['AdminSubscriptionListItem']>,
): Schemas['AdminSubscriptionListItem'] => ({
  brandId,
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

export const subscriptionsFixture: Schemas['AdminSubscriptionListItem'][] = [
  subscription('0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa201', 'Berber Kaan', {
    status: 'PastDue',
    pastDueDays: 6,
    creditBalance: 210,
    nextStep: { type: 'ReadOnlyOn', date: '2026-11-21' },
  }),
  subscription('0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa204', 'Saç Atölyesi', {
    status: 'ReadOnly',
    creditBalance: 0,
    nextStep: { type: 'ReactivatesOnPayment', date: null },
  }),
  subscription('0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5001', 'Kuaför Elif', {
    status: 'Trialing',
    trialDaysLeft: 9,
    creditBalance: 1240,
    nextStep: { type: 'TrialEnds', date: '2026-11-29' },
  }),
  subscription('0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa202', 'Nail Studio Moda', {
    creditBalance: 0,
    creditsLow: true,
    nextStep: { type: 'SmsStopped', date: null },
  }),
];

const actor = (name: string, role: 'PlatformSupport' | 'PlatformAdmin') => ({
  type: 'PlatformUser' as const,
  id: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa101',
  name,
  platformRole: role,
});

export const auditFixture: Schemas['AuditLogEntry'][] = [
  {
    id: 918204,
    occurredUtc: '2026-11-19T22:12:00Z',
    actor: actor('Ayşe Demir', 'PlatformSupport'),
    action: 'View',
    entityType: 'Brand',
    entityId: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5001',
    brand: { id: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5001', name: 'Kuaför Elif' },
    changes: { section: 'Summary' },
  },
  {
    id: 918203,
    occurredUtc: '2026-11-19T21:58:00Z',
    actor: actor('Ayşe Demir', 'PlatformSupport'),
    action: 'Match',
    entityType: 'PaymentNotice',
    entityId: noticeIds.spa,
    brand: { id: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa203', name: 'Spa Moda' },
    changes: { purpose: 'Credits', credits: 5000, transferCode: 'KNT-SM-0412' },
  },
  {
    id: 918190,
    occurredUtc: '2026-11-19T20:40:00Z',
    actor: { type: 'System', id: null, name: null, platformRole: null },
    action: 'Update',
    entityType: 'Subscription',
    entityId: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa601',
    brand: { id: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa204', name: 'Saç Atölyesi' },
    changes: { status: 'ReadOnly', previousStatus: 'PastDue' },
  },
];

const plan = (
  code: string,
  name: string,
  brandCount: number,
  patch: Partial<Schemas['AdminPlan']> = {},
): Schemas['AdminPlan'] => ({
  code,
  name,
  description: null,
  monthlyPrice: null,
  yearlyPrice: null,
  includedBranchCount: 1,
  extraBranchPrice: null,
  staffLimit: null,
  includedSmsCredits: null,
  features: {
    walkInQueue: false,
    waitlist: false,
    packages: false,
    crossBranchReports: false,
    prioritySupport: false,
  },
  isPublic: true,
  isActive: true,
  brandCount,
  ...patch,
});

export const planCatalogFixture: Schemas['AdminPlanCatalog'] = {
  plans: [
    plan('solo', 'Tekil', 96, { description: 'Tek başına çalışanlar', staffLimit: 1 }),
    plan('salon', 'Salon', 241, {
      description: 'Küçük ve orta salonlar',
      monthlyPrice: money(100000),
      extraBranchPrice: money(25000),
      features: {
        walkInQueue: true,
        waitlist: true,
        packages: true,
        crossBranchReports: false,
        prioritySupport: false,
      },
    }),
    plan('salon_plus', 'Salon Plus', 75, {
      description: 'Çok şubeli işletmeler',
      includedBranchCount: null,
      features: {
        walkInQueue: true,
        waitlist: true,
        packages: true,
        crossBranchReports: true,
        prioritySupport: true,
      },
    }),
  ],
  rules: { trialDays: 14, pastDueGraceDays: 7, trialCredits: null },
  version: 'W/"301"',
};

export const packageCatalogFixture: Schemas['AdminCreditPackageCatalog'] = {
  packages: [
    { code: 'starter', name: 'Başlangıç', credits: 1000, price: null, isActive: true },
    { code: 'medium', name: 'Orta', credits: 5000, price: null, isActive: true },
    { code: 'large', name: 'Büyük', credits: 20000, price: null, isActive: true },
  ],
  version: 'W/"302"',
};

function json(status: number, body: unknown): Response {
  return new Response(JSON.stringify(body), {
    status,
    headers: { 'Content-Type': 'application/json' },
  });
}

const problem = (
  status: number,
  code: string,
  extra: { meta?: Record<string, unknown>; errors?: Record<string, string[]> } = {},
) =>
  json(status, {
    type: 'about:blank',
    title: code,
    status,
    code,
    meta: extra.meta ?? {},
    ...extra,
  });

const resolvedNotice = (
  id: string,
  status: 'Matched' | 'NotFound' | 'Rejected',
  message: string | null,
): Schemas['AdminPaymentNoticeDetail'] => ({
  ...noticeDetail(id),
  status,
  candidates: [],
  resolution: {
    resolvedUtc: '2026-11-20T07:00:00Z',
    resolvedByName: 'Ayşe Demir',
    matchedBankTransactionId: status === 'Matched' ? txIds.elif : null,
    message,
  },
  version: 'W/"7010"',
});

/** PP2 uçlarını yanıtlar; tanımadığı istekte undefined (çağıran 404 verir). */
export async function platformRoute(
  key: string,
  url: URL,
  request: Request,
  options: PlatformFakeOptions,
): Promise<Response | undefined> {
  const resolved = (options.resolved ??= new Set());
  const noticeMatch =
    /^(GET|POST) \/api\/v1\/admin\/payment-notices\/([^/]+)(?:\/([a-z-]+))?$/.exec(key);
  if (noticeMatch) {
    const [, method, id = '', action] = noticeMatch;
    if (method === 'GET' && !action) return json(200, noticeDetail(id));
    if (method === 'GET' && action === 'receipt') {
      return new Response(new Blob(['%PDF-1.4'], { type: 'application/pdf' }), {
        status: 200,
        headers: {
          'Content-Type': 'application/pdf',
          'Content-Disposition': 'attachment; filename="dekont-ziraat.pdf"',
        },
      });
    }

    if (action === 'match') {
      if (options.matchError === 'concurrency_conflict')
        return problem(412, 'concurrency_conflict');
      if (options.matchError) return problem(409, options.matchError);
      resolved.add(id);
      return json(200, resolvedNotice(id, 'Matched', null));
    }

    const body = (await request
      .clone()
      .json()
      .catch(() => ({}))) as { message?: string };
    if (action === 'not-found') {
      resolved.add(id);
      return json(200, resolvedNotice(id, 'NotFound', body.message ?? null));
    }

    if (action === 'reject') {
      resolved.add(id);
      return json(200, resolvedNotice(id, 'Rejected', body.message ?? null));
    }
  }

  switch (key) {
    case 'GET /api/v1/admin/approval-queue': {
      const items = options.emptyQueue ? [] : queueItems.filter((i) => !resolved.has(i.id));
      return json(200, {
        paymentNoticeCount: items.filter((i) => i.type === 'PaymentNotice').length,
        smsSenderCount: items.filter((i) => i.type === 'SmsSender').length,
        oldestSubmittedUtc: items[0]?.submittedUtc ?? null,
        items,
      });
    }
    case `POST /api/v1/admin/sms-senders/${senderId}/approve`:
      resolved.add(senderId);
      return json(200, {
        senderId,
        brandId: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa202',
        name: 'NAILSTUDIO',
        status: 'Approved',
        decidedUtc: '2026-11-20T07:00:00Z',
        rejectionReason: null,
      });
    case `POST /api/v1/admin/sms-senders/${senderId}/reject`: {
      const body = (await request.clone().json()) as { reason: string };
      resolved.add(senderId);
      return json(200, {
        senderId,
        brandId: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa202',
        name: 'NAILSTUDIO',
        status: 'Rejected',
        decidedUtc: '2026-11-20T07:00:00Z',
        rejectionReason: body.reason,
      });
    }
    case 'GET /api/v1/admin/bank-transactions': {
      const q = url.searchParams.get('q')?.toLocaleLowerCase('tr');
      const unmatched = url.searchParams.get('unmatchedOnly') === 'true';
      const items = bankTransactionsFixture.filter(
        (tx) =>
          (!unmatched || tx.matchedNoticeId === null) &&
          (!q ||
            `${tx.senderName} ${tx.description ?? ''} ${tx.bankReference}`
              .toLocaleLowerCase('tr')
              .includes(q)),
      );
      return json(200, { items, nextCursor: null });
    }
    case 'POST /api/v1/admin/bank-transactions': {
      if (options.duplicateReference) {
        return problem(409, 'bank_transaction_duplicate', {
          meta: { bankTransactionId: txIds.elif },
        });
      }

      const body = (await request.clone().json()) as Schemas['BankTransactionCreate'];
      return json(201, {
        id: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa499',
        receivedUtc: null,
        description: null,
        ...body,
        source: 'Manual',
        matchedNoticeId: null,
        matchedBrandName: null,
      });
    }
    case 'POST /api/v1/admin/bank-transactions/import': {
      if (options.importInvalidFile) {
        return problem(400, 'validation_failed', { errors: { file: ['invalid_format'] } });
      }

      // jsdom'da formData() ayrıştırması güvenilmez; ham multipart gövdesine bakılır (salon paneli deseni).
      const contentType = request.headers.get('Content-Type') ?? '';
      const raw = await request.clone().text();
      if (!contentType.startsWith('multipart/form-data') || !raw.includes('name="file"')) {
        return problem(400, 'validation_failed', { errors: { file: ['invalid_format'] } });
      }

      const dryRun = !/name="dryRun"\r?\n\r?\nfalse/.test(raw);
      return json(200, {
        dryRun,
        fileName: 'ekstre-2026-11-18.csv',
        totalRows: 42,
        imported: 38,
        duplicates: 3,
        skippedOutgoing: 0,
        invalidRows: 1,
        errors: [{ rowNumber: 17, column: 'Tutar', code: 'invalid_format' }],
      });
    }
    case 'GET /api/v1/admin/subscriptions': {
      const filter = url.searchParams.get('filter') ?? 'All';
      const items = subscriptionsFixture.filter((s) =>
        filter === 'All' ? true : filter === 'CreditsLow' ? s.creditsLow : s.status === filter,
      );
      return json(200, {
        counts: { active: 348, trialing: 64, trialEndingThisWeek: 9, pastDue: 11, readOnly: 3 },
        items,
        nextCursor: null,
      });
    }
    case 'GET /api/v1/admin/audit-log': {
      const category = url.searchParams.get('category');
      const brand = url.searchParams.get('brandId');
      const items = auditFixture.filter((e) =>
        brand && e.brand?.id !== brand
          ? false
          : category === 'View'
            ? e.action === 'View'
            : category === 'ApproveReject'
              ? e.action === 'Match' || e.action === 'Approve'
              : category === 'CreditsAndPlan'
                ? e.entityType === 'Subscription' || e.entityType === 'CreditTransaction'
                : category === 'Kvkk'
                  ? e.action === 'Anonymize'
                  : true,
      );
      return json(200, { items, nextCursor: null });
    }
    case 'GET /api/v1/admin/audit-log/export':
      return new Response('﻿Zaman;Kim\n', {
        status: 200,
        headers: {
          'Content-Type': 'text/csv',
          'Content-Disposition': 'attachment; filename="denetim-kaydi-2026-11-20.csv"',
        },
      });
    case 'GET /api/v1/admin/plans':
      return json(200, planCatalogFixture);
    case 'PUT /api/v1/admin/plans': {
      if (options.plansConflict) return problem(412, 'concurrency_conflict');
      const body = (await request.clone().json()) as Schemas['AdminPlanCatalogUpdate'];
      return json(200, {
        ...planCatalogFixture,
        plans: planCatalogFixture.plans.map((p) => ({
          ...p,
          ...body.plans.find((u) => u.code === p.code),
        })),
        version: 'W/"303"',
      });
    }
    case 'GET /api/v1/admin/credit-packages':
      return json(200, packageCatalogFixture);
    case 'PUT /api/v1/admin/credit-packages': {
      if (options.plansConflict) return problem(412, 'concurrency_conflict');
      const body = (await request.clone().json()) as Schemas['AdminCreditPackageCatalogUpdate'];
      return json(200, { packages: body.packages, version: 'W/"304"' });
    }
    default:
      return undefined;
  }
}
