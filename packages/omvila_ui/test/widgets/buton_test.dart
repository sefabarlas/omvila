import 'package:alchemist/alchemist.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_ui/omvila_ui.dart';

import '../helpers/golden.dart';

// Kural: components/Button — lg 52, md 48, sm 36; dokunma alanı ≥ 44;
// yüklenirken dokunuş yok sayılır ve "Onaylanıyor…" yazılır.
void main() {
  testWidgets('boyutlar ve dokunma alanı', (tester) async {
    await tester.pumpWidget(
      omvilaApp(
        Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            OmvilaButon(
              key: const Key('lg'),
              etiket: 'Randevu al',
              boyut: OmvilaButonBoyutu.lg,
              onPressed: () {},
            ),
            OmvilaButon(
              key: const Key('sm'),
              etiket: 'Geldi',
              boyut: OmvilaButonBoyutu.sm,
              onPressed: () {},
            ),
          ],
        ),
      ),
    );
    expect(tester.getSize(find.byType(FilledButton).first).height, 52);
    // sm görsel 36, dokunma alanı (padded) 48 ≥ 44.
    expect(
      tester.getSize(find.byKey(const Key('sm'))).height,
      greaterThanOrEqualTo(OmvilaBoyut.dokunmaMin),
    );
  });

  testWidgets('yüklenirken dokunuş yok sayılır, metin değişir', (
    tester,
  ) async {
    var taps = 0;
    await tester.pumpWidget(
      omvilaApp(
        OmvilaButon(
          etiket: 'Onayla',
          yukleniyorEtiketi: 'Onaylanıyor…',
          yukleniyor: true,
          onPressed: () => taps++,
        ),
      ),
    );
    expect(find.text('Onaylanıyor…'), findsOneWidget);
    expect(find.byType(CircularProgressIndicator), findsOneWidget);
    await tester.tap(find.byType(OmvilaButon), warnIfMissed: false);
    expect(taps, 0);
  });

  testWidgets('tehlike varyantı hata renginde', (tester) async {
    await tester.pumpWidget(
      omvilaApp(
        OmvilaButon(
          etiket: 'Hesabı sil',
          tur: OmvilaButonTuru.tehlike,
          onPressed: () {},
        ),
      ),
    );
    final material = tester.widget<Material>(
      find.descendant(
        of: find.byType(FilledButton),
        matching: find.byType(Material),
      ),
    );
    expect(material.color, OmvilaRenkler.acik.hata);
  });

  // Tasarım: secondary dolu nötr zemin (`zemin-ikincil`), çerçevesiz; koyu
  // hap `metin` zemininde `zemin` rengi yazı.
  testWidgets('ikincil dolu ve çerçevesiz, koyu hap metin renginde', (
    tester,
  ) async {
    await tester.pumpWidget(
      omvilaApp(
        const Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            OmvilaButon(
              key: Key('ikincil'),
              etiket: 'Başka saat seç',
              tur: OmvilaButonTuru.ikincil,
              onPressed: _noop,
            ),
            OmvilaButon(
              key: Key('koyu'),
              etiket: 'Randevu',
              tur: OmvilaButonTuru.koyu,
              onPressed: _noop,
            ),
          ],
        ),
      ),
    );
    Material materialOf(String key) => tester.widget<Material>(
      find
          .descendant(
            of: find.byKey(Key(key)),
            matching: find.byType(Material),
          )
          .first,
    );
    final ikincil = materialOf('ikincil');
    expect(ikincil.color, OmvilaRenkler.acik.zeminIkincil);
    expect(ikincil.shape, isA<RoundedRectangleBorder>());
    expect((ikincil.shape! as RoundedRectangleBorder).side, BorderSide.none);
    expect(ikincil.textStyle?.color, OmvilaRenkler.acik.metin);
    expect(materialOf('koyu').color, OmvilaRenkler.acik.metin);
    expect(materialOf('koyu').textStyle?.color, OmvilaRenkler.acik.zemin);
  });

  omvilaGoldenTest(
    'OmvilaButon',
    fileName: 'buton',
    // Yükleme göstergesi sürekli döner; tek kare çizilir.
    pumpBeforeTest: pumpOnce,
    builder: (_) => Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      spacing: OmvilaBosluk.bosluk3,
      children: [
        OmvilaButon(
          etiket: 'Randevu al',
          boyut: OmvilaButonBoyutu.lg,
          tamGenislik: true,
          onPressed: () {},
        ),
        OmvilaButon(
          etiket: 'Başka saat seç',
          tur: OmvilaButonTuru.ikincil,
          onPressed: () {},
        ),
        OmvilaButon(
          etiket: 'Randevu',
          tur: OmvilaButonTuru.koyu,
          ikon: Icons.add,
          boyut: OmvilaButonBoyutu.sm,
          onPressed: () {},
        ),
        OmvilaButon(
          etiket: 'Daha sonra',
          tur: OmvilaButonTuru.hayalet,
          onPressed: () {},
        ),
        OmvilaButon(
          etiket: 'Randevuyu iptal et',
          tur: OmvilaButonTuru.tehlike,
          onPressed: () {},
        ),
        OmvilaButon(
          etiket: 'Onayla',
          yukleniyorEtiketi: 'Onaylanıyor…',
          yukleniyor: true,
          onPressed: () {},
        ),
        const Row(
          spacing: OmvilaBosluk.bosluk2,
          children: [
            OmvilaButon(
              etiket: 'Geldi',
              boyut: OmvilaButonBoyutu.sm,
              ikon: Icons.check,
              onPressed: null,
            ),
          ],
        ),
      ],
    ),
  );
}

void _noop() {}
