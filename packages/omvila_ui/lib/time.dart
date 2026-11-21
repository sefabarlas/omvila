// Şube saat dilimiyle zaman (AGENTS.md § 5; packages/ui-web/src/time ile aynı
// kurallar). "Şimdi" yalnız enjekte edilen saatten (Clock) alınır; takvim günü
// ve saat şubenin timeZone değerine göre hesaplanır, cihaz saat dilimi
// kullanılmaz. API'den gelen DateTime UTC'dir.
import 'package:timezone/data/latest.dart' as tzdata;
import 'package:timezone/timezone.dart' as tz;

/// Enjekte edilen saat (backend IClock karşılığı; testte FixedClock).
// Arayüz bilerek: uygulamalar Riverpod ile kendi saatini verebilir.
// ignore: one_member_abstracts
abstract interface class Clock {
  /// Şimdiki an (UTC).
  DateTime now();
}

/// Gerçek saat; uygulama kökünde bir kez verilir.
final class SystemClock implements Clock {
  /// Gerçek saat.
  const SystemClock();

  @override
  DateTime now() => DateTime.now().toUtc();
}

/// Sabit saat (test ve önizleme).
final class FixedClock implements Clock {
  /// [instant] UTC'ye çevrilerek saklanır.
  FixedClock(DateTime instant) : _instant = instant.toUtc();

  /// ISO 8601 metinden: `FixedClock.parse('2026-11-25T09:00:00Z')`.
  FixedClock.parse(String instantUtc) : this(DateTime.parse(instantUtc));

  final DateTime _instant;

  @override
  DateTime now() => _instant;
}

var _initialized = false;

tz.Location _location(String timeZone) {
  if (!_initialized) {
    tzdata.initializeTimeZones();
    _initialized = true;
  }

  return tz.getLocation(timeZone);
}

/// Anın şubenin saat dilimindeki duvar saati.
tz.TZDateTime toBranchTime(DateTime instant, String timeZone) =>
    tz.TZDateTime.from(instant.toUtc(), _location(timeZone));

String _two(int n) => n.toString().padLeft(2, '0');

/// Anın şubedeki takvim günü: "2026-11-25".
String toBranchDate(DateTime instant, String timeZone) {
  final t = toBranchTime(instant, timeZone);
  return '${t.year.toString().padLeft(4, '0')}-${_two(t.month)}-${_two(t.day)}';
}

/// Şubede bugün.
String branchToday(Clock clock, String timeZone) =>
    toBranchDate(clock.now(), timeZone);

/// Şubedeki saat (0–23); selamlama gibi gün içi metinler için.
int branchHour(Clock clock, String timeZone) =>
    toBranchTime(clock.now(), timeZone).hour;

/// Anın şubedeki saati, 24 saat: "09:30".
String formatTime(DateTime instant, String timeZone) {
  final t = toBranchTime(instant, timeZone);
  return '${_two(t.hour)}:${_two(t.minute)}';
}

/// Duvar saati, gece yarısından dakika (yaz saati geçişinde 10:00 = 600).
int localMinutesOfDay(DateTime instant, String timeZone) {
  final t = toBranchTime(instant, timeZone);
  return t.hour * 60 + t.minute;
}

/// Şubenin yerel tarih ve saatini UTC ana çevirir:
/// ("2026-12-02", "14:00", "Europe/Istanbul") → 2026-12-02T11:00:00Z.
DateTime localDateTimeToUtc(
  String localDate,
  String localTime,
  String timeZone,
) {
  final [y, m, d] = localDate.split('-').map(int.parse).toList();
  final minutes = parseClock(localTime);
  return tz.TZDateTime(
    _location(timeZone),
    y,
    m,
    d,
    minutes ~/ 60,
    minutes % 60,
  ).toUtc();
}

/// "09:30" → 570.
int parseClock(String localTime) {
  final match = RegExp(r'^(\d{2}):(\d{2})$').firstMatch(localTime);
  final hours = int.tryParse(match?.group(1) ?? '');
  final minutes = int.tryParse(match?.group(2) ?? '');
  if (hours == null ||
      minutes == null ||
      hours > 24 ||
      minutes > 59 ||
      (hours == 24 && minutes > 0)) {
    throw RangeError('Saat ss:dd olmalı: $localTime');
  }

  return hours * 60 + minutes;
}

/// 570 → "09:30".
String formatClock(int minutesOfDay) {
  if (minutesOfDay < 0 || minutesOfDay > 24 * 60) {
    throw RangeError('Dakika 0–1440 arası tamsayı olmalı.');
  }

  return '${_two(minutesOfDay ~/ 60)}:${_two(minutesOfDay % 60)}';
}

DateTime _utcDate(String localDate) {
  final [y, m, d] = localDate.split('-').map(int.parse).toList();
  return DateTime.utc(y, m, d);
}

/// Takvim gününe gün ekler: ("2026-11-30", 1) → "2026-12-01".
String addDays(String localDate, int days) {
  final date = _utcDate(localDate).add(Duration(days: days));
  return '${date.year.toString().padLeft(4, '0')}-'
      '${_two(date.month)}-${_two(date.day)}';
}

/// İki takvim günü arasındaki gün farkı (b - a).
int daysBetween(String a, String b) =>
    _utcDate(b).difference(_utcDate(a)).inDays;
