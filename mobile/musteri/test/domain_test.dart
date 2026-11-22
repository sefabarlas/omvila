import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/config.dart';
import 'package:omvila_musteri/features/appointments/domain/randevu_metinleri.dart';
import 'package:omvila_musteri/features/auth/data/customer_session.dart';
import 'package:omvila_musteri/features/auth/presentation/giris_ekrani.dart';
import 'package:omvila_musteri/features/brands/domain/brand_link.dart';
import 'package:omvila_musteri/shared/external_actions.dart';
import 'package:omvila_ui/omvila_ui.dart';

import 'helpers/fakes.dart';
import 'helpers/fixtures.dart';

void main() {
  group('salon bağlantısı', () {
    const hosts = AppConfig.defaultLinkHosts;

    test('yapıştırılan bağlantıdan ad çıkar', () {
      expect(
        brandSlugFromText('https://randevu.omvila.com.tr/kuafor-elif', hosts),
        'kuafor-elif',
      );
      expect(
        brandSlugFromText('omvila.com/Kuafor-Elif/', hosts),
        'kuafor-elif',
      );
      expect(
        brandSlugFromText(
          "Kuaför Elif'ten randevu: www.omvila.com.tr/kuafor-elif",
          hosts,
        ),
        'kuafor-elif',
      );
    });

    test('kişiye özel, yabancı ve bozuk bağlantı salon değildir', () {
      expect(brandSlugFromText('omvila.com/r/ABC123', hosts), isNull);
      expect(brandSlugFromText('omvila.com/r', hosts), isNull);
      expect(brandSlugFromText('https://ornek.com/kuafor-elif', hosts), isNull);
      expect(brandSlugFromText('omvila.com/', hosts), isNull);
      expect(brandSlugFromText('omvila.com/-elif', hosts), isNull);
      expect(brandSlugFromText('merhaba', hosts), isNull);
    });

    test('uygulama içi adres', () {
      expect(isBrandSlug('kuafor-elif'), isTrue);
      expect(isBrandSlug('d'), isFalse);
      expect(isBrandSlug('Kuafor'), isFalse);
    });
  });

  group('randevu metinleri', () {
    test('gün etiketi şubenin saat dilimiyle', () {
      // Şimdi 24 Kasım 20:30 İstanbul.
      final clock = testClock;
      String day(String utc) =>
          gunEtiketi(DateTime.parse(utc), 'Europe/Istanbul', clock);
      expect(day('2026-11-24T20:00:00Z'), 'Bugün'); // 23:00 yerel
      expect(day('2026-11-24T21:30:00Z'), 'Yarın'); // 00:30 yerel
      expect(day('2026-11-28T08:00:00Z'), 'Cmt 28 Kas');
    });

    test('kalan süre', () {
      String? left(String utc) =>
          kalanSureEtiketi(DateTime.parse(utc), testClock);
      expect(left('2026-11-24T18:15:00Z'), '45 dk sonra');
      expect(left('2026-11-25T11:30:00Z'), '18 saat sonra');
      expect(left('2026-11-28T11:30:00Z'), '3 gün sonra');
      expect(left('2026-11-24T17:00:00Z'), isNull);
    });

    test('hizmet ve personel', () {
      expect(hizmetlerMetni(yarinKuaforElif.items), 'Saç kesimi, Fön');
      expect(personelMetni(yarinKuaforElif.items), 'Elif Kaya');
      expect(ozetSatiri(['Manikür', '', 'Kadıköy']), 'Manikür · Kadıköy');
    });

    test('bulunma eki', () {
      expect(bulunmaGecmis('Kuaför Elif'), "Kuaför Elif'teydi");
      expect(bulunmaGecmis('Spa Moda'), "Spa Moda'daydı");
      expect(bulunmaGecmis('Nail Studio Moda'), "Nail Studio Moda'daydı");
      expect(bulunmaGecmis('Diyetisyen Ece'), "Diyetisyen Ece'deydi");
      expect(bulunma('12 Kasım'), "12 Kasım'da");
      expect(bulunma('3 Eylül'), "3 Eylül'de");
    });
  });

  test('telefon alanı biçimi', () {
    final formatter = PhoneInputFormatter();
    String format(String input) => formatter
        .formatEditUpdate(TextEditingValue.empty, TextEditingValue(text: input))
        .text;
    expect(format('05326112467'), '532 611 24 67');
    expect(format('+90 532 611 24 67'), '532 611 24 67');
    expect(format('532611246799'), '532 611 24 67');
    expect(format('53a2'), '532');
  });

  group('yol tarifi', () {
    test('konum varsa koordinat; iOS Apple Haritalar', () {
      expect(
        directionsUrl(
          address: 'Moda Caddesi 10, Kadıköy',
          latitude: 40.98,
          longitude: 29.02,
          platform: TargetPlatform.iOS,
        ).toString(),
        'https://maps.apple.com/?daddr=40.98%2C29.02',
      );
    });

    test('konum yoksa adres; Android Google Haritalar', () {
      final url = directionsUrl(
        address: 'Moda Caddesi 10, Kadıköy',
        platform: TargetPlatform.android,
      );
      expect(url.host, 'www.google.com');
      expect(url.queryParameters['destination'], 'Moda Caddesi 10, Kadıköy');
    });
  });

  group('oturum', () {
    DioException rejected(int status) => DioException(
      requestOptions: RequestOptions(),
      response: Response(requestOptions: RequestOptions(), statusCode: status),
    );

    test('yenileme yeni jetonları saklar', () async {
      final store = MemoryKeyValueStore(signedInStore());
      final session = CustomerSession(
        store: store,
        refresher: (token) async {
          expect(token, 'yenileme');
          return TokenPair(
            accessToken: 'yeni-erisim',
            expiresInSeconds: 900,
            refreshToken: 'yeni-yenileme',
          );
        },
      );
      expect(await session.load(), isTrue);
      expect(await session.refresh(), isTrue);
      expect(await session.accessToken(), 'yeni-erisim');
      expect(store.values['me.refreshToken'], 'yeni-yenileme');
    });

    test('reddedilen yenileme oturumu kapatır', () async {
      final store = MemoryKeyValueStore(signedInStore());
      var signedOut = false;
      final session = CustomerSession(
        store: store,
        refresher: (_) => Future.error(rejected(401)),
      )..onSignedOut = () => signedOut = true;
      await session.load();
      expect(await session.refresh(), isFalse);
      expect(signedOut, isTrue);
      expect(session.isSignedIn, isFalse);
      expect(store.values.containsKey('me.refreshToken'), isFalse);
    });

    test('bağlantı yoksa oturum korunur', () async {
      final store = MemoryKeyValueStore(signedInStore());
      final session = CustomerSession(
        store: store,
        refresher: (_) => Future.error(
          DioException.connectionError(
            requestOptions: RequestOptions(),
            reason: 'yok',
          ),
        ),
      );
      await session.load();
      expect(await session.refresh(), isFalse);
      expect(session.isSignedIn, isTrue);
    });
  });

  test('ApiProblem bağlantı hatası', () {
    final problem = ApiProblem.fromDioException(
      DioException.connectionError(
        requestOptions: RequestOptions(),
        reason: '',
      ),
    );
    expect(problem.isNetworkError, isTrue);
  });
}
