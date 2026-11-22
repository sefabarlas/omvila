import 'package:flutter/foundation.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:omvila_musteri/app/providers.dart';
import 'package:omvila_musteri/features/notifications/data/push_service.dart';

/// Bildirim izni ekranı ve "Bildirimler kapalı" şeridi için durum.
@immutable
class NotificationPrefs {
  /// Durum.
  const NotificationPrefs({required this.asked, required this.showBanner});

  /// BildirimIzni ekranı gösterildi ve yanıtlandı.
  final bool asked;

  /// Ana ekranda "Bildirimler kapalı" şeridi gösterilir.
  final bool showBanner;
}

/// Bildirim izni tercihleri.
final notificationPrefsProvider =
    AsyncNotifierProvider<NotificationPrefsController, NotificationPrefs>(
      NotificationPrefsController.new,
    );

/// İzin önce kendi ekranımızla istenir; sistem istemi yalnız "Bildirimlere
/// izin ver"de açılır. "Şimdi değil" kapatılabilir şerit bırakır.
class NotificationPrefsController extends AsyncNotifier<NotificationPrefs> {
  static const _askedKey = 'push.asked';
  static const _skippedKey = 'push.skipped';
  static const _dismissedKey = 'push.bannerDismissed';

  @override
  Future<NotificationPrefs> build() async {
    final store = ref.read(keyValueStoreProvider);
    final asked = await store.read(_askedKey) == '1';
    final skipped = await store.read(_skippedKey) == '1';
    final dismissed = await store.read(_dismissedKey) == '1';
    return NotificationPrefs(asked: asked, showBanner: skipped && !dismissed);
  }

  /// "Bildirimlere izin ver".
  Future<void> allow() async {
    final permission = await ref.read(pushServiceProvider).requestPermission();
    await _registerDevice(permission);
    await _save(skipped: permission != PushPermission.granted);
  }

  /// "Şimdi değil".
  Future<void> skip() => _save(skipped: true);

  /// Şeridi kapatır.
  Future<void> dismissBanner() async {
    await ref.read(keyValueStoreProvider).write(_dismissedKey, '1');
    state = AsyncData(
      NotificationPrefs(asked: state.value?.asked ?? true, showBanner: false),
    );
  }

  /// "Ayarlardan aç": hiç sorulmadıysa sistem istemi, reddedildiyse sistem
  /// ayarları.
  Future<void> openSettings() async {
    final push = ref.read(pushServiceProvider);
    if (await push.permission() == PushPermission.notDetermined) {
      await allow();
    } else {
      await push.openSystemSettings();
    }
  }

  Future<void> _registerDevice(PushPermission permission) async {
    if (permission != PushPermission.granted) {
      return;
    }

    final token = await ref.read(pushServiceProvider).token();
    if (token != null) {
      try {
        await ref.read(deviceRepositoryProvider).register(token);
      } on Exception {
        // Kayıt hatası izni geri almaz; push katmanı jeton yenilenince yeniden
        // kaydeder.
      }
    }
  }

  Future<void> _save({required bool skipped}) async {
    final store = ref.read(keyValueStoreProvider);
    await store.write(_askedKey, '1');
    await store.write(_skippedKey, skipped ? '1' : '0');
    await store.delete(_dismissedKey);
    state = AsyncData(NotificationPrefs(asked: true, showBanner: skipped));
  }
}
