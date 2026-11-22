import 'dart:async';

import 'package:dio/dio.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/shared/key_value_store.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Yenileme jetonuyla yeni jeton çifti alan çağrı (meRefreshToken).
typedef TokenRefresher = Future<TokenPair> Function(String refreshToken);

/// Müşteri oturumu: erişim jetonu bellekte ve cihazda, yenileme jetonu
/// cihazda (`flutter_secure_storage`). Yenileme reddedilirse oturum silinir
/// ve [onSignedOut] çağrılır (uygulama girişe döner).
final class CustomerSession implements AuthSession {
  /// Oturum.
  CustomerSession({required this.store, required this.refresher});

  static const _accessKey = 'me.accessToken';
  static const _refreshKey = 'me.refreshToken';

  /// Jeton deposu.
  final KeyValueStore store;

  /// Yenileme çağrısı.
  final TokenRefresher refresher;

  /// Oturum sunucuca kapatıldı (yenileme reddedildi).
  void Function()? onSignedOut;

  String? _access;
  String? _refresh;

  /// Cihazda oturum var mı.
  bool get isSignedIn => _refresh != null;

  /// Cihazdaki jetonları okur; oturum varsa true.
  Future<bool> load() async {
    _access = await store.read(_accessKey);
    _refresh = await store.read(_refreshKey);
    return isSignedIn;
  }

  /// Giriş veya yenileme sonrası jetonları saklar.
  Future<void> save({
    required String accessToken,
    required String refreshToken,
  }) async {
    _access = accessToken;
    _refresh = refreshToken;
    await store.write(_accessKey, accessToken);
    await store.write(_refreshKey, refreshToken);
  }

  /// Oturumu cihazdan siler.
  Future<void> clear() async {
    _access = null;
    _refresh = null;
    await store.delete(_accessKey);
    await store.delete(_refreshKey);
  }

  @override
  FutureOr<String?> accessToken() => _access;

  @override
  Future<bool> refresh() async {
    final current = _refresh;
    if (current == null) {
      return false;
    }

    try {
      final pair = await refresher(current);
      await save(
        accessToken: pair.accessToken,
        refreshToken: pair.refreshToken ?? current,
      );
      return true;
    } on DioException catch (e) {
      // Bağlantı yoksa oturum korunur; sunucu reddettiyse (401/400) kapanır.
      final status = e.response?.statusCode;
      if (status == 400 || status == 401) {
        await clear();
        onSignedOut?.call();
      }

      return false;
    }
  }
}
