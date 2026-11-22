import 'package:alchemist/alchemist.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_riverpod/misc.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/features/auth/data/auth_controller.dart';
import 'package:omvila_isletme/features/push/data/push_prompt_controller.dart';
import 'package:omvila_isletme/features/push/data/push_service.dart';
import 'package:omvila_isletme/main.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Ekranı tasarım boyutunda (390×844) açık, koyu ve açık 1.3× yazıda tek
/// golden dosyasında çizer (dilim-05 KK11, test-stratejisi.md).
Future<void> screenGoldenTest(
  String description, {
  required String fileName,
  required WidgetBuilder builder,
  List<Override> overrides = const [],
}) {
  Widget scenario(ThemeData theme, double textScale) => ProviderScope(
    overrides: overrides,
    retry: noRetry,
    child: Theme(
      data: theme,
      child: Builder(
        builder: (context) => MediaQuery(
          data: MediaQuery.of(context).copyWith(
            size: const Size(390, 844),
            textScaler: TextScaler.linear(textScale),
          ),
          child: SizedBox(
            width: 390,
            height: 844,
            child: ColoredBox(
              color: context.omvilaRenkler.zemin,
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

/// Sabit oturum.
class FixedAuth extends AuthController {
  /// [session] null ise giriş yapılmamış.
  FixedAuth(this.session);

  /// Oturum.
  final ManageSession? session;

  @override
  Future<ManageSession?> build() async => session;
}

/// Sabit bildirim izni tercihi.
class FixedPushPrompt extends PushPromptController {
  /// Tercih.
  FixedPushPrompt({this.permission = PushPermission.granted});

  /// İzin.
  final PushPermission permission;

  @override
  Future<PushPromptState> build() async => PushPromptState(
    asked: true,
    bannerDismissed: false,
    permission: permission,
  );
}
