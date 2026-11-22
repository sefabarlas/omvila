import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/features/profile/data/profile_repository.dart';

import 'fixtures.dart';

// Paketler, ödemeler, profil, izinler, bildirimler ve bekleme teklifi test
// verisi (gerçek kişi yok). "Şimdi": 24 Kasım 2026 20:30 İstanbul.

MeBrandRef ref(String name, [String slug = 'kuafor-elif']) =>
    MeBrandRef(id: 'brand-$slug', name: name, slug: slug);

MePackage paket(
  String name, {
  required MeBrandRef brand,
  required int total,
  required int left,
  String expires = '2026-12-31',
  String? branch = 'Kadıköy',
  bool usable = true,
  bool low = false,
  String? finished,
}) => MePackage(
  id: 'pkg-$name',
  brand: brand,
  branchName: branch,
  isAvailableAllBranches: branch == null,
  packageName: name,
  status: usable ? CustomerPackageStatus.active : CustomerPackageStatus.expired,
  items: [
    CustomerPackageItemView(
      customerPackageItemId: 'cpi-$name',
      serviceId: 'svc-$name',
      serviceName: name,
      totalSessions: total,
      remainingSessions: left,
    ),
  ],
  totalSessions: total,
  remainingSessions: left,
  expiresOn: DateTime.parse(expires),
  isUsable: usable,
  isRunningLow: low,
  finishedOn: finished == null ? null : DateTime.parse(finished),
);

/// Paketler.png.
MePackageList paketler() => MePackageList(
  activeCount: 2,
  items: [
    paket(
      'Fön paketi',
      brand: ref('Kuaför Elif'),
      total: 5,
      left: 2,
      low: true,
    ),
    paket(
      'Lazer epilasyon',
      brand: ref('Spa Moda', 'spa-moda'),
      total: 8,
      left: 2,
      branch: null,
      low: true,
      expires: '2027-03-15',
    ),
    paket(
      'Cilt bakımı',
      brand: ref('Kuaför Elif'),
      total: 4,
      left: 0,
      usable: false,
      finished: '2026-09-20',
    ),
  ],
);

MePaymentRow odeme(
  String id, {
  required MePaymentKind kind,
  required MeBrandRef brand,
  required int lira,
  required String at,
  PaymentMethod method = PaymentMethod.transfer,
  MePaymentStatus status = MePaymentStatus.recorded,
  String? appointmentStart,
  List<String> services = const [],
  String? packageName,
  int? sessions,
}) => MePaymentRow(
  id: id,
  kind: kind,
  status: status,
  brand: brand,
  amount: tl(lira),
  method: method,
  occurredUtc: DateTime.parse(at),
  timeZone: 'Europe/Istanbul',
  appointmentStartUtc: appointmentStart == null
      ? null
      : DateTime.parse(appointmentStart),
  serviceNames: services,
  packageName: packageName,
  packageSessionCount: sessions,
);

/// Odemelerim.png.
MePaymentPage odemeler({String? next}) => MePaymentPage(
  nextCursor: next,
  items: [
    odeme(
      'p1',
      kind: MePaymentKind.deposit,
      brand: ref('Spa Moda', 'spa-moda'),
      lira: 200,
      at: '2026-11-28T09:00:00Z',
      status: MePaymentStatus.awaitingConfirmation,
    ),
    odeme(
      'p2',
      kind: MePaymentKind.deposit,
      brand: ref('Kuaför Elif'),
      lira: 150,
      at: '2026-11-24T09:00:00Z',
      appointmentStart: '2026-11-26T11:00:00Z',
    ),
    odeme(
      'p3',
      kind: MePaymentKind.appointmentPayment,
      brand: ref('Kuaför Elif'),
      lira: 1240,
      at: '2026-11-12T12:00:00Z',
      method: PaymentMethod.card,
      services: ['Boya', 'Fön'],
    ),
    odeme(
      'p4',
      kind: MePaymentKind.deposit,
      brand: ref('Spa Moda', 'spa-moda'),
      lira: 200,
      at: '2026-10-01T09:00:00Z',
      status: MePaymentStatus.refunded,
    ),
    odeme(
      'p5',
      kind: MePaymentKind.packagePurchase,
      brand: ref('Kuaför Elif'),
      lira: 1000,
      at: '2026-10-02T09:00:00Z',
      packageName: 'Fön paketi',
      sessions: 5,
    ),
  ],
);

MeProfile profil({String? name = 'Merve Yıldız', String? email}) => MeProfile(
  id: 'cus-1',
  fullName: name,
  reviewerDisplayName: name == null ? null : 'Merve Y.',
  phone: '+905326112467',
  email: email,
  language: MeLanguage.tr,
  version: 'v1',
);

List<MeConsent> izinler() => [
  MeConsent(
    brand: ref('Kuaför Elif'),
    marketingGranted: true,
    updatedUtc: DateTime.parse('2026-10-01T09:00:00Z'),
  ),
  MeConsent(
    brand: ref('Nail Studio Moda', 'nail-studio'),
    marketingGranted: false,
    updatedUtc: null,
  ),
];

MeNotification bildirim(
  String id, {
  required MeNotificationEvent event,
  required String title,
  required String body,
  required String at,
  required MeNotificationTarget target,
  bool read = false,
}) => MeNotification(
  id: id,
  event: event,
  title: title,
  body: body,
  createdUtc: DateTime.parse(at),
  readUtc: read ? DateTime.parse(at) : null,
  brand: null,
  target: target,
);

/// Bildirimler.png.
MeNotificationPage bildirimler() => MeNotificationPage(
  nextCursor: null,
  unreadCount: 2,
  items: [
    bildirim(
      'n4',
      event: MeNotificationEvent.waitlistPeriodOffer,
      title: 'Beklediğiniz saat boşaldı',
      body: 'Spa Moda · Cuma 16:00. Saat kısa süre sizin için tutuluyor.',
      at: '2026-11-24T17:20:00Z',
      target: MeNotificationTarget(
        type: MeNotificationTargetTypeEnum.waitlistOffer,
        accessCode: 'TEKLIF1',
      ),
    ),
    bildirim(
      'n1',
      event: MeNotificationEvent.appointmentPeriodConfirmed,
      title: 'Randevunuz onaylandı',
      body: 'Kuaför Elif · Yarın 14:30. Görüşmek üzere.',
      at: '2026-11-24T09:00:00Z',
      target: MeNotificationTarget(
        type: MeNotificationTargetTypeEnum.appointment,
        appointmentId: 'a1',
      ),
    ),
    bildirim(
      'n2',
      event: MeNotificationEvent.appointmentPeriodRequested,
      title: 'Talebiniz alındı',
      body:
          'Nail Studio Moda talebinizi inceliyor. Onaylayınca haber '
          'vereceğiz.',
      at: '2026-11-23T09:00:00Z',
      read: true,
      target: MeNotificationTarget(
        type: MeNotificationTargetTypeEnum.appointment,
        appointmentId: 'a2',
      ),
    ),
    bildirim(
      'n3',
      event: MeNotificationEvent.reviewPeriodInvite,
      title: 'Nasıldı?',
      body: "Kuaför Elif'teki randevunuzu değerlendirin.",
      at: '2026-11-12T15:00:00Z',
      read: true,
      target: MeNotificationTarget(
        type: MeNotificationTargetTypeEnum.review,
        appointmentId: 'a0',
      ),
    ),
  ],
);

MeNotificationPage bosBildirimler() =>
    MeNotificationPage(items: const [], nextCursor: null, unreadCount: 0);

/// BeklemeTeklif.png: Cuma 27 Kasım 16:00 Spa Moda; 12:48 kaldı.
PublicWaitlistOffer teklif({
  WaitlistOfferStatus status = WaitlistOfferStatus.pending,
  int depositLira = 0,
  String? appointmentCode,
}) => PublicWaitlistOffer(
  id: 'off-1',
  status: status,
  brand: PublicWaitlistOfferBrand(name: 'Spa Moda', slug: 'spa-moda'),
  branch: PublicWaitlistOfferBranch(id: 'br-spa', name: 'Moda'),
  startUtc: DateTime.parse('2026-11-27T13:00:00Z'),
  endUtc: DateTime.parse('2026-11-27T14:00:00Z'),
  timeZone: 'Europe/Istanbul',
  holdExpiresUtc: DateTime.parse('2026-11-24T17:42:48Z'),
  generatedUtc: DateTime.parse('2026-11-24T17:30:00Z'),
  items: [
    PublicWaitlistOfferItemsInner(
      serviceName: 'Masaj',
      staffName: 'Seda Kurt',
      durationMinutes: 60,
      price: tl(900),
    ),
  ],
  total: tl(900),
  amountDue: tl(900 - depositLira),
  requiresApproval: depositLira > 0,
  depositAmount: depositLira > 0 ? tl(depositLira) : null,
  appointmentAccessCode: appointmentCode,
  waitlistAccessCode: 'W3nB8xQ2',
  version: 'o1',
);

DeletionPreview silmeOnizleme({bool upcoming = true, int packages = 1}) =>
    DeletionPreview(
      upcoming: upcoming ? [yarinKuaforElif, nailStudio] : const [],
      usablePackages: packages,
    );

OtpChallengeResponse silmeKodu() => OtpChallengeResponse(
  otpId: 'otp-sil',
  expiresInSeconds: 180,
  resendAfterSeconds: 60,
  phoneMasked: '0532 *** ** 67',
);

PublicWaitlistEntry beklemeKaydi() => PublicWaitlistEntry(
  id: 'wl-1',
  accessCode: 'W3nB8xQ2',
  status: WaitlistEntryStatus.waiting,
  branchId: 'br-kuafor-elif',
  timeZone: 'Europe/Istanbul',
  serviceNames: const ['Saç kesimi'],
  acceptOtherStaff: false,
  dates: const [],
  timeWindows: const [],
);
