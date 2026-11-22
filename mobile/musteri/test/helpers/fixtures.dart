import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/features/appointments/data/appointments_repository.dart';
import 'package:omvila_ui/omvila_ui.dart';

// Test verisi (gerçek kişi yok). "Şimdi": 24 Kasım 2026 Salı 20:30 İstanbul.

/// Testlerin saati.
final testClock = FixedClock.parse('2026-11-24T17:30:00Z');

Money tl(int lira) =>
    Money(amountMinor: lira * 100, currency: MoneyCurrencyEnum.TRY);

MeAppointmentItem item(
  String service,
  String start,
  int minutes, {
  String? staff,
  int price = 0,
}) {
  final startUtc = DateTime.parse(start);
  return MeAppointmentItem(
    serviceId: 'svc-$service',
    serviceName: service,
    staffId: staff == null ? null : 'stf-$staff',
    staffName: staff,
    startUtc: startUtc,
    endUtc: startUtc.add(Duration(minutes: minutes)),
    price: tl(price),
    paidFromPackage: false,
  );
}

MeAppointmentSummary appointment({
  required String id,
  required String brand,
  required String start,
  required List<MeAppointmentItem> items,
  DisplayStatus status = DisplayStatus.confirmed,
  String district = 'Kadıköy',
  int dueLira = 0,
  bool canRebook = true,
}) {
  final startUtc = DateTime.parse(start);
  final end = items.map((i) => i.endUtc).reduce((a, b) => a.isAfter(b) ? a : b);
  return MeAppointmentSummary(
    id: id,
    brand: MeBrandRef(id: 'brand-$id', name: brand, slug: 'salon-$id'),
    branch: MeBranchRef(id: 'branch-$id', name: district, district: district),
    timeZone: 'Europe/Istanbul',
    startUtc: startUtc,
    endUtc: end,
    status: switch (status) {
      DisplayStatus.pendingApproval => AppointmentStatus.pendingApproval,
      DisplayStatus.completed => AppointmentStatus.completed,
      _ => AppointmentStatus.confirmed,
    },
    displayStatus: status,
    items: items,
    total: tl(dueLira),
    amountDue: tl(dueLira),
    review: null,
    canReview: false,
    canRebook: canRebook,
  );
}

/// Yarın 14:30 Kuaför Elif (Main.png kahraman kartı).
final MeAppointmentSummary yarinKuaforElif = appointment(
  id: 'a1',
  brand: 'Kuaför Elif',
  start: '2026-11-25T11:30:00Z',
  items: [
    item('Saç kesimi', '2026-11-25T11:30:00Z', 45, staff: 'Elif Kaya'),
    item('Fön', '2026-11-25T12:15:00Z', 30, staff: 'Elif Kaya'),
  ],
  dueLira: 1440,
);

/// 3 Aralık 11:00 Nail Studio Moda, onay bekliyor.
final MeAppointmentSummary nailStudio = appointment(
  id: 'a2',
  brand: 'Nail Studio Moda',
  start: '2026-12-03T08:00:00Z',
  status: DisplayStatus.pendingApproval,
  items: [item('Manikür', '2026-12-03T08:00:00Z', 40, staff: 'Gizem')],
  dueLira: 350,
);

/// 12 Kasım Kuaför Elif, tamamlandı.
final MeAppointmentSummary gecenKuaforElif = appointment(
  id: 'a0',
  brand: 'Kuaför Elif',
  start: '2026-11-12T11:30:00Z',
  status: DisplayStatus.completed,
  items: [
    item('Saç kesimi', '2026-11-12T11:30:00Z', 45, staff: 'Elif Kaya'),
    item('Fön', '2026-11-12T12:15:00Z', 30, staff: 'Elif Kaya'),
  ],
);

HomeData homeData({
  String? firstName = 'Merve',
  List<MeAppointmentSummary> upcoming = const [],
  MeAppointmentSummary? last,
  bool hasBrands = true,
}) => HomeData(
  firstName: firstName,
  upcoming: upcoming,
  last: last,
  hasBrands: hasBrands,
);

final HomeData doluHome = homeData(upcoming: [yarinKuaforElif, nailStudio]);
final HomeData bosHome = homeData(hasBrands: false);
final HomeData yaklasanYokHome = homeData(last: gecenKuaforElif);

MeBrand brand(
  String name, {
  String slug = 'kuafor-elif',
  String? lastVisit = '2026-11-12',
  List<MeBrandPackageBadge> packages = const [],
}) => MeBrand(
  id: 'brand-$slug',
  name: name,
  slug: slug,
  logoUrl: null,
  coverColor: MeBrandCoverColorEnum.kum,
  bookingEnabled: true,
  hasBooked: lastVisit != null,
  savedUtc: null,
  lastVisitOn: lastVisit == null ? null : DateTime.parse(lastVisit),
  branches: [
    MeBranchRef(id: 'br-$slug', name: 'Kadıköy', district: 'Kadıköy'),
  ],
  activePackages: packages,
);

MeAppointment appointmentDetail({num? latitude, num? longitude}) =>
    MeAppointment(
      id: 'a1',
      brand: MeBrandRef(id: 'b', name: 'Kuaför Elif', slug: 'kuafor-elif'),
      branch: MeAppointmentBranch(
        id: 'br',
        name: 'Kadıköy',
        addressLine: 'Moda Caddesi 10',
        district: 'Kadıköy',
        latitude: latitude,
        longitude: longitude,
      ),
      timeZone: 'Europe/Istanbul',
      startUtc: yarinKuaforElif.startUtc,
      endUtc: yarinKuaforElif.endUtc,
      status: AppointmentStatus.confirmed,
      displayStatus: DisplayStatus.confirmed,
      items: yarinKuaforElif.items,
      total: tl(1440),
      amountDue: tl(1440),
      deposit: null,
      canCancel: true,
      cancelDeadlineUtc: null,
      canReschedule: true,
      rescheduleDeadlineUtc: null,
      canReview: false,
      canRebook: true,
      review: null,
      cancellation: null,
      version: '1',
    );
