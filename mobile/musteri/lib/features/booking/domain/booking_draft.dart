import 'package:flutter/foundation.dart';
import 'package:omvila_api/omvila_api.dart';

/// Randevu alma akışının seçimleri (Hizmet → Personel → Saat → Onay).
@immutable
class BookingDraft {
  /// Taslak.
  const BookingDraft({
    required this.brandSlug,
    required this.brandName,
    required this.branchId,
    required this.branchName,
    this.serviceIds = const [],
    this.sameStaffForAll = true,
    this.staff = const {},
    this.date,
    this.slot,
    this.packageItems = const {},
  });

  /// "Aynısını tekrar al" (KK2): aynı şube, hizmetler ve kişiler.
  factory BookingDraft.rebook({
    required MeBrandRef brand,
    required String branchId,
    required String branchName,
    required List<MeAppointmentItem> items,
  }) {
    final staffIds = {for (final i in items) i.staffId};
    return BookingDraft(
      brandSlug: brand.slug,
      brandName: brand.name,
      branchId: branchId,
      branchName: branchName,
      serviceIds: [for (final i in items) i.serviceId],
      sameStaffForAll: staffIds.length <= 1,
      staff: {for (final i in items) i.serviceId: i.staffId},
    );
  }

  /// Salonun bağlantı adı.
  final String brandSlug;

  /// "Kuaför Elif".
  final String brandName;

  /// Şube.
  final String branchId;

  /// "Kadıköy".
  final String branchName;

  /// Seçilen hizmetler, seçim sırasıyla.
  final List<String> serviceIds;

  /// "İki hizmet de aynı kişiden".
  final bool sameStaffForAll;

  /// Hizmet → personel; değer null ise "Fark etmez". Anahtarı olmayan
  /// hizmette kişi henüz seçilmedi.
  final Map<String, String?> staff;

  /// Seçili yerel gün.
  final String? date;

  /// Seçili saat.
  final Slot? slot;

  /// Paketten düşülecek hizmetler: hizmet → müşteri paket kalemi.
  final Map<String, String> packageItems;

  /// Her hizmet için kişi seçildi (veya "Fark etmez").
  bool get staffChosen => serviceIds.every(staff.containsKey);

  /// Arama ve oluşturma kalemleri.
  List<AppointmentItemRequest> get items => [
    for (final id in serviceIds)
      AppointmentItemRequest(serviceId: id, staffId: staff[id]),
  ];

  /// Kopya; [clearSlot] saati, [clearDate] günü siler.
  BookingDraft copyWith({
    List<String>? serviceIds,
    bool? sameStaffForAll,
    Map<String, String?>? staff,
    String? date,
    Slot? slot,
    Map<String, String>? packageItems,
    bool clearSlot = false,
    bool clearDate = false,
  }) => BookingDraft(
    brandSlug: brandSlug,
    brandName: brandName,
    branchId: branchId,
    branchName: branchName,
    serviceIds: serviceIds ?? this.serviceIds,
    sameStaffForAll: sameStaffForAll ?? this.sameStaffForAll,
    staff: staff ?? this.staff,
    date: clearDate ? null : (date ?? this.date),
    slot: clearSlot || clearDate ? null : (slot ?? this.slot),
    packageItems: packageItems ?? this.packageItems,
  );
}

/// Seçimin tutarı ve süresi (alt çubuk, Onay özeti).
@immutable
class BookingSummary {
  const BookingSummary._({
    required this.services,
    required this.durationMinutes,
    required this.totalMinor,
    required this.depositMinor,
    required this.startingFrom,
    required this.pricesHidden,
  });

  /// [catalog] ve [draft]'tan; paketten düşülen hizmet tutara girmez.
  factory BookingSummary.of(PublicCatalog catalog, BookingDraft draft) {
    final byId = {for (final s in catalog.services) s.id: s};
    final services = [
      for (final id in draft.serviceIds)
        if (byId[id] case final s?) s,
    ];
    var total = 0;
    var deposit = 0;
    var startingFrom = false;
    var hidden = false;
    for (final s in services) {
      if (draft.packageItems.containsKey(s.id)) {
        continue;
      }

      final price = s.price;
      if (price == null) {
        hidden = true;
        continue;
      }

      total += price.amountMinor + _surcharge(catalog, draft.staff[s.id], s.id);
      deposit += s.hasDeposit ? (s.depositHint?.amountMinor ?? 0) : 0;
      startingFrom |=
          s.priceDisplay ==
          PublicCatalogServicesInnerPriceDisplayEnum.startingFrom;
    }

    return BookingSummary._(
      services: services,
      durationMinutes: services.fold(0, (sum, s) => sum + s.durationMinutes),
      totalMinor: total,
      depositMinor: deposit,
      startingFrom: startingFrom,
      pricesHidden: hidden,
    );
  }

  /// Seçilen hizmetler.
  final List<PublicCatalogServicesInner> services;

  /// Toplam süre.
  final int durationMinutes;

  /// Toplam (paket hariç, kişi farkı dahil).
  final int totalMinor;

  /// Bilgi amaçlı kapora (kesin tutar oluşturmada sunucudan).
  final int depositMinor;

  /// Bir hizmet "…'den" fiyatlı.
  final bool startingFrom;

  /// Salon fiyat göstermiyor.
  final bool pricesHidden;

  /// Hizmetin kişi farkı ("+50 ₺").
  static int surchargeOf(
    PublicCatalog catalog,
    String? staffId,
    String serviceId,
  ) => _surcharge(catalog, staffId, serviceId);

  static int _surcharge(PublicCatalog catalog, String? staffId, String id) {
    if (staffId == null) {
      return 0;
    }

    for (final st in catalog.staff) {
      if (st.id == staffId) {
        for (final x in st.surcharges ?? const <Never>[]) {
          if (x.serviceId == id) {
            return x.amount.amountMinor;
          }
        }
      }
    }

    return 0;
  }
}

/// Sayının ardına iyelik eki: 2 → "2'si", 3 → "3'ü", 10 → "10'u".
String sayiIyelik(int n) {
  const birler = ["'i", "'si", "'ü", "'ü", "'i", "'sı", "'si", "'i", "'u"];
  const onlar = ["'u", "'si", "'u", "'ı", "'si", "'ı", "'i", "'i", "'ı"];
  if (n == 0) {
    return "0'ı";
  }

  if (n % 10 != 0) {
    return '$n${birler[n % 10 - 1]}';
  }

  if (n % 1000 == 0) {
    return "$n'i";
  }

  if (n % 100 == 0) {
    return "$n'ü";
  }

  return '$n${onlar[(n ~/ 10) % 10 - 1]}';
}
