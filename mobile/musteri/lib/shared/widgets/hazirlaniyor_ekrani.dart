import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Ekranı sonraki PR'da gelen adres için geçici sayfa (sahte veri yok).
class HazirlaniyorEkrani extends StatelessWidget {
  /// Sayfa.
  const HazirlaniyorEkrani({required this.baslik, super.key});

  /// Üst bar başlığı.
  final String baslik;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    return Scaffold(
      backgroundColor: r.zemin,
      appBar: AppBar(
        backgroundColor: r.zemin,
        centerTitle: true,
        title: Text(baslik),
        leading: IconButton(
          tooltip: 'Geri',
          icon: const Icon(LucideIcons.arrowLeft),
          onPressed: () => context.canPop() ? context.pop() : context.go('/'),
        ),
      ),
      body: const Center(
        child: SingleChildScrollView(
          child: OmvilaBosDurum(
            ikon: LucideIcons.hourglass,
            baslik: 'Bu bölüm hazırlanıyor',
            aciklama: 'Bu bölüm uygulamanın bir sonraki sürümünde açılacak.',
          ),
        ),
      ),
    );
  }
}
