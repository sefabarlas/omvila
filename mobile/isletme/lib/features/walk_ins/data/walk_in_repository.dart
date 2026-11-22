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

/// Şubenin bugünkü sırası (Sira, SiraBos).
final FutureProviderFamily<WalkInQueue, String> walkInQueueProvider =
    FutureProvider.autoDispose.family<WalkInQueue, String>(
      (ref, branchId) => _call(
        () => ref
            .watch(apiProvider)
            .getManageWalkInsApi()
            .manageListWalkIns(branchId: branchId),
      ),
    );

/// Sıra eylemleri: "Sıraya ekle" (Idempotency-Key), "Başlat" ve "Sıradan
/// çıkar" (biletin `version`'ı ile If-Match).
class WalkInActions {
  /// Eylemler.
  const WalkInActions(this._api);

  final ManageWalkInsApi _api;

  /// "Sıraya ekle"; dönen bilette sunucunun verdiği numara vardır.
  Future<WalkInTicketView> create(WalkInCreate body, {required String key}) =>
      _call(
        () => _api.manageCreateWalkIn(idempotencyKey: key, walkInCreate: body),
      );

  /// "Başlat": bileti şimdi başlayan, gelmiş randevuya çevirir.
  Future<WalkInStarted> start(
    WalkInTicketView ticket, {
    String? serviceId,
    String? staffId,
  }) => _call(
    () => _api.manageStartWalkIn(
      ticketId: ticket.id,
      ifMatch: ifMatch(ticket.version),
      walkInStartRequest: serviceId == null && staffId == null
          ? null
          : WalkInStartRequest(serviceId: serviceId, staffId: staffId),
    ),
  );

  /// "Sıradan çıkar".
  Future<WalkInTicketView> leave(WalkInTicketView ticket) => _call(
    () => _api.manageLeaveWalkIn(
      ticketId: ticket.id,
      ifMatch: ifMatch(ticket.version),
    ),
  );
}

/// Sıra eylemleri.
final walkInActionsProvider = Provider<WalkInActions>(
  (ref) => WalkInActions(ref.watch(apiProvider).getManageWalkInsApi()),
);
