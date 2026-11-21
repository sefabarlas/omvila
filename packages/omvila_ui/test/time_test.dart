import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_ui/time.dart';

// Kural: AGENTS.md § 5 — saklama UTC, gösterim şubenin saat diliminde, "şimdi"
// enjekte saatten. packages/ui-web/src/time testleriyle aynı örnekler.
void main() {
  test('duvar saati dakikası şubenin saat diliminde', () {
    expect(
      localMinutesOfDay(
        DateTime.parse('2026-11-25T07:30:00Z'),
        'Europe/Istanbul',
      ),
      630,
    );
    expect(
      localMinutesOfDay(
        DateTime.parse('2026-11-25T21:30:00Z'),
        'Europe/Istanbul',
      ),
      30,
    );
    // Berlin yaz saati geçiş günü: 10:00 CEST = 08:00Z, bir gün önce 09:00Z.
    expect(
      localMinutesOfDay(
        DateTime.parse('2026-03-29T08:00:00Z'),
        'Europe/Berlin',
      ),
      600,
    );
    expect(
      localMinutesOfDay(
        DateTime.parse('2026-03-28T09:00:00Z'),
        'Europe/Berlin',
      ),
      600,
    );
  });

  test('saat metni', () {
    expect(
      formatTime(DateTime.parse('2026-11-25T06:30:00Z'), 'Europe/Istanbul'),
      '09:30',
    );
    expect(
      formatTime(DateTime.parse('2026-11-25T21:05:00Z'), 'Europe/Istanbul'),
      '00:05',
    );
    expect(parseClock('09:30'), 570);
    expect(parseClock('24:00'), 1440);
    expect(() => parseClock('9:30'), throwsRangeError);
    expect(() => parseClock('10:75'), throwsRangeError);
    expect(formatClock(570), '09:30');
    expect(formatClock(0), '00:00');
    expect(() => formatClock(-15), throwsRangeError);
  });

  test('takvim günü şubenin saat dilimine göre', () {
    final late = DateTime.parse('2026-11-25T21:30:00Z');
    expect(toBranchDate(late, 'Europe/Istanbul'), '2026-11-26');
    expect(toBranchDate(late, 'UTC'), '2026-11-25');
    expect(
      branchToday(FixedClock.parse('2026-11-25T09:00:00Z'), 'Europe/Istanbul'),
      '2026-11-25',
    );
    expect(
      branchHour(FixedClock.parse('2026-11-25T21:30:00Z'), 'Europe/Istanbul'),
      0,
    );
  });

  test('gün ekleme ve fark', () {
    expect(addDays('2026-11-30', 1), '2026-12-01');
    expect(addDays('2026-03-28', 1), '2026-03-29');
    expect(addDays('2026-01-01', -1), '2025-12-31');
    expect(daysBetween('2026-11-25', '2026-12-09'), 14);
    expect(daysBetween('2026-03-28', '2026-03-30'), 2);
    expect(daysBetween('2026-12-09', '2026-11-25'), -14);
  });

  test('şube yerel saati UTC ana çevrilir', () {
    expect(
      localDateTimeToUtc('2026-12-02', '14:00', 'Europe/Istanbul'),
      DateTime.utc(2026, 12, 2, 11),
    );
    expect(
      localDateTimeToUtc('2026-07-01', '09:30', 'Europe/Berlin'),
      DateTime.utc(2026, 7, 1, 7, 30),
    );
    expect(
      localDateTimeToUtc('2026-01-15', '09:30', 'Europe/Berlin'),
      DateTime.utc(2026, 1, 15, 8, 30),
    );
  });

  test('sabit saat UTC döner', () {
    final clock = FixedClock(DateTime.utc(2026, 11, 25, 9));
    expect(clock.now().isUtc, isTrue);
    expect(const SystemClock().now().isUtc, isTrue);
  });
}
