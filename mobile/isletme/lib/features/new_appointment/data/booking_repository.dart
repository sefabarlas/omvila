import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_riverpod/misc.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/app/providers.dart';
import 'package:omvila_ui/omvila_ui.dart';

Future<T> _get<T>(Future<Response<T>> Function() request) async {
  try {
    return (await request()).data as T;
  } on DioException catch (e) {
    throw ApiProblem.fromDioException(e);
  }
}

/// Şubenin etkin hizmetleri (fiyat ve süre şubeye göre).
final FutureProviderFamily<List<Service>, String> servicesProvider =
    FutureProvider.autoDispose.family<List<Service>, String>((
      ref,
      branchId,
    ) async {
      final page = await _get(
        () => ref
            .watch(apiProvider)
            .getManageCatalogApi()
            .manageListServices(branchId: branchId),
      );
      return [
        for (final s in page.items)
          if (s.isActive) s,
      ]..sort((a, b) => (a.sortOrder ?? 0).compareTo(b.sortOrder ?? 0));
    });

/// Şubenin etkin personeli.
final FutureProviderFamily<List<StaffSummary>, String> staffProvider =
    FutureProvider.autoDispose.family<List<StaffSummary>, String>((
      ref,
      branchId,
    ) async {
      final page = await _get(
        () => ref
            .watch(apiProvider)
            .getManageStaffApi()
            .manageListStaff(branchId: branchId),
      );
      return [
        for (final s in page.items)
          if (s.isActive) s,
      ];
    });

/// Boş saat araması.
@immutable
class SlotQuery {
  /// Arama.
  const SlotQuery({
    required this.branchId,
    required this.date,
    required this.serviceIds,
    this.staffId,
  });

  /// Şube.
  final String branchId;

  /// Yerel gün.
  final String date;

  /// Hizmetler (sırayla zincirlenir).
  final List<String> serviceIds;

  /// null = fark etmez.
  final String? staffId;

  @override
  bool operator ==(Object other) =>
      other is SlotQuery &&
      other.branchId == branchId &&
      other.date == date &&
      other.staffId == staffId &&
      listEquals(other.serviceIds, serviceIds);

  @override
  int get hashCode =>
      Object.hash(branchId, date, staffId, Object.hashAll(serviceIds));
}

/// Panel için boş saatler (manageSearchSlots; "Elif için boş").
final FutureProviderFamily<SlotSearchResult, SlotQuery> slotsProvider =
    FutureProvider.autoDispose.family<SlotSearchResult, SlotQuery>((
      ref,
      query,
    ) async {
      return _get(
        () => ref
            .watch(apiProvider)
            .getManageCalendarApi()
            .manageSearchSlots(
              branchId: query.branchId,
              slotSearchRequest: SlotSearchRequest(
                items: [
                  for (final id in query.serviceIds)
                    AppointmentItemRequest(
                      serviceId: id,
                      staffId: query.staffId,
                    ),
                ],
                date: DateTime.parse(query.date),
              ),
            ),
      );
    });

/// Müşteri araması (ad, telefon veya son 4 hane). Personel rolünde telefon
/// maskeli döner.
final FutureProviderFamily<List<CustomerSummary>, String>
customerSearchProvider = FutureProvider.autoDispose
    .family<List<CustomerSummary>, String>((ref, q) async {
      final page = await _get(
        () => ref
            .watch(apiProvider)
            .getManageCustomersApi()
            .manageListCustomers(q: q, limit: 8),
      );
      return page.items;
    });

/// "Oluştur".
final createAppointmentProvider =
    Provider<
      Future<AppointmentCreated> Function(
        ManageAppointmentCreate request, {
        required String key,
      })
    >((ref) {
      return (request, {required key}) => _get(
        () => ref
            .read(apiProvider)
            .getManageCalendarApi()
            .manageCreateAppointment(
              idempotencyKey: key,
              manageAppointmentCreate: request,
            ),
      );
    });
