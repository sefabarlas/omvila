// Ayarlar → Bildirimler ve SMS uçlarının sahtesi (manageGet/UpdateNotificationSettings, manageGetSmsOverview,
// manageRequestSmsSender). Veriler sözleşme örnekleriyle aynı.

export interface NotificationSettingsFakeOptions {
  /** Kaydetme yanıtı: 412 kayıt çakışması veya ilk gönderilen şablon için 400 too_long. */
  saveError?: 'concurrency_conflict' | 'template_too_long';
  /** Gönderen adı durumu (varsayılan: onaylı KUAFORELIF). */
  senderStatus?: 'None' | 'Pending' | 'Approved' | 'Rejected';
  /** Kontör tahmini yok (yeni marka, tüketim geçmişi yok). */
  noEstimate?: boolean;
}

export function notificationSettingsFixture() {
  return {
    reminderOffsetsMinutes: [1440, 120],
    reviewInviteEnabled: true,
    templates: [
      {
        eventCode: 'appointment.confirmed',
        variant: null,
        channel: 'Sms',
        systemBody:
          '{salon}: Randevunuz onaylandı. {tarih} {saat}, {hizmet} ({personel}). Değişiklik ve iptal: {bağlantı}',
        customBody:
          'Merhaba {ad}, {tarih} {saat} randevunuz onaylandı. Değişiklik ve iptal: {bağlantı}',
        variables: ['salon', 'ad', 'tarih', 'saat', 'hizmet', 'personel', 'bağlantı'],
        requiredVariables: ['bağlantı'],
        maxLength: 155,
      },
      {
        eventCode: 'appointment.reminder',
        variant: null,
        channel: 'Sms',
        systemBody:
          "Hatırlatma: Yarın {saat}'da {salon}'te randevunuz var. Değişiklik için: {bağlantı}",
        customBody: null,
        variables: ['salon', 'ad', 'tarih', 'saat', 'hizmet', 'personel', 'bağlantı'],
        requiredVariables: ['bağlantı'],
        maxLength: 155,
      },
    ],
    version: 'W/"9120"',
  };
}

function smsOverview(options: NotificationSettingsFakeOptions) {
  const status = options.senderStatus ?? 'Approved';
  return {
    creditBalance: 1240,
    estimatedDaysLeft: options.noEstimate ? null : 18,
    lowCreditThreshold: 200,
    isLow: false,
    sentThisMonth: {
      month: '2026-11-01',
      smsCount: 862,
      creditsUsed: 862,
      notificationCount: 1904,
    },
    sender: {
      name: status === 'None' ? null : 'KUAFORELIF',
      status,
      effectiveName: status === 'Approved' ? 'KUAFORELIF' : 'OMVILA',
      submittedUtc: status === 'None' ? null : '2026-11-03T10:12:00Z',
      decidedUtc: null,
      rejectionReason: status === 'Rejected' ? 'Ad, işletme unvanıyla eşleşmiyor.' : null,
    },
  };
}

/** İstek bu sahtenin uçlarındansa yanıt, değilse null. */
export function createNotificationSettingsFake(options: NotificationSettingsFakeOptions = {}) {
  const json = (status: number, body: unknown) =>
    new Response(JSON.stringify(body), {
      status,
      headers: { 'Content-Type': 'application/json' },
    });
  return async (request: Request): Promise<Response | null> => {
    const path = new URL(request.url).pathname;
    if (path === '/api/v1/manage/brand/notification-settings') {
      const settings = notificationSettingsFixture();
      if (request.method === 'GET') {
        return json(200, settings);
      }

      if (options.saveError === 'concurrency_conflict') {
        return json(412, { code: 'concurrency_conflict' });
      }

      if (options.saveError === 'template_too_long') {
        return json(400, {
          code: 'validation_failed',
          errors: { 'templates[0].customBody': ['too_long'] },
        });
      }

      const body = (await request.clone().json()) as {
        reminderOffsetsMinutes: number[];
        reviewInviteEnabled: boolean;
        templates?: { eventCode: string; variant: string | null; customBody: string | null }[];
      };
      return json(200, {
        ...settings,
        reminderOffsetsMinutes: body.reminderOffsetsMinutes,
        reviewInviteEnabled: body.reviewInviteEnabled,
        templates: settings.templates.map((t) => {
          const sent = body.templates?.find((s) => s.eventCode === t.eventCode);
          return sent ? { ...t, customBody: sent.customBody } : t;
        }),
        version: 'W/"9121"',
      });
    }

    if (path === '/api/v1/manage/brand/sms') {
      return json(200, smsOverview(options));
    }

    if (path === '/api/v1/manage/brand/sms-sender') {
      const { name } = (await request.clone().json()) as { name: string };
      return json(201, {
        name,
        status: 'Pending',
        effectiveName: options.senderStatus === 'Approved' ? 'KUAFORELIF' : 'OMVILA',
        submittedUtc: '2026-11-25T09:30:00Z',
        decidedUtc: null,
        rejectionReason: null,
      });
    }

    return null;
  };
}
