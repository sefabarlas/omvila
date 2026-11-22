import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_ui/omvila_ui.dart';

import '../helpers/golden.dart';

// Kural: mobil components/TimeSlotGrid — 4 kolon; dolu saat üstü çizili ve
// devre dışı; seçili saat koyu dolgu.
const _saatler = [
  SaatSecenegi(etiket: '12:00', dolu: true),
  SaatSecenegi(etiket: '12:45'),
  SaatSecenegi(etiket: '13:30', dolu: true),
  SaatSecenegi(etiket: '14:30'),
  SaatSecenegi(etiket: '15:15'),
  SaatSecenegi(etiket: '16:00', dolu: true),
];

void main() {
  testWidgets('boş saat seçilir; dolu saat seçilemez', (tester) async {
    final secilen = <String>[];
    await tester.pumpWidget(
      omvilaApp(
        SizedBox(
          width: 358,
          child: SaatIzgarasi(
            saatler: _saatler,
            secili: '14:30',
            onSecildi: secilen.add,
          ),
        ),
      ),
    );
    await tester.tap(find.text('12:45'));
    await tester.tap(find.text('12:00'), warnIfMissed: false);
    expect(secilen, ['12:45']);
    expect(find.bySemanticsLabel('16:00, dolu'), findsOneWidget);
    final first = tester.getTopLeft(find.text('12:00'));
    final fifth = tester.getTopLeft(find.text('15:15'));
    expect(fifth.dy, greaterThan(first.dy));
  });

  omvilaGoldenTest(
    'SaatIzgarasi',
    fileName: 'saat_izgarasi',
    builder: (_) =>
        SaatIzgarasi(saatler: _saatler, secili: '14:30', onSecildi: (_) {}),
  );
}
