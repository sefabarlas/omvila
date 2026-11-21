import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_ui/omvila_ui.dart';

import '../helpers/golden.dart';

// Kural: mobil README § Renk — blok-marka/vurgu/kum ve üstlerindeki metin
// tokenı; üst etiket Türkçe büyük harf.
void main() {
  testWidgets('üst etiket Türkçe büyük harfle, ton rengi', (tester) async {
    await tester.pumpWidget(
      omvilaApp(
        const BentoBlok(
          ton: BentoTonu.kum,
          ustEtiket: 'Sıradaki',
          deger: '3',
        ),
      ),
    );
    expect(find.text('SIRADAKİ'), findsOneWidget);
    final value = tester.widget<Text>(find.text('3'));
    expect(value.style?.color, OmvilaRenkler.acik.blokKumMetin);
    expect(find.byIcon(Icons.chevron_right), findsNothing);
  });

  testWidgets('dokunulabilir blok ok gösterir', (tester) async {
    var taps = 0;
    await tester.pumpWidget(
      omvilaApp(
        BentoBlok(
          ton: BentoTonu.vurgu,
          ustEtiket: 'Onay bekliyor',
          deger: '2',
          onTap: () => taps++,
        ),
      ),
    );
    await tester.tap(find.byType(BentoBlok));
    expect(taps, 1);
    expect(find.byIcon(Icons.chevron_right), findsOneWidget);
  });

  omvilaGoldenTest(
    'BentoBlok',
    fileName: 'bento_blok',
    builder: (context) => Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      spacing: OmvilaBosluk.bosluk3,
      children: [
        Expanded(
          child: BentoBlok(
            ton: BentoTonu.marka,
            ustEtiket: 'Sıradaki',
            deger: '13:00',
            baslik: 'Derya Şahin',
            aciklama: 'Saç kesimi · 45 dk',
            alt: Container(
              height: OmvilaBoyut.kontrolSm,
              alignment: Alignment.center,
              decoration: BoxDecoration(
                color: context.omvilaRenkler.marka,
                borderRadius: BorderRadius.circular(OmvilaKose.koseSm),
              ),
              child: Text(
                '50 dk sonra',
                style: OmvilaYazi.etiketKucuk.copyWith(
                  color: context.omvilaRenkler.markaUstu,
                ),
              ),
            ),
          ),
        ),
        Expanded(
          child: Column(
            spacing: OmvilaBosluk.bosluk3,
            children: [
              BentoBlok(
                ton: BentoTonu.vurgu,
                ustEtiket: 'Onay bekliyor',
                deger: '2',
                onTap: () {},
              ),
              const BentoBlok(
                ton: BentoTonu.kum,
                ustEtiket: 'Sırada',
                deger: '3',
                aciklama: '~35 dk bekleme',
              ),
            ],
          ),
        ),
      ],
    ),
  );
}
