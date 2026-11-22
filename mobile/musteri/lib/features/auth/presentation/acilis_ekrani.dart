import 'package:flutter/material.dart';
import 'package:omvila_musteri/shared/widgets/omvila_logo.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Açılış: cihazdaki oturum okunurken (Acilis.png). Petrol zemin iki temada
/// da aynıdır.
class AcilisEkrani extends StatelessWidget {
  /// Ekran.
  const AcilisEkrani({super.key});

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    return Scaffold(
      backgroundColor: r.blokMarka,
      body: Stack(
        children: [
          Positioned(
            top: 120,
            right: -60,
            child: DekorKare(
              renk: r.metinTers.withValues(alpha: 0.08),
              boyut: 220,
              aci: 0.26,
            ),
          ),
          Positioned(
            bottom: 160,
            left: -70,
            child: DekorKare(renk: r.vurgu, boyut: 170, aci: -0.2),
          ),
          Center(
            child: OmvilaLogo(
              renk: r.metinTers,
              amblem: 56,
              dikey: true,
              ters: true,
            ),
          ),
          Positioned(
            left: OmvilaBosluk.bosluk4,
            right: OmvilaBosluk.bosluk4,
            bottom:
                MediaQuery.paddingOf(context).bottom + OmvilaBosluk.bosluk12,
            child: Text(
              'Randevularım bir arada',
              textAlign: TextAlign.center,
              style: OmvilaYazi.ikincil.copyWith(
                color: r.metinTers.withValues(alpha: 0.8),
              ),
            ),
          ),
        ],
      ),
    );
  }
}
