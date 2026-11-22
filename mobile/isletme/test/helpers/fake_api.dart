import 'dart:async';
import 'dart:convert';
import 'dart:typed_data';

import 'package:dio/dio.dart';
import 'package:flutter/widgets.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_riverpod/misc.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_isletme/app/app.dart';
import 'package:omvila_isletme/app/providers.dart';
import 'package:omvila_isletme/features/push/data/push_service.dart';
import 'package:omvila_isletme/main.dart';
import 'package:omvila_isletme/shared/key_value_store.dart';
import 'package:omvila_ui/omvila_ui.dart';

import 'fixtures.dart';

/// Sahte yanıt.
class FakeResponse {
  /// Yanıt.
  const FakeResponse(this.status, [this.body]);

  /// HTTP durumu.
  final int status;

  /// JSON gövde.
  final Object? body;
}

/// Kayıtlı istek.
class RecordedRequest {
  /// İstek.
  RecordedRequest(this.options, this.body);

  /// Dio seçenekleri.
  final RequestOptions options;

  /// Çözülmüş JSON gövde.
  final Object? body;

  /// Başlık.
  String? header(String name) => options.headers[name]?.toString();
}

/// Sözleşme uçlarını taklit eden HTTP bağdaştırıcısı: üretilmiş istemci
/// gerçek seri hâle getirmeyle çalışır, yalnız ağ sahtedir.
class FakeApi implements HttpClientAdapter {
  final _handlers =
      <String, FutureOr<FakeResponse> Function(RecordedRequest)>{};

  /// Gelen istekler.
  final requests = <RecordedRequest>[];

  /// `on('GET', '/api/v1/manage/today', ...)`.
  void on(
    String method,
    String path,
    FutureOr<FakeResponse> Function(RecordedRequest request) handler,
  ) => _handlers['$method $path'] = handler;

  /// Sabit JSON yanıt.
  void reply(String method, String path, Object? body, {int status = 200}) =>
      on(method, path, (_) => FakeResponse(status, body));

  /// Yola giden istekler.
  List<RecordedRequest> to(String method, String path) => [
    for (final r in requests)
      if (r.options.method == method && r.options.path == path) r,
  ];

  @override
  Future<ResponseBody> fetch(
    RequestOptions options,
    Stream<Uint8List>? requestStream,
    Future<void>? cancelFuture,
  ) async {
    Object? body;
    if (requestStream != null) {
      final bytes = await requestStream.expand((c) => c).toList();
      body = bytes.isEmpty ? null : jsonDecode(utf8.decode(bytes));
    }

    final request = RecordedRequest(options, body);
    requests.add(request);
    final handler = _handlers['${options.method} ${options.path}'];
    final response = handler == null
        ? const FakeResponse(404, {'code': 'not_found'})
        : await handler(request);
    return ResponseBody.fromString(
      response.body == null ? '' : jsonEncode(response.body),
      response.status,
      headers: {
        Headers.contentTypeHeader: [Headers.jsonContentType],
      },
    );
  }

  @override
  void close({bool force = false}) {}
}

/// Bellekte anahtar-değer deposu.
class MemoryKeyValueStore implements KeyValueStore {
  /// Değerler.
  final values = <String, String>{};

  @override
  Future<String?> read(String key) async => values[key];

  @override
  Future<void> write(String key, String? value) async =>
      value == null ? values.remove(key) : values[key] = value;
}

/// Uygulamanın tamamını sahte API, depo, push ve sabit saatle kurar.
class Harness {
  /// Sahte API.
  final api = FakeApi();

  /// Depo.
  final store = MemoryKeyValueStore();

  /// Push (varsayılan izin verilmiş; şerit testleri değiştirir).
  FakePushService push = FakePushService(initial: PushPermission.granted);

  /// Sözleşme örneğindeki an (Bugün `generatedUtc`).
  Clock clock = FixedClock.parse('2026-11-25T09:10:00Z');

  /// Sağlayıcı ezmeleri.
  List<Override> get overrides => [
    httpAdapterProvider.overrideWithValue(api),
    keyValueStoreProvider.overrideWithValue(store),
    pushServiceProvider.overrideWithValue(push),
    clockProvider.overrideWithValue(clock),
  ];

  /// Yenileme jetonu kayıtlı, oturum ve izin ekranı geçilmiş.
  void signIn({Map<String, Object?>? session, bool promptAsked = true}) {
    store.values['auth.refreshToken'] = 'rt-1';
    if (promptAsked) {
      store.values['push.asked'] = '1';
    }

    api
      ..reply('POST', '/api/v1/manage/auth/refresh', tokenPairJson)
      ..reply('GET', '/api/v1/manage/session', session ?? ownerSessionJson);
  }

  /// Uygulamayı açar ve yerleşmesini bekler.
  Future<void> pumpApp(WidgetTester tester, {bool settle = true}) async {
    // Tasarım referansı 390×844.
    tester.view
      ..physicalSize = const Size(390, 844)
      ..devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    await tester.pumpWidget(
      ProviderScope(
        overrides: overrides,
        retry: noRetry,
        child: const IsletmeApp(),
      ),
    );
    if (settle) {
      await tester.pumpAndSettle();
    } else {
      for (var i = 0; i < 10; i++) {
        await tester.pump(const Duration(milliseconds: 50));
      }
    }
  }
}
