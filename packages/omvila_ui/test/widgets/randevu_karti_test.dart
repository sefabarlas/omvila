import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_ui/omvila_ui.dart';

import '../helpers/golden.dart';

// Kural: components/AppointmentTicket — onay bekleyen randevuda eylem yerine
// "Salon onaylayınca SMS ile bildireceğiz"; durum rozeti; tarih/saat/süre.
void main() {
  RandevuKarti kart({List<Widget> eylemler = const [], String? not}) =>
      RandevuKarti(
        salonAdi: 'Nail Studio Moda',
        hizmetVePersonel: 'Manikür · Gizem',
        durum: RandevuDurumu.onayBekliyor,
        tarih: formatDate('2026-12-03', DateStyle.compact),
        saat: '11:00',
        sure: formatDuration(40),
        tutar: '${formatMoney(35000)} salonda',
        not: not,
        eylemler: eylemler,
      );

  testWidgets('bilgiler ve baş harfler', (tester) async {
    await tester.pumpWidget(
      omvilaApp(kart(not: 'Salon onaylayınca SMS ile bildireceğiz')),
    );
    expect(find.text('NS'), findsOneWidget);
    expect(find.text('Onay bekliyor'), findsOneWidget);
    expect(find.text('Per 3 Ara'), findsOneWidget);
    expect(find.text('40 dk'), findsOneWidget);
    expect(find.text('350\u00a0₺ salonda'), findsOneWidget);
    expect(
      find.text('Salon onaylayınca SMS ile bildireceğiz'),
      findsOneWidget,
    );
  });

  // Test yazı tipi her harfi 1 em genişlikte çizer (gerçekten geniş); yatayda
  // taşma olmamalı, satırlar kırılır. Dikeyde ekran kaydırılır.
  testWidgets('1.3× yazıda yatay taşma yok', (tester) async {
    await tester.pumpWidget(
      omvilaApp(
        SingleChildScrollView(
          child: SizedBox(
            width: 358,
            child: kart(not: 'Salon onaylayınca SMS ile bildireceğiz'),
          ),
        ),
        textScale: 1.3,
      ),
    );
    expect(tester.takeException(), isNull);
  });

  omvilaGoldenTest(
    'RandevuKarti',
    fileName: 'randevu_karti',
    builder: (_) => Column(
      spacing: OmvilaBosluk.bosluk4,
      children: [
        kart(not: 'Salon onaylayınca SMS ile bildireceğiz'),
        RandevuKarti(
          salonAdi: 'Kuaför Elif',
          hizmetVePersonel: 'Saç kesimi, Fön · Elif Kaya',
          durum: RandevuDurumu.onaylandi,
          tarih: formatDate('2026-11-26', DateStyle.compact),
          saat: '14:30',
          sure: formatDuration(75),
          tutar: formatMoney(124000),
          eylemler: [
            OmvilaButon(
              etiket: 'İptal et',
              tur: OmvilaButonTuru.ikincil,
              boyut: OmvilaButonBoyutu.sm,
              onPressed: () {},
            ),
            OmvilaButon(
              etiket: 'Takvime ekle',
              tur: OmvilaButonTuru.ikincil,
              boyut: OmvilaButonBoyutu.sm,
              onPressed: () {},
            ),
          ],
        ),
      ],
    ),
  );
}
