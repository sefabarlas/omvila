import 'package:flutter/material.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Yükleme göstergesi (ekran ortası).
class YukleniyorDurumu extends StatelessWidget {
  /// Yükleniyor.
  const YukleniyorDurumu({super.key});

  @override
  Widget build(BuildContext context) => Center(
    child: Semantics(
      label: 'Yükleniyor',
      child: CircularProgressIndicator(color: context.omvilaRenkler.marka),
    ),
  );
}

/// Okuma hatası: bağlantı yoksa "Bağlantı yok", değilse hata metni; "Tekrar
/// dene" (Durumlar.png § Bağlantı yok).
class HataDurumu extends StatelessWidget {
  /// [error] genelde [ApiProblem].
  const HataDurumu({required this.error, required this.onRetry, super.key});

  /// Hata.
  final Object error;

  /// Yeniden yükler.
  final VoidCallback onRetry;

  @override
  Widget build(BuildContext context) {
    final problem = error is ApiProblem ? error as ApiProblem : null;
    final offline = problem?.isNetworkError ?? false;
    return Center(
      child: SingleChildScrollView(
        child: OmvilaBosDurum(
          ikon: offline ? LucideIcons.wifiOff : LucideIcons.alertCircle,
          baslik: offline ? 'Bağlantı yok' : 'Sayfa yüklenemedi',
          aciklama: offline
              ? 'İnternet bağlantınızı kontrol edip tekrar deneyin.'
              : (problem?.message ?? unknownErrorMessage),
          eylemEtiketi: 'Tekrar dene',
          eylemIkonu: LucideIcons.refreshCw,
          onEylem: onRetry,
        ),
      ),
    );
  }
}

/// "Bu bağlantı geçersiz" kartı (Durumlar.png § Bağlantı geçersiz): tanımsız
/// adres ve bulunamayan salon bağlantısı.
class BaglantiGecersizKarti extends StatelessWidget {
  /// Kart.
  const BaglantiGecersizKarti({required this.onHome, super.key});

  /// "Randevularıma git".
  final VoidCallback onHome;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    return DecoratedBox(
      decoration: BoxDecoration(
        color: r.zeminYukseltilmis,
        borderRadius: BorderRadius.circular(OmvilaKose.koseXl),
        boxShadow: context.omvilaGolgeler.golgeSm,
      ),
      child: OmvilaBosDurum(
        ikon: LucideIcons.link,
        baslik: 'Bu bağlantı geçersiz',
        aciklama:
            'Bağlantı yanlış yazılmış ya da salon bağlantısını değiştirmiş '
            'olabilir. Güncel bağlantıyı salondan isteyebilirsiniz.',
        eylemEtiketi: 'Randevularıma git',
        onEylem: onHome,
      ),
    );
  }
}
