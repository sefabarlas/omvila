import 'dart:async';

import 'package:dio/dio.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/shared/key_value_store.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Yenileme sonucu.
enum RefreshOutcome {
  /// Yeni jetonlar alındı.
  refreshed,

  /// Oturum yok veya sunucu reddetti; jetonlar silindi.
  signedOut,

  /// Bağlantı veya sunucu hatası; jetonlar korunur.
  failed,
}

/// Jeton deposu: erişim jetonu bellekte, yenileme jetonu güvenli depoda
/// (kodlama-flutter.md § API). Yenileme jetonu her kullanımda döner.
class AuthTokens implements AuthSession {
  /// [refresher] yenileme ucunu kimliksiz çağırır.
  AuthTokens({required this.store, required this.refresher});

  /// Güvenli depo.
  final KeyValueStore store;

  /// `manageRefreshToken`.
  final Future<TokenPair> Function(String refreshToken) refresher;

  /// Sunucu oturumu reddedince (yenileme 401) çağrılır.
  void Function()? onSignedOut;

  static const _refreshKey = 'auth.refreshToken';
  String? _accessToken;
  Future<RefreshOutcome>? _pending;

  @override
  FutureOr<String?> accessToken() => _accessToken;

  @override
  Future<bool> refresh() async =>
      await refreshOutcome() == RefreshOutcome.refreshed;

  /// Eşzamanlı çağrılar tek yenilemeyi paylaşır.
  Future<RefreshOutcome> refreshOutcome() =>
      _pending ??= _refresh().whenComplete(() => _pending = null);

  Future<RefreshOutcome> _refresh() async {
    final refreshToken = await store.read(_refreshKey);
    if (refreshToken == null) {
      return RefreshOutcome.signedOut;
    }

    try {
      final pair = await refresher(refreshToken);
      await save(
        accessToken: pair.accessToken,
        refreshToken: pair.refreshToken,
      );
      return RefreshOutcome.refreshed;
    } on DioException catch (e) {
      final status = e.response?.statusCode;
      if (status == 401 || status == 400) {
        await clear();
        onSignedOut?.call();
        return RefreshOutcome.signedOut;
      }

      return RefreshOutcome.failed;
    }
  }

  /// Girişte ve yenilemede.
  Future<void> save({required String accessToken, String? refreshToken}) async {
    _accessToken = accessToken;
    if (refreshToken != null) {
      await store.write(_refreshKey, refreshToken);
    }
  }

  /// Çıkışta.
  Future<void> clear() async {
    _accessToken = null;
    await store.write(_refreshKey, null);
  }
}
