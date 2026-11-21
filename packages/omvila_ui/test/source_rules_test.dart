import 'dart:io';

import 'package:flutter_test/flutter_test.dart';

// Kural: packages/omvila_ui/AGENTS.md, kodlama-flutter.md — renk yalnız tema
// uzantısından; sabit hex (`Color(0x…)`) ve ham `Colors.*` yalnız üretilmiş
// tema dosyasında. Saydamlık renk seçimi olmadığından `Colors.transparent`
// serbesttir. "Şimdi" yalnız time.dart'taki SystemClock'tan.
void main() {
  final files = Directory('lib')
      .listSync(recursive: true)
      .whereType<File>()
      .where((f) => f.path.endsWith('.dart'))
      .where((f) => !f.path.endsWith('theme/omvila_theme.dart'))
      .toList();

  test('lib/ taranıyor', () {
    expect(files.length, greaterThan(5));
  });

  test('sabit renk yok', () {
    final offenders = <String>[];
    for (final file in files) {
      final source = file.readAsStringSync();
      if (RegExp(r'Color\(0x|Color\.fromRGBO|Color\.fromARGB')
              .hasMatch(source) ||
          RegExp(r'Colors\.(?!transparent\b)\w+').hasMatch(source)) {
        offenders.add(file.path);
      }
    }

    expect(offenders, isEmpty);
  });

  test('DateTime.now yalnız SystemClock', () {
    final offenders = [
      for (final file in files)
        if (file.readAsStringSync().contains('DateTime.now()') &&
            !file.path.endsWith('time.dart'))
          file.path,
    ];
    expect(offenders, isEmpty);
  });
}
