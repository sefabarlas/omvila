import 'dart:convert';

import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_ui/api/api_date.dart';

// Kural: sözleşmede `format: date` alanı "yyyy-MM-dd" yazılır (NodaTime
// LocalDate, Prism doğrulaması).
void main() {
  test('üretilmiş istemcinin gövdesinde yalnız tarih yazılır', () {
    final body = jsonDecode(
      jsonEncode(
        DaySearchRequest(
          items: [AppointmentItemRequest(serviceId: 's1')],
          from: ApiDate('2026-11-24'),
          to: ApiDate('2026-12-24'),
        ),
      ),
    ) as Map<String, dynamic>;
    expect(body['from'], '2026-11-24');
    expect(body['to'], '2026-12-24');
  });

  test('aynı gün, UTC', () {
    final d = ApiDate('2026-02-03');
    expect(d, DateTime.utc(2026, 2, 3));
    expect(localDateOf(d), '2026-02-03');
    expect(() => ApiDate('2026-02-30'), throwsRangeError);
  });

  test('yanıttaki tarih yerel güne çevrilir', () {
    expect(localDateOf(DateTime.parse('2026-11-26')), '2026-11-26');
  });

  test('sorgu parametresinde (toString) de yalnız tarih yazılır', () {
    expect(ApiDate('2026-11-25').toString(), '2026-11-25');
  });
}
