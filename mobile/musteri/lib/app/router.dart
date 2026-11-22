import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/features/appointments/presentation/randevularim_ekrani.dart';
import 'package:omvila_musteri/features/auth/presentation/acilis_ekrani.dart';
import 'package:omvila_musteri/features/auth/presentation/auth_controller.dart';
import 'package:omvila_musteri/features/auth/presentation/giris_ekrani.dart';
import 'package:omvila_musteri/features/auth/presentation/giris_kod_ekrani.dart';
import 'package:omvila_musteri/features/booking/presentation/salon_secimi_ekrani.dart';
import 'package:omvila_musteri/features/brands/domain/brand_link.dart';
import 'package:omvila_musteri/features/brands/presentation/qr_tarama_ekrani.dart';
import 'package:omvila_musteri/features/brands/presentation/salon_ekle_ekrani.dart';
import 'package:omvila_musteri/features/notifications/presentation/bildirim_izni_ekrani.dart';
import 'package:omvila_musteri/features/notifications/presentation/notification_prefs_controller.dart';
import 'package:omvila_musteri/features/shell/sekme_kabugu.dart';
import 'package:omvila_musteri/shared/widgets/baglanti_gecersiz_ekrani.dart';

/// Uygulama adresleri. Derin bağlantılar (K3) randevu.omvila.com.tr ile aynı
/// yolları kullanır: `/{brandSlug}`, `/r/{code}`, `/b/{code}`, `/d/{code}`.
abstract final class Routes {
  /// Açılış.
  static const acilis = '/acilis';

  /// Giriş (telefon).
  static const giris = '/giris';

  /// SMS kodu.
  static const girisKod = '/giris/kod';

  /// Bildirim izni.
  static const bildirimIzni = '/bildirim-izni';

  /// Randevularım (sekme).
  static const randevularim = '/';

  /// Paketlerim (sekme).
  static const paketler = '/paketler';

  /// Profil (sekme).
  static const profil = '/profil';

  /// QR okutma.
  static const qr = '/qr';

  /// Randevu alma akışının başlangıcı ("Hangi salon?").
  static const randevuAl = '/randevu-al';

  /// Girişten sonra dönülecek adres parametresi.
  static const fromParam = 'from';

  static const Set<String> _public = {giris, girisKod};
}

/// Oturum ve bildirim izni durumuna göre yönlendirme.
final routerProvider = Provider<GoRouter>((ref) {
  final refresh = ValueNotifier(0);
  ref
    ..listen(authControllerProvider, (_, _) => refresh.value++)
    ..listen(notificationPrefsProvider, (_, _) => refresh.value++)
    ..onDispose(refresh.dispose);

  final router = GoRouter(
    initialLocation: Routes.randevularim,
    refreshListenable: refresh,
    redirect: (context, state) => _redirect(ref, state),
    errorBuilder: (context, state) => const BaglantiGecersizEkrani(),
    routes: [
      GoRoute(
        path: Routes.acilis,
        builder: (context, state) => const AcilisEkrani(),
      ),
      GoRoute(
        path: Routes.giris,
        builder: (context, state) => const GirisEkrani(),
      ),
      GoRoute(
        path: Routes.girisKod,
        redirect: (context, state) =>
            state.extra is GirisKodArgs ? null : Routes.giris,
        builder: (context, state) =>
            GirisKodEkrani(args: state.extra! as GirisKodArgs),
      ),
      GoRoute(
        path: Routes.bildirimIzni,
        builder: (context, state) => const BildirimIzniEkrani(),
      ),
      GoRoute(
        path: Routes.qr,
        builder: (context, state) => const QrTaramaEkrani(),
      ),
      GoRoute(
        path: Routes.randevuAl,
        builder: (context, state) => const SalonSecimiEkrani(),
      ),
      StatefulShellRoute.indexedStack(
        builder: (context, state, shell) => SekmeKabugu.shell(shell),
        branches: [
          StatefulShellBranch(
            routes: [
              GoRoute(
                path: Routes.randevularim,
                builder: (context, state) => const RandevularimEkrani(),
              ),
            ],
          ),
          StatefulShellBranch(
            routes: [
              GoRoute(
                path: Routes.paketler,
                builder: (context, state) => const PaketlerSekmesi(),
              ),
            ],
          ),
          StatefulShellBranch(
            routes: [
              GoRoute(
                path: Routes.profil,
                builder: (context, state) => const ProfilSekmesi(),
              ),
            ],
          ),
        ],
      ),
      // Kişiye özel bağlantı ekranları (RandevuDetay, BeklemeTeklif,
      // Degerlendir) sonraki PR'larda; o zamana kadar Randevularım açılır
      // (T-106).
      for (final prefix in linkPrefixes)
        GoRoute(
          path: '/$prefix/:code',
          redirect: (context, state) => Routes.randevularim,
        ),
      GoRoute(
        path: '/:brandSlug',
        // Geçişsiz: salon eklenip hemen Randevularım'a dönülürse sekme
        // kabuğu geçiş bitmeden iki kez kurulmasın.
        pageBuilder: (context, state) {
          final slug = state.pathParameters['brandSlug']!;
          return NoTransitionPage(
            child: isBrandSlug(slug)
                ? SalonEkleEkrani(brandSlug: slug)
                : const BaglantiGecersizEkrani(),
          );
        },
      ),
    ],
  );
  ref.onDispose(router.dispose);
  return router;
});

String? _redirect(Ref ref, GoRouterState state) {
  final auth = ref.read(authControllerProvider);
  final prefs = ref.read(notificationPrefsProvider);
  final location = state.uri.toString();
  final path = state.uri.path;
  final from = state.uri.queryParameters[Routes.fromParam];

  if (auth == AuthStatus.loading ||
      (auth == AuthStatus.signedIn && !prefs.hasValue && !prefs.hasError)) {
    return path == Routes.acilis
        ? null
        : Uri(
            path: Routes.acilis,
            queryParameters: {Routes.fromParam: from ?? location},
          ).toString();
  }

  if (auth == AuthStatus.signedOut) {
    if (Routes._public.contains(path)) {
      return null;
    }

    final target = path == Routes.acilis ? from : location;
    return Uri(
      path: Routes.giris,
      queryParameters: target == null || target == Routes.randevularim
          ? null
          : {Routes.fromParam: target},
    ).toString();
  }

  // Oturum açık.
  final asked = prefs.value?.asked ?? true;
  if (!asked && path != Routes.bildirimIzni) {
    return Uri(
      path: Routes.bildirimIzni,
      queryParameters: from == null ? null : {Routes.fromParam: from},
    ).toString();
  }

  if (path == Routes.acilis ||
      Routes._public.contains(path) ||
      (asked && path == Routes.bildirimIzni)) {
    return from ?? Routes.randevularim;
  }

  return null;
}

/// Kod ekranına taşınan bilgi.
@immutable
class GirisKodArgs {
  /// Bilgi.
  const GirisKodArgs({required this.phone, required this.challenge});

  /// E.164 telefon (yeniden gönderim için).
  final String phone;

  /// Sunucunun kod yanıtı.
  final OtpChallengeResponse challenge;
}
