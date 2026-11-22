import 'package:flutter/material.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/features/notifications/presentation/bildirimler_ekrani.dart';
import 'package:omvila_musteri/features/profile/presentation/hesap_silme_ekranlari.dart';
import 'package:omvila_musteri/features/profile/presentation/izinler_ekrani.dart';
import 'package:omvila_musteri/features/profile/presentation/profil_duzenle_ekrani.dart';
import 'package:omvila_musteri/features/profile/presentation/profil_ekrani.dart';
import 'package:omvila_musteri/features/shell/sekme_kabugu.dart';
import 'package:omvila_musteri/features/waitlist/presentation/bekleme_katil_sayfasi.dart';
import 'package:omvila_musteri/features/waitlist/presentation/bekleme_teklif_ekrani.dart';
import 'package:omvila_musteri/features/wallet/presentation/odemelerim_ekrani.dart';
import 'package:omvila_musteri/features/wallet/presentation/paketler_ekrani.dart';
import 'package:omvila_musteri/shared/widgets/durum_desenleri.dart';
import 'package:omvila_ui/omvila_ui.dart';

import '../helpers/booking_fixtures.dart';
import '../helpers/fakes.dart';
import '../helpers/fixtures.dart';
import '../helpers/harness.dart';
import '../helpers/hesap_fixtures.dart';

// Paketler, profil, ödemeler, izinler, hesap silme, bildirimler ve bekleme
// listesi ekranları açık, koyu ve 1.3× yazıda (KK11). Tasarım:
// design/ekranlar/musteri-uygulamasi/.

Widget _sekme(Widget child, int index) =>
    SekmeKabugu(seciliIndex: index, onSecildi: (_) {}, child: child);

void main() {
  screenGoldenTest(
    'Paketlerim',
    fileName: 'paketler',
    screen: () => _sekme(const PaketlerEkrani(), 1),
    deps: () => TestDeps(wallet: FakeWalletRepository(packageList: paketler())),
  );

  screenGoldenTest(
    'Profil',
    fileName: 'profil',
    screen: () => _sekme(const ProfilEkrani(), 2),
    deps: () => TestDeps(
      brands: FakeBrandsRepository(
        brands: [
          brand('Kuaför Elif'),
          brand('Nail Studio', slug: 'nail-studio'),
          brand('Spa Moda', slug: 'spa-moda'),
        ],
      ),
      profile: FakeProfileRepository(profile: profil(), consentList: izinler()),
    ),
  );

  screenGoldenTest(
    'Profil bilgilerim',
    fileName: 'profil_duzenle',
    screen: () => const ProfilDuzenleEkrani(),
    deps: () => TestDeps(
      profile: FakeProfileRepository(
        profile: profil(email: 'merve@ornek.test'),
      ),
    ),
  );

  screenGoldenTest(
    'Ödemelerim',
    fileName: 'odemelerim',
    screen: () => const OdemelerimEkrani(),
    deps: () => TestDeps(
      wallet: FakeWalletRepository(paymentPages: {null: odemeler()}),
    ),
  );

  screenGoldenTest(
    'Ödemelerim boş',
    fileName: 'odemelerim_bos',
    screen: () => const OdemelerimEkrani(),
    deps: TestDeps.new,
  );

  screenGoldenTest(
    'İzinlerim',
    fileName: 'izinler',
    screen: () => const IzinlerEkrani(),
    deps: () =>
        TestDeps(profile: FakeProfileRepository(consentList: izinler())),
  );

  screenGoldenTest(
    'Hesabı sil',
    fileName: 'hesap_sil',
    screen: () => const HesapSilEkrani(),
    deps: () => TestDeps(
      profile: FakeProfileRepository(preview: silmeOnizleme()),
    ),
  );

  screenGoldenTest(
    'Hesabınız silindi',
    fileName: 'hesap_silindi',
    screen: () => const HesapSilindiEkrani(),
    deps: TestDeps.new,
  );

  screenGoldenTest(
    'Bildirimler',
    fileName: 'bildirimler',
    screen: () => const BildirimlerEkrani(),
    deps: () => TestDeps(
      notifications: FakeNotificationsRepository(page: bildirimler()),
    ),
  );

  screenGoldenTest(
    'Bildirimler boş',
    fileName: 'bildirimler_bos',
    screen: () => const BildirimlerEkrani(),
    deps: TestDeps.new,
  );

  screenGoldenTest(
    'Bekleme listesine katıl',
    fileName: 'bekleme_katil',
    screen: () => Builder(
      builder: (context) => Scaffold(
        backgroundColor: context.omvilaRenkler.zeminYukseltilmis,
        body: const Align(
          alignment: Alignment.bottomCenter,
          child: BeklemeKatilSayfasi(),
        ),
      ),
    ),
    deps: () => TestDeps(
      booking: FakeBookingRepository(catalogValue: kuaforElifKatalog()),
    ),
    overrides: [
      seedBooking(
        taslak(
          services: ['kesim', 'fon'],
          staff: {'kesim': 'elif', 'fon': 'elif'},
          date: '2026-11-26',
        ),
      ),
    ],
  );

  screenGoldenTest(
    'Bekleme teklifi',
    fileName: 'bekleme_teklif',
    screen: () => const BeklemeTeklifEkrani(code: 'TEKLIF1'),
    deps: () =>
        TestDeps(waitlist: FakeWaitlistRepository(offerValue: teklif())),
  );

  screenGoldenTest(
    'Bekleme süresi doldu',
    fileName: 'bekleme_doldu',
    screen: () => const BeklemeTeklifEkrani(code: 'TEKLIF1'),
    deps: () => TestDeps(
      waitlist: FakeWaitlistRepository(
        offerValue: teklif(status: WaitlistOfferStatus.expired),
      ),
    ),
  );

  screenGoldenTest(
    'Durum desenleri (OTP sınırı, online randevu kapalı)',
    fileName: 'durumlar',
    screen: () => Builder(
      builder: (context) => Scaffold(
        backgroundColor: context.omvilaRenkler.zemin,
        body: ListView(
          padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
          children: [
            const OtpSiniriKutusu(
              problem: ApiProblem(
                status: 429,
                code: 'otp_attempts_exceeded',
                meta: {'retryAfterSeconds': 300},
              ),
            ),
            const SizedBox(height: OmvilaBosluk.bosluk4),
            OnlineRandevuKapaliKutusu(salonAdi: 'Kuaför Elif', onAra: () {}),
          ],
        ),
      ),
    ),
    deps: TestDeps.new,
  );
}
