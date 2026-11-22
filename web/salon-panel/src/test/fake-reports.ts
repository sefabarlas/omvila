import type { components } from '@omvila/api-client-ts';
import type { ManageSession } from '@/app/api';

// Sahte rapor uçları (manageGetOverview, manageGetReportOverview, manageGetCommissionReport,
// manageGetMyCommissionSummary). Veriler salon-paneli/Raporlar, GenelBakis ve işletme uygulaması Profil'den; kurgusal
// kişiler; gün Çarşamba 25 Kasım 2026 (İstanbul).
type ManageOverview = components['schemas']['ManageOverview'];
type ReportOverview = components['schemas']['ReportOverview'];
type CommissionReport = components['schemas']['CommissionReport'];
type MyCommissionSummary = components['schemas']['MyCommissionSummary'];

export interface ReportsFakeOptions {
  /** Raporda hiç veri yok (RaporlarBos). */
  empty?: boolean;
  /** Rapor ucu 500 döner. */
  fails?: boolean;
  /** Primim: kullanıcının personel kaydı yok (404). */
  noStaffRecord?: boolean;
  /** Genel bakışta hiç çalışma dakikası yok (doluluk null). */
  noWorkingMinutes?: boolean;
}

const money = (amountMinor: number) => ({ amountMinor, currency: 'TRY' as const });
const json = (status: number, body: unknown) =>
  new Response(JSON.stringify(body), { status, headers: { 'Content-Type': 'application/json' } });
const staffIds = {
  elif: '0193a8f0-0000-7000-8000-0000000000c1',
  mert: '0193a8f0-0000-7000-8000-0000000000c2',
  can: '0193a8f0-0000-7000-8000-0000000000c3',
  zeynep: '0193a8f0-0000-7000-8000-0000000000c4',
};

/** "Günlük tahsilat": Pazartesi kapalı (salon-paneli/Raporlar). */
const dailyAmounts: Record<string, number> = {
  '2026-11-24': 1120000,
  '2026-11-25': 950000,
};

function addDay(day: string, days: number): string {
  const d = new Date(`${day}T00:00:00Z`);
  d.setUTCDate(d.getUTCDate() + days);
  return d.toISOString().slice(0, 10);
}

function reportFixture(branchId: string | null, from: string, to: string, empty: boolean) {
  const days: ReportOverview['dailyCollections'] = [];
  for (let day = from; day <= to; day = addDay(day, 1)) {
    const closed = new Date(`${day}T00:00:00Z`).getUTCDay() === 1;
    days.push({
      date: day,
      amountMinor: empty || closed ? 0 : (dailyAmounts[day] ?? 0),
      isClosed: closed,
    });
  }

  const report: ReportOverview = empty
    ? {
        branchId,
        timeZone: 'Europe/Istanbul',
        from,
        to,
        earnedRevenue: money(0),
        completedAppointmentCount: 0,
        earnedRevenueChangePercent: null,
        collections: money(0),
        noShow: { count: 0, percent: null, lostRevenue: money(0) },
        online: { appointmentCount: 0, percent: null },
        dailyCollections: days,
        currency: 'TRY',
      }
    : {
        branchId,
        timeZone: 'Europe/Istanbul',
        from,
        to,
        earnedRevenue: money(6120000),
        completedAppointmentCount: 118,
        earnedRevenueChangePercent: 9,
        collections: money(6795000),
        noShow: { count: 5, percent: 4, lostRevenue: money(215000) },
        online: { appointmentCount: 54, percent: 46 },
        dailyCollections: days,
        currency: 'TRY',
      };
  return report;
}

function commissionFixture(
  session: ManageSession,
  branchId: string | null,
  from: string,
  to: string,
  empty: boolean,
): CommissionReport {
  const row = (
    staffId: string,
    staffName: string,
    count: number,
    revenue: number,
    pct: number,
  ) => ({
    staffId,
    staffName,
    appointmentCount: count,
    serviceRevenueMinor: revenue,
    serviceCommissionPercent: pct,
    serviceCommissionMinor: Math.round((revenue * pct) / 100),
    saleCommissionMinor: 0,
    totalCommissionMinor: Math.round((revenue * pct) / 100),
  });
  const all = empty
    ? []
    : [
        row(staffIds.elif, 'Elif Kaya', 38, 2430000, 24),
        row(staffIds.can, 'Can Yücel', 27, 1620000, 22),
        row(staffIds.mert, 'Mert Demir', 31, 1595000, 22),
        row(staffIds.zeynep, 'Zeynep Aksoy', 22, 1150000, 24),
      ];
  // Personel (report.view yok) yalnız kendi satırını görür.
  const items = session.permissions.includes('report.view')
    ? all
    : all.filter((r) => r.staffId === session.membership.staffId);
  return {
    branchId,
    from,
    to,
    currency: 'TRY',
    items,
    totalCommissionMinor: items.reduce((sum, r) => sum + r.totalCommissionMinor, 0),
  };
}

function overviewFixture(
  session: ManageSession,
  branchId: string,
  noWorkingMinutes: boolean,
): ManageOverview {
  const can = (permission: string) => session.permissions.includes(permission);
  return {
    branchId,
    branchName: session.branches.find((b) => b.id === branchId)?.name ?? '',
    timeZone: 'Europe/Istanbul',
    date: '2026-11-25',
    generatedUtc: '2026-11-25T09:10:00Z',
    todayRevenue: money(334000),
    todayAppointmentCount: 7,
    pendingApproval: can('appointment.approve')
      ? { count: 1, earliestHoldExpiresUtc: '2026-11-25T10:45:00Z' }
      : null,
    walkIns: can('walkin.manage') ? { waitingCount: 3, estimatedWaitMinutesForNew: 35 } : null,
    weeklyOccupancy: noWorkingMinutes
      ? { weekStart: '2026-11-23', percent: null, previousWeekPercent: null, staff: [] }
      : {
          weekStart: '2026-11-23',
          percent: 74,
          previousWeekPercent: 68,
          staff: [
            {
              staffId: staffIds.elif,
              staffName: 'Elif Kaya',
              calendarColor: '#0F6E5C',
              percent: 86,
            },
            {
              staffId: staffIds.mert,
              staffName: 'Mert Demir',
              calendarColor: '#C2410C',
              percent: 72,
            },
            {
              staffId: staffIds.can,
              staffName: 'Can Yücel',
              calendarColor: '#1D4ED8',
              percent: 68,
            },
            {
              staffId: staffIds.zeynep,
              staffName: 'Zeynep Aksoy',
              calendarColor: '#8B7355',
              percent: 61,
            },
          ],
        },
  };
}

function myCommissionFixture(month: string, empty: boolean): MyCommissionSummary {
  const entry = (
    id: string,
    amountMinor: number,
    occurredUtc: string,
    customerName: string,
    description: string,
    status: 'Accrued' | 'Reversed' = 'Accrued',
  ) => ({
    id,
    type: 'Service' as const,
    amountMinor,
    status,
    occurredUtc,
    customerName,
    description,
    appointmentId: null,
    customerPackageId: null,
  });
  return {
    staffId: staffIds.elif,
    staffName: 'Elif Kaya',
    timeZone: 'Europe/Istanbul',
    month,
    from: `${month}-01`,
    to: '2026-11-25',
    totalMinor: empty ? 0 : 684000,
    serviceRevenueMinor: empty ? 0 : 2850000,
    currency: 'TRY',
    appointmentCount: empty ? 0 : 94,
    noShowPercent: empty ? null : 4,
    averageRating: empty ? null : 4.9,
    daily: [],
    thisWeek: empty
      ? []
      : [
          { date: '2026-11-23', amountMinor: 0, appointmentCount: 0 },
          { date: '2026-11-24', amountMinor: 42000, appointmentCount: 6 },
          { date: '2026-11-25', amountMinor: 9000, appointmentCount: 2 },
        ],
    entries: empty
      ? []
      : [
          entry(
            '0193a8f0-0000-7000-8000-00000000a101',
            9000,
            '2026-11-25T06:30:00Z',
            'Ayşe Yılmaz',
            'Saç kesimi',
          ),
          entry(
            '0193a8f0-0000-7000-8000-00000000a102',
            42000,
            '2026-11-24T14:00:00Z',
            'Buse Kaya',
            'Keratin',
          ),
          entry(
            '0193a8f0-0000-7000-8000-00000000a103',
            -9000,
            '2026-11-20T08:00:00Z',
            'Okan Er',
            'Saç kesimi',
            'Reversed',
          ),
          entry(
            '0193a8f0-0000-7000-8000-00000000a104',
            12000,
            '2026-11-19T08:00:00Z',
            'Derya Şahin',
            'Fön',
          ),
          entry(
            '0193a8f0-0000-7000-8000-00000000a105',
            30000,
            '2026-11-18T08:00:00Z',
            'Aslı Kurt',
            'Balyaj',
          ),
          entry(
            '0193a8f0-0000-7000-8000-00000000a106',
            9000,
            '2026-11-17T08:00:00Z',
            'Melis Tunç',
            'Saç kesimi',
          ),
        ],
    nextAccrual: empty
      ? null
      : { appointmentId: '0193a8f0-0000-7000-8000-000000000001', customerName: 'Selin Öztürk' },
  };
}

/** Rapor uçlarını karşılar; ilgisiz istekte null döner (fakeFetch devam eder). */
export function createReportsFake(session: ManageSession, options: ReportsFakeOptions = {}) {
  const can = (permission: string) => session.permissions.includes(permission);
  return (request: Request): Response | null => {
    if (request.method !== 'GET') {
      return null;
    }

    const url = new URL(request.url);
    const path = url.pathname;
    const branchId = url.searchParams.get('branchId');
    const from = url.searchParams.get('from') ?? '';
    const to = url.searchParams.get('to') ?? '';

    if (path === '/api/v1/manage/overview') {
      return can('report.view')
        ? json(
            200,
            overviewFixture(
              session,
              branchId ?? session.branches[0]?.id ?? '',
              options.noWorkingMinutes ?? false,
            ),
          )
        : json(403, { code: 'forbidden' });
    }

    if (path === '/api/v1/manage/reports/overview') {
      if (!can('report.view')) {
        return json(403, { code: 'forbidden' });
      }

      return options.fails
        ? json(500, { code: 'internal_error' })
        : json(200, reportFixture(branchId, from, to, options.empty ?? false));
    }

    if (path === '/api/v1/manage/reports/commissions') {
      return can('commission.view')
        ? json(200, commissionFixture(session, branchId, from, to, options.empty ?? false))
        : json(403, { code: 'forbidden' });
    }

    if (path === '/api/v1/manage/me/commission-summary') {
      if (!can('commission.view')) {
        return json(403, { code: 'forbidden' });
      }

      return options.noStaffRecord
        ? json(404, { code: 'not_found' })
        : json(
            200,
            myCommissionFixture(url.searchParams.get('month') ?? '2026-11', options.empty ?? false),
          );
    }

    return null;
  };
}
