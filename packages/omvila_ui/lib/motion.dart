// Hareket (design/tasarim-sistemleri/mobil/README.md § Hareket ve geçiş).
// Süre ve eğri sabitleri üretilmiş temadaki OmvilaHareket'tedir; burada
// azaltılmış hareket kuralı: kayma ve büyüme kapanır, yalnız kısa opaklık
// geçişi kalır.
import 'package:flutter/widgets.dart';
import 'package:omvila_ui/theme/omvila_theme.dart';

/// Sistem ayarında hareket azaltma açık mı.
bool reduceMotion(BuildContext context) =>
    MediaQuery.maybeDisableAnimationsOf(context) ?? false;

/// Geçiş süresi: hareket azaltma açıksa `OmvilaHareket.sureAzaltilmis`.
Duration motionDuration(BuildContext context, Duration duration) =>
    reduceMotion(context) ? OmvilaHareket.sureAzaltilmis : duration;
