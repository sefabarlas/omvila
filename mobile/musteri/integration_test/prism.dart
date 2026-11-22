import 'package:dio/dio.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/misc.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_musteri/app/config.dart';
import 'package:omvila_musteri/app/providers.dart';
import 'package:omvila_musteri/features/auth/data/auth_repository.dart';
import 'package:omvila_musteri/features/auth/data/sms_code_retriever.dart';
import 'package:omvila_musteri/shared/external_actions.dart';
import 'package:omvila_musteri/shared/key_value_store.dart';
import 'package:omvila_ui/omvila_ui.dart';

// Prism mock'a karşı uçtan uca akışın bağımlılıkları
// (docs/standartlar/test-stratejisi.md § Flutter entegrasyon). İstekler gerçek
// HTTP ile Prism'e gider; yalnız cihaza bağlı katmanlar (güvenli depo, SMS
// okuma, harita/takvim) ve saat değiştirilir. Push zaten sahte (K4).

/// Prism adresi: masaüstünde/flutter_tester'da 127.0.0.1 (Prism yalnız IPv4
/// dinler), Android emülatörde `--dart-define=API_URL=http://10.0.2.2:4010`.
const prismUrl = String.fromEnvironment(
  'API_URL',
  defaultValue: 'http://127.0.0.1:4010',
);

/// Sözleşme örnekleriyle uyumlu sabit saat: 24 Kasım 2026 Salı 20:30
/// İstanbul (örnek randevu 26 Kasım'da).
final prismClock = FixedClock.parse('2026-11-24T17:30:00Z');

/// Bellekte depo (flutter_secure_storage yerine).
final class MemoryStore implements KeyValueStore {
  final _values = <String, String>{};

  @override
  Future<String?> read(String key) async => _values[key];

  @override
  Future<void> write(String key, String value) async => _values[key] = value;

  @override
  Future<void> delete(String key) async => _values.remove(key);
}

/// Dış uygulama açmaz.
final class NoExternalActions implements ExternalActions {
  @override
  Future<bool> openUrl(Uri url) async => false;

  @override
  Future<bool> addToCalendar(CalendarEntry entry) async => false;
}

// Sözleşmede katalog, gün ve saat yanıtlarının örneği yok; Prism şemadan
// tutarsız veri üretir (kategorisi olmayan hizmet, 2019 tarihleri). Web
// uçtan uca testleri gibi (web/rezervasyon/e2e) bu üç uç test verisiyle
// yanıtlanır, diğer her istek Prism'e gider. Kimlikler sözleşme
// örneklerindeki salonla aynı (Kuaför Elif, Kadıköy şubesi).
const _staffId = '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5e41';
const _serviceId = '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5d32';
const _categoryId = '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5c01';

Map<String, Object?> _money(int lira) => {
  'amountMinor': lira * 100,
  'currency': 'TRY',
};

Map<String, Object?> _catalog(String branchId) => {
  'branchId': branchId,
  'timeZone': 'Europe/Istanbul',
  'allowStaffSelection': true,
  'allowAnyStaffOption': true,
  'categories': [
    {'id': _categoryId, 'name': 'Saç'},
  ],
  'services': [
    {
      'id': _serviceId,
      'categoryId': _categoryId,
      'name': 'Saç kesimi',
      'durationMinutes': 45,
      'price': _money(450),
      'priceDisplay': 'Fixed',
      'hasDeposit': false,
      'staffIds': [_staffId],
    },
  ],
  'staff': [
    {'id': _staffId, 'displayName': 'Elif Kaya', 'title': 'Kurucu'},
  ],
};

/// 25-27 Kasım boş saatli.
Map<String, Object?> _days() => {
  'timeZone': 'Europe/Istanbul',
  'days': [
    for (var day = 24; day <= 30; day++)
      {
        'date': '2026-11-$day',
        'hasAvailability': day >= 25 && day <= 27,
      },
  ],
};

Map<String, Object?> _slots(String date) {
  Map<String, Object?> slot(int hour) {
    final start = DateTime.parse('${date}T00:00:00Z').add(
      Duration(hours: hour - 3, minutes: 30),
    );
    return {
      'startUtc': start.toIso8601String(),
      'localTime': '$hour:30'.padLeft(5, '0'),
      'period': hour < 12 ? 'Morning' : (hour < 17 ? 'Afternoon' : 'Evening'),
      'items': [
        {
          'serviceId': _serviceId,
          'staffId': _staffId,
          'staffName': 'Elif Kaya',
          'startUtc': start.toIso8601String(),
          'endUtc': start.add(const Duration(minutes: 45)).toIso8601String(),
          'price': _money(450),
        },
      ],
      'total': _money(450),
    };
  }

  return {
    'timeZone': 'Europe/Istanbul',
    'date': date,
    'slots': [slot(10), slot(14)],
    'unavailableTimes': ['12:00'],
  };
}

final _catalogPath = RegExp(r'/public/branches/([^/]+)/catalog$');
final _daysPath = RegExp(r'/public/branches/[^/]+/availability/days$');
final _slotsPath = RegExp(r'/public/branches/[^/]+/availability/slots$');

/// Katalog, gün ve saat uçlarını test verisiyle yanıtlar.
final class CatalogStub extends Interceptor {
  /// Yanıtlanan istekler (yol).
  final answered = <String>[];

  @override
  void onRequest(RequestOptions options, RequestInterceptorHandler handler) {
    final path = options.uri.path;
    final Object? body = switch (path) {
      _ when _catalogPath.hasMatch(path) => _catalog(
        _catalogPath.firstMatch(path)!.group(1)!,
      ),
      _ when _daysPath.hasMatch(path) => _days(),
      _ when _slotsPath.hasMatch(path) => _slots(_dateOf(options.data)),
      _ => null,
    };
    if (body == null) {
      handler.next(options);
      return;
    }

    answered.add(path);
    handler.resolve(
      Response<Object?>(requestOptions: options, statusCode: 200, data: body),
    );
  }

  static String _dateOf(Object? data) {
    final map = data is Map ? data : const <String, Object?>{};
    final date = map['date'];
    return date is String ? date.substring(0, 10) : '2026-11-25';
  }
}

/// Prism'e giden uygulamanın sağlayıcı değişiklikleri.
List<Override> prismOverrides(CatalogStub stub) => [
  configProvider.overrideWithValue(const AppConfig(apiUrl: prismUrl)),
  clockProvider.overrideWithValue(prismClock),
  keyValueStoreProvider.overrideWithValue(MemoryStore()),
  smsCodeRetrieverProvider.overrideWithValue(const NoSmsCodeRetriever()),
  externalActionsProvider.overrideWithValue(NoExternalActions()),
  // Uygulamanın kendi kurulumu (providers.dart) + katalog yanıtlayıcı.
  omvilaApiProvider.overrideWith((ref) {
    final dio = createOmvilaDio(
      baseUrl: prismUrl,
      session: ref.watch(customerSessionProvider),
      headers: const {'X-Client': customerAppClient},
    );
    dio.interceptors.add(stub);
    return createOmvilaApi(dio);
  }),
];

/// [until] görünene kadar (gerçek HTTP; pumpAndSettle ağ beklemez) en çok
/// 5 sn bekler, sonra ekran oturur.
Future<void> settleNetwork(WidgetTester tester, {required Finder until}) async {
  for (var i = 0; i < 100 && until.evaluate().isEmpty; i++) {
    await tester.pump(const Duration(milliseconds: 50));
  }

  await tester.pumpAndSettle();
}

/// Metne dokunur (görünür değilse kaydırır); [expect] verilirse o metni
/// bekler (ağ isteği), verilmezse yalnız ekran oturur.
Future<void> tapText(WidgetTester tester, String text, {String? expect}) async {
  final finder = find.text(text).last;
  await tester.ensureVisible(finder);
  await tester.pumpAndSettle();
  await tester.tap(finder);
  if (expect == null) {
    await tester.pumpAndSettle();
  } else {
    await settleNetwork(tester, until: find.text(expect));
  }
}

/// Telefon ekranı (390×844).
void usePhone(WidgetTester tester) {
  tester.view
    ..physicalSize = const Size(390, 844)
    ..devicePixelRatio = 1;
  addTearDown(tester.view.reset);
}
