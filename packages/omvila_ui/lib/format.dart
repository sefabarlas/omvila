// Biçimlendirme (docs/urun/urun-kurallari.md § Biçimler). Kurallar ve örnekler
// packages/ui-web/src/format ile birebir aynıdır; ekranlarda intl doğrudan
// çağrılmaz. Para kuruş cinsinden tamsayıdır; ondalıklı sayıyla hesap yok.

const _symbols = <String, String>{'TRY': '₺'};

String _group(BigInt integer) => integer.toString().replaceAllMapped(
  RegExp(r'\B(?=(\d{3})+(?!\d))'),
  (_) => '.',
);

/// "1.250 ₺"; kuruş varsa "85,50 ₺".
String formatMoney(int amountMinor, {String currency = 'TRY'}) {
  final minor = BigInt.from(amountMinor);
  final absolute = minor.abs();
  final lira = absolute ~/ BigInt.from(100);
  final kurus = absolute % BigInt.from(100);
  final amount = kurus == BigInt.zero
      ? _group(lira)
      : '${_group(lira)},${kurus.toString().padLeft(2, '0')}';
  final sign = minor.isNegative ? '-' : '';
  return '$sign$amount ${_symbols[currency] ?? currency}';
}

/// Hizmetin fiyat gösterimi (PublicCatalog.priceDisplay).
enum PriceDisplay {
  /// "450 ₺"
  fixed,

  /// "2.800 ₺'den"
  startingFrom,
}

/// Fixed "450 ₺", StartingFrom "2.800 ₺'den".
String formatPrice(
  int amountMinor, {
  String currency = 'TRY',
  PriceDisplay display = PriceDisplay.fixed,
}) {
  final amount = formatMoney(amountMinor, currency: currency);
  return display == PriceDisplay.startingFrom ? "$amount'den" : amount;
}

/// Kullanıcının yazdığı tutarı kuruşa çevirir: "450" → 45000, "1.250" →
/// 125000, "85,50" → 8550, "1.250,5 ₺" → 125050. Geçersiz veya negatifse null.
int? parseMoneyInput(String input) {
  final text = input.replaceAll(RegExp(r'₺|TL|\s', caseSensitive: false), '');
  final match = RegExp(r'^(\d{1,3}(?:\.\d{3})+|\d+)(?:,(\d{1,2}))?$')
      .firstMatch(text);
  if (match == null) {
    return null;
  }

  final lira = int.tryParse(match.group(1)!.replaceAll('.', ''));
  final kurus = int.parse((match.group(2) ?? '').padRight(2, '0'));
  if (lira == null || lira > 90071992547409) {
    return null;
  }

  return lira * 100 + kurus;
}

/// Düzenleme alanına yazılacak tutar: 45000 → "450", 8550 → "85,50".
String moneyInputValue(int amountMinor) =>
    formatMoney(amountMinor).replaceFirst(RegExp(r'\s₺$'), '');

/// Tamsayıyı binlik noktayla yazar: 1240 → "1.240", 862 → "862".
String formatCount(int value) =>
    '${value < 0 ? '-' : ''}${_group(BigInt.from(value).abs())}';

const _months = [
  'Ocak',
  'Şubat',
  'Mart',
  'Nisan',
  'Mayıs',
  'Haziran',
  'Temmuz',
  'Ağustos',
  'Eylül',
  'Ekim',
  'Kasım',
  'Aralık',
];
const _shortMonths = [
  'Oca',
  'Şub',
  'Mar',
  'Nis',
  'May',
  'Haz',
  'Tem',
  'Ağu',
  'Eyl',
  'Eki',
  'Kas',
  'Ara',
];
// DateTime.weekday sırası: 1 = Pazartesi … 7 = Pazar.
const _weekdays = [
  'Pazartesi',
  'Salı',
  'Çarşamba',
  'Perşembe',
  'Cuma',
  'Cumartesi',
  'Pazar',
];
const _shortWeekdays = ['Pzt', 'Sal', 'Çar', 'Per', 'Cum', 'Cmt', 'Paz'];

/// Takvim günü biçimleri (web DateStyle ile aynı adlar ve çıktılar).
enum DateStyle {
  /// "21 Kasım Cuma"
  long,

  /// "Salı, 25 Kasım"
  panel,

  /// "21 Kas Cum"
  short,

  /// "21.11.2026"
  numeric,

  /// "31 Aralık"
  dayMonth,

  /// "Cum 28 Kas"
  compact,

  /// "Cuma 28 Kasım"
  weekdayDayMonth,

  /// "Kasım 2026"
  monthYear,

  /// "Cuma"
  weekday,

  /// "Cum"
  weekdayShort,

  /// "28"
  day,
}

/// Yerel takvim günü ("2026-11-21"); saat dilimi taşımaz.
DateTime parseLocalDate(String localDate) {
  final match = RegExp(r'^(\d{4})-(\d{2})-(\d{2})$').firstMatch(localDate);
  if (match == null) {
    throw RangeError('Yerel tarih yyyy-aa-gg olmalı: $localDate');
  }

  final [year, month, day] = [
    1,
    2,
    3,
  ].map((i) => int.parse(match.group(i)!)).toList();
  final date = DateTime.utc(year, month, day);
  if (date.month != month || date.day != day) {
    throw RangeError('Geçersiz tarih: $localDate');
  }

  return date;
}

/// Yanıttaki `format: date` değeri (`DateTime`) → yerel gün ("2026-11-26");
/// bileşenler olduğu gibi alınır. İsteklerde `format: date` alanına
/// [parseLocalDate] verilir; üretilmiş istemci günü "yyyy-MM-dd" yazar (T-110).
String localDateOf(DateTime date) =>
    '${date.year.toString().padLeft(4, '0')}-'
    '${_two(date.month)}-${_two(date.day)}';

String _two(int n) => n.toString().padLeft(2, '0');

/// Takvim gününü Türkçe yazar (web formatDate ile aynı).
String formatDate(String localDate, [DateStyle style = DateStyle.long]) {
  final d = parseLocalDate(localDate);
  final month = _months[d.month - 1];
  final shortMonth = _shortMonths[d.month - 1];
  final weekday = _weekdays[d.weekday - 1];
  final shortWeekday = _shortWeekdays[d.weekday - 1];
  return switch (style) {
    DateStyle.long => '${d.day} $month $weekday',
    DateStyle.panel => '$weekday, ${d.day} $month',
    DateStyle.short => '${d.day} $shortMonth $shortWeekday',
    DateStyle.numeric => '${_two(d.day)}.${_two(d.month)}.${d.year}',
    DateStyle.dayMonth => '${d.day} $month',
    DateStyle.compact => '$shortWeekday ${d.day} $shortMonth',
    DateStyle.weekdayDayMonth => '$weekday ${d.day} $month',
    DateStyle.monthYear => '$month ${d.year}',
    DateStyle.weekday => weekday,
    DateStyle.weekdayShort => shortWeekday,
    DateStyle.day => '${d.day}',
  };
}

/// Süre: "45 dk", "1 sa 30 dk", "2 sa".
String formatDuration(int minutes) {
  if (minutes < 0) {
    throw RangeError('Süre negatif olmayan tamsayı dakika olmalı.');
  }

  final hours = minutes ~/ 60;
  final rest = minutes % 60;
  if (hours == 0) {
    return '$rest dk';
  }

  return rest == 0 ? '$hours sa' : '$hours sa $rest dk';
}

String? _national(String e164) =>
    RegExp(r'^\+90(\d{10})$').firstMatch(e164)?.group(1);

/// "+905321234567" → "0532 123 45 67". Türkiye dışı değer olduğu gibi döner.
String formatPhone(String e164) {
  final n = _national(e164);
  return n == null
      ? e164
      : '0${n.substring(0, 3)} ${n.substring(3, 6)} '
            '${n.substring(6, 8)} ${n.substring(8)}';
}

/// "+905321234567" → "0532 *** ** 67" (Personel rolü).
String maskPhone(String e164) {
  final n = _national(e164);
  return n == null ? e164 : '0${n.substring(0, 3)} *** ** ${n.substring(8)}';
}

/// Yazılan cep telefonunu E.164'e çevirir ("0532 418 77 05", "+90 532…").
/// Cep telefonu değilse null. Sunucu da ayrıca doğrular.
String? parseMobilePhone(String input) {
  final digits = input.replaceAll(RegExp(r'[\s()-]'), '');
  final match = RegExp(r'^(?:\+90|0090|90|0)?(5\d{9})$').firstMatch(digits);
  return match == null ? null : '+90${match.group(1)}';
}

/// Türkçe büyük harf: "i" → "İ", "ı" → "I" ("Sıradaki" → "SIRADAKİ").
String toUpperTr(String text) =>
    text.replaceAll('i', 'İ').replaceAll('ı', 'I').toUpperCase();

/// Boşlukları atar, büyük harfe çevirir: "tr12 0006…" → "TR120006…".
String normalizeIban(String input) =>
    toUpperTr(input.replaceAll(RegExp(r'\s+'), ''));

/// "TR120006400000112345678901" → "TR12 0006 4000 0011 2345 6789 01".
String formatIban(String iban) => normalizeIban(
  iban,
).replaceAllMapped(RegExp('(.{4})(?=.)'), (m) => '${m.group(1)} ');

/// Türkiye IBAN'ı mı: TR + 24 hane ve ISO 13616 mod-97 denetimi 1.
bool isValidIban(String input) {
  final iban = normalizeIban(input);
  if (!RegExp(r'^TR\d{24}$').hasMatch(iban)) {
    return false;
  }

  // İlk dört karakter sona alınır, harfler sayıya çevrilir (T=29, R=27).
  final rearranged = '${iban.substring(4)}2927${iban.substring(2, 4)}';
  var remainder = 0;
  for (final unit in rearranged.codeUnits) {
    remainder = (remainder * 10 + unit - 48) % 97;
  }

  return remainder == 1;
}

/// Baş harfler Türkçe büyük harfle, en çok iki: "Kuaför Elif" → "KE".
String initials(String name) => name
    .trim()
    .split(RegExp(r'\s+'))
    .where((part) => part.isNotEmpty)
    .take(2)
    .map((part) => toUpperTr(part.substring(0, 1)))
    .join();
