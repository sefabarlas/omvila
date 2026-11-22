import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:omvila_isletme/features/auth/data/auth_controller.dart';
import 'package:omvila_isletme/features/push/data/push_prompt_controller.dart';
import 'package:omvila_isletme/shared/brand_backdrop.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Açılış (Acilis): oturum okunurken. Bağlantı hatasında "Tekrar dene".
class SplashScreen extends ConsumerWidget {
  /// Açılış.
  const SplashScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final r = context.omvilaRenkler;
    final error = ref.watch(authControllerProvider).error;
    final onBrand = r.metinTers;
    return Scaffold(
      body: SizedBox.expand(
        child: BrandBackdrop(
          child: SafeArea(
            child: Padding(
              padding: const EdgeInsets.all(OmvilaBosluk.bosluk6),
              child: Column(
                children: [
                  const Spacer(),
                  const AppIcon(size: 58),
                  const SizedBox(height: OmvilaBosluk.bosluk3),
                  Text(
                    'OMVILA',
                    style: OmvilaYazi.baslik1.copyWith(
                      color: onBrand,
                      fontWeight: FontWeight.w800,
                    ),
                  ),
                  const SizedBox(height: OmvilaBosluk.bosluk2),
                  Text(
                    'İŞLETME',
                    style: OmvilaYazi.aciklama.copyWith(
                      color: onBrand.withValues(alpha: 0.8),
                      fontWeight: FontWeight.w700,
                      letterSpacing: 3,
                    ),
                  ),
                  const Spacer(),
                  if (error != null) ...[
                    Text(
                      errorMessage(
                        error is ApiProblem ? error.code : null,
                      ),
                      textAlign: TextAlign.center,
                      style: OmvilaYazi.ikincil.copyWith(color: onBrand),
                    ),
                    const SizedBox(height: OmvilaBosluk.bosluk3),
                    OmvilaButon(
                      etiket: 'Tekrar dene',
                      tur: OmvilaButonTuru.ikincil,
                      onPressed: () {
                        ref
                          ..invalidate(authControllerProvider)
                          ..invalidate(pushPromptProvider);
                      },
                    ),
                    const SizedBox(height: OmvilaBosluk.bosluk6),
                  ],
                  Text(
                    'İşim cebimde',
                    style: OmvilaYazi.ikincil.copyWith(
                      color: onBrand,
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}
