import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/router.dart';
import 'package:omvila_musteri/features/appointments/presentation/basari_ekrani.dart';
import 'package:omvila_musteri/features/appointments/presentation/degerlendir_ekrani.dart';
import 'package:omvila_musteri/features/appointments/presentation/ertele_ekrani.dart';
import 'package:omvila_musteri/features/appointments/presentation/gecmis_ekrani.dart';
import 'package:omvila_musteri/features/appointments/presentation/havale_ekrani.dart';
import 'package:omvila_musteri/features/appointments/presentation/randevu_detay_ekrani.dart';
import 'package:omvila_musteri/features/appointments/presentation/randevu_sayfalari.dart';
import 'package:omvila_musteri/features/appointments/presentation/randevularim_ekrani.dart';
import 'package:omvila_musteri/features/auth/presentation/acilis_ekrani.dart';
import 'package:omvila_musteri/features/auth/presentation/giris_ekrani.dart';
import 'package:omvila_musteri/features/auth/presentation/giris_kod_ekrani.dart';
import 'package:omvila_musteri/features/booking/presentation/hizmet_ekrani.dart';
import 'package:omvila_musteri/features/booking/presentation/onay_ekrani.dart';
import 'package:omvila_musteri/features/booking/presentation/personel_ekrani.dart';
import 'package:omvila_musteri/features/booking/presentation/saat_ekrani.dart';
import 'package:omvila_musteri/features/booking/presentation/salon_secimi_ekrani.dart';
import 'package:omvila_musteri/features/notifications/presentation/bildirim_izni_ekrani.dart';
import 'package:omvila_musteri/features/shell/sekme_kabugu.dart';
import 'package:omvila_musteri/shared/widgets/baglanti_gecersiz_ekrani.dart';

import '../helpers/booking_fixtures.dart';
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

  screenGoldenTest(
    'Hangi salon boş',
    fileName: 'yr_salon_bos',
    screen: () => const SalonSecimiEkrani(),
    deps: TestDeps.new,
  );

  screenGoldenTest(
    'Hizmet seçin',
    fileName: 'yr_hizmet',
    screen: () => const HizmetEkrani(),
    deps: () => TestDeps(
      booking: FakeBookingRepository(
        catalogValue: kuaforElifKatalog(),
        usable: [fonPaketi],
      ),
    ),
    overrides: [
      seedBooking(
        taslak(services: ['kesim', 'fon'], packages: {'fon': 'cpi-fon'}),
      ),
    ],
  );

  screenGoldenTest(
    'Kimden olsun',
    fileName: 'yr_personel',
    screen: () => const PersonelEkrani(),
    deps: () => TestDeps(
      booking: FakeBookingRepository(catalogValue: kuaforElifKatalog()),
    ),
    overrides: [
      seedBooking(
        taslak(
          services: ['kesim', 'fon'],
          staff: {'kesim': 'elif', 'fon': 'elif'},
          packages: {'fon': 'cpi-fon'},
        ),
      ),
    ],
  );

  screenGoldenTest(
    'Ne zaman',
    fileName: 'yr_saat',
    screen: () => const SaatEkrani(),
    deps: () => TestDeps(
      booking: FakeBookingRepository(
        catalogValue: kuaforElifKatalog(),
        dayResult: gunler(),
        slotResult: saatler(),
      ),
    ),
    overrides: [
      seedBooking(
        taslak(
          services: ['kesim', 'fon'],
          staff: {'kesim': 'elif', 'fon': 'elif'},
          date: '2026-11-25',
          slot: saat('2026-11-25', '14:30'),
        ),
      ),
    ],
  );

  screenGoldenTest(
    'Son kontrol',
    fileName: 'yr_onay',
    screen: () => const OnayEkrani(),
    deps: () => TestDeps(
      booking: FakeBookingRepository(
        catalogValue: kuaforElifKatalog(depositLira: 150),
        usable: [fonPaketi],
      ),
    ),
    overrides: [
      seedBooking(
        taslak(
          services: ['kesim', 'fon'],
          staff: {'kesim': 'elif', 'fon': 'elif'},
          packages: {'fon': 'cpi-fon'},
          date: '2026-11-25',
          slot: saat('2026-11-25', '14:30'),
        ),
      ),
    ],
  );

  screenGoldenTest(
    'Kapora havalesi',
    fileName: 'yr_havale',
    screen: () => const HavaleEkrani(id: 'a1', yeni: true),
    deps: () => TestDeps(
      appointments: FakeAppointmentsRepository(
        detail: detay(
          status: DisplayStatus.depositPending,
          deposit: kapora(DepositStatus.awaiting),
        ),
      ),
    ),
  );

  screenGoldenTest(
    'Randevu onaylandı',
    fileName: 'yr_basari',
    screen: () => const BasariEkrani(id: 'a1'),
    deps: () =>
        TestDeps(appointments: FakeAppointmentsRepository(detail: detay())),
  );

  screenGoldenTest(
    'Talep alındı',
    fileName: 'yr_basari_onay_bekliyor',
    screen: () => const BasariEkrani(id: 'a1'),
    deps: () => TestDeps(
      appointments: FakeAppointmentsRepository(
        detail: detay(status: DisplayStatus.pendingApproval),
      ),
    ),
  );

  screenGoldenTest(
    'Randevu detayı',
    fileName: 'randevu_detay',
    screen: () => const RandevuDetayEkrani(id: 'a1'),
    deps: () => TestDeps(
      appointments: FakeAppointmentsRepository(
        detail: detay(
          deposit: kapora(DepositStatus.received, instructions: false),
        ),
      ),
    ),
  );

  screenGoldenTest(
    'Saati değiştir',
    fileName: 'randevu_ertele',
    screen: () => const ErteleEkrani(id: 'a1'),
    deps: () => TestDeps(
      appointments:
          FakeAppointmentsRepository(
              detail: detay(
                deposit: kapora(DepositStatus.received, instructions: false),
              ),
            )
            ..days = gunler()
            ..slots = saatler(),
    ),
  );

  screenGoldenTest(
    'Saat değişikliği kapalı',
    fileName: 'ertele_kapali',
    screen: () => Scaffold(
      body: Align(
        alignment: Alignment.bottomCenter,
        child: GecKaldinizSayfasi(
          randevu: detay(canReschedule: false),
          iptal: false,
        ),
      ),
    ),
    deps: TestDeps.new,
  );

  screenGoldenTest(
    'İptal',
    fileName: 'iptal',
    screen: () => Scaffold(
      body: Align(
        alignment: Alignment.bottomCenter,
        child: IptalSayfasi(
          randevu: detay(
            deposit: kapora(DepositStatus.received, instructions: false),
          ),
        ),
      ),
    ),
    deps: TestDeps.new,
  );

  screenGoldenTest(
    'Geçmiş',
    fileName: 'gecmis',
    screen: () => const GecmisEkrani(),
    deps: () => TestDeps(
      appointments: FakeAppointmentsRepository()
        ..pastPages = {
          (false, null): MeAppointmentPage(
            items: gecmisListesi(),
            nextCursor: null,
          ),
        },
    ),
  );

  screenGoldenTest(
    'Geçmiş boş',
    fileName: 'gecmis_bos',
    screen: () => const GecmisEkrani(),
    deps: () => TestDeps(appointments: FakeAppointmentsRepository()),
  );

  screenGoldenTest(
    'Değerlendir',
    fileName: 'degerlendir',
    screen: () => const DegerlendirEkrani(appointmentId: 'g1'),
    deps: () => TestDeps(
      appointments: FakeAppointmentsRepository()..review = degerlendirme(),
    ),
  );
}
