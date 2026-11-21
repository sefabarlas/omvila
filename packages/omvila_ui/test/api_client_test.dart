import 'dart:convert';
import 'dart:typed_data';

import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_ui/api/api_client.dart';
import 'package:omvila_ui/api/api_problem.dart';

// Kural: docs/sozlesme/api-kurallari.md § Idempotency, § Hatalar ve
// packages/api-client-ts/src/client.ts ile aynı davranış.
class _FakeAdapter implements HttpClientAdapter {
  _FakeAdapter(this.respond);

  final ResponseBody Function(RequestOptions options) respond;

  /// İstek başlıkları, gönderildiği andaki haliyle (tekrar aynı nesneyi
  /// değiştirir).
  final requests = <Map<String, dynamic>>[];

  @override
  Future<ResponseBody> fetch(
    RequestOptions options,
    Stream<Uint8List>? requestStream,
    Future<void>? cancelFuture,
  ) async {
    requests.add(Map.of(options.headers));
    return respond(options);
  }

  @override
  void close({bool force = false}) {}
}

ResponseBody _json(int status, Object body) => ResponseBody.fromString(
  jsonEncode(body),
  status,
  headers: {
    Headers.contentTypeHeader: [
      if (status >= 400) 'application/problem+json' else 'application/json',
    ],
  },
);

class _Session implements AuthSession {
  String token = 'eski';
  int refreshes = 0;
  bool canRefresh = true;

  @override
  String? accessToken() => token;

  @override
  Future<bool> refresh() async {
    refreshes++;
    await Future<void>.delayed(const Duration(milliseconds: 10));
    if (!canRefresh) {
      return false;
    }

    token = 'yeni';
    return true;
  }
}

void main() {
  test('jeton eklenir; token_expired sonrası tek yenileme ve tekrar', () async {
    final session = _Session();
    final adapter = _FakeAdapter(
      (o) => o.headers['Authorization'] == 'Bearer yeni'
          ? _json(200, {'ok': true})
          : _json(401, {'code': 'token_expired'}),
    );
    final dio = createOmvilaDio(
      baseUrl: 'http://api.test',
      session: session,
      httpClientAdapter: adapter,
    );

    final responses = await Future.wait([
      dio.get<Object?>('/a'),
      dio.get<Object?>('/b'),
    ]);

    expect(responses.map((r) => r.statusCode), [200, 200]);
    expect(session.refreshes, 1);
    expect(
      adapter.requests.map((r) => r['Authorization']),
      containsAll(['Bearer eski', 'Bearer yeni']),
    );
  });

  test('yenilenemezse hata ApiProblem olarak okunur', () async {
    final session = _Session()..canRefresh = false;
    final dio = createOmvilaDio(
      baseUrl: 'http://api.test',
      session: session,
      httpClientAdapter: _FakeAdapter(
        (_) => _json(401, {'code': 'token_expired', 'traceId': 'iz-1'}),
      ),
    );

    final error = await dio
        .get<Object?>('/a')
        .then<DioException?>(
          (_) => null,
          onError: (Object e) => e as DioException,
        );
    final problem = ApiProblem.fromDioException(error!);
    expect(problem.status, 401);
    expect(problem.code, 'token_expired');
    expect(problem.traceId, 'iz-1');
    expect(problem.message, 'Oturumunuzun süresi doldu. Yeniden giriş yapın.');
  });

  test(
    'POST eksikse Idempotency-Key alır; tekrar aynı anahtarı taşır',
    () async {
      var calls = 0;
      final adapter = _FakeAdapter(
        (_) => ++calls == 1
            ? _json(503, {'code': 'try_again'})
            : _json(201, {'id': '1'}),
      );
      final dio = createOmvilaDio(
        baseUrl: 'http://api.test',
        httpClientAdapter: adapter,
        idempotencyKey: () => 'anahtar-$calls',
      );

      final response = await dio.post<Object?>('/r', data: {'x': 1});
      await dio.post<Object?>(
        '/r',
        options: Options(headers: {'Idempotency-Key': 'verilen'}),
      );
      await dio.get<Object?>('/r');

      expect(response.statusCode, 201);
      final keys = adapter.requests.map((r) => r['Idempotency-Key']).toList();
      expect(keys, ['anahtar-0', 'anahtar-0', 'verilen', null]);
    },
  );

  test('giriş uçlarında jeton eklenmez ve yenileme denenmez', () async {
    final session = _Session();
    final adapter = _FakeAdapter((_) => _json(401, {'code': 'token_expired'}));
    final dio = createOmvilaDio(
      baseUrl: 'http://api.test',
      session: session,
      httpClientAdapter: adapter,
    );

    await expectLater(
      dio.post<Object?>(
        '/auth/refresh',
        options: Options(extra: {skipAuthExtra: true}),
      ),
      throwsA(isA<DioException>()),
    );
    expect(adapter.requests.single['Authorization'], isNull);
    expect(session.refreshes, 0);
  });

  test('alan hataları ve bağlantı hatası', () {
    final options = RequestOptions(path: '/x');
    final validation = ApiProblem.fromDioException(
      DioException(
        requestOptions: options,
        response: Response(
          requestOptions: options,
          statusCode: 400,
          data: {
            'code': 'validation_failed',
            'errors': {
              'amount': ['out_of_range'],
            },
          },
        ),
      ),
    );
    expect(validation.errors, {
      'amount': ['out_of_range'],
    });

    final offline = ApiProblem.fromDioException(
      DioException.connectionError(requestOptions: options, reason: 'yok'),
    );
    expect(offline.isNetworkError, isTrue);
    expect(offline.status, 0);
  });

  test('yardımcılar', () {
    final key = newIdempotencyKey();
    expect(
      key,
      matches(
        RegExp(
          '^[0-9a-f]{8}-[0-9a-f]{4}-4[0-9a-f]{3}-'
          r'[89ab][0-9a-f]{3}-[0-9a-f]{12}$',
        ),
      ),
    );
    expect(newIdempotencyKey(), isNot(key));
    expect(ifMatch('W/"42"'), 'W/"42"');
  });
}
