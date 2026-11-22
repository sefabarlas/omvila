import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/features/auth/data/auth_repository.dart';
import 'package:omvila_musteri/shared/api_call.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Bekleme listesi: katılma (publicJoinWaitlist, müşteri jetonuyla) ve
/// `/b/{code}` teklifi (public uçlar, bağlantı koduyla).
abstract interface class WaitlistRepository {
  /// "Listeye katıl"; [idempotencyKey] aynı denemede sabit kalır.
  Future<PublicWaitlistEntry> join(
    String branchId,
    PublicWaitlistJoin body, {
    required String idempotencyKey,
  });

  /// Teklif.
  Future<PublicWaitlistOffer> offer(String accessCode);

  /// "Randevuyu al" (If-Match).
  Future<PublicAppointment> accept(String accessCode, PublicWaitlistOffer o);

  /// "Bu saat olmaz" (If-Match).
  Future<PublicWaitlistOffer> decline(String accessCode, PublicWaitlistOffer o);

  /// "Listeden çık" (bekleme kaydının kodu).
  Future<void> leave(String waitlistAccessCode);
}

/// Üretilmiş istemciyle.
final class ApiWaitlistRepository implements WaitlistRepository {
  /// [api] müşteri jetonuyla.
  const ApiWaitlistRepository(this.api);

  /// API.
  final OmvilaApi api;

  @override
  Future<PublicWaitlistEntry> join(
    String branchId,
    PublicWaitlistJoin body, {
    required String idempotencyKey,
  }) => apiCall(() async {
    final response = await api.getPublicWaitlistApi().publicJoinWaitlist(
      idempotencyKey: idempotencyKey,
      branchId: branchId,
      publicWaitlistJoin: body,
      xClient: customerAppClient,
    );
    return response.data!;
  });

  @override
  Future<PublicWaitlistOffer> offer(String accessCode) => apiCall(() async {
    final response = await api.getPublicWaitlistApi().publicGetWaitlistOffer(
      accessCode: accessCode,
    );
    return response.data!;
  });

  @override
  Future<PublicAppointment> accept(String accessCode, PublicWaitlistOffer o) =>
      apiCall(() async {
        final response = await api
            .getPublicWaitlistApi()
            .publicAcceptWaitlistOffer(
              ifMatch: ifMatch(o.version),
              accessCode: accessCode,
              xClient: customerAppClient,
            );
        return response.data!;
      });

  @override
  Future<PublicWaitlistOffer> decline(
    String accessCode,
    PublicWaitlistOffer o,
  ) => apiCall(() async {
    final response = await api
        .getPublicWaitlistApi()
        .publicDeclineWaitlistOffer(
          ifMatch: ifMatch(o.version),
          accessCode: accessCode,
        );
    return response.data!;
  });

  @override
  Future<void> leave(String waitlistAccessCode) => apiCall(() async {
    await api.getPublicWaitlistApi().publicLeaveWaitlist(
      accessCode: waitlistAccessCode,
    );
  });
}
