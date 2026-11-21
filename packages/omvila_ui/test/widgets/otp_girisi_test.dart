import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_ui/omvila_ui.dart';

import '../helpers/golden.dart';

// Kural: components/OtpInput — 6 hane, SMS'ten otomatik doldurma
// (oneTimeCode), kod tamamlanınca kendiliğinden doğrulanır.
void main() {
  testWidgets('yalnız rakam, altı hanede tamamlanır', (tester) async {
    final completed = <String>[];
    await tester.pumpWidget(
      omvilaApp(
        SizedBox(
          width: 358,
          child: OtpGirisi(onTamamlandi: completed.add),
        ),
      ),
    );
    await tester.enterText(find.byType(TextField), '48a21');
    await tester.pump();
    expect(find.text('4'), findsOneWidget);
    expect(find.text('1'), findsOneWidget);
    expect(completed, isEmpty);

    await tester.enterText(find.byType(TextField), '4821073');
    await tester.pump();
    expect(completed, ['482107']);
  });

  testWidgets('otomatik doldurma ipucu ve ekran okuyucu etiketi', (
    tester,
  ) async {
    await tester.pumpWidget(
      omvilaApp(const SizedBox(width: 358, child: OtpGirisi())),
    );
    final field = tester.widget<TextField>(find.byType(TextField));
    expect(field.autofillHints, [AutofillHints.oneTimeCode]);
    expect(field.keyboardType, TextInputType.number);
    expect(find.bySemanticsLabel('Doğrulama kodu'), findsOneWidget);
  });

  omvilaGoldenTest(
    'OtpGirisi',
    fileName: 'otp_girisi',
    builder: (_) => Column(
      spacing: OmvilaBosluk.bosluk4,
      children: [
        OtpGirisi(controller: TextEditingController(text: '4821')),
        OtpGirisi(
          controller: TextEditingController(text: '482107'),
          hatali: true,
        ),
      ],
    ),
  );
}
