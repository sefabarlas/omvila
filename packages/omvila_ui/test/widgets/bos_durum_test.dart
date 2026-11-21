import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_ui/omvila_ui.dart';

import '../helpers/golden.dart';

// Kural: mobil README § Boş durumlar — ikon kutusu, başlık, cümle, en çok bir
// eylem; sahte sayı yok.
void main() {
  testWidgets('başlık, açıklama ve tek eylem', (tester) async {
    var taps = 0;
    await tester.pumpWidget(
      omvilaApp(
        OmvilaBosDurum(
          ikon: Icons.notifications_none,
          baslik: 'Yeni bildirim yok',
          aciklama: 'Yeni randevu ve iptaller burada görünür.',
          eylemEtiketi: 'Bildirim ayarları',
          onEylem: () => taps++,
        ),
      ),
    );
    expect(find.text('Yeni bildirim yok'), findsOneWidget);
    await tester.tap(find.text('Bildirim ayarları'));
    expect(taps, 1);
  });

  testWidgets('eylem verilmezse buton yok', (tester) async {
    await tester.pumpWidget(
      omvilaApp(
        const OmvilaBosDurum(
          ikon: Icons.event,
          baslik: 'Henüz randevu yok',
          aciklama: 'Randevu aldığınızda burada görünür.',
        ),
      ),
    );
    expect(find.byType(FilledButton), findsNothing);
  });

  omvilaGoldenTest(
    'OmvilaBosDurum',
    fileName: 'bos_durum',
    builder: (_) => OmvilaBosDurum(
      ikon: Icons.notifications_none,
      baslik: 'Yeni bildirim yok',
      aciklama:
          'Yeni randevu, onay bekleyen talep, iptal ve sıra hareketleri '
          'burada görünür.',
      eylemEtiketi: 'Bildirim ayarları',
      eylemIkonu: Icons.settings_outlined,
      onEylem: () {},
    ),
  );
}
