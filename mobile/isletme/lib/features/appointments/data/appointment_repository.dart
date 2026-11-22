import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_riverpod/misc.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/app/providers.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Randevu ayrıntısı (RandevuDetay). Eylemler sonrası yenilenir.
final FutureProviderFamily<AppointmentDetail, String> appointmentProvider =
    FutureProvider.autoDispose.family<AppointmentDetail, String>((
      ref,
      appointmentId,
    ) async {
      try {
        final response = await ref
            .watch(apiProvider)
            .getManageCalendarApi()
            .manageGetAppointment(appointmentId: appointmentId);
        return response.data!;
      } on DioException catch (e) {
        throw ApiProblem.fromDioException(e);
      }
    });

/// Müşterinin paketleri (OdemeAl "Paketten"). Bir müşterinin paketleri tek
/// sayfaya sığar (salon paneliyle aynı varsayım).
final FutureProviderFamily<List<CustomerPackage>, String>
customerPackagesProvider = FutureProvider.autoDispose
    .family<List<CustomerPackage>, String>((ref, brandCustomerId) async {
      try {
        final response = await ref
            .watch(apiProvider)
            .getManageCustomersApi()
            .manageListCustomerPackages(
              brandCustomerId: brandCustomerId,
            );
        return response.data!.items;
      } on DioException catch (e) {
        throw ApiProblem.fromDioException(e);
      }
    });

/// Randevu eylemleri: her biri okunan kaydın `version`'ı ile If-Match
/// gönderir; para kaydedenler (kapora, tamamla) ayrıca Idempotency-Key.
class AppointmentActions {
  /// Eylemler.
  const AppointmentActions(this._api);

  final OmvilaApi _api;

  ManageCalendarApi get _calendar => _api.getManageCalendarApi();

  Future<T> _call<T>(Future<Response<T>> Function() request) async {
    try {
      return (await request()).data as T;
    } on DioException catch (e) {
      throw ApiProblem.fromDioException(e);
    }
  }

  /// "Geldi" (A8).
  Future<AppointmentDetail> checkIn(AppointmentDetail a) => _call(
    () => _calendar.manageCheckInAppointment(
      appointmentId: a.id,
      ifMatch: ifMatch(a.version),
    ),
  );

  /// "Gelmedi işaretini geri al" (aynı yerel gün).
  Future<AppointmentDetail> undoNoShow(AppointmentDetail a) => _call(
    () => _calendar.manageUndoNoShow(
      appointmentId: a.id,
      ifMatch: ifMatch(a.version),
    ),
  );

  /// "Kapora geldi" (A3). [key] aynı kullanıcı eylemi için sabittir.
  Future<AppointmentDetail> markDepositReceived(
    AppointmentDetail a, {
    required String key,
  }) => _call(
    () => _calendar.manageMarkDepositReceived(
      appointmentId: a.id,
      ifMatch: ifMatch(a.version),
      idempotencyKey: key,
    ),
  );

  /// "Henüz gelmedi, hatırlat".
  Future<void> remindDeposit(AppointmentDetail a) =>
      _call(() => _calendar.manageRemindDeposit(appointmentId: a.id));

  /// "{tutar} al ve tamamla" (A11).
  Future<AppointmentDetail> complete(
    AppointmentDetail a, {
    required String key,
    required ManageCompleteRequest request,
  }) => _call(
    () => _calendar.manageCompleteAppointment(
      appointmentId: a.id,
      ifMatch: ifMatch(a.version),
      idempotencyKey: key,
      manageCompleteRequest: request,
    ),
  );

  /// "Randevuyu iptal et".
  Future<AppointmentDetail> cancel(
    AppointmentDetail a,
    ManageCancelRequest request,
  ) => _call(
    () => _calendar.manageCancelAppointment(
      appointmentId: a.id,
      ifMatch: ifMatch(a.version),
      manageCancelRequest: request,
    ),
  );

  /// "Saati veya personeli değiştir".
  Future<AppointmentDetail> reschedule(
    AppointmentDetail a,
    ManageRescheduleRequest request,
  ) => _call(
    () => _calendar.manageRescheduleAppointment(
      appointmentId: a.id,
      ifMatch: ifMatch(a.version),
      manageRescheduleRequest: request,
    ),
  );

  /// Maskeli numarayı arama anında açar ("Ara" / "SMS"); numara
  /// gösterilmez, saklanmaz, günlüğe yazılmaz.
  Future<String> revealPhone(
    String brandCustomerId,
    PhoneRevealRequestPurposeEnum purpose,
  ) async {
    final revealed = await _call(
      () => _api.getManageCustomersApi().manageRevealCustomerPhone(
        brandCustomerId: brandCustomerId,
        phoneRevealRequest: PhoneRevealRequest(purpose: purpose),
      ),
    );
    return revealed.phone;
  }
}

/// Randevu eylemleri.
final appointmentActionsProvider = Provider<AppointmentActions>(
  (ref) => AppointmentActions(ref.watch(apiProvider)),
);
