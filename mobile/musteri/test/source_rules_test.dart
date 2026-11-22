import 'dart:io';

import 'package:flutter_test/flutter_test.dart';

// Kural: mobile/musteri/AGENTS.md, kodlama-flutter.md (packages/omvila_ui
// ile aynı denetim): renk yalnız tema uzantısından; sabit hex (`Color(0x…)`)
// ve ham `Colors.*` yasak. Saydamlık renk seçimi olmadığından
// `Colors.transparent` serbesttir. "Şimdi" yalnız enjekte saatten
// (clockProvider); `DateTime.now()` yasak.
void main() {
  final files = Directory('lib')
      .listSync(recursive: true)
      .whereType<File>()
      .where((f) => f.path.endsWith('.dart'))
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

  test('DateTime.now yok', () {
    final offenders = [
      for (final file in files)
        if (file.readAsStringSync().contains('DateTime.now()')) file.path,
    ];
    expect(offenders, isEmpty);
  });
}
