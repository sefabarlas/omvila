import 'package:flutter/widgets.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_riverpod/misc.dart';
import 'package:go_router/go_router.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/providers.dart';
import 'package:omvila_musteri/app/router.dart';
import 'package:omvila_musteri/features/booking/domain/booking_draft.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Akışın seçimleri; "Hangi salon?" veya "Aynısını tekrar al" başlatır.
final NotifierProvider<BookingController, BookingDraft?> bookingProvider =
    NotifierProvider<BookingController, BookingDraft?>(BookingController.new);

/// "Aynısını tekrar al" (KK2): akış önceki randevuyla dolar ve saat adımı
/// açılır; geri tuşu hizmet adımına döner.
void aynisiniTekrarAl(BuildContext context, WidgetRef ref, BookingDraft draft) {
  ref.read(bookingProvider.notifier).start(draft);
  context
    ..push<void>(Routes.yrHizmet)
    ..push<void>(Routes.yrSaat);
}

/// Şubenin kataloğu.
final FutureProviderFamily<PublicCatalog, String> catalogProvider =
    FutureProvider.autoDispose.family<PublicCatalog, String>(
      (ref, branchId) => ref.watch(bookingRepositoryProvider).catalog(branchId),
    );

/// Kullanılabilir paket seansları; okunamazsa boş (paketten düşme isteğe
/// bağlı bir kolaylık, akışı durdurmaz).
final FutureProviderFamily<List<UsablePackageItem>, String>
usablePackagesProvider = FutureProvider.autoDispose
    .family<List<UsablePackageItem>, String>((ref, branchId) async {
      try {
        return await ref.watch(bookingRepositoryProvider).packages(branchId);
      } on ApiProblem {
        return const [];
      }
    });

/// Taslağın değişiklikleri.
class BookingController extends Notifier<BookingDraft?> {
  @override
  BookingDraft? build() => null;

  /// Akışı başlatır (önceki seçimler silinir).
  // Akışın tek giriş noktası; ayarlayıcı yerine açık ad.
  // ignore: use_setters_to_change_properties
  void start(BookingDraft draft) => state = draft;

  BookingDraft get _d => state!;

  /// Hizmeti seçer veya bırakır; kişi, gün ve saat sıfırlanır. Hizmet için
  /// kullanılabilir paket kalemi ([packageItemId]) varsa "Paketten"
  /// işaretlenir.
  void toggleService(String serviceId, {String? packageItemId}) {
    final ids = [..._d.serviceIds];
    final packages = {..._d.packageItems}..remove(serviceId);
    if (!ids.remove(serviceId)) {
      ids.add(serviceId);
      if (packageItemId != null) {
        packages[serviceId] = packageItemId;
      }
    }

    state = _d.copyWith(
      serviceIds: ids,
      staff: const {},
      packageItems: packages,
      clearDate: true,
    );
  }

  /// Bütün hizmetlere aynı kişi ([staffId] null: "Fark etmez").
  void chooseStaff(String? staffId) => state = _d.copyWith(
    staff: {for (final id in _d.serviceIds) id: staffId},
    clearSlot: true,
  );

  /// Tek hizmete kişi.
  void chooseStaffFor(String serviceId, String? staffId) => state = _d.copyWith(
    staff: {..._d.staff, serviceId: staffId},
    clearSlot: true,
  );

  /// "İki hizmet de aynı kişiden".
  void setSameStaff({required bool value}) => state = _d.copyWith(
    sameStaffForAll: value,
    staff: const {},
    clearSlot: true,
  );

  /// Gün.
  void chooseDate(String date) =>
      state = date == _d.date ? _d : _d.copyWith(date: date, clearSlot: true);

  /// Saat.
  void chooseSlot(Slot slot) => state = _d.copyWith(slot: slot);

  /// Paket kalemi seçimi.
  void usePackage(String serviceId, String? customerPackageItemId) {
    final next = {..._d.packageItems};
    if (customerPackageItemId == null) {
      next.remove(serviceId);
    } else {
      next[serviceId] = customerPackageItemId;
    }

    state = _d.copyWith(packageItems: next);
  }
}
