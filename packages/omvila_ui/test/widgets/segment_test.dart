import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_ui/omvila_ui.dart';

import '../helpers/golden.dart';

// Kural: components/SegmentedControl — eşit genişlik, seçili zemin
// yükseltilmiş; 1.3× yazıda etiketler küçülür, birbirine değmez.
void main() {
  const ogeler = [
    OmvilaSegmentOgesi('tumu', 'Tümü'),
    OmvilaSegmentOgesi('onay', 'Onay bekleyen'),
    OmvilaSegmentOgesi('okunmamis', 'Okunmamış'),
  ];

  testWidgets('dokununca seçer; seçili işaretlenir', (tester) async {
    String? secilen;
    await tester.pumpWidget(
      omvilaApp(
        OmvilaSegment<String>(
          ogeler: ogeler,
          secili: 'tumu',
          onSecildi: (v) => secilen = v,
        ),
      ),
    );
    await tester.tap(find.text('Okunmamış'));
    expect(secilen, 'okunmamis');
    final node = tester.getSemantics(find.text('Tümü'));
    expect(node.label, 'Tümü');
    expect(
      tester.getSemantics(find.text('Tümü')),
      isSemantics(isSelected: true, isButton: true),
    );
    expect(
      tester.getSemantics(find.text('Okunmamış')),
      isSemantics(isSelected: false, isButton: true),
    );
  });

  testWidgets('1.3× yazıda etiketler kendi diliminde kalır', (tester) async {
    tester.view
      ..physicalSize = const Size(358, 200)
      ..devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    await tester.pumpWidget(
      omvilaApp(
        MediaQuery(
          data: const MediaQueryData(textScaler: TextScaler.linear(1.3)),
          child: OmvilaSegment<String>(
            ogeler: ogeler,
            secili: 'tumu',
            onSecildi: (_) {},
          ),
        ),
      ),
    );
    final a = tester.getRect(find.text('Onay bekleyen'));
    final b = tester.getRect(find.text('Okunmamış'));
    expect(a.right, lessThan(b.left));
  });

  omvilaGoldenTest(
    'OmvilaSegment',
    fileName: 'segment',
    builder: (_) => OmvilaSegment<String>(
      ogeler: ogeler,
      secili: 'onay',
      onSecildi: (_) {},
    ),
  );
}
