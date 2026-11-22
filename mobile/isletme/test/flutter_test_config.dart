import 'dart:async';
import 'dart:io';
import 'dart:ui' as ui;

import 'package:alchemist/alchemist.dart';
import 'package:flutter/services.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_ui/theme/omvila_theme.dart';

/// packages/omvila_ui/test/flutter_test_config.dart ile aynı karşılaştırıcı
/// (test dosyaları paketler arasında içe aktarılamaz); fark: CI goldenleri
/// test yazı tipiyle çizilir (aşağıda).
/// Golden ayarı (docs/standartlar/test-stratejisi.md: alchemist, açık/koyu,
/// 1.3× yazı). Depoya yalnız CI goldenleri girer (`goldens/ci/`): metin blok
/// olarak çizilir, gölge çizilmez; yazı tipinden bağımsız. macOS ve Linux
/// arasındaki kenar yumuşatma farkını [_AntiAliasTolerantComparator] tolere
/// eder. Gerçek yazı tipiyle bakmak için `OMVILA_PLATFORM_GOLDENS=1 flutter
/// test --update-goldens` (çıktı `goldens/macos/` gibi, depo dışı).
Future<void> testExecutable(FutureOr<void> Function() testMain) async {
  TestWidgetsFlutterBinding.ensureInitialized();
  final platformGoldens =
      Platform.environment['OMVILA_PLATFORM_GOLDENS'] == '1';
  // Ekran goldenlerinde çok sayıda metin var; gerçek yazı tipinin alt piksel
  // genişlikleri macOS ve Linux'ta farklı yuvarlanıp metin bloklarını 1 px
  // kaydırıyor. CI goldenleri bu yüzden test yazı tipiyle (FlutterTest, her
  // platformda aynı) çizilir; gerçek yazı tipi yalnız platform goldenlerinde.
  if (platformGoldens) {
    await _loadPackageFonts();
  }

  if (goldenFileComparator case final LocalFileComparator local) {
    goldenFileComparator = _AntiAliasTolerantComparator(
      local.basedir.resolve('flutter_test_config.dart'),
    );
  }

  return AlchemistConfig.runWithConfig(
    config: AlchemistConfig(
      theme: OmvilaTema.acik(),
      platformGoldensConfig: PlatformGoldensConfig(enabled: platformGoldens),
    ),
    run: () async => testMain(),
  );
}

/// Tema yazı tipini paket önekiyle ister (`packages/omvila_ui/…`); test
/// ortamı yazı tiplerini kendiliğinden yüklemediği için elle yüklenir.
Future<void> _loadPackageFonts() async {
  const files = {
    OmvilaYazi.aile:
        'packages/omvila_ui/fonts/PlusJakartaSans-Latin-Variable.ttf',
    'PlusJakartaSansTR':
        'packages/omvila_ui/fonts/PlusJakartaSans-LatinExt-Variable.ttf',
  };
  for (final MapEntry(key: family, value: file) in files.entries) {
    final loader = FontLoader('packages/${OmvilaYazi.paket}/$family')
      ..addFont(rootBundle.load(file));
    await loader.load();
  }
}

/// macOS ve Linux kenar yumuşatmayı (eğri kenar, nokta, ikon) birkaç tonla
/// farklı çizer: CI'da ölçülen en büyük kanal farkı 17/255, farklı piksel oranı
/// en çok %0,65. Bu yüzden iki koşul birlikte aranır: hiçbir kanal
/// [_maxChannelDelta]'dan fazla farklı değil ve farklı piksellerin oranı
/// [_maxDiffRatio]'yu aşmıyor. Düz bir rengin veya yerleşimin değişmesi çok
/// sayıda pikseli etkilediği için yine yakalanır. Aksi halde Flutter'ın
/// karşılaştırıcısı çalışır ve `failures/` görüntülerini yazar.
class _AntiAliasTolerantComparator extends LocalFileComparator {
  _AntiAliasTolerantComparator(super.testFile);

  static const _maxChannelDelta = 24;
  static const _maxDiffRatio = 0.01;

  @override
  Future<bool> compare(Uint8List imageBytes, Uri golden) async {
    final goldenBytes = await getGoldenBytes(golden);
    final actual = await _rgba(imageBytes);
    final expected = await _rgba(Uint8List.fromList(goldenBytes));
    if (actual.width == expected.width &&
        actual.height == expected.height &&
        _withinTolerance(actual.bytes, expected.bytes)) {
      return true;
    }

    return super.compare(imageBytes, golden);
  }

  static bool _withinTolerance(Uint8List a, Uint8List b) {
    if (a.length != b.length) {
      return false;
    }

    var differing = 0;
    for (var i = 0; i < a.length; i += 4) {
      var delta = 0;
      for (var c = 0; c < 4; c++) {
        final d = (a[i + c] - b[i + c]).abs();
        if (d > delta) {
          delta = d;
        }
      }

      if (delta > _maxChannelDelta) {
        return false;
      }

      if (delta > 0) {
        differing++;
      }
    }

    return differing <= (a.length ~/ 4) * _maxDiffRatio;
  }

  static Future<({int width, int height, Uint8List bytes})> _rgba(
    Uint8List png,
  ) async {
    final codec = await ui.instantiateImageCodec(png);
    final image = (await codec.getNextFrame()).image;
    final data = await image.toByteData();
    final result = (
      width: image.width,
      height: image.height,
      bytes: data!.buffer.asUint8List(),
    );
    image.dispose();
    codec.dispose();
    return result;
  }
}
