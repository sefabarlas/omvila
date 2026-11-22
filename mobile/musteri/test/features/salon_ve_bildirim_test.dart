import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:go_router/go_router.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/router.dart';
import 'package:omvila_musteri/features/booking/presentation/salon_secimi_ekrani.dart';
import 'package:omvila_musteri/features/brands/presentation/qr_tarama_ekrani.dart';
import 'package:omvila_musteri/features/notifications/presentation/bildirim_izni_ekrani.dart';
import 'package:omvila_musteri/features/shell/sekme_kabugu.dart';
import 'package:omvila_ui/omvila_ui.dart';

import '../helpers/fakes.dart';
import '../helpers/fixtures.dart';
import '../helpers/harness.dart';

void main() {
  group('Bildirim izni', () {
    testWidgets('izin verilince jeton kaydedilir', (tester) async {
      final deps = TestDeps(
        store: signedInStore(asked: false),
        push: FakePush(pushToken: 'fcm-jetonu'),
      );
      usePhone(tester);
      await tester.pumpWidget(screenApp(const BildirimIzniEkrani(), deps));
      expect(find.text('Randevunuzu kaçırmayın'), findsOneWidget);
      await tester.tap(find.text('Bildirimlere izin ver'));
      await tester.pump();
      await tester.pump();
      expect(deps.push.requests, 1);
      expect(deps.device.tokens, ['fcm-jetonu']);
      expect(deps.store.values['push.asked'], '1');
      expect(deps.store.values['push.skipped'], '0');
    });

    testWidgets('"Şimdi değil" sistem istemini açmaz', (tester) async {
      final deps = TestDeps(store: signedInStore(asked: false));
      usePhone(tester);
      await tester.pumpWidget(screenApp(const BildirimIzniEkrani(), deps));
      await tester.tap(find.text('Şimdi değil'));
      await tester.pump();
      expect(deps.push.requests, 0);
      expect(deps.store.values['push.skipped'], '1');
    });

    testWidgets('reddedilen izin şerit bırakır', (tester) async {
      final deps = TestDeps(
        store: signedInStore(asked: false),
        push: FakePush(grant: false),
      );
      usePhone(tester);
      await tester.pumpWidget(screenApp(const BildirimIzniEkrani(), deps));
      await tester.tap(find.text('Bildirimlere izin ver'));
      await tester.pump();
      expect(deps.device.tokens, isEmpty);
      expect(deps.store.values['push.skipped'], '1');
    });
  });

  group('Hangi salon? (akış başlangıcı)', () {
    testWidgets('yükleniyor', (tester) async {
      await tester.pumpWidget(
        screenApp(
          const SalonSecimiEkrani(),
          TestDeps(brands: FakeBrandsRepository(pending: true)),
        ),
      );
      await tester.pump();
      expect(find.byType(CircularProgressIndicator), findsOneWidget);
    });

    testWidgets('salonlar ve paket rozeti', (tester) async {
      await tester.pumpWidget(
        screenApp(
          const SalonSecimiEkrani(),
          TestDeps(
            brands: FakeBrandsRepository(
              brands: [
                brand(
                  'Kuaför Elif',
                  packages: [
                    MeBrandPackageBadge(
                      customerPackageId: 'p1',
                      packageName: 'Fön paketi',
                      remainingSessions: 2,
                    ),
                  ],
                ),
                brand('Spa Moda', slug: 'spa-moda', lastVisit: null),
              ],
            ),
          ),
        ),
      );
      await tester.pump();
      expect(find.text('Hangi salon?'), findsOneWidget);
      expect(find.text('Kadıköy · son ziyaret 12 Kasım'), findsOneWidget);
      expect(find.text('Fön paketi var'), findsOneWidget);
      expect(find.text('Spa Moda'), findsOneWidget);
    });

    testWidgets('boş', (tester) async {
      await tester.pumpWidget(
        screenApp(const SalonSecimiEkrani(), TestDeps()),
      );
      await tester.pump();
      expect(find.text('Henüz bir salonunuz yok'), findsOneWidget);
      expect(find.text('Bağlantıyı yapıştır'), findsOneWidget);
    });

    testWidgets('hata', (tester) async {
      await tester.pumpWidget(
        screenApp(
          const SalonSecimiEkrani(),
          TestDeps(
            brands: FakeBrandsRepository(
              error: const ApiProblem(status: 500, code: 'try_again'),
            ),
          ),
        ),
      );
      await tester.pump();
      expect(find.text('Tekrar dene'), findsOneWidget);
    });
  });

  group('QR', () {
    late void Function(String) scan;

    Widget app() {
      final router = GoRouter(
        routes: [
          GoRoute(
            path: '/',
            builder: (context, _) => Scaffold(
              body: Builder(
                builder: (context) => TextButton(
                  onPressed: () async {
                    final slug = await context.push<String>(Routes.qr);
                    if (context.mounted) {
                      ScaffoldMessenger.of(
                        context,
                      ).showSnackBar(SnackBar(content: Text('sonuc:$slug')));
                    }
                  },
                  child: const Text('aç'),
                ),
              ),
            ),
          ),
          GoRoute(
            path: Routes.qr,
            builder: (_, _) => QrTaramaEkrani(
              scanner: (onValue) {
                scan = onValue;
                return const SizedBox.expand();
              },
            ),
          ),
        ],
      );
      return screenAppWithRouter(router, TestDeps());
    }

    testWidgets('salon QR kodu bağlantı adıyla döner', (tester) async {
      await tester.pumpWidget(app());
      await tester.tap(find.text('aç'));
      await tester.pumpAndSettle();
      expect(
        find.text('Salondaki QR kartı çerçeveye getirin.'),
        findsOneWidget,
      );
      scan('https://randevu.omvila.com.tr/kuafor-elif');
      await tester.pumpAndSettle();
      expect(find.text('sonuc:kuafor-elif'), findsOneWidget);
    });

    testWidgets('salon olmayan QR kodunda uyarı', (tester) async {
      await tester.pumpWidget(app());
      await tester.tap(find.text('aç'));
      await tester.pumpAndSettle();
      scan('https://ornek.com/menu');
      await tester.pump();
      expect(find.textContaining('bir salon bağlantısı değil'), findsOneWidget);
    });
  });

  group('Sekmeler', () {
    testWidgets('Paketlerim ve Profil başlık ve boş durum', (tester) async {
      await tester.pumpWidget(screenApp(const PaketlerSekmesi(), TestDeps()));
      expect(find.text('Paketlerim'), findsOneWidget);
      expect(find.text('Henüz paketiniz yok'), findsOneWidget);
      await tester.pumpWidget(screenApp(const ProfilSekmesi(), TestDeps()));
      expect(find.text('Profil'), findsOneWidget);
      expect(find.byType(OmvilaBosDurum), findsOneWidget);
    });
  });
}
