import 'package:omvila_ui/format.dart';

/// Sözleşmedeki `format: date` alanı için değer ("2026-11-26").
///
/// Üretilmiş Dart istemcisi (openapi-generator dart-dio, json_serializable)
/// bu alanları `DateTime` olarak üretir ve `toIso8601String()` ile tam an
/// olarak yazar ("2026-11-26T00:00:00.000Z"); sunucu (NodaTime `LocalDate`)
/// ve Prism yalnız "yyyy-MM-dd" kabul eder. [ApiDate] aynı günü taşır ve
/// JSON'a yalnız tarih olarak yazılır. Üretici ayarı düzelince kaldırılabilir.
final class ApiDate extends DateTime {
  /// [localDate] yerel takvim günü ("2026-11-26").
  factory ApiDate(String localDate) {
    final d = parseLocalDate(localDate);
    return ApiDate._(d.year, d.month, d.day);
  }

  ApiDate._(super.year, super.month, super.day) : super.utc();

  @override
  String toIso8601String() =>
      '${year.toString().padLeft(4, '0')}-'
      '${month.toString().padLeft(2, '0')}-'
      '${day.toString().padLeft(2, '0')}';
}

/// Yanıttaki `format: date` değerini ("2026-11-26" → DateTime) yerel güne
/// çevirir.
String localDateOf(DateTime date) =>
    '${date.year.toString().padLeft(4, '0')}-'
    '${date.month.toString().padLeft(2, '0')}-'
    '${date.day.toString().padLeft(2, '0')}';
