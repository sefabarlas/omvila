import 'package:alchemist/alchemist.dart';
import 'package:flutter/material.dart';
import 'package:omvila_ui/theme/omvila_theme.dart';

/// Tasarım referans genişliği (390).
const double goldenWidth = OmvilaBoyut.ekranGenislik;

/// Widget'ı açık, koyu ve açık 1.3× yazı ölçeğinde tek golden dosyasında
/// çizer (packages/omvila_ui/AGENTS.md).
Future<void> omvilaGoldenTest(
  String description, {
  required String fileName,
  required WidgetBuilder builder,
  double width = goldenWidth,
  PumpAction pumpBeforeTest = onlyPumpAndSettle,
}) {
  Widget scenario(ThemeData theme, double textScale) => Theme(
    data: theme,
    child: Builder(
      builder: (context) => MediaQuery(
        data: MediaQuery.of(
          context,
        ).copyWith(textScaler: TextScaler.linear(textScale)),
        child: Material(
          color: context.omvilaRenkler.zemin,
          child: SizedBox(
            width: width,
            child: Padding(
              padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
              child: Builder(builder: builder),
            ),
          ),
        ),
      ),
    ),
  );

  return goldenTest(
    description,
    fileName: fileName,
    pumpBeforeTest: pumpBeforeTest,
    builder: () => GoldenTestGroup(
      columns: 3,
      children: [
        GoldenTestScenario(
          name: 'acik',
          child: scenario(OmvilaTema.acik(), 1),
        ),
        GoldenTestScenario(
          name: 'koyu',
          child: scenario(OmvilaTema.koyu(), 1),
        ),
        GoldenTestScenario(
          name: 'acik 1.3x',
          child: scenario(OmvilaTema.acik(), 1.3),
        ),
      ],
    ),
  );
}

/// Widget testleri için tema sarmalayıcı.
Widget omvilaApp(
  Widget child, {
  ThemeData? theme,
  double textScale = 1,
}) => MaterialApp(
  theme: theme ?? OmvilaTema.acik(),
  home: Builder(
    builder: (context) => MediaQuery(
      data: MediaQuery.of(
        context,
      ).copyWith(textScaler: TextScaler.linear(textScale)),
      child: Scaffold(body: Center(child: child)),
    ),
  ),
);
