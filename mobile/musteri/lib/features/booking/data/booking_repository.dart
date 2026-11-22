import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/shared/api_call.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Randevu alma: public katalog, gün/saat ve paket uçları ile
/// `publicCreateAppointment`, müşteri jetonuyla (`meBearer`; kod adımı yok).
abstract interface class BookingRepository {
  /// Salon ve şubeleri.
  Future<PublicBrand> brand(String brandSlug);

  /// Şubenin online hizmetleri ve personeli.
  Future<PublicCatalog> catalog(String branchId);

  /// Gün şeridi [from]…[to] (yerel günler, en çok 31 gün).
  Future<DaySearchResult> days(
    String branchId,
    List<AppointmentItemRequest> items, {
    required bool sameStaffForAll,
    required String from,
    required String to,
  });

  /// Bir günün saatleri.
  Future<SlotSearchResult> slots(
    String branchId,
    List<AppointmentItemRequest> items, {
    required bool sameStaffForAll,
    required String date,
  });

  /// Bu salonda kullanılabilir paket seansları.
  Future<List<UsablePackageItem>> packages(String branchId);

  /// Profildeki ad; henüz yoksa null (Onay'da ad sorulur).
  Future<String?> profileName();

  /// Randevuyu oluşturur; [idempotencyKey] aynı denemede sabit kalır.
  Future<PublicAppointment> create(
    String branchId,
    PublicAppointmentCreate body, {
    required String idempotencyKey,
  });
}

/// Üretilmiş istemciyle.
final class ApiBookingRepository implements BookingRepository {
  /// [api] müşteri jetonuyla.
  const ApiBookingRepository(this.api);

  /// API.
  final OmvilaApi api;

  PublicBookingApi get _booking => api.getPublicBookingApi();

  @override
  Future<PublicBrand> brand(String brandSlug) => apiCall(() async {
    final response = await _booking.publicGetBrand(brandSlug: brandSlug);
    return response.data!;
  });

  @override
  Future<PublicCatalog> catalog(String branchId) => apiCall(() async {
    final response = await _booking.publicGetBranchCatalog(branchId: branchId);
    return response.data!;
  });

  @override
  Future<DaySearchResult> days(
    String branchId,
    List<AppointmentItemRequest> items, {
    required bool sameStaffForAll,
    required String from,
    required String to,
  }) => apiCall(() async {
    final response = await _booking.publicSearchDays(
      branchId: branchId,
      daySearchRequest: DaySearchRequest(
        items: items,
        sameStaffForAll: sameStaffForAll,
        from: parseLocalDate(from),
        to: parseLocalDate(to),
      ),
    );
    return response.data!;
  });

  @override
  Future<SlotSearchResult> slots(
    String branchId,
    List<AppointmentItemRequest> items, {
    required bool sameStaffForAll,
    required String date,
  }) => apiCall(() async {
    final response = await _booking.publicSearchSlots(
      branchId: branchId,
      slotSearchRequest: SlotSearchRequest(
        items: items,
        sameStaffForAll: sameStaffForAll,
        date: parseLocalDate(date),
      ),
    );
    return response.data!;
  });

  @override
  Future<List<UsablePackageItem>> packages(String branchId) =>
      apiCall(() async {
        final response = await _booking.publicListMyPackages(
          branchId: branchId,
        );
        return response.data!.items;
      });

  @override
  Future<String?> profileName() => apiCall(() async {
    final response = await api.getMeProfileApi().meGetProfile();
    final name = response.data!.fullName?.trim();
    return name == null || name.isEmpty ? null : name;
  });

  @override
  Future<PublicAppointment> create(
    String branchId,
    PublicAppointmentCreate body, {
    required String idempotencyKey,
  }) => apiCall(() async {
    final response = await _booking.publicCreateAppointment(
      idempotencyKey: idempotencyKey,
      branchId: branchId,
      publicAppointmentCreate: body,
    );
    return response.data!;
  });
}
