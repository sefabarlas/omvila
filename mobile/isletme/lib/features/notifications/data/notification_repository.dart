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

/// Sekme çubuğundaki Bildirimler rozeti: sunucunun `unreadCount`'u (tek
/// satırlık sayfa). Okunamazsa rozet yok (sahte sayı gösterilmez).
final FutureProvider<int> unreadNotificationCountProvider =
    FutureProvider.autoDispose<int>(
      (ref) async => (await _call(
        () => ref
            .watch(apiProvider)
            .getManageNotificationsApi()
            .manageListInAppNotifications(limit: 1),
      )).unreadCount,
    );

/// Okundu say: `ids` verilirse onlar, yoksa hepsi ("Okundu say"). Sonra
/// sekme rozeti yeniden okunur.
final markNotificationsReadProvider =
    Provider<Future<void> Function({Set<String>? ids})>(
      (ref) => ({ids}) async {
        await _call(
          () => ref
              .read(apiProvider)
              .getManageNotificationsApi()
              .manageMarkNotificationsRead(
                markNotificationsReadRequest: ids == null
                    ? MarkNotificationsReadRequest(all: true)
                    : MarkNotificationsReadRequest(notificationIds: ids),
              ),
        );
        ref.invalidate(unreadNotificationCountProvider);
      },
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
