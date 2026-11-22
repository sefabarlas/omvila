import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:integration_test/integration_test.dart';
import 'package:omvila_musteri/app/app.dart';

import 'prism.dart';

// Uçtan uca: gerçek uygulama, gerçek router ve depolar, Prism mock
// (docs/standartlar/test-stratejisi.md). Çalıştırma: mobile/musteri/AGENTS.md
// § Komutlar. Ekranlardaki veriler sözleşme örneklerinden gelir (Merve,
// Kuaför Elif).
void main() {
  IntegrationTestWidgetsFlutterBinding.ensureInitialized();

  testWidgets('giriş → randevu al → randevu detayı', (tester) async {
    usePhone(tester);
    final stub = CatalogStub();
    await tester.pumpWidget(
      ProviderScope(
        overrides: prismOverrides(stub),
        retry: (_, _) => null,
        child: const OmvilaMusteriApp(),
      ),
    );
    await settleNetwork(tester, until: find.text('Kod gönder'));

    // Giriş: telefon → SMS kodu (meRequestOtp, meVerifyOtp).
    await tester.enterText(find.byType(TextField), '05550001234');
    await tester.pump();
    await tapText(tester, 'Kod gönder', expect: 'Kodu girin');
    await tester.enterText(find.byType(TextField).first, '123456');
    await settleNetwork(tester, until: find.text('Şimdi değil'));
    expect(find.text('Randevunuzu kaçırmayın'), findsOneWidget);
    await tapText(tester, 'Şimdi değil', expect: 'Merhaba Merve');
    expect(
      find.text('Bildirimler kapalı. Hatırlatmalar SMS ile gelecek.'),
      findsOneWidget,
    );

    // Randevu al: salon (meListBrands) → hizmet → kişi → saat → onay
    // (publicCreateAppointment, müşteri jetonuyla).
    await tapText(tester, 'Yeni randevu al', expect: 'Hangi salon?');
    await tapText(tester, 'Kuaför Elif', expect: 'Saç kesimi');
    expect(find.text('Hizmet seçin'), findsOneWidget);
    await tapText(tester, 'Saç kesimi');
    await tapText(tester, 'Devam et', expect: 'Kimden olsun?');
    await tapText(tester, 'Elif Kaya');
    await tapText(tester, 'Devam et', expect: 'Ne zaman?');
    await tapText(tester, '14:30');
    await tapText(tester, 'Devam et', expect: 'Son kontrol');
    expect(find.text('ÇARŞAMBA 25 KASIM'), findsOneWidget);
    expect(find.text('14:30–15:15'), findsOneWidget);
    await tapText(
      tester,
      'Randevuyu oluştur',
      expect: 'Randevularıma dön',
    );
    expect(find.text('Randevunuz onaylandı'), findsOneWidget);
    expect(stub.answered, hasLength(3));

    // Randevularım → randevu detayı (meGetAppointment).
    await tapText(tester, 'Randevularıma dön', expect: 'Merhaba Merve');
    await tapText(tester, '14:00', expect: 'Randevuyu iptal et');
    expect(find.text('26 KASIM PERŞEMBE'), findsOneWidget);
    expect(find.text('Moda Cad. 42, Kadıköy'), findsOneWidget);
    expect(find.text('Saati değiştir'), findsOneWidget);
  });
}
