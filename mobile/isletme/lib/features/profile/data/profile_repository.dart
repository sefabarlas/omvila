import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
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

/// Primim (Profil). Kullanıcının bu markada personel kaydı yoksa sunucu 404
/// döner: null (prim bölümü gizlenir).
final FutureProvider<MyCommissionSummary?> myCommissionProvider =
    FutureProvider.autoDispose<MyCommissionSummary?>((ref) async {
      try {
        return await _call(
          () => ref
              .watch(apiProvider)
              .getManageTodayApi()
              .manageGetMyCommissionSummary(),
        );
      } on ApiProblem catch (e) {
        if (e.status == 404) {
          return null;
        }

        rethrow;
      }
    });

/// Kendi bildirim tercihlerim (BildirimAyar).
final FutureProvider<MyNotificationPreferences> myPreferencesProvider =
    FutureProvider.autoDispose<MyNotificationPreferences>(
      (ref) => _call(
        () => ref
            .watch(apiProvider)
            .getManageNotificationsApi()
            .manageGetMyNotificationPreferences(),
      ),
    );

/// Tercihleri kaydeder (anahtarların hepsi, If-Match = okunan sürüm).
final savePreferencesProvider =
    Provider<
      Future<MyNotificationPreferences> Function(MyNotificationPreferences)
    >(
      (ref) =>
          (prefs) => _call(
            () => ref
                .read(apiProvider)
                .getManageNotificationsApi()
                .manageUpdateMyNotificationPreferences(
                  ifMatch: ifMatch(prefs.version ?? '*'),
                  myNotificationPreferences: prefs,
                ),
          ),
    );
