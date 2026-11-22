import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/features/appointments/presentation/basari_ekrani.dart';
import 'package:omvila_musteri/features/appointments/presentation/degerlendir_ekrani.dart';
import 'package:omvila_musteri/features/appointments/presentation/ertele_ekrani.dart';
import 'package:omvila_musteri/features/appointments/presentation/gecmis_ekrani.dart';
import 'package:omvila_musteri/features/appointments/presentation/havale_ekrani.dart';
import 'package:omvila_musteri/features/appointments/presentation/randevu_detay_ekrani.dart';
import 'package:omvila_musteri/features/appointments/presentation/randevularim_ekrani.dart';
import 'package:omvila_musteri/features/auth/presentation/acilis_ekrani.dart';
import 'package:omvila_musteri/features/auth/presentation/auth_controller.dart';
import 'package:omvila_musteri/features/auth/presentation/giris_ekrani.dart';
import 'package:omvila_musteri/features/auth/presentation/giris_kod_ekrani.dart';
import 'package:omvila_musteri/features/booking/presentation/booking_controller.dart';
import 'package:omvila_musteri/features/booking/presentation/hizmet_ekrani.dart';
import 'package:omvila_musteri/features/booking/presentation/onay_ekrani.dart';
import 'package:omvila_musteri/features/booking/presentation/personel_ekrani.dart';
import 'package:omvila_musteri/features/booking/presentation/saat_ekrani.dart';
import 'package:omvila_musteri/features/booking/presentation/salon_secimi_ekrani.dart';
import 'package:omvila_musteri/features/brands/domain/brand_link.dart';
import 'package:omvila_musteri/features/brands/presentation/qr_tarama_ekrani.dart';
import 'package:omvila_musteri/features/brands/presentation/salon_ekle_ekrani.dart';
import 'package:omvila_musteri/features/notifications/presentation/bildirim_izni_ekrani.dart';
import 'package:omvila_musteri/features/notifications/presentation/notification_prefs_controller.dart';
import 'package:omvila_musteri/features/shell/sekme_kabugu.dart';
import 'package:omvila_musteri/shared/widgets/baglanti_gecersiz_ekrani.dart';
import 'package:omvila_musteri/shared/widgets/hazirlaniyor_ekrani.dart';

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

  /// Hizmet seçin.
  static const yrHizmet = '/randevu-al/hizmet';

  /// Kimden olsun?
  static const yrPersonel = '/randevu-al/personel';

  /// Ne zaman?
  static const yrSaat = '/randevu-al/saat';

  /// Son kontrol.
  static const yrOnay = '/randevu-al/onay';

  /// Bekleme listesine katıl (BeklemeKatil; sonraki PR).
  static const beklemeKatil = '/randevu-al/bekleme';

  /// Geçmiş.
  static const gecmis = '/gecmis';

  /// Randevu detayı.
  static String randevu(String id) => '/randevu/$id';

  /// Saati değiştir.
  static String ertele(String id) => '/randevu/$id/ertele';

  /// Kapora havalesi; [yeni] randevu yeni oluşturuldu ("Daha sonra" →
  /// başarı ekranı).
  static String havale(String id, {bool yeni = false}) =>
      '/randevu/$id/havale${yeni ? '?yeni=1' : ''}';

  /// Randevu oluşturuldu.
  static String tamam(String id) => '/randevu/$id/tamam';

  /// Değerlendir.
  static String degerlendir(String id) => '/randevu/$id/degerlendir';

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
        routes: [
          for (final (path, screen) in const [
            ('hizmet', HizmetEkrani()),
            ('personel', PersonelEkrani()),
            ('saat', SaatEkrani()),
            ('onay', OnayEkrani()),
            ('bekleme', HazirlaniyorEkrani(baslik: 'Bekleme listesi')),
          ])
            GoRoute(
              path: path,
              // Taslak yoksa (uygulama yeniden açıldı) akış baştan başlar.
              redirect: (context, state) =>
                  ref.read(bookingProvider) == null ? Routes.randevuAl : null,
              builder: (context, state) => screen,
            ),
        ],
      ),
      GoRoute(
        path: Routes.gecmis,
        builder: (context, state) => const GecmisEkrani(),
      ),
      GoRoute(
        path: '/randevu/:id',
        builder: (context, state) =>
            RandevuDetayEkrani(id: state.pathParameters['id']!),
        routes: [
          GoRoute(
            path: 'ertele',
            builder: (context, state) =>
                ErteleEkrani(id: state.pathParameters['id']!),
          ),
          GoRoute(
            path: 'havale',
            builder: (context, state) => HavaleEkrani(
              id: state.pathParameters['id']!,
              yeni: state.uri.queryParameters['yeni'] == '1',
            ),
          ),
          GoRoute(
            path: 'tamam',
            builder: (context, state) =>
                BasariEkrani(id: state.pathParameters['id']!),
          ),
          GoRoute(
            path: 'degerlendir',
            builder: (context, state) =>
                DegerlendirEkrani(appointmentId: state.pathParameters['id']),
          ),
        ],
      ),
      // Kişiye özel bağlantılar (K3): /r/ randevu, /d/ değerlendirme; /b/
      // bekleme teklifi sonraki PR'da (şimdilik Randevularım).
      GoRoute(
        path: '/r/:code',
        builder: (context, state) =>
            KodluRandevuEkrani(code: state.pathParameters['code']!),
      ),
      GoRoute(
        path: '/d/:code',
        builder: (context, state) =>
            DegerlendirEkrani(accessCode: state.pathParameters['code']),
      ),
      GoRoute(
        path: '/b/:code',
        redirect: (context, state) => Routes.randevularim,
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
