import 'package:flutter/material.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
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

/// Durum kutusunun tonu (Durumlar.png): hata kırmızı, uyarı sarı.
enum DurumTonu {
  /// "Çok fazla deneme yaptınız."
  hata,

  /// "… şu an online randevu almıyor."
  uyari,
}

/// Renkli durum kutusu: ikon, kalın başlık, açıklama, isteğe bağlı eylem
/// (Durumlar.png § SMS kodu deneme sınırı, Online randevu kapalı).
class DurumKutusu extends StatelessWidget {
  /// Kutu.
  const DurumKutusu({
    required this.ikon,
    required this.baslik,
    required this.aciklama,
    this.ton = DurumTonu.hata,
    this.eylem,
    super.key,
  });

  /// İkon.
  final IconData ikon;

  /// Kalın ilk satır.
  final String baslik;

  /// Açıklama.
  final String aciklama;

  /// Ton.
  final DurumTonu ton;

  /// Alt eylem ("Salonu ara").
  final Widget? eylem;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final (zemin, renk) = switch (ton) {
      DurumTonu.hata => (r.hataZemin, r.hata),
      DurumTonu.uyari => (r.uyariZemin, r.uyari),
    };
    final action = eylem;
    return Semantics(
      liveRegion: true,
      child: DecoratedBox(
        decoration: BoxDecoration(
          color: zemin,
          borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
        ),
        child: Padding(
          padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Icon(ikon, size: 20, color: renk),
              const SizedBox(width: OmvilaBosluk.bosluk3),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      baslik,
                      style: OmvilaYazi.govdeGuclu.copyWith(color: renk),
                    ),
                    Text(
                      aciklama,
                      style: OmvilaYazi.ikincil.copyWith(color: renk),
                    ),
                    if (action != null) ...[
                      const SizedBox(height: OmvilaBosluk.bosluk3),
                      action,
                    ],
                  ],
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

/// SMS kodu deneme sınırı mı (`otp_attempts_exceeded`).
bool isOtpLimit(Object? error) =>
    error is ApiProblem && error.code == ErrorCode.otpAttemptsExceeded.value;

/// "Çok fazla deneme yaptınız." (Durumlar.png): bekleme süresi
/// `meta.retryAfterSeconds`'tan; yoksa süre yazılmaz.
class OtpSiniriKutusu extends StatelessWidget {
  /// Kutu.
  const OtpSiniriKutusu({required this.problem, super.key});

  /// `otp_attempts_exceeded` yanıtı.
  final ApiProblem problem;

  @override
  Widget build(BuildContext context) {
    final seconds = problem.meta['retryAfterSeconds'];
    final wait = seconds is int && seconds > 0
        ? '${(seconds / 60).ceil()} dakika sonra'
        : 'biraz bekledikten sonra';
    return DurumKutusu(
      ikon: LucideIcons.clockAlert,
      baslik: 'Çok fazla deneme yaptınız.',
      aciklama: 'Güvenliğiniz için $wait yeni kod isteyebilirsiniz.',
    );
  }
}

/// Online randevu kapalı mı (`booking_disabled`).
bool isBookingDisabled(Object? error) =>
    error is ApiProblem && error.code == ErrorCode.bookingDisabled.value;

/// "Kuaför Elif şu an online randevu almıyor." (Durumlar.png): şube telefonu
/// biliniyorsa "Salonu ara".
class OnlineRandevuKapaliKutusu extends StatelessWidget {
  /// Kutu.
  const OnlineRandevuKapaliKutusu({
    required this.salonAdi,
    this.onAra,
    super.key,
  });

  /// "Kuaför Elif".
  final String salonAdi;

  /// "Salonu ara"; telefon yoksa null.
  final VoidCallback? onAra;

  @override
  Widget build(BuildContext context) {
    final call = onAra;
    final r = context.omvilaRenkler;
    return DurumKutusu(
      ikon: LucideIcons.calendarOff,
      ton: DurumTonu.uyari,
      baslik: '$salonAdi şu an online randevu almıyor.',
      aciklama: 'Randevu için salonu arayabilirsiniz.',
      eylem: call == null
          ? null
          : FilledButton.icon(
              onPressed: call,
              icon: const Icon(LucideIcons.phone, size: 18),
              label: const Text('Salonu ara'),
              style: FilledButton.styleFrom(
                backgroundColor: r.zeminYukseltilmis,
                foregroundColor: r.metin,
                textStyle: OmvilaYazi.etiketKucuk,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(OmvilaKose.koseSm),
                ),
              ),
            ),
    );
  }
}
