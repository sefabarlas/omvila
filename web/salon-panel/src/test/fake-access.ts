import type { components } from '@omvila/api-client-ts';

// Sahte kullanıcı, rol, KVKK ayarı, veri dışa aktarma ve Google Takvim uçları (Dilim 6; salon-paneli/AyarlarRoller,
// AyarlarKvkk, Personel). Kurgusal kişiler; sözleşme örnekleriyle aynı biçim.
type ManageUser = components['schemas']['ManageUser'];
type PrivacySettings = components['schemas']['PrivacySettings'];
type GoogleCalendarConnection = components['schemas']['GoogleCalendarConnection'];

export interface AccessFakeOptions {
  /** Kullanıcı listesi boş. */
  noUsers?: boolean;
  /** Kullanıcı ve rol uçları 500 döner. */
  fails?: boolean;
  /** Rol kaydı yanıtı: 409 last_owner_required, 412 veya 400 branchIds. */
  updateError?: 'last_owner_required' | 'concurrency_conflict' | 'branch_required';
  /** KVKK ayarları ucu 500 döner. */
  privacyFails?: boolean;
  /** Kayıtlı aydınlatma metni modu. */
  noticeMode?: 'Standard' | 'Custom';
  /** Oturumdaki personelin Google Takvim durumu; 'missing' = personel kaydı yok (404). */
  google?: GoogleCalendarConnection['status'] | 'missing';
  /** Geri dönüşte sunucu state'i reddeder (400 validation_failed). */
  googleCallbackInvalid?: boolean;
}

const json = (status: number, body: unknown) =>
  new Response(JSON.stringify(body), { status, headers: { 'Content-Type': 'application/json' } });
const problem = (status: number, code: string, errors?: Record<string, string[]>) =>
  json(status, { code, title: code, status, ...(errors ? { errors } : {}) });

export const userIds = {
  elif: '0193a8f0-0000-7000-8000-000000000001',
  can: '0193a8f0-0000-7000-8000-000000000003',
  deniz: '0193a8f0-0000-7000-8000-000000000004',
};
const merkez = '0193a8f0-0000-7000-8000-0000000000b1';

export function usersFixture(): ManageUser[] {
  return [
    {
      userId: userIds.can,
      fullName: 'Can Yücel',
      phoneMasked: '0533 *** ** 12',
      role: 'BranchManager',
      branchIds: [merkez],
      status: 'Active',
      staffId: '0193a8f0-0000-7000-8000-0000000000c3',
      isCurrentUser: false,
      version: 'W/"31"',
    },
    {
      userId: userIds.deniz,
      fullName: 'Deniz Kara',
      phoneMasked: '0533 *** ** 18',
      role: 'Staff',
      branchIds: [merkez],
      status: 'Invited',
      staffId: '0193a8f0-0000-7000-8000-0000000000c5',
      isCurrentUser: false,
      version: 'W/"44"',
    },
    {
      userId: userIds.elif,
      fullName: 'Elif Kaya',
      phoneMasked: '0532 *** ** 05',
      role: 'Owner',
      branchIds: [],
      status: 'Active',
      staffId: '0193a8f0-0000-7000-8000-0000000000c1',
      isCurrentUser: true,
      version: 'W/"8812"',
    },
  ];
}

export const roleMatrixFixture = {
  rows: [
    {
      group: 'CalendarAndAppointments',
      permissions: ['calendar.view'],
      owner: 'All',
      branchManager: 'Branch',
      staff: 'Own',
    },
    {
      group: 'CustomerPhone',
      permissions: ['customer.phone.view'],
      owner: 'All',
      branchManager: 'All',
      staff: 'None',
    },
    {
      group: 'CustomerCardAndNotes',
      permissions: ['customer.view', 'customer.edit'],
      owner: 'All',
      branchManager: 'All',
      staff: 'All',
    },
    {
      group: 'CashAndPayments',
      permissions: ['payment.collect', 'cash.close'],
      owner: 'All',
      branchManager: 'Branch',
      staff: 'None',
    },
    {
      group: 'ReportsAndCommission',
      permissions: ['report.view', 'commission.view'],
      owner: 'All',
      branchManager: 'Branch',
      staff: 'Own',
    },
    {
      group: 'ServicesAndPrices',
      permissions: ['service.manage'],
      owner: 'All',
      branchManager: 'All',
      staff: 'None',
    },
    {
      group: 'Settings',
      permissions: ['settings.manage', 'settings.payment', 'settings.roles'],
      owner: 'All',
      branchManager: 'None',
      staff: 'None',
    },
  ],
};

function privacyFixture(mode: 'Standard' | 'Custom'): PrivacySettings {
  return {
    noticeMode: mode,
    standardNotice: {
      version: '2026-11-01',
      updatedOn: '2026-11-01',
      text: 'Kuaför Elif olarak kişisel verilerinizi randevu hizmeti sunmak amacıyla işliyoruz.',
    },
    customNoticeText: mode === 'Custom' ? 'Kuaför Elif aydınlatma metni.' : null,
    customNoticeUpdatedUtc: mode === 'Custom' ? '2026-11-10T09:00:00Z' : null,
    transactionRetentionYears: 3,
    allowedRetentionYears: [3, 5, 10],
    version: 'W/"4410"',
  };
}

function connection(status: GoogleCalendarConnection['status']): GoogleCalendarConnection {
  return status === 'Active' || status === 'TokenExpired'
    ? {
        status,
        accountEmailMasked: 'el***@gmail.com',
        connectedUtc: '2026-11-12T08:30:00Z',
        lastSyncedUtc: '2026-11-25T08:55:00Z',
      }
    : { status, accountEmailMasked: null, connectedUtc: null, lastSyncedUtc: null };
}

export function createAccessFake(options: AccessFakeOptions) {
  let users = options.noUsers ? [] : usersFixture();
  let privacy = privacyFixture(options.noticeMode ?? 'Standard');
  let google = options.google ?? 'NotConnected';
  return async (request: Request): Promise<Response | undefined> => {
    const url = new URL(request.url);
    const path = url.pathname;
    const method = request.method;

    if (path === '/api/v1/manage/users' && method === 'GET') {
      return options.fails ? problem(500, 'try_again') : json(200, { items: users });
    }

    if (path === '/api/v1/manage/roles' && method === 'GET') {
      return options.fails ? problem(500, 'try_again') : json(200, roleMatrixFixture);
    }

    const userMatch = /^\/api\/v1\/manage\/users\/([^/]+)$/.exec(path);
    if (userMatch && method === 'PATCH') {
      switch (options.updateError) {
        case 'last_owner_required':
          return problem(409, 'last_owner_required');
        case 'concurrency_conflict':
          return problem(412, 'concurrency_conflict');
        case 'branch_required':
          return problem(400, 'validation_failed', { branchIds: ['required'] });
        default: {
          const body = (await request.json()) as Pick<ManageUser, 'role' | 'branchIds'>;
          const user = users.find((u) => u.userId === userMatch[1]);
          if (!user) {
            return problem(404, 'not_found');
          }

          const saved = {
            ...user,
            role: body.role,
            branchIds: body.role === 'Owner' ? [] : body.branchIds,
            version: 'W/"9999"',
          };
          users = users.map((u) => (u.userId === saved.userId ? saved : u));
          return json(200, saved);
        }
      }
    }

    if (path === '/api/v1/manage/brand/privacy') {
      if (method === 'GET') {
        return options.privacyFails ? problem(500, 'try_again') : json(200, privacy);
      }

      if (method === 'PUT') {
        const body = (await request.json()) as {
          noticeMode: 'Standard' | 'Custom';
          customNoticeText?: string | null;
          transactionRetentionYears: number;
        };
        privacy = {
          ...privacy,
          noticeMode: body.noticeMode,
          ...(body.customNoticeText !== undefined
            ? {
                customNoticeText: body.customNoticeText,
                customNoticeUpdatedUtc: '2026-11-25T09:00:00Z',
              }
            : {}),
          transactionRetentionYears: body.transactionRetentionYears,
          version: 'W/"4411"',
        };
        return json(200, privacy);
      }
    }

    if (path === '/api/v1/manage/brand/data-exports' && method === 'POST') {
      return json(202, {
        exportId: '0193a8f0-0000-7000-8000-000000009d01',
        status: 'Queued',
        requestedUtc: '2026-11-25T09:00:00Z',
        emailMasked: 'el***@ornek.com',
      });
    }

    if (path === '/api/v1/manage/integrations/google') {
      if (google === 'missing') {
        return problem(404, 'not_found');
      }

      if (method === 'GET') {
        return json(200, connection(google));
      }

      if (method === 'DELETE') {
        google = 'Revoked';
        return new Response(null, { status: 204 });
      }
    }

    if (path === '/api/v1/manage/integrations/google/connect-url' && method === 'GET') {
      return json(200, {
        url: 'http://localhost:5173/ayarlar/google-takvim?code=fake-kod&state=ornek-state',
        state: 'ornek-state',
        expiresUtc: '2026-11-25T09:10:00Z',
      });
    }

    if (path === '/api/v1/manage/integrations/google/callback' && method === 'POST') {
      if (options.googleCallbackInvalid) {
        return problem(400, 'validation_failed', { state: ['invalid_format'] });
      }

      google = 'Active';
      return json(200, connection('Active'));
    }

    return undefined;
  };
}
