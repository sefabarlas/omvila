import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_ui/omvila_ui.dart';

import '../helpers/golden.dart';

// Kural: components/TabBar ve mobil README § Yerleşim — seçili sekme adıyla,
// diğerleri yalnız ikonla; rozet yalnız okunmamış sayı > 0 iken.
const _isletme = [
  SekmeOgesi(ikon: Icons.home_outlined, etiket: 'Bugün'),
  SekmeOgesi(ikon: Icons.calendar_today_outlined, etiket: 'Takvim'),
  SekmeOgesi(ikon: Icons.people_outline, etiket: 'Müşteriler'),
  SekmeOgesi(
    ikon: Icons.notifications_none,
    etiket: 'Bildirimler',
    rozet: 3,
  ),
  SekmeOgesi(ikon: Icons.person_outline, etiket: 'Profil'),
];

const _musteri = [
  SekmeOgesi(ikon: Icons.calendar_today_outlined, etiket: 'Randevularım'),
  SekmeOgesi(ikon: Icons.inventory_2_outlined, etiket: 'Paketlerim'),
  SekmeOgesi(ikon: Icons.person_outline, etiket: 'Profil', rozet: 0),
];

void main() {
  testWidgets('seçili sekme adı görünür, dokunma seçimi bildirir', (
    tester,
  ) async {
    final selected = <int>[];
    await tester.pumpWidget(
      omvilaApp(
        YuzenSekmeCubugu(
          ogeler: _isletme,
          seciliIndex: 0,
          onSecildi: selected.add,
        ),
      ),
    );
    expect(find.text('Bugün'), findsOneWidget);
    expect(find.text('Takvim'), findsNothing);
    await tester.tap(find.byIcon(Icons.calendar_today_outlined));
    expect(selected, [1]);
  });

  testWidgets('ekran okuyucu her sekmenin adını ve rozeti okur', (
    tester,
  ) async {
    await tester.pumpWidget(
      omvilaApp(
        YuzenSekmeCubugu(ogeler: _isletme, seciliIndex: 0, onSecildi: (_) {}),
      ),
    );
    expect(find.bySemanticsLabel('Takvim'), findsOneWidget);
    expect(find.bySemanticsLabel('Bildirimler, 3 okunmamış'), findsOneWidget);
    expect(find.text('3'), findsOneWidget);
  });

  testWidgets('sıfır rozet çizilmez', (tester) async {
    await tester.pumpWidget(
      omvilaApp(
        YuzenSekmeCubugu(ogeler: _musteri, seciliIndex: 0, onSecildi: (_) {}),
      ),
    );
    expect(find.byType(Badge), findsNothing);
  });

  omvilaGoldenTest(
    'YuzenSekmeCubugu',
    fileName: 'yuzen_sekme_cubugu',
    builder: (_) => Column(
      spacing: OmvilaBosluk.bosluk2,
      children: [
        YuzenSekmeCubugu(ogeler: _isletme, seciliIndex: 0, onSecildi: (_) {}),
        YuzenSekmeCubugu(ogeler: _isletme, seciliIndex: 3, onSecildi: (_) {}),
        YuzenSekmeCubugu(ogeler: _musteri, seciliIndex: 0, onSecildi: (_) {}),
      ],
    ),
  );
}
