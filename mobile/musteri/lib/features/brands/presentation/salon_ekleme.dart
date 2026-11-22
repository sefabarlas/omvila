import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/providers.dart';
import 'package:omvila_musteri/app/router.dart';
import 'package:omvila_musteri/features/appointments/presentation/home_provider.dart';
import 'package:omvila_musteri/features/brands/data/brands_repository.dart';
import 'package:omvila_musteri/features/brands/domain/brand_link.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Bilinmeyen salon bağlantısı (meSaveBrand 404).
const salonBulunamadiMetni = 'Bu bağlantıda salon bulunamadı.';

/// "Bağlantıyı yapıştır" ve "QR kodu okut" (MainBos, KK1): bağlantı adını
/// çıkarır, salonu ekler (müşteri kaydı açılmaz), ana ekranı yeniler.
/// Sonucu alt bilgi şeridiyle bildirir.
abstract final class SalonEkleme {
  /// Panodaki bağlantıyla.
  static Future<void> panodan(BuildContext context, WidgetRef ref) async {
    final data = await Clipboard.getData(Clipboard.kTextPlain);
    if (!context.mounted) {
      return;
    }

    final slug = brandSlugFromText(
      data?.text ?? '',
      ref.read(configProvider).linkHosts,
    );
    if (slug == null) {
      _bildir(
        context,
        'Kopyaladığınız metin bir salon bağlantısı değil. Salonun '
        'bağlantısını kopyalayıp tekrar deneyin.',
      );
      return;
    }

    await _kaydet(context, ref, slug, BrandSource.link);
  }

  /// QR okutma ekranıyla.
  static Future<void> qrIle(BuildContext context, WidgetRef ref) async {
    final slug = await context.push<String>(Routes.qr);
    if (slug == null || !context.mounted) {
      return;
    }

    await _kaydet(context, ref, slug, BrandSource.qr);
  }

  static Future<void> _kaydet(
    BuildContext context,
    WidgetRef ref,
    String slug,
    BrandSource source,
  ) async {
    try {
      final brand = await ref.read(brandsRepositoryProvider).save(slug, source);
      ref.invalidate(homeProvider);
      if (context.mounted) {
        _bildir(context, '${brand.name} salonlarınıza eklendi.');
      }
    } on ApiProblem catch (e) {
      if (context.mounted) {
        _bildir(
          context,
          e.code == ErrorCode.notFound.value ? salonBulunamadiMetni : e.message,
        );
      }
    }
  }

  static void _bildir(BuildContext context, String message) {
    ScaffoldMessenger.of(context)
      ..hideCurrentSnackBar()
      ..showSnackBar(SnackBar(content: Text(message)));
  }
}
