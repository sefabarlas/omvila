import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/config.dart';
import 'package:omvila_musteri/app/router.dart';
import 'package:omvila_musteri/features/profile/presentation/izinler_ekrani.dart';
import 'package:omvila_musteri/features/profile/presentation/profil_duzenle_ekrani.dart';
import 'package:omvila_musteri/features/profile/presentation/profil_ekrani.dart';
import 'package:omvila_musteri/features/wallet/presentation/odemelerim_ekrani.dart';
import 'package:omvila_musteri/features/wallet/presentation/paketler_ekrani.dart';
import 'package:omvila_musteri/shared/widgets/durum_desenleri.dart';
import 'package:omvila_ui/omvila_ui.dart';

import '../helpers/fakes.dart';
import '../helpers/fixtures.dart';
import '../helpers/harness.dart';
import '../helpers/hesap_fixtures.dart';

// Paketlerim, Ödemelerim, Profil, Profil bilgilerim, İzinlerim ve hesap
// silme akışı.

const _offline = ApiProblem(status: 0, code: 'network_error');
const _server = ApiProblem(status: 500, code: 'try_again');

Future<TestDeps> _pump(
  WidgetTester tester,
  Widget screen,
  TestDeps deps, {
  bool settle = true,
}) async {
  usePhone(tester);
  await tester.pumpWidget(
    screenApp(
      screen,
      deps,
      routes: [
        for (final r in [
          Routes.profilBilgileri,
          Routes.odemeler,
          Routes.izinler,
          Routes.hesapSil,
          Routes.yrHizmet,
        ])
          markerRoute(r),
      ],
    ),
  );
  if (settle) {
    await tester.pumpAndSettle();
  } else {
    await tester.pump();
  }

  return deps;
}

void main() {
  group('Paketlerim (Paketler)', () {
    testWidgets('dolu: seans, "Az kaldı", son kullanım, biten paketler', (
      tester,
    ) async {
      await _pump(
        tester,
        const PaketlerEkrani(),
        TestDeps(wallet: FakeWalletRepository(packageList: paketler())),
      );
      expect(find.text('2 aktif paket'), findsOneWidget);
      expect(find.text('Fön paketi'), findsOneWidget);
      expect(find.text('Kuaför Elif · Kadıköy'), findsOneWidget);
      expect(find.text('Spa Moda'), findsOneWidget);
      expect(find.text('Az kaldı'), findsNWidgets(2));
      expect(find.textContaining('/ 5 seans kaldı'), findsOneWidget);
      expect(find.text('Son kullanım 31 Aralık'), findsOneWidget);
      expect(find.text('Son kullanım 15 Mart 2027'), findsOneWidget);
      expect(find.text('Biten paketler'), findsOneWidget);
      expect(find.text('Cilt bakımı · 4 seans'), findsOneWidget);
      expect(find.text("Kuaför Elif · Eylül'de bitti"), findsOneWidget);
      expect(find.text('Bitti'), findsOneWidget);
    });

    testWidgets('boş: paketler uygulamadan satılmaz', (tester) async {
      await _pump(tester, const PaketlerEkrani(), TestDeps());
      expect(find.text('Henüz paketiniz yok'), findsOneWidget);
      expect(
        find.textContaining('Paketler uygulamadan satılmaz'),
        findsOneWidget,
      );
      expect(find.text('2 aktif paket'), findsNothing);
    });

    testWidgets('yükleniyor ve hata', (tester) async {
      final wallet = FakeWalletRepository()..packagesPending = true;
      await _pump(
        tester,
        const PaketlerEkrani(),
        TestDeps(wallet: wallet),
        settle: false,
      );
      expect(find.byType(YukleniyorDurumu), findsOneWidget);

      await _pump(
        tester,
        const PaketlerEkrani(),
        TestDeps(wallet: FakeWalletRepository()..packagesError = _offline),
      );
      expect(find.text('Bağlantı yok'), findsOneWidget);
    });
  });

  group('Ödemelerim (Odemelerim)', () {
    testWidgets('dolu: kapora çipleri, salon tahsilatı, paket, ay başlığı', (
      tester,
    ) async {
      await _pump(
        tester,
        const OdemelerimEkrani(),
        TestDeps(
          wallet: FakeWalletRepository(paymentPages: {null: odemeler()}),
        ),
      );
      expect(find.textContaining('Uygulamadan ödeme alınmaz'), findsOneWidget);
      expect(find.text('Kasım'), findsOneWidget);
      expect(find.text('Ekim'), findsOneWidget);
      expect(find.text('Salon onayı bekleniyor'), findsOneWidget);
      expect(find.text('Alındı · 26 Kasım randevusu'), findsOneWidget);
      expect(
        find.text('Boya ve fön · 12 Kas · Kartla, salonda'),
        findsOneWidget,
      );
      expect(find.text('Salon iade etti'), findsOneWidget);
      expect(
        find.text('Fön paketi · 5 seans · 2 Eki · Havale'),
        findsOneWidget,
      );
      expect(find.text('1.240 ₺'), findsOneWidget);
      // Kart formu yok: yalnız kayıt.
      expect(find.byType(TextField), findsNothing);
    });

    testWidgets('sonraki sayfa "Daha fazla göster" ile', (tester) async {
      final wallet = FakeWalletRepository(
        paymentPages: {
          null: odemeler(next: 'c2'),
          'c2': MePaymentPage(
            items: [
              odeme(
                'p9',
                kind: MePaymentKind.appointmentPayment,
                brand: ref('Nail Studio Moda', 'nail-studio'),
                lira: 350,
                at: '2026-09-04T09:00:00Z',
                method: PaymentMethod.cash,
                services: ['Manikür'],
              ),
            ],
            nextCursor: null,
          ),
        },
      );
      await _pump(tester, const OdemelerimEkrani(), TestDeps(wallet: wallet));
      await tester.ensureVisible(find.text('Daha fazla göster'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Daha fazla göster'));
      await tester.pumpAndSettle();
      expect(wallet.paymentRequests, [null, 'c2']);
      expect(find.text('Manikür · 4 Eyl · Nakit'), findsOneWidget);
      expect(find.text('Daha fazla göster'), findsNothing);
    });

    testWidgets('boş, yükleniyor, hata', (tester) async {
      await _pump(tester, const OdemelerimEkrani(), TestDeps());
      expect(find.text('Ödeme kaydı yok'), findsOneWidget);

      await _pump(
        tester,
        const OdemelerimEkrani(),
        TestDeps(wallet: FakeWalletRepository()..paymentsPending = true),
        settle: false,
      );
      expect(find.byType(YukleniyorDurumu), findsOneWidget);

      await _pump(
        tester,
        const OdemelerimEkrani(),
        TestDeps(wallet: FakeWalletRepository()..paymentsError = _server),
      );
      expect(find.text('Sayfa yüklenemedi'), findsOneWidget);
    });
  });

  group('Profil', () {
    TestDeps deps({AppConfig? config}) => TestDeps(
      brands: FakeBrandsRepository(
        brands: [
          brand('Kuaför Elif'),
          brand('Nail Studio', slug: 'nail-studio'),
        ],
      ),
      profile: FakeProfileRepository(profile: profil(), consentList: izinler()),
      config: config ?? const AppConfig(apiUrl: 'http://test'),
    );

    testWidgets('dolu: ad, telefon, Salonlarım, menü', (tester) async {
      await _pump(tester, const ProfilEkrani(), deps());
      expect(find.text('Merve Yıldız'), findsOneWidget);
      expect(find.text('MY'), findsOneWidget);
      expect(find.text('0532 611 24 67'), findsOneWidget);
      expect(find.text('Salonlarım'), findsOneWidget);
      expect(find.text('Nail Studio'), findsOneWidget);
      expect(find.text('1 salonda kampanya mesajları açık'), findsOneWidget);
      expect(find.text('Çıkış yap'), findsOneWidget);
      // Dil seçimi yok (T-101).
      expect(find.textContaining('dil'), findsNothing);
    });

    testWidgets('menü satırları ekranları açar', (tester) async {
      await _pump(tester, const ProfilEkrani(), deps());
      await tester.tap(find.text('Ödemelerim'));
      await tester.pumpAndSettle();
      expect(find.text('adres:${Routes.odemeler}'), findsOneWidget);
    });

    testWidgets('salona dokununca randevu alma başlar', (tester) async {
      await _pump(tester, const ProfilEkrani(), deps());
      await tester.tap(find.text('Kuaför Elif'));
      await tester.pumpAndSettle();
      expect(find.text('adres:${Routes.yrHizmet}'), findsOneWidget);
    });

    testWidgets('"Çıkış yap" meLogout', (tester) async {
      final d = await _pump(tester, const ProfilEkrani(), deps());
      await tester.ensureVisible(find.text('Çıkış yap'));
      await tester.tap(find.text('Çıkış yap'));
      await tester.pumpAndSettle();
      expect(d.auth.logouts, hasLength(1));
    });

    testWidgets('Yardım: destek adresi varsa e-posta', (tester) async {
      final d = await _pump(
        tester,
        const ProfilEkrani(),
        deps(
          config: const AppConfig(
            apiUrl: 'http://test',
            supportEmail: 'destek@ornek.test',
          ),
        ),
      );
      await tester.ensureVisible(find.text('Yardım ve iletişim'));
      await tester.tap(find.text('Yardım ve iletişim'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('E-posta gönder'));
      expect(d.external.urls.single.toString(), 'mailto:destek@ornek.test');
    });

    testWidgets('yükleniyor ve hata', (tester) async {
      await _pump(
        tester,
        const ProfilEkrani(),
        TestDeps(profile: FakeProfileRepository()..profilePending = true),
        settle: false,
      );
      expect(find.byType(YukleniyorDurumu), findsOneWidget);

      await _pump(
        tester,
        const ProfilEkrani(),
        TestDeps(profile: FakeProfileRepository()..profileError = _offline),
      );
      expect(find.text('Bağlantı yok'), findsOneWidget);
    });

    testWidgets('salon yoksa boş metin', (tester) async {
      await _pump(tester, const ProfilEkrani(), TestDeps());
      expect(find.textContaining('Kayıtlı salonunuz yok'), findsOneWidget);
    });
  });

  group('Profil bilgilerim (ProfilDuzenle)', () {
    testWidgets('kaydeder (If-Match), telefon salt okunur', (tester) async {
      final repo = FakeProfileRepository(profile: profil());
      await _pump(
        tester,
        const ProfilDuzenleEkrani(),
        TestDeps(profile: repo),
      );
      expect(find.text('Değiştirmek için destek'), findsOneWidget);
      expect(find.text('Türkçe'), findsNothing);
      expect(find.byType(TextField), findsNWidgets(2));
      await tester.enterText(find.byType(TextField).last, 'merve@ornek.test');
      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();
      expect(repo.updates.single, ('Merve Yıldız', 'merve@ornek.test', 'v1'));
      expect(find.text('Bilgileriniz kaydedildi.'), findsOneWidget);
    });

    testWidgets('doğrulama: ad, e-posta biçimi, e-postayı silme', (
      tester,
    ) async {
      final repo = FakeProfileRepository(
        profile: profil(email: 'eski@ornek.test'),
      );
      await _pump(tester, const ProfilDuzenleEkrani(), TestDeps(profile: repo));
      await tester.enterText(find.byType(TextField).first, 'M');
      await tester.enterText(find.byType(TextField).last, '');
      await tester.tap(find.text('Kaydet'));
      await tester.pump();
      expect(find.text('Adınızı ve soyadınızı yazın.'), findsOneWidget);
      expect(
        find.text('E-postayı kaldırmak için destekle görüşün.'),
        findsOneWidget,
      );
      await tester.enterText(find.byType(TextField).last, 'yanlis');
      await tester.tap(find.text('Kaydet'));
      await tester.pump();
      expect(find.text('Geçerli bir e-posta yazın.'), findsOneWidget);
      expect(repo.updates, isEmpty);
    });

    testWidgets('sunucu hatası kutuda', (tester) async {
      final repo = FakeProfileRepository(profile: profil())
        ..updateError = const ApiProblem(
          status: 412,
          code: 'concurrency_conflict',
        );
      await _pump(tester, const ProfilDuzenleEkrani(), TestDeps(profile: repo));
      await tester.tap(find.text('Kaydet'));
      await tester.pumpAndSettle();
      expect(find.text('Kaydedilemedi.'), findsOneWidget);
      expect(find.text('Kayıt çakışması. Sayfayı yenileyin.'), findsOneWidget);
    });

    testWidgets('yükleniyor ve hata', (tester) async {
      await _pump(
        tester,
        const ProfilDuzenleEkrani(),
        TestDeps(profile: FakeProfileRepository()..profilePending = true),
        settle: false,
      );
      expect(find.byType(YukleniyorDurumu), findsOneWidget);
      await _pump(
        tester,
        const ProfilDuzenleEkrani(),
        TestDeps(profile: FakeProfileRepository()..profileError = _server),
      );
      expect(find.text('Sayfa yüklenemedi'), findsOneWidget);
    });
  });

  group('İzinlerim (Izinler)', () {
    testWidgets('randevu mesajları açıklaması, salon anahtarı', (tester) async {
      final repo = FakeProfileRepository(consentList: izinler());
      await _pump(tester, const IzinlerEkrani(), TestDeps(profile: repo));
      expect(find.textContaining('her zaman gönderilir'), findsOneWidget);
      expect(find.byType(Switch), findsNWidgets(2));
      await tester.tap(find.text('Nail Studio Moda'));
      await tester.pumpAndSettle();
      expect(repo.consentUpdates.single, ('brand-nail-studio', true));
      final switches = tester.widgetList<Switch>(find.byType(Switch));
      expect(switches.every((s) => s.value), isTrue);
    });

    testWidgets('anahtar hatasında mesaj, değer değişmez', (tester) async {
      final repo = FakeProfileRepository(consentList: izinler())
        ..consentUpdateError = _server;
      await _pump(tester, const IzinlerEkrani(), TestDeps(profile: repo));
      await tester.tap(find.text('Kuaför Elif'));
      await tester.pumpAndSettle();
      expect(find.text('Bir sorun oluştu. Tekrar deneyin.'), findsOneWidget);
      expect(tester.widget<Switch>(find.byType(Switch).first).value, isTrue);
    });

    testWidgets('boş, hata ve "Hesabımı ve verilerimi sil"', (tester) async {
      await _pump(tester, const IzinlerEkrani(), TestDeps());
      expect(find.textContaining('Salon eklediğinizde'), findsOneWidget);
      await tester.tap(find.text('Hesabımı ve verilerimi sil'));
      await tester.pumpAndSettle();
      expect(find.text('adres:${Routes.hesapSil}'), findsOneWidget);

      await _pump(
        tester,
        const IzinlerEkrani(),
        TestDeps(profile: FakeProfileRepository()..consentsError = _offline),
      );
      expect(find.text('Bağlantı yok'), findsOneWidget);
    });
  });

  group('Hesap silme akışı (HesapSil → kod → HesapSilindi → Giriş)', () {
    Future<TestDeps> start(WidgetTester tester, FakeProfileRepository repo) =>
        _routed(tester, repo);

    testWidgets('uyarı, SMS kodu, silme, oturum kapanır', (tester) async {
      final repo = FakeProfileRepository(
        profile: profil(),
        preview: silmeOnizleme(),
      )..challenge = silmeKodu();
      final d = await start(tester, repo);

      expect(
        find.text('Hesabınız ve verileriniz silinsin mi?'),
        findsOneWidget,
      );
      expect(
        find.text('2 yaklaşan randevunuz iptal edilecek.'),
        findsOneWidget,
      );
      expect(
        find.textContaining('Kuaför Elif · 25 Kas, Nail Studio Moda · 3 Ara'),
        findsOneWidget,
      );
      expect(find.text('SİLİNENLER'), findsOneWidget);
      expect(find.text('SALONLARDA KALANLAR'), findsOneWidget);
      expect(find.text('Kalan paket seansları'), findsOneWidget);

      await tester.tap(find.text('SMS koduyla onayla ve sil'));
      await tester.pumpAndSettle();
      expect(repo.otpRequests, 1);
      expect(find.textContaining('0532 *** ** 67'), findsOneWidget);

      await tester.enterText(find.byType(EditableText).first, '482193');
      await tester.pumpAndSettle();
      expect(repo.deletions.single, ('otp-sil', '482193'));
      expect(d.auth.forgets, 1);
      expect(find.text('Hesabınız silindi'), findsOneWidget);

      await tester.tap(find.text('Kapat'));
      await tester.pumpAndSettle();
      expect(find.text('Kod gönder'), findsOneWidget);
    });

    testWidgets('kod hatalıysa kalan deneme, hesap silinmez', (tester) async {
      final repo =
          FakeProfileRepository(profile: profil(), preview: silmeOnizleme())
            ..challenge = silmeKodu()
            ..deleteError = const ApiProblem(
              status: 422,
              code: 'otp_invalid',
              meta: {'attemptsLeft': 2},
            );
      final d = await start(tester, repo);
      await tester.tap(find.text('SMS koduyla onayla ve sil'));
      await tester.pumpAndSettle();
      await tester.enterText(find.byType(EditableText).first, '000000');
      await tester.pumpAndSettle();
      expect(find.text('Kod hatalı. 2 deneme hakkınız kaldı.'), findsOneWidget);
      expect(d.auth.forgets, 0);
    });

    testWidgets('deneme sınırında "Çok fazla deneme yaptınız."', (
      tester,
    ) async {
      final repo =
          FakeProfileRepository(
              profile: profil(),
              preview: silmeOnizleme(upcoming: false, packages: 0),
            )
            ..otpError = const ApiProblem(
              status: 429,
              code: 'otp_attempts_exceeded',
              meta: {'retryAfterSeconds': 300},
            );
      await start(tester, repo);
      expect(find.textContaining('yaklaşan randevunuz'), findsNothing);
      expect(find.text('Kalan paket seansları'), findsNothing);
      await tester.tap(find.text('SMS koduyla onayla ve sil'));
      await tester.pumpAndSettle();
      expect(find.byType(OtpSiniriKutusu), findsOneWidget);
      expect(
        find.text(
          'Güvenliğiniz için 5 dakika sonra yeni kod isteyebilirsiniz.',
        ),
        findsOneWidget,
      );
    });

    testWidgets('önizleme okunamazsa hata', (tester) async {
      final repo = FakeProfileRepository(profile: profil())
        ..previewError = _offline;
      await start(tester, repo);
      expect(find.text('Bağlantı yok'), findsOneWidget);
      expect(find.text('SMS koduyla onayla ve sil'), findsNothing);
    });
  });
}

Future<TestDeps> _routed(
  WidgetTester tester,
  FakeProfileRepository repo,
) async {
  usePhone(tester);
  final deps = TestDeps(store: signedInStore(), profile: repo);
  final container = deps.container();
  addTearDown(container.dispose);
  await tester.pumpWidget(routedApp(container));
  await tester.pump();
  container.read(routerProvider).go(Routes.hesapSil);
  await tester.pumpAndSettle();
  return deps;
}
