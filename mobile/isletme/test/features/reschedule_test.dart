import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_isletme/features/push/data/push_service.dart';
import 'package:omvila_ui/omvila_ui.dart';

import '../helpers/appointment_fixtures.dart';
import '../helpers/fake_api.dart';
import '../helpers/fixtures.dart';
import 'today_screen_test.dart' show tapCentered;

// Kural: manageRescheduleAppointment (If-Match; personel değişmezse items
// gönderilmez; slot_taken'da saat sıfırlanır), RandevuIslem "Saati veya
// personeli değiştir".
void main() {
  const slotsPath = '/api/v1/manage/branches/$branchId/availability/slots';
  const reschedulePath = '/api/v1/manage/appointments/$confirmedId/reschedule';

  Future<Harness> open(WidgetTester tester) async {
    final h = Harness()..signIn();
    h.api
      ..reply('GET', '/api/v1/manage/today', todayJson())
      ..reply(
        'GET',
        '/api/v1/manage/appointments/$confirmedId',
        confirmedJson(),
      )
      ..reply('GET', '/api/v1/manage/staff', staffJson())
      ..on('POST', slotsPath, (r) {
        final items = (r.body! as Map)['items'] as List;
        final staffId = (items.first as Map)['staffId'] as String?;
        return FakeResponse(200, slotsJson(staffId: staffId ?? elifId));
      });
    await h.pumpApp(tester);
    h.push.simulateOpen(const AppointmentTarget(confirmedId));
    await tester.pumpAndSettle();
    await tester.tap(find.byTooltip('Diğer işlemler'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Saati veya personeli değiştir'));
    await tester.pumpAndSettle();
    return h;
  }

  testWidgets('aynı personelle yeni saat', (tester) async {
    final h = await open(tester);
    h.api.reply('POST', reschedulePath, confirmedJson());
    expect(find.text('Derya Şahin'), findsOneWidget);
    await tapCentered(tester, find.bySemanticsLabel('Perşembe 26 Kasım'));
    await tester.pumpAndSettle();
    await tapCentered(tester, find.bySemanticsLabel('14:00'));
    await tester.pumpAndSettle();
    await tapCentered(tester, find.text('Saati değiştir'));
    await tester.pumpAndSettle();
    final post = h.api.to('POST', reschedulePath).single;
    expect(post.header('If-Match'), 'v5');
    expect(post.body, {
      'startUtc': '2026-11-26T11:00:00.000Z',
      'notifyCustomer': true,
      'allowOutsideWorkingHours': false,
    });
    expect(find.text('Randevu saati değişti.'), findsOneWidget);
  });

  testWidgets('personel değişince kalemler yeni personelle', (tester) async {
    final h = await open(tester);
    h.api.reply('POST', reschedulePath, confirmedJson());
    await tapCentered(tester, find.bySemanticsLabel('Zeynep'));
    await tester.pumpAndSettle();
    await tapCentered(tester, find.bySemanticsLabel('Perşembe 26 Kasım'));
    await tester.pumpAndSettle();
    await tapCentered(tester, find.bySemanticsLabel('13:15'));
    await tester.pumpAndSettle();
    await tapCentered(tester, find.text('Saati değiştir'));
    await tester.pumpAndSettle();
    final body = h.api.to('POST', reschedulePath).single.body! as Map;
    expect(body['items'], [
      {'itemId': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5c24', 'staffId': zeynepId},
    ]);
  });

  testWidgets('slot_taken: mesaj', (tester) async {
    final h = await open(tester);
    h.api.reply(
      'POST',
      reschedulePath,
      {'code': 'slot_taken'},
      status: 409,
    );
    await tapCentered(tester, find.bySemanticsLabel('13:15'));
    await tester.pumpAndSettle();
    await tapCentered(tester, find.text('Saati değiştir'));
    await tester.pumpAndSettle();
    expect(find.text(errorMessage('slot_taken')), findsOneWidget);
  });
}
