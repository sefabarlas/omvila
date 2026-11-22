import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:integration_test/integration_test.dart';
import 'package:omvila_isletme/app/app.dart';

import 'prism.dart';

// Uçtan uca: gerçek uygulama, router ve depolar, Prism mock
// (docs/standartlar/test-stratejisi.md). Çalıştırma: mobile/isletme/AGENTS.md
// § Komutlar. Veriler sözleşme örneklerinden gelir; randevu detayının
// (manageGetAppointment) örneği olmadığı için Prism şemadan üretir, bu yüzden
// detayda yalnız ekranın yapısı ve örnekli alanlar denetlenir.
void main() {
  IntegrationTestWidgetsFlutterBinding.ensureInitialized();

  testWidgets('giriş → Bugün → randevu detayı', (tester) async {
    usePhone(tester);
    await tester.pumpWidget(
      ProviderScope(
        overrides: prismOverrides(),
        retry: (_, _) => null,
        child: const IsletmeApp(),
      ),
    );
    await settleNetwork(tester, until: find.text('Kod gönder'));

    // Giriş: telefon → SMS kodu (manageRequestOtp, manageVerifyOtp,
    // manageGetSession); kod altı hane olunca kendiliğinden gönderilir.
    await tester.enterText(find.byType(TextField), '05550001234');
    await tester.pump();
    await tapText(tester, 'Kod gönder', expect: 'Kodu girin');
    await tester.enterText(find.byType(TextField).first, '123456');
    await settleNetwork(tester, until: find.text('Şimdi değil'));
    expect(find.text('Talepleri anında görün'), findsOneWidget);

    // Bugün (manageGetToday).
    await tapText(tester, 'Şimdi değil', expect: 'Gün akışı');
    expect(find.text('Çarşamba 25 Kasım · Kadıköy'), findsOneWidget);
    expect(find.text('SIRADAKİ'), findsOneWidget);
    expect(find.text('Derya Şahin'), findsOneWidget);
    expect(
      find.text('Bildirimler kapalı. Yeni randevuları kaçırabilirsiniz.'),
      findsOneWidget,
    );

    // Randevu detayı (manageGetAppointment).
    await tapText(tester, 'Selin Öztürk', expect: 'Hizmetler');
    expect(find.text('Randevu'), findsOneWidget);
    expect(find.text('Toplam'), findsOneWidget);
    expect(find.text('Pınar Yalçın'), findsOneWidget);
  });
}
