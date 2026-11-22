import 'package:alchemist/alchemist.dart';
import 'package:flutter/material.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_riverpod/misc.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:go_router/go_router.dart';
import 'package:omvila_musteri/app/app.dart';
import 'package:omvila_musteri/app/config.dart';
import 'package:omvila_musteri/app/providers.dart';
import 'package:omvila_ui/omvila_ui.dart';

import 'fakes.dart';
import 'fixtures.dart';

/// Ekranın sahte bağımlılıkları.
class TestDeps {
  TestDeps({
    Map<String, String>? store,
    FakeAuthRepository? auth,
    FakeAppointmentsRepository? appointments,
    FakeBrandsRepository? brands,
    FakePush? push,
    Clock? clock,
    this.config = const AppConfig(apiUrl: 'http://test'),
  }) : store = MemoryKeyValueStore(store),
       appointments =
           appointments ?? FakeAppointmentsRepository(home: doluHome),
       brands = brands ?? FakeBrandsRepository(),
       push = push ?? FakePush(),
       clock = clock ?? testClock {
    this.auth = auth ?? FakeAuthRepository(store: this.store);
  }

  final MemoryKeyValueStore store;
  late final FakeAuthRepository auth;
  final FakeAppointmentsRepository appointments;
  final FakeBrandsRepository brands;
  final FakePush push;
  final AppConfig config;
  final Clock clock;
  final device = FakeDeviceRepository();
  final sms = FakeSmsCodeRetriever();
  final external = FakeExternalActions();

  List<Override> get overrides => [
    configProvider.overrideWithValue(config),
    clockProvider.overrideWithValue(clock),
    keyValueStoreProvider.overrideWithValue(store),
    authRepositoryProvider.overrideWithValue(auth),
    appointmentsRepositoryProvider.overrideWithValue(appointments),
    brandsRepositoryProvider.overrideWithValue(brands),
    deviceRepositoryProvider.overrideWithValue(device),
    pushServiceProvider.overrideWithValue(push),
    smsCodeRetrieverProvider.overrideWithValue(sms),
    externalActionsProvider.overrideWithValue(external),
  ];

  ProviderContainer container() =>
      ProviderContainer(overrides: overrides, retry: (_, _) => null);
}

Widget _textScale(BuildContext context, Widget? child, double scale) =>
    MediaQuery(
      data: MediaQuery.of(
        context,
      ).copyWith(textScaler: TextScaler.linear(scale)),
      child: child!,
    );

/// Tek ekran; [routes] verilirse ekran '/' adresinde, diğer adresler
/// yönlendirme testleri için.
Widget screenApp(
  Widget screen,
  TestDeps deps, {
  ThemeData? theme,
  double textScale = 1,
  List<RouteBase> routes = const [],
  List<Override> extraOverrides = const [],
}) {
  final router = GoRouter(
    routes: [
      GoRoute(path: '/', builder: (_, _) => screen),
      ...routes,
    ],
  );
  return screenAppWithRouter(
    router,
    deps,
    theme: theme,
    textScale: textScale,
    extraOverrides: extraOverrides,
  );
}

/// Verilen router'la uygulama.
Widget screenAppWithRouter(
  GoRouter router,
  TestDeps deps, {
  ThemeData? theme,
  double textScale = 1,
  List<Override> extraOverrides = const [],
}) => ProviderScope(
  overrides: [...deps.overrides, ...extraOverrides],
  retry: (_, _) => null,
  child: MaterialApp.router(
    debugShowCheckedModeBanner: false,
    theme: theme ?? OmvilaTema.acik(),
    locale: trLocale,
    supportedLocales: const [trLocale],
    localizationsDelegates: GlobalMaterialLocalizations.delegates,
    routerConfig: router,
    builder: (context, child) => _textScale(context, child, textScale),
  ),
);

/// Adres testi için hedef ekran.
GoRoute markerRoute(String path) => GoRoute(
  path: path,
  builder: (_, state) => Scaffold(body: Text('adres:${state.uri}')),
);

/// Gerçek router'la uygulama.
Widget routedApp(ProviderContainer container) => UncontrolledProviderScope(
  container: container,
  child: const OmvilaMusteriApp(),
);

/// Ekran boyutunda golden: açık, koyu, açık 1.3× (KK11).
Future<void> screenGoldenTest(
  String description, {
  required String fileName,
  required Widget Function() screen,
  required TestDeps Function() deps,
  PumpAction pumpBeforeTest = onlyPumpAndSettle,
}) {
  Widget scenario(ThemeData theme, double scale) => SizedBox(
    width: OmvilaBoyut.ekranGenislik,
    height: 844,
    child: screenApp(screen(), deps(), theme: theme, textScale: scale),
  );

  return goldenTest(
    description,
    fileName: fileName,
    pumpBeforeTest: pumpBeforeTest,
    builder: () => GoldenTestGroup(
      columns: 3,
      children: [
        GoldenTestScenario(name: 'acik', child: scenario(OmvilaTema.acik(), 1)),
        GoldenTestScenario(name: 'koyu', child: scenario(OmvilaTema.koyu(), 1)),
        GoldenTestScenario(
          name: 'acik 1.3x',
          child: scenario(OmvilaTema.acik(), 1.3),
        ),
      ],
    ),
  );
}

/// Bütün zamanlayıcılar ve mikro görevler bitene kadar.
Future<void> settle(WidgetTester tester) async {
  await tester.pump();
  await tester.pumpAndSettle();
}

/// Telefon ekranı boyutu (390×844).
void usePhone(WidgetTester tester) {
  tester.view
    ..physicalSize = const Size(390, 844)
    ..devicePixelRatio = 1;
  addTearDown(tester.view.reset);
}
