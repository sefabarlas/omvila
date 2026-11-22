/// API'nin `date` alanı → "2026-11-25" (bileşenler olduğu gibi alınır).
///
/// İsteklerde `format: date` alanlarına `DateTime.parse("2026-11-25")`
/// verilir; üretilmiş istemci günü "yyyy-MM-dd" yazar (T-110).
String localDateOf(DateTime date) =>
    '${date.year.toString().padLeft(4, '0')}-'
    '${date.month.toString().padLeft(2, '0')}-'
    '${date.day.toString().padLeft(2, '0')}';
