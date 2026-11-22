import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/shared/api_call.dart';

/// Uygulama içi bildirimler (meListNotifications, meMarkNotificationsRead).
abstract interface class NotificationsRepository {
  /// Bildirimler, yeniden eskiye; [limit] zildeki sayı için 1.
  Future<MeNotificationPage> list({String? cursor, int? limit});

  /// Hepsini okundu sayar; güncel okunmamış sayısını döner.
  Future<int> markAllRead();
}

/// Üretilmiş istemciyle.
final class ApiNotificationsRepository implements NotificationsRepository {
  /// [api] müşteri jetonuyla.
  const ApiNotificationsRepository(this.api);

  /// API.
  final OmvilaApi api;

  @override
  Future<MeNotificationPage> list({String? cursor, int? limit}) =>
      apiCall(() async {
        final response = await api.getMeNotificationsApi().meListNotifications(
          cursor: cursor,
          limit: limit ?? 50,
        );
        return response.data!;
      });

  @override
  Future<int> markAllRead() => apiCall(() async {
    final response = await api.getMeNotificationsApi().meMarkNotificationsRead(
      markNotificationsReadRequest: MarkNotificationsReadRequest(all: true),
    );
    return response.data!.unreadCount;
  });
}
