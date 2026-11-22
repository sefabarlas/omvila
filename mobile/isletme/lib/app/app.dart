import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:omvila_isletme/app/providers.dart';
import 'package:omvila_isletme/app/router.dart';
import 'package:omvila_isletme/features/push/data/push_service.dart';
import 'package:omvila_isletme/features/shell/presentation/tabs.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// OMVILA İşletme kökü: tema sistemi izler, dil Türkçe.
class IsletmeApp extends ConsumerStatefulWidget {
  /// Uygulama.
  const IsletmeApp({super.key});

  @override
  ConsumerState<IsletmeApp> createState() => _IsletmeAppState();
}

class _IsletmeAppState extends ConsumerState<IsletmeApp> {
  StreamSubscription<PushTarget>? _pushTaps;

  @override
  void initState() {
    super.initState();
    // Bildirime dokununca ilgili ekran açılır.
    _pushTaps = ref.read(pushServiceProvider).openedTargets.listen((target) {
      final router = ref.read(routerProvider);
      switch (target) {
        case AppointmentTarget(:final appointmentId):
          unawaited(router.push(Routes.appointment(appointmentId)));
      }
    });
  }

  @override
  void dispose() {
    unawaited(_pushTaps?.cancel());
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return MaterialApp.router(
      title: 'OMVILA İşletme',
      debugShowCheckedModeBanner: false,
      theme: OmvilaTema.acik(),
      darkTheme: OmvilaTema.koyu(),
      locale: const Locale('tr', 'TR'),
      supportedLocales: const [Locale('tr', 'TR')],
      localizationsDelegates: GlobalMaterialLocalizations.delegates,
      routerConfig: ref.watch(routerProvider),
    );
  }
}
