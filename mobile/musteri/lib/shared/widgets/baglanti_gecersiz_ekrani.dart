import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:omvila_musteri/shared/widgets/durum_desenleri.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Tanımsız adres veya bulunamayan salon bağlantısı.
class BaglantiGecersizEkrani extends StatelessWidget {
  /// Ekran.
  const BaglantiGecersizEkrani({super.key});

  @override
  Widget build(BuildContext context) => Scaffold(
    backgroundColor: context.omvilaRenkler.zemin,
    body: SafeArea(
      child: Center(
        child: SingleChildScrollView(
          padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
          child: BaglantiGecersizKarti(onHome: () => context.go('/')),
        ),
      ),
    ),
  );
}
