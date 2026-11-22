import 'package:flutter_test/flutter_test.dart';

import 'fake_api.dart';
import 'fixtures.dart';

/// Uygulamayı açar (Bugün), [setup] ile uçları kurar ve [tab] sekmesine
/// geçer. [settle] false ise yükleniyor hâli görülebilsin diye beklemez.
Future<Harness> openTab(
  WidgetTester tester,
  String tab, {
  Map<String, Object?>? session,
  void Function(FakeApi api)? setup,
  bool settle = true,
}) async {
  final h = Harness()..signIn(session: session);
  h.api.reply('GET', '/api/v1/manage/today', todayJson());
  setup?.call(h.api);
  await h.pumpApp(tester);
  await tester.tap(find.bySemanticsLabel(tab));
  if (settle) {
    await tester.pumpAndSettle();
  } else {
    await tester.pump();
    await tester.pump();
  }
  return h;
}
