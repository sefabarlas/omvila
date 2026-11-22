import 'dart:convert';
import 'dart:typed_data';

import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_api/omvila_api.dart';

// Üretilmiş Dart istemcisinin (packages/api-client-dart) tarih ve dizi sorgu
// parametresi biçimi. Şablonlar: contracts/templates/dart-dio (T-110).
// Sözleşme: `format: date` "yyyy-MM-dd"; `staffIds` style: form,
// explode: false.
class _RecordingAdapter implements HttpClientAdapter {
  final requests = <RequestOptions>[];

  @override
  Future<ResponseBody> fetch(
    RequestOptions options,
    Stream<Uint8List>? requestStream,
    Future<void>? cancelFuture,
  ) async {
    requests.add(options);
    // Gövde testin konusu değil; çözümleme hatası beklenir ve yutulur.
    return ResponseBody.fromString(
      '{}',
      200,
      headers: {
        Headers.contentTypeHeader: ['application/json'],
      },
    );
  }

  @override
  void close({bool force = false}) {}
}

void main() {
  late _RecordingAdapter adapter;
  late OmvilaApi api;

  setUp(() {
    adapter = _RecordingAdapter();
    final dio = Dio(BaseOptions(baseUrl: 'http://api.test'))
      ..httpClientAdapter = adapter;
    api = OmvilaApi(dio: dio, interceptors: const []);
  });

  Future<void> send(Future<void> Function() call) async {
    try {
      await call();
    } on DioException {
      // Sahte yanıt modele uymaz; yalnız giden istek incelenir.
    }
  }

  group('format: date gövde alanları', () {
    test('"yyyy-MM-dd" olarak yazılır', () {
      final body = WorkingHoursOverrideCreate(
        fromDate: DateTime(2026, 11, 25),
        toDate: DateTime.utc(2026, 11, 27, 23, 30),
        kind: WorkingHoursOverrideCreateKindEnum.holiday,
        isClosed: true,
      ).toJson();

      expect(body['fromDate'], '2026-11-25');
      expect(body['toDate'], '2026-11-27');
    });

    test('"yyyy-MM-dd" değeri okunur', () {
      final model = WorkingHoursOverrideCreate.fromJson({
        'fromDate': '2026-11-25',
        'kind': 'Holiday',
        'isClosed': true,
      });

      expect(model.fromDate, DateTime(2026, 11, 25));
      expect(model.toDate, isNull);
    });

    test('istek gövdesinde gün olarak gider', () async {
      await send(
        () => api.getManageBranchesApi().manageCreateBranchOverride(
          branchId: 'b1',
          workingHoursOverrideCreate: WorkingHoursOverrideCreate(
            fromDate: DateTime(2026, 11, 25),
            kind: WorkingHoursOverrideCreateKindEnum.closure,
            isClosed: true,
          ),
        ),
      );

      final sent = jsonDecode(adapter.requests.single.data as String) as Map;
      expect(sent['fromDate'], '2026-11-25');
    });
  });

  test('date-time alanları UTC ISO 8601 olarak kalır', () {
    final body = MeRescheduleRequest(
      startUtc: DateTime.utc(2026, 11, 25, 7, 30),
    ).toJson();

    expect(body['startUtc'], '2026-11-25T07:30:00.000Z');
  });

  group('sorgu parametreleri', () {
    test('date parametresi "yyyy-MM-dd" olarak gider', () async {
      await send(
        () => api.getManageSalesApi().manageGetCashDay(
          branchId: 'b1',
          date: DateTime(2026, 11, 25),
        ),
      );

      final uri = adapter.requests.single.uri;
      expect(uri.queryParameters['date'], '2026-11-25');
      expect(uri.query, contains('date=2026-11-25'));
    });

    test('staffIds virgülle ayrılmış tek parametre olarak gider', () async {
      await send(
        () => api.getManageCalendarApi().manageGetCalendar(
          branchId: 'b1',
          from: DateTime(2026, 11, 25),
          to: DateTime(2026, 11, 26),
          staffIds: ['a', 'b'],
        ),
      );

      final uri = adapter.requests.single.uri;
      expect(uri.queryParametersAll['staffIds'], ['a,b']);
      expect(uri.query, contains('staffIds=a,b'));
      expect(uri.queryParameters['from'], '2026-11-25');
      expect(uri.queryParameters['to'], '2026-11-26');
    });
  });
}
