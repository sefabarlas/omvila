import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_musteri/app/router.dart';
import 'package:omvila_musteri/features/appointments/presentation/randevularim_ekrani.dart';
import 'package:omvila_musteri/features/auth/presentation/acilis_ekrani.dart';
import 'package:omvila_musteri/features/auth/presentation/giris_ekrani.dart';
import 'package:omvila_musteri/features/auth/presentation/giris_kod_ekrani.dart';
import 'package:omvila_musteri/features/booking/presentation/salon_secimi_ekrani.dart';
import 'package:omvila_musteri/features/notifications/presentation/bildirim_izni_ekrani.dart';
import 'package:omvila_musteri/features/shell/sekme_kabugu.dart';
import 'package:omvila_musteri/shared/widgets/baglanti_gecersiz_ekrani.dart';

import '../helpers/fakes.dart';
import '../helpers/fixtures.dart';
import '../helpers/harness.dart';

// Ana ekranlar açık, koyu ve 1.3× yazıda (KK11). Tasarım:
// design/ekranlar/musteri-uygulamasi/.

Widget _sekme(Widget child, [int index = 0]) =>
    SekmeKabugu(seciliIndex: index, onSecildi: (_) {}, child: child);

Future<void> _twoFrames(WidgetTester tester) async {
  await tester.pump();
  await tester.pump();
}

void main() {
  screenGoldenTest(
    'Açılış',
    fileName: 'acilis',
    screen: () => const AcilisEkrani(),
    deps: TestDeps.new,
  );

  screenGoldenTest(
    'Giriş',
    fileName: 'giris',
    screen: () => const GirisEkrani(),
    deps: TestDeps.new,
  );

  screenGoldenTest(
    'Giriş kodu',
    fileName: 'giris_kod',
    screen: () => GirisKodEkrani(
      args: GirisKodArgs(
        phone: '+905326112467',
        challenge: FakeAuthRepository().challenge,
      ),
    ),
    deps: TestDeps.new,
    pumpBeforeTest: _twoFrames,
  );

  screenGoldenTest(
    'Bildirim izni',
    fileName: 'bildirim_izni',
    screen: () => const BildirimIzniEkrani(),
    deps: () => TestDeps(store: signedInStore(asked: false)),
  );

  screenGoldenTest(
    'Randevularım',
    fileName: 'main',
    screen: () => _sekme(const RandevularimEkrani()),
    deps: () => TestDeps(store: signedInStore()),
  );

  screenGoldenTest(
    'Randevularım ilk giriş',
    fileName: 'main_bos',
    screen: () => _sekme(const RandevularimEkrani()),
    deps: () => TestDeps(
      store: signedInStore(),
      appointments: FakeAppointmentsRepository(home: bosHome),
    ),
  );

  screenGoldenTest(
    'Randevularım yaklaşan yok',
    fileName: 'main_yaklasan_yok',
    screen: () => _sekme(const RandevularimEkrani()),
    deps: () => TestDeps(
      store: signedInStore(),
      appointments: FakeAppointmentsRepository(home: yaklasanYokHome),
    ),
  );

  screenGoldenTest(
    'Randevularım bildirim kapalı',
    fileName: 'main_bildirim_kapali',
    screen: () => _sekme(const RandevularimEkrani()),
    deps: () => TestDeps(store: signedInStore(skipped: true)),
  );

  screenGoldenTest(
    'Randevularım yükleniyor',
    fileName: 'main_yukleniyor',
    screen: () => _sekme(const RandevularimEkrani()),
    deps: () => TestDeps(
      store: signedInStore(),
      appointments: FakeAppointmentsRepository(pending: true),
    ),
    pumpBeforeTest: _twoFrames,
  );

  screenGoldenTest(
    'Paketlerim',
    fileName: 'paketler_sekmesi',
    screen: () => _sekme(const PaketlerSekmesi(), 1),
    deps: TestDeps.new,
  );

  screenGoldenTest(
    'Hangi salon',
    fileName: 'salon_secimi',
    screen: () => const SalonSecimiEkrani(),
    deps: () => TestDeps(
      brands: FakeBrandsRepository(
        brands: [
          brand('Kuaför Elif'),
          brand('Spa Moda', slug: 'spa-moda', lastVisit: '2026-10-02'),
        ],
      ),
    ),
  );

  screenGoldenTest(
    'Bağlantı geçersiz',
    fileName: 'baglanti_gecersiz',
    screen: () => const BaglantiGecersizEkrani(),
    deps: TestDeps.new,
  );
}
