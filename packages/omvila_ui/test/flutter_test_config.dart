import 'dart:async';
import 'dart:io';

import 'package:alchemist/alchemist.dart';
import 'package:flutter/services.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_ui/theme/omvila_theme.dart';

/// Golden ayarı (docs/standartlar/test-stratejisi.md: alchemist, açık/koyu,
/// 1.3× yazı). Depoya yalnız CI goldenleri girer (`goldens/ci/`): metin blok
/// olarak çizilir, gölge çizilmez; yazı tipi ve işletim sisteminden bağımsız,
/// macOS ve Linux'ta aynı piksel. Gerçek yazı tipiyle bakmak için
/// `OMVILA_PLATFORM_GOLDENS=1 flutter test --update-goldens` (çıktı
/// `goldens/macos/` gibi, git dışı).
Future<void> testExecutable(FutureOr<void> Function() testMain) async {
  TestWidgetsFlutterBinding.ensureInitialized();
  await _loadPackageFonts();
  final platformGoldens =
      Platform.environment['OMVILA_PLATFORM_GOLDENS'] == '1';
  return AlchemistConfig.runWithConfig(
    config: AlchemistConfig(
      theme: OmvilaTema.acik(),
      platformGoldensConfig: PlatformGoldensConfig(enabled: platformGoldens),
    ),
    run: () async => testMain(),
  );
}

/// Tema yazı tipini paket önekiyle ister (`packages/omvila_ui/…`, uygulamada
/// böyle kayıtlıdır); paketin kendi testinde varlıklar öneksiz kaydedildiği
/// için aynı dosyalar önekli adla da yüklenir.
Future<void> _loadPackageFonts() async {
  const files = {
    OmvilaYazi.aile: 'fonts/PlusJakartaSans-Latin-Variable.ttf',
    'PlusJakartaSansTR': 'fonts/PlusJakartaSans-LatinExt-Variable.ttf',
  };
  for (final MapEntry(key: family, value: file) in files.entries) {
    final loader = FontLoader('packages/${OmvilaYazi.paket}/$family')
      ..addFont(rootBundle.load(file));
    await loader.load();
  }
}
