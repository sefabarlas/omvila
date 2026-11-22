import 'package:flutter/material.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:omvila_musteri/app/router.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Türkçe yerel ayar (dil yalnız tr).
const trLocale = Locale('tr', 'TR');

/// Uygulama kökü: tema sistem ayarını izler, dil Türkçe.
class OmvilaMusteriApp extends ConsumerWidget {
  /// Uygulama.
  const OmvilaMusteriApp({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) => MaterialApp.router(
    title: 'OMVILA',
    debugShowCheckedModeBanner: false,
    theme: OmvilaTema.acik(),
    darkTheme: OmvilaTema.koyu(),
    locale: trLocale,
    supportedLocales: const [trLocale],
    localizationsDelegates: GlobalMaterialLocalizations.delegates,
    routerConfig: ref.watch(routerProvider),
  );
}
