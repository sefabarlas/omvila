import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/features/appointments/domain/review_target.dart';
import 'package:omvila_musteri/features/booking/domain/booking_draft.dart';

import 'fixtures.dart';

// Randevu alma ve randevu yönetimi test verisi (gerçek kişi yok). Saat:
// fixtures.dart testClock (24 Kasım 2026 Salı 20:30 İstanbul).

const istanbul = 'Europe/Istanbul';

PublicCatalogServicesInner service(
  String id,
  String name,
  String category,
  int minutes,
  int lira, {
  bool startingFrom = false,
  String? description,
  List<String> staff = const ['elif', 'zeynep', 'mert'],
  int depositLira = 0,
}) => PublicCatalogServicesInner(
  id: id,
  categoryId: category,
  name: name,
  description: description,
  durationMinutes: minutes,
  price: tl(lira),
  priceDisplay: startingFrom
      ? PublicCatalogServicesInnerPriceDisplayEnum.startingFrom
      : PublicCatalogServicesInnerPriceDisplayEnum.fixed,
  hasDeposit: depositLira > 0,
  depositHint: depositLira > 0 ? tl(depositLira) : null,
  staffIds: staff,
);

/// Kuaför Elif Kadıköy kataloğu (YRHizmet, YRPersonel).
PublicCatalog kuaforElifKatalog({int depositLira = 0}) => PublicCatalog(
  branchId: 'br-kuafor-elif',
  timeZone: istanbul,
  allowStaffSelection: true,
  allowAnyStaffOption: true,
  categories: [
    PublicCatalogCategoriesInner(id: 'sac', name: 'Saç'),
    PublicCatalogCategoriesInner(id: 'renk', name: 'Renk'),
    PublicCatalogCategoriesInner(id: 'bakim', name: 'Bakım'),
  ],
  services: [
    service('kesim', 'Saç kesimi', 'sac', 45, 450, depositLira: depositLira),
    service('fon', 'Fön', 'sac', 30, 250),
    service(
      'bakim',
      'Saç bakımı',
      'sac',
      40,
      600,
      description: 'Keratin maske',
    ),
    service(
      'boya',
      'Dip boya',
      'renk',
      90,
      1100,
      startingFrom: true,
      staff: ['elif', 'zeynep'],
    ),
  ],
  staff: [
    PublicCatalogStaffInner(
      id: 'elif',
      displayName: 'Elif Kaya',
      title: 'Kurucu',
    ),
    PublicCatalogStaffInner(
      id: 'zeynep',
      displayName: 'Zeynep Aksoy',
      title: 'Saç ve renk uzmanı',
      surcharges: [
        PublicCatalogStaffInnerSurchargesInner(
          serviceId: 'kesim',
          amount: tl(50),
        ),
      ],
    ),
    PublicCatalogStaffInner(
      id: 'mert',
      displayName: 'Mert Demir',
      title: 'Kesim ve sakal',
    ),
  ],
);

/// Fön paketi: 5 seansın 2'si kaldı.
final UsablePackageItem fonPaketi = UsablePackageItem(
  customerPackageItemId: 'cpi-fon',
  serviceId: 'fon',
  packageName: 'Fön paketi',
  remainingSessions: 2,
  totalSessions: 5,
);

/// Akışın taslağı: Kuaför Elif Kadıköy.
BookingDraft taslak({
  List<String> services = const [],
  Map<String, String?> staff = const {},
  Map<String, String> packages = const {},
  bool same = true,
  String? date,
  Slot? slot,
}) => BookingDraft(
  brandSlug: 'kuafor-elif',
  brandName: 'Kuaför Elif',
  branchId: 'br-kuafor-elif',
  branchName: 'Kadıköy',
  serviceIds: services,
  staff: staff,
  packageItems: packages,
  sameStaffForAll: same,
  date: date,
  slot: slot,
);

/// Gün şeridi: bugün dolu, 25–28 boş saat var, 29 kapalı.
DaySearchResult gunler({bool allFull = false, DateTime? nextUtc}) =>
    DaySearchResult(
      timeZone: istanbul,
      days: [
        for (var i = 0; i <= 30; i++)
          DaySearchResultDaysInner(
            date: DateTime.utc(2026, 11, 24 + i),
            hasAvailability: !allFull && i >= 1 && i <= 4,
            isClosed: i == 5,
          ),
      ],
      nextAvailableUtc: nextUtc,
    );

/// Bir saat (Elif Kaya; 75 dk).
Slot saat(String date, String local, {String staff = 'Elif Kaya'}) {
  final [h, m] = local.split(':').map(int.parse).toList();
  final [y, mo, d] = date.split('-').map(int.parse).toList();
  final start = DateTime.utc(y, mo, d, h - 3, m);
  return Slot(
    startUtc: start,
    localTime: local,
    period: h < 12
        ? SlotPeriodEnum.morning
        : h < 17
        ? SlotPeriodEnum.afternoon
        : SlotPeriodEnum.evening,
    items: [
      SlotItemsInner(
        serviceId: 'kesim',
        staffId: 'elif',
        staffName: staff,
        startUtc: start,
        endUtc: start.add(const Duration(minutes: 45)),
      ),
      SlotItemsInner(
        serviceId: 'fon',
        staffId: 'elif',
        staffName: staff,
        startUtc: start.add(const Duration(minutes: 45)),
        endUtc: start.add(const Duration(minutes: 75)),
      ),
    ],
    total: tl(450),
  );
}

/// 25 Kasım saatleri (YRSaat): 12:00, 13:30, 16:00 dolu.
SlotSearchResult saatler([String date = '2026-11-25']) => SlotSearchResult(
  timeZone: istanbul,
  date: DateTime.parse(date),
  slots: [
    for (final t in ['12:45', '14:30', '15:15', '16:45', '17:30'])
      saat(date, t),
  ],
  unavailableTimes: const ['12:00', '13:30', '16:00'],
);

/// Boş gün.
SlotSearchResult bosSaatler(String date) => SlotSearchResult(
  timeZone: istanbul,
  date: DateTime.parse(date),
  slots: const [],
  unavailableTimes: const ['10:00', '11:00'],
);

/// Kapora talimatı: 150 ₺, son 2 saat sonra.
DepositInstructions havaleBilgisi() => DepositInstructions(
  accountHolderName: 'Elif Kaya',
  bankName: 'Ziraat Bankası',
  iban: 'TR12 0006 4000 0011 2345 6789 01',
  amount: tl(150),
  transferCode: 'OMV-7K2X',
  dueUtc: DateTime.utc(2026, 11, 24, 19, 28),
);

MeAppointmentDeposit kapora(DepositStatus status, {bool instructions = true}) =>
    MeAppointmentDeposit(
      required_: tl(150),
      status: status,
      transferCode: 'OMV-7K2X',
      instructions: instructions ? havaleBilgisi() : null,
    );

/// Randevu detayı (RandevuDetay): Çarşamba 26 Kasım 14:30, Kuaför Elif.
MeAppointment detay({
  DisplayStatus status = DisplayStatus.confirmed,
  MeAppointmentDeposit? deposit,
  bool canCancel = true,
  bool canReschedule = true,
  bool canReview = false,
  bool canRebook = true,
  MeAppointmentReview? review,
  String start = '2026-11-25T11:30:00Z',
  int dueLira = 500,
  String? phone = '+902163450012',
}) {
  final startUtc = DateTime.parse(start);
  return MeAppointment(
    id: 'a1',
    reference: 'A-20512',
    brand: MeBrandRef(id: 'b', name: 'Kuaför Elif', slug: 'kuafor-elif'),
    branch: MeAppointmentBranch(
      id: 'br-kuafor-elif',
      name: 'Kadıköy',
      addressLine: 'Moda Cad. 42',
      district: 'Kadıköy',
      phone: phone,
      latitude: 40.9871,
      longitude: 29.0262,
    ),
    timeZone: istanbul,
    startUtc: startUtc,
    endUtc: startUtc.add(const Duration(minutes: 75)),
    status: switch (status) {
      DisplayStatus.pendingApproval ||
      DisplayStatus.depositPending => AppointmentStatus.pendingApproval,
      DisplayStatus.completed => AppointmentStatus.completed,
      DisplayStatus.cancelled => AppointmentStatus.cancelled,
      _ => AppointmentStatus.confirmed,
    },
    displayStatus: status,
    items: [
      MeAppointmentItem(
        serviceId: 'kesim',
        serviceName: 'Saç kesimi',
        staffId: 'elif',
        staffName: 'Elif Kaya',
        startUtc: startUtc,
        endUtc: startUtc.add(const Duration(minutes: 45)),
        price: tl(450),
        paidFromPackage: false,
      ),
      MeAppointmentItem(
        serviceId: 'fon',
        serviceName: 'Fön',
        staffId: 'elif',
        staffName: 'Elif Kaya',
        startUtc: startUtc.add(const Duration(minutes: 45)),
        endUtc: startUtc.add(const Duration(minutes: 75)),
        price: tl(250),
        paidFromPackage: false,
      ),
    ],
    total: tl(700),
    amountDue: tl(dueLira),
    deposit: deposit,
    canCancel: canCancel,
    cancelDeadlineUtc: startUtc.subtract(const Duration(hours: 24)),
    canReschedule: canReschedule,
    rescheduleDeadlineUtc: startUtc.subtract(const Duration(hours: 12)),
    canReview: canReview,
    canRebook: canRebook,
    review: review,
    cancellation: null,
    version: 'W/"9012"',
  );
}

/// Oluşturma yanıtı.
PublicAppointment olusan({
  DisplayStatus status = DisplayStatus.confirmed,
  bool deposit = false,
}) => PublicAppointment(
  id: 'yeni-1',
  accessCode: '7Kx2Pq9a',
  manageUrl: 'https://omvila.com/r/7Kx2Pq9a',
  status: status == DisplayStatus.confirmed
      ? AppointmentStatus.confirmed
      : AppointmentStatus.pendingApproval,
  displayStatus: status,
  brand: PublicAppointmentBrand(name: 'Kuaför Elif', slug: 'kuafor-elif'),
  branch: PublicAppointmentBranch(
    id: 'br-kuafor-elif',
    name: 'Kadıköy',
    addressLine: 'Moda Cad. 42',
    district: 'Kadıköy',
  ),
  startUtc: DateTime.utc(2026, 11, 25, 11, 30),
  endUtc: DateTime.utc(2026, 11, 25, 12, 45),
  timeZone: istanbul,
  items: const [],
  total: tl(700),
  amountDue: tl(700),
  deposit: deposit
      ? PublicAppointmentDeposit(
          required_: tl(150),
          status: DepositStatus.awaiting,
          instructions: havaleBilgisi(),
        )
      : null,
  canCancel: true,
  canReschedule: true,
  version: '1',
);

/// Geçmiş satırı.
MeAppointmentSummary gecmisSatiri({
  required String id,
  required String brand,
  required String start,
  required String service,
  int lira = 450,
  DisplayStatus status = DisplayStatus.completed,
  int? rating,
  bool canReview = false,
}) {
  final base = appointment(
    id: id,
    brand: brand,
    start: start,
    status: status,
    items: [item(service, start, 45, staff: 'Elif Kaya', price: lira)],
    dueLira: lira,
  );
  return MeAppointmentSummary(
    id: base.id,
    brand: base.brand,
    branch: base.branch,
    timeZone: base.timeZone,
    startUtc: base.startUtc,
    endUtc: base.endUtc,
    status: status == DisplayStatus.cancelled
        ? AppointmentStatus.cancelled
        : AppointmentStatus.completed,
    displayStatus: status,
    items: base.items,
    total: tl(lira),
    amountDue: tl(0),
    review: rating == null
        ? null
        : MeAppointmentReview(
            rating: rating,
            createdUtc: DateTime.parse(start).add(const Duration(hours: 3)),
          ),
    canReview: canReview,
    canRebook: true,
  );
}

/// Geçmiş (Gecmis.png).
List<MeAppointmentSummary> gecmisListesi() => [
  gecmisSatiri(
    id: 'g1',
    brand: 'Kuaför Elif',
    start: '2026-11-12T11:00:00Z',
    service: 'Boya ve fön',
    lira: 1440,
    canReview: true,
  ),
  gecmisSatiri(
    id: 'g2',
    brand: 'Nail Studio Moda',
    start: '2026-11-04T08:00:00Z',
    service: 'Manikür',
    lira: 350,
    rating: 5,
  ),
  gecmisSatiri(
    id: 'g3',
    brand: 'Diyetisyen Ece Arslan',
    start: '2026-10-09T08:00:00Z',
    service: 'Kontrol seansı',
    lira: 600,
    rating: 4,
  ),
  gecmisSatiri(
    id: 'g4',
    brand: 'Spa Moda',
    start: '2026-10-02T08:00:00Z',
    service: 'Masaj',
    status: DisplayStatus.cancelled,
  ),
];

/// Değerlendirme hedefi (Degerlendir.png).
ReviewTarget degerlendirme({int? existing, String? code}) => ReviewTarget(
  appointmentId: code == null ? 'g1' : null,
  accessCode: code,
  brandName: 'Kuaför Elif',
  serviceNames: const ['Boya ve fön'],
  staffName: 'Elif Kaya',
  startUtc: DateTime.utc(2026, 11, 12, 11),
  timeZone: istanbul,
  displayName: 'Merve Y.',
  existingRating: existing,
);
