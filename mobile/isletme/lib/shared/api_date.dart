/// Sözleşmede `format: date` olan alanlar ve sorgu parametreleri. Üretilmiş
/// Dart istemcisi bunları `DateTime` olarak tipler; gövdede
/// `toIso8601String()` ("2026-11-25T00:00:00.000Z"), sorguda `toString()`
/// gönderir, sunucu ise yalnız "2026-11-25" kabul eder. Bu tip ikisini de
/// yerel gün olarak yazar (docs/acik-sorular.md T-109).
class ApiDate extends DateTime {
  /// "2026-11-25" → gün (saat dilimi taşımaz).
  ApiDate(String localDate)
    : super.utc(
        int.parse(localDate.substring(0, 4)),
        int.parse(localDate.substring(5, 7)),
        int.parse(localDate.substring(8, 10)),
      );

  @override
  String toIso8601String() =>
      '${year.toString().padLeft(4, '0')}-'
      '${month.toString().padLeft(2, '0')}-'
      '${day.toString().padLeft(2, '0')}';

  @override
  String toString() => toIso8601String();
}

/// API'nin `date` alanı → "2026-11-25" (bileşenler olduğu gibi alınır).
String localDateOf(DateTime date) =>
    '${date.year.toString().padLeft(4, '0')}-'
    '${date.month.toString().padLeft(2, '0')}-'
    '${date.day.toString().padLeft(2, '0')}';
