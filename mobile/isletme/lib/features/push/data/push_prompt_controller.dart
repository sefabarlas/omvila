import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:omvila_isletme/app/providers.dart';
import 'package:omvila_isletme/features/push/data/push_service.dart';

/// Bildirim izni ekranı ve Bugün'deki "Bildirimler kapalı" şeridi.
class PushPromptState {
  /// Durum.
  const PushPromptState({
    required this.asked,
    required this.bannerDismissed,
    required this.permission,
  });

  /// BildirimIzni ekranı gösterildi ve yanıtlandı.
  final bool asked;

  /// Şerit kapatıldı.
  final bool bannerDismissed;

  /// Sistem izni.
  final PushPermission permission;

  /// Bugün'de şerit görünür mü.
  bool get showBanner =>
      asked && !bannerDismissed && permission != PushPermission.granted;
}

/// İzin tercihi (cihazda kalır).
final pushPromptProvider =
    AsyncNotifierProvider<PushPromptController, PushPromptState>(
      PushPromptController.new,
    );

/// "Bildirimlere izin ver", "Şimdi değil", şeridi kapatma.
class PushPromptController extends AsyncNotifier<PushPromptState> {
  static const _askedKey = 'push.asked';
  static const _dismissedKey = 'push.bannerDismissed';

  @override
  Future<PushPromptState> build() async {
    final store = ref.watch(keyValueStoreProvider);
    return PushPromptState(
      asked: await store.read(_askedKey) == '1',
      bannerDismissed: await store.read(_dismissedKey) == '1',
      permission: await ref.watch(pushServiceProvider).permission(),
    );
  }

  /// Sistem istemini açar.
  Future<void> request() async {
    final permission = await ref.read(pushServiceProvider).requestPermission();
    await ref.read(keyValueStoreProvider).write(_askedKey, '1');
    final current = await future;
    state = AsyncData(
      PushPromptState(
        asked: true,
        bannerDismissed: current.bannerDismissed,
        permission: permission,
      ),
    );
  }

  /// "Şimdi değil".
  Future<void> skip() async {
    await ref.read(keyValueStoreProvider).write(_askedKey, '1');
    final current = await future;
    state = AsyncData(
      PushPromptState(
        asked: true,
        bannerDismissed: current.bannerDismissed,
        permission: current.permission,
      ),
    );
  }

  /// Şeridi kapatır.
  Future<void> dismissBanner() async {
    await ref.read(keyValueStoreProvider).write(_dismissedKey, '1');
    final current = await future;
    state = AsyncData(
      PushPromptState(
        asked: current.asked,
        bannerDismissed: true,
        permission: current.permission,
      ),
    );
  }
}
