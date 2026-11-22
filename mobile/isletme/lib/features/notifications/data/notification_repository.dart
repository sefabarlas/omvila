import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_riverpod/misc.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/app/providers.dart';
import 'package:omvila_ui/omvila_ui.dart';

Future<T> _call<T>(Future<Response<T>> Function() request) async {
  try {
    return (await request()).data as T;
  } on DioException catch (e) {
    throw ApiProblem.fromDioException(e);
  }
}

/// Bildirimler (Bildirimler, BildirimlerBos); `unreadOnly` "Okunmamış"
/// sekmesi. İlk sayfa (sözleşme varsayılanı 50) gösterilir.
final FutureProviderFamily<InAppNotificationPage, bool> notificationsProvider =
    FutureProvider.autoDispose.family<InAppNotificationPage, bool>(
      (ref, unreadOnly) => _call(
        () => ref
            .watch(apiProvider)
            .getManageNotificationsApi()
            .manageListInAppNotifications(unreadOnly: unreadOnly),
      ),
    );

/// Okundu say: `ids` verilirse onlar, yoksa hepsi ("Okundu say").
final markNotificationsReadProvider =
    Provider<Future<void> Function({Set<String>? ids})>(
      (ref) =>
          ({ids}) => _call(
            () => ref
                .read(apiProvider)
                .getManageNotificationsApi()
                .manageMarkNotificationsRead(
                  markNotificationsReadRequest: ids == null
                      ? MarkNotificationsReadRequest(all: true)
                      : MarkNotificationsReadRequest(notificationIds: ids),
                ),
          ),
    );

/// Satır içi "Reddet": If-Match için güncel sürüm okunur (A4; müşteriye
/// "iptal" olarak bildirilir).
final declineAppointmentProvider = Provider<Future<void> Function(String)>((
  ref,
) {
  final calendar = ref.watch(apiProvider).getManageCalendarApi();
  return (appointmentId) => _call(() async {
    final detail = await calendar.manageGetAppointment(
      appointmentId: appointmentId,
    );
    return calendar.manageDeclineAppointment(
      appointmentId: appointmentId,
      ifMatch: ifMatch(detail.data!.version),
    );
  });
});
