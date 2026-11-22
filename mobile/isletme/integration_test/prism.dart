import 'package:flutter/material.dart';
import 'package:flutter_riverpod/misc.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_isletme/app/providers.dart';
import 'package:omvila_isletme/shared/external_launcher.dart';
import 'package:omvila_isletme/shared/key_value_store.dart';
import 'package:omvila_ui/omvila_ui.dart';

// Prism mock'a karşı uçtan uca akışın bağımlılıkları
// (docs/standartlar/test-stratejisi.md § Flutter entegrasyon). İstekler gerçek
// HTTP ile `--dart-define=API_URL` adresindeki Prism'e gider (masaüstünde ve
// flutter_tester'da http://127.0.0.1:4010; Prism yalnız IPv4 dinler). Yalnız
// cihaza bağlı katmanlar (güvenli depo, tel:/sms:) ve saat değiştirilir; push
// zaten sahte (K4).

/// Sözleşme örnekleriyle uyumlu sabit saat: 25 Kasım 2026 Çarşamba 12:10
/// İstanbul (Bugün örneğinde sıradaki randevu 13:00).
final prismClock = FixedClock.parse('2026-11-25T09:10:00Z');

/// Bellekte depo (flutter_secure_storage yerine).
final class MemoryStore implements KeyValueStore {
  final _values = <String, String>{};

  @override
  Future<String?> read(String key) async => _values[key];

  @override
  Future<void> write(String key, String? value) async {
    if (value == null) {
      _values.remove(key);
    } else {
      _values[key] = value;
    }
  }
}

/// tel:/sms: açmaz.
final class NoLauncher implements ExternalLauncher {
  @override
  Future<bool> open(Uri url) async => false;
}

/// Uygulamanın Prism için sağlayıcı değişiklikleri.
List<Override> prismOverrides() => [
  keyValueStoreProvider.overrideWithValue(MemoryStore()),
  clockProvider.overrideWithValue(prismClock),
  externalLauncherProvider.overrideWithValue(NoLauncher()),
];

/// [until] görünene kadar (gerçek HTTP; pumpAndSettle ağ beklemez) en çok
/// 5 sn bekler, sonra ekran oturur.
Future<void> settleNetwork(WidgetTester tester, {required Finder until}) async {
  for (var i = 0; i < 100 && until.evaluate().isEmpty; i++) {
    await tester.pump(const Duration(milliseconds: 50));
  }

  await tester.pumpAndSettle();
}

/// Metne dokunur (görünür değilse kaydırır) ve [expect] metnini bekler.
Future<void> tapText(
  WidgetTester tester,
  String text, {
  required String expect,
}) async {
  final finder = find.text(text).last;
  await tester.ensureVisible(finder);
  await tester.pumpAndSettle();
  await tester.tap(finder);
  await settleNetwork(tester, until: find.text(expect));
}

/// Telefon ekranı (390×844).
void usePhone(WidgetTester tester) {
  tester.view
    ..physicalSize = const Size(390, 844)
    ..devicePixelRatio = 1;
  addTearDown(tester.view.reset);
}
