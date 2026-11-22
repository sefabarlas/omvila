import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_ui/omvila_ui.dart';

import '../helpers/golden.dart';

// Kural: mobil components/DayStrip — kısa gün adı ve numara; boş saatli günde
// nokta; kapalı gün devre dışı ("Kapalı"); seçili gün marka dolgu.
const _gunler = [
  GunSecenegi(tarih: '2026-11-24', uygun: true),
  GunSecenegi(tarih: '2026-11-25', uygun: true),
  GunSecenegi(tarih: '2026-11-26'),
  GunSecenegi(tarih: '2026-11-27', uygun: true),
  GunSecenegi(tarih: '2026-11-28', uygun: true),
  GunSecenegi(tarih: '2026-11-29', kapali: true),
];

void main() {
  testWidgets('gün seçilir; kapalı gün seçilemez', (tester) async {
    final secilen = <String>[];
    await tester.pumpWidget(
      omvilaApp(
        SizedBox(
          width: 390,
          child: GunSeridi(
            gunler: _gunler,
            secili: '2026-11-25',
            onSecildi: secilen.add,
          ),
        ),
      ),
    );
    expect(find.text('Sal'), findsOneWidget);
    expect(find.text('26'), findsOneWidget);
    expect(find.text('Kapalı'), findsOneWidget);
    await tester.tap(find.text('27'));
    await tester.tap(find.text('29'), warnIfMissed: false);
    expect(secilen, ['2026-11-27']);
    expect(
      find.bySemanticsLabel('Çarşamba 25 Kasım, boş saat var'),
      findsOneWidget,
    );
  });

  omvilaGoldenTest(
    'GunSeridi',
    fileName: 'gun_seridi',
    builder: (_) => GunSeridi(
      gunler: _gunler,
      secili: '2026-11-25',
      onSecildi: (_) {},
    ),
  );
}
