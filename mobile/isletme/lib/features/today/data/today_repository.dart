import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/app/providers.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Bugün verisi ve okunduğu an (enjekte saatle). Kalan süreler
/// `generatedUtc + (şimdi − okunma)` ile sayılır; cihaz saat dilimi yok.
class TodaySnapshot {
  /// Anlık görüntü.
  const TodaySnapshot({required this.today, required this.fetchedAt});

  /// `manageGetToday` yanıtı.
  final ManageToday today;

  /// Okunduğu an (UTC, [Clock]).
  final DateTime fetchedAt;

  /// Sunucu saatine göre "şimdi".
  DateTime serverNow(Clock clock) {
    final elapsed = clock.now().difference(fetchedAt);
    return today.generatedUtc.add(elapsed.isNegative ? Duration.zero : elapsed);
  }
}

/// Bugün (Main, MainBos). Arka plandan dönünce ve aşağı çekince yenilenir.
final FutureProvider<TodaySnapshot> todayProvider =
    FutureProvider.autoDispose<TodaySnapshot>((ref) async {
      final api = ref.watch(apiProvider);
      final clock = ref.watch(clockProvider);
      try {
        final response = await api.getManageTodayApi().manageGetToday();
        return TodaySnapshot(today: response.data!, fetchedAt: clock.now());
      } on DioException catch (e) {
        throw ApiProblem.fromDioException(e);
      }
    });

/// Rezervasyon bağlantısı (MainBos "Rezervasyon bağlantısını paylaş").
final FutureProvider<String> bookingUrlProvider =
    FutureProvider.autoDispose<String>((ref) async {
      try {
        final response = await ref
            .watch(apiProvider)
            .getManageBrandApi()
            .manageGetBookingPageSettings();
        return response.data!.url;
      } on DioException catch (e) {
        throw ApiProblem.fromDioException(e);
      }
    });

/// Satır içi "Onayla": If-Match için güncel sürüm okunur, sonra onaylanır.
/// Kapora bekleniyorsa sunucu `409 deposit_required` döner; uygulama
/// randevu ayrıntısına (kapora seçenekleri) yönlendirir.
final approveAppointmentProvider = Provider<Future<void> Function(String)>((
  ref,
) {
  final calendar = ref.watch(apiProvider).getManageCalendarApi();
  return (appointmentId) async {
    try {
      final detail = await calendar.manageGetAppointment(
        appointmentId: appointmentId,
      );
      await calendar.manageApproveAppointment(
        appointmentId: appointmentId,
        ifMatch: ifMatch(detail.data!.version),
      );
    } on DioException catch (e) {
      throw ApiProblem.fromDioException(e);
    }
  };
});
