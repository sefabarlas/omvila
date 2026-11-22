import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_ui/omvila_ui.dart';

import '../helpers/golden.dart';

// Kural: components/Avatar — Türkçe baş harf, ton addan türetilir (anlam
// taşımaz); web Avatar `tone` karmasıyla aynı ton, kişiden kişiye değişir.
void main() {
  test('ton addan türetilir ve web karmasıyla aynıdır', () {
    // packages/ui-web Avatar.tsx: hash = (hash * 31 + kod) % 997, % 3.
    int webTone(String name) {
      var hash = 0;
      for (final c in name.runes) {
        hash = (hash * 31 + c) % 997;
      }
      return hash % 3;
    }

    for (final name in ['Elif Kaya', 'Mert Demir', 'Can Yıldız', 'Zeynep']) {
      expect(OmvilaAvatarTonu.adiIcin(name).index, webTone(name));
      expect(OmvilaAvatarTonu.adiIcin(name), OmvilaAvatarTonu.adiIcin(name));
    }
    final tones = {
      for (final n in ['Elif Kaya', 'Mert Demir', 'Can Yıldız', 'Zeynep Ak'])
        OmvilaAvatarTonu.adiIcin(n),
    };
    expect(tones.length, greaterThan(1));
  });

  testWidgets('baş harfler Türkçe büyük harf, tonun renkleri', (tester) async {
    await tester.pumpWidget(
      omvilaApp(
        const OmvilaAvatar(ad: 'ilker şahin', ton: OmvilaAvatarTonu.uc),
      ),
    );
    final text = tester.widget<Text>(find.text('İŞ'));
    expect(text.style?.color, OmvilaRenkler.acik.avatar3Metin);
    final box = tester.widget<Container>(find.byType(Container));
    expect(
      (box.decoration! as BoxDecoration).color,
      OmvilaRenkler.acik.avatar3Zemin,
    );
  });

  omvilaGoldenTest(
    'OmvilaAvatar',
    fileName: 'avatar',
    builder: (_) => const Wrap(
      spacing: OmvilaBosluk.bosluk3,
      runSpacing: OmvilaBosluk.bosluk3,
      children: [
        OmvilaAvatar(ad: 'Elif Kaya', ton: OmvilaAvatarTonu.bir, boyut: 32),
        OmvilaAvatar(ad: 'Mert Demir', ton: OmvilaAvatarTonu.iki),
        OmvilaAvatar(ad: 'Can Yıldız', ton: OmvilaAvatarTonu.uc, boyut: 56),
        OmvilaAvatar(ad: 'Fark etmez', ikon: Icons.groups),
      ],
    ),
  );
}
