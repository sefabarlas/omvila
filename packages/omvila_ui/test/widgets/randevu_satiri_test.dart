import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_ui/omvila_ui.dart';

import '../helpers/golden.dart';

// Kural: mobil components/AppointmentListItem — saat, durum noktası, müşteri,
// hizmet; durum adı rozetle yazılır (yalnız renk değil); eylem sağda.
void main() {
  testWidgets('saat, müşteri, ayrıntı ve durum adı yazılır', (tester) async {
    var taps = 0;
    await tester.pumpWidget(
      omvilaApp(
        RandevuSatiri(
          baslangic: '12:00',
          bitis: '12:45',
          musteri: 'Deniz Acar',
          ayrinti: 'Saç kesimi · Can',
          durum: RandevuDurumu.onayBekliyor,
          onTap: () => taps++,
        ),
      ),
    );
    expect(find.text('12:00'), findsOneWidget);
    expect(find.text('12:45'), findsOneWidget);
    expect(find.text('Deniz Acar'), findsOneWidget);
    expect(find.text('Saç kesimi · Can'), findsOneWidget);
    expect(find.text('Onay bekliyor'), findsOneWidget);
    await tester.tap(find.text('Deniz Acar'));
    expect(taps, 1);
  });

  testWidgets('bilinmeyen durumda rozet yok', (tester) async {
    await tester.pumpWidget(
      omvilaApp(
        const RandevuSatiri(
          baslangic: '10:30',
          bitis: '12:00',
          musteri: 'Selin Öztürk',
          ayrinti: 'Boya ve fön · Elif',
        ),
      ),
    );
    expect(find.byType(OmvilaDurumRozeti), findsNothing);
  });

  test('nokta rengi durum tokenından', () {
    expect(
      RandevuDurumu.geldi.nokta(OmvilaRenkler.acik),
      OmvilaRenkler.acik.durumGeldiNokta,
    );
  });

  omvilaGoldenTest(
    'RandevuSatiri',
    fileName: 'randevu_satiri',
    builder: (context) => Column(
      spacing: OmvilaBosluk.bosluk2,
      children: [
        RandevuSatiri(
          baslangic: '12:00',
          bitis: '12:45',
          musteri: 'Deniz Acar',
          ayrinti: 'Saç kesimi · Can',
          durum: RandevuDurumu.onayBekliyor,
          eylem: OmvilaButon(
            etiket: 'Onayla',
            ikon: Icons.check,
            boyut: OmvilaButonBoyutu.sm,
            onPressed: () {},
          ),
        ),
        const RandevuSatiri(
          baslangic: '10:30',
          bitis: '12:00',
          musteri: 'Selin Öztürk',
          ayrinti: 'Boya ve fön · Elif',
          durum: RandevuDurumu.geldi,
        ),
      ],
    ),
  );
}
