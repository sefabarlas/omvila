import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_riverpod/misc.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/app/providers.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Takvim penceresi: şube, yerel gün aralığı ("2026-11-23" … "2026-12-06") ve
/// personel filtresi (sıralı, virgülle ayrılmış kimlikler; `null` = tümü).
/// Liste yerine metin: kayıt eşitliği aile anahtarı olarak çalışsın.
typedef CalendarRange = ({
  String branchId,
  String from,
  String to,
  String? staffIds,
});

/// Gün şeridi ve gün görünümü aynı pencereden çizilir (tek istek; şeritteki
/// nokta ve "Kapalı" bilgisi de buradan). Personel rolünde sunucu yalnız
/// kendi kolonunu döner. Personel filtresi sunucuda uygulanır (`staffIds`).
final FutureProviderFamily<CalendarView, CalendarRange> calendarProvider =
    FutureProvider.autoDispose.family<CalendarView, CalendarRange>((
      ref,
      range,
    ) async {
      try {
        final response = await ref
            .watch(apiProvider)
            .getManageCalendarApi()
            .manageGetCalendar(
              branchId: range.branchId,
              from: DateTime.parse(range.from),
              to: DateTime.parse(range.to),
              staffIds: range.staffIds?.split(','),
            );
        return response.data!;
      } on DioException catch (e) {
        throw ApiProblem.fromDioException(e);
      }
    });
