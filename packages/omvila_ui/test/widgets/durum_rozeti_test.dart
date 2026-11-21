import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_ui/omvila_ui.dart';

import '../helpers/golden.dart';

// Kural: AGENTS.md § 1.6 ve § 5 — yedi durum, sabit adlar; "Kapora bekleniyor"
// türetilmiş gösterim; Draft gösterilmez.
void main() {
  test('displayStatus eşlemesi ve sabit durum adları', () {
    expect(
      {
        for (final s in DisplayStatus.values)
          s.value: RandevuDurumu.fromDisplayStatus(s)?.etiket,
      },
      {
        'PendingApproval': 'Onay bekliyor',
        'DepositPending': 'Kapora bekleniyor',
        'Confirmed': 'Onaylandı',
        'CheckedIn': 'Geldi',
        'Completed': 'Tamamlandı',
        'NoShow': 'Gelmedi',
        'Cancelled': 'İptal',
        'unknown_default_open_api': null,
      },
    );
  });

  testWidgets('durum adı yazılır ve renk durum tokenından gelir', (
    tester,
  ) async {
    await tester.pumpWidget(
      omvilaApp(
        const OmvilaDurumRozeti(durum: RandevuDurumu.kaporaBekleniyor),
      ),
    );
    final text = tester.widget<Text>(find.text('Kapora bekleniyor'));
    expect(text.style?.color, OmvilaRenkler.acik.durumKaporaMetin);
    expect(find.bySemanticsLabel('Kapora bekleniyor'), findsOneWidget);
  });

  omvilaGoldenTest(
    'OmvilaDurumRozeti',
    fileName: 'durum_rozeti',
    builder: (_) => Wrap(
      spacing: OmvilaBosluk.bosluk2,
      runSpacing: OmvilaBosluk.bosluk2,
      children: [
        for (final durum in RandevuDurumu.values)
          OmvilaDurumRozeti(durum: durum),
      ],
    ),
  );
}
