// Dio kurulumu (docs/sozlesme/api-kurallari.md § Idempotency, eşzamanlı
// düzenleme, hatalar). packages/api-client-ts/src/client.ts ile aynı davranış:
// her denemede güncel erişim jetonu, POST'ta eksikse Idempotency-Key,
// `401 token_expired` sonrası tek (paylaşılan) yenileme ve bir tekrar,
// `503 try_again` sonrası bir tekrar. Jeton deposu ve yenileme çağrısı
// uygulamanındır (flutter_secure_storage, manage/me refresh ucu).
import 'dart:async';
import 'dart:math';

import 'package:dio/dio.dart';
import 'package:omvila_api/omvila_api.dart';

/// Uygulamanın verdiği oturum: jeton okuma ve yenileme.
abstract interface class AuthSession {
  /// Geçerli erişim jetonu; oturum yoksa null. Her denemede okunur.
  FutureOr<String?> accessToken();

  /// `401 token_expired` sonrası çağrılır; yenilendiyse true. Eşzamanlı
  /// 401'ler tek yenilemeyi paylaşır. Yenileme isteği bu Dio'dan geçiyorsa
  /// [skipAuthExtra] ile işaretlenmelidir.
  Future<bool> refresh();
}

/// İsteğin `extra` alanında true ise Authorization eklenmez ve yenileme
/// denenmez (giriş ve yenileme uçları).
const skipAuthExtra = 'omvilaSkipAuth';

const _retriedExtra = 'omvilaRetried';

/// Kaynak oluşturan ve para kaydeden POST'lar için anahtar (UUID v4). Aynı
/// kullanıcı eylemi tekrar denenirken aynı anahtar kullanılır.
String newIdempotencyKey([Random? random]) {
  final r = random ?? Random.secure();
  final bytes = List<int>.generate(16, (_) => r.nextInt(256));
  bytes[6] = (bytes[6] & 0x0f) | 0x40;
  bytes[8] = (bytes[8] & 0x3f) | 0x80;
  final hex = bytes.map((b) => b.toRadixString(16).padLeft(2, '0')).join();
  return '${hex.substring(0, 8)}-${hex.substring(8, 12)}-'
      '${hex.substring(12, 16)}-${hex.substring(16, 20)}-${hex.substring(20)}';
}

/// Düzenleme ve durum eylemleri için `If-Match` değeri: okunan kaydın
/// `version` alanı (ETag ile aynı değer).
String ifMatch(String version) => version;

/// Yanıtın `ETag` başlığı (varsa).
String? etagOf(Response<Object?> response) => response.headers.value('etag');

/// OMVILA API'si için yapılandırılmış Dio.
Dio createOmvilaDio({
  required String baseUrl,
  AuthSession? session,
  Map<String, String> headers = const {},
  String Function()? idempotencyKey,
  HttpClientAdapter? httpClientAdapter,
}) {
  final dio = Dio(
    BaseOptions(
      baseUrl: baseUrl,
      headers: headers,
      connectTimeout: const Duration(seconds: 10),
      receiveTimeout: const Duration(seconds: 20),
    ),
  );
  if (httpClientAdapter != null) {
    dio.httpClientAdapter = httpClientAdapter;
  }

  dio.interceptors.add(
    OmvilaInterceptor(
      dio: dio,
      session: session,
      idempotencyKey: idempotencyKey ?? newIdempotencyKey,
    ),
  );
  return dio;
}

/// Üretilmiş istemci; üreticinin varsayılan kimlik interceptor'ları eklenmez.
OmvilaApi createOmvilaApi(Dio dio) =>
    OmvilaApi(dio: dio, interceptors: const []);

/// Jeton, Idempotency-Key ve tekrar deneme interceptor'ı.
class OmvilaInterceptor extends Interceptor {
  /// [dio] tekrar denemede kullanılır (aynı interceptor'lardan geçer).
  OmvilaInterceptor({
    required this.dio,
    required this.idempotencyKey,
    this.session,
  });

  /// Tekrar denemede kullanılan Dio.
  final Dio dio;

  /// Uygulamanın oturumu; null ise jeton eklenmez.
  final AuthSession? session;

  /// Anahtar üreticisi.
  final String Function() idempotencyKey;

  Future<bool>? _refreshing;

  @override
  Future<void> onRequest(
    RequestOptions options,
    RequestInterceptorHandler handler,
  ) async {
    // Anahtar bir kez üretilir; tekrar denemeler aynı anahtarı taşır.
    if (options.method.toUpperCase() == 'POST' &&
        options.headers['Idempotency-Key'] == null) {
      options.headers['Idempotency-Key'] = idempotencyKey();
    }

    final session = this.session;
    if (session != null && options.extra[skipAuthExtra] != true) {
      final token = await session.accessToken();
      if (token != null && token.isNotEmpty) {
        options.headers['Authorization'] = 'Bearer $token';
      } else {
        options.headers.remove('Authorization');
      }
    }

    handler.next(options);
  }

  @override
  Future<void> onError(
    DioException err,
    ErrorInterceptorHandler handler,
  ) async {
    final options = err.requestOptions;
    final status = err.response?.statusCode;
    final code = _code(err.response?.data);
    if (options.extra[_retriedExtra] == true) {
      return handler.next(err);
    }

    final expired =
        status == 401 &&
        code == 'token_expired' &&
        options.extra[skipAuthExtra] != true &&
        session != null;
    final retryable = status == 503 && code == 'try_again';
    if (!expired && !retryable) {
      return handler.next(err);
    }

    if (expired && !await _refreshOnce()) {
      return handler.next(err);
    }

    try {
      options.extra[_retriedExtra] = true;
      handler.resolve(await dio.fetch<Object?>(options));
    } on DioException catch (e) {
      handler.next(e);
    }
  }

  Future<bool> _refreshOnce() {
    return _refreshing ??= session!.refresh().whenComplete(() {
      _refreshing = null;
    });
  }

  static String? _code(Object? data) {
    if (data is Map<String, dynamic> && data['code'] is String) {
      return data['code'] as String;
    }

    return null;
  }
}
