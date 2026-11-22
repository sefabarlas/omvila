import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_ui/format.dart';

// Kural: docs/urun/urun-kurallari.md § Biçimler — örnekler birebir;
// packages/ui-web/src/format testleriyle aynı girdiler ve çıktılar.
void main() {
  group('formatMoney', () {
    for (final (minor, expected) in [
      (125000, '1.250\u00a0₺'),
      (8550, '85,50\u00a0₺'),
      (45000, '450\u00a0₺'),
      (0, '0\u00a0₺'),
      (123456789, '1.234.567,89\u00a0₺'),
      (5, '0,05\u00a0₺'),
      (-8550, '-85,50\u00a0₺'),
    ]) {
      test('$minor kuruş → $expected', () {
        expect(formatMoney(minor), expected);
      });
    }

    // "600 ₺" satır sonunda bölünmez (Dilim 5 Geçmiş; ui-web ile aynı kural).
    test('tutar ile sembol arasında bölünmez boşluk', () {
      expect(formatMoney(60000), '600\u00a0₺');
      expect(formatMoney(60000), isNot(contains(' ')));
      expect(moneyInputValue(60000), '600');
      expect(parseMoneyInput(formatMoney(8550)), 8550);
    });

    test('fiyat gösterimi', () {
      expect(formatPrice(45000), '450\u00a0₺');
      expect(
        formatPrice(280000, display: PriceDisplay.startingFrom),
        "2.800\u00a0₺'den",
      );
    });
  });

  group('parseMoneyInput', () {
    for (final (input, expected) in [
      ('450', 45000),
      ('1.250', 125000),
      ('85,50', 8550),
      ('1.250,5\u00a0₺', 125050),
      ('2800', 280000),
      ('0', 0),
    ]) {
      test('$input → $expected', () {
        expect(parseMoneyInput(input), expected);
      });
    }

    for (final input in ['', 'abc', '-5', '12,345', '1.25', '1,2,3']) {
      test('geçersiz: "$input"', () {
        expect(parseMoneyInput(input), isNull);
      });
    }

    test('düzenleme değeri geri çevrilir', () {
      expect(moneyInputValue(125000), '1.250');
      expect(moneyInputValue(8550), '85,50');
      expect(parseMoneyInput(moneyInputValue(8550)), 8550);
    });
  });

  test('formatCount binlik nokta', () {
    expect(formatCount(1240), '1.240');
    expect(formatCount(862), '862');
    expect(formatCount(0), '0');
    expect(formatCount(1250000), '1.250.000');
  });

  group('formatDate', () {
    // 21 Kasım 2025 Cuma.
    for (final (style, expected) in [
      (DateStyle.long, '21 Kasım Cuma'),
      (DateStyle.short, '21 Kas Cum'),
      (DateStyle.numeric, '21.11.2025'),
      (DateStyle.weekdayShort, 'Cum'),
    ]) {
      test('${style.name}: $expected', () {
        expect(formatDate('2025-11-21', style), expected);
      });
    }

    // 26 Kasım 2026 Perşembe, 27 Kasım 2026 Cuma, 28 Kasım 2026 Cumartesi.
    for (final (style, date, expected) in [
      (DateStyle.compact, '2026-11-27', 'Cum 27 Kas'),
      (DateStyle.weekdayDayMonth, '2026-11-27', 'Cuma 27 Kasım'),
      (DateStyle.monthYear, '2026-11-27', 'Kasım 2026'),
      (DateStyle.weekday, '2026-11-26', 'Perşembe'),
      (DateStyle.weekdayShort, '2026-11-28', 'Cmt'),
      (DateStyle.day, '2026-11-05', '5'),
      (DateStyle.dayMonth, '2026-12-31', '31 Aralık'),
      (DateStyle.panel, '2026-11-24', 'Salı, 24 Kasım'),
      (DateStyle.panel, '2026-11-25', 'Çarşamba, 25 Kasım'),
    ]) {
      test('${style.name} $date: $expected', () {
        expect(formatDate(date, style), expected);
      });
    }

    test('geçersiz tarih hata', () {
      expect(() => formatDate('2026-02-30'), throwsRangeError);
      expect(() => formatDate('2026-2-3'), throwsRangeError);
    });
  });

  test('formatDuration', () {
    expect(formatDuration(45), '45 dk');
    expect(formatDuration(90), '1 sa 30 dk');
    expect(formatDuration(120), '2 sa');
    expect(() => formatDuration(-1), throwsRangeError);
  });

  group('telefon', () {
    test('gösterim ve maske', () {
      expect(formatPhone('+905321234567'), '0532 123 45 67');
      expect(maskPhone('+905321234567'), '0532 *** ** 67');
      expect(formatPhone('+4930123456'), '+4930123456');
    });

    for (final input in [
      '0532 418 77 05',
      '532 418 77 05',
      '+90 532 418 77 05',
      '(0532) 418-77-05',
    ]) {
      test('$input → E.164', () {
        expect(parseMobilePhone(input), '+905324187705');
      });
    }

    for (final input in ['0212 555 12 34', '0532 418 77', 'abc']) {
      test('$input → null', () {
        expect(parseMobilePhone(input), isNull);
      });
    }
  });

  group('IBAN', () {
    test('dörtlü gruplar, kopyalanan değer boşluksuz', () {
      expect(
        formatIban('TR120006400000112345678901'),
        'TR12 0006 4000 0011 2345 6789 01',
      );
      expect(
        normalizeIban('tr12 0006 4000 0011 2345 6789 01'),
        'TR120006400000112345678901',
      );
    });

    for (final iban in [
      'TR29 0006 4000 0011 2345 6789 01',
      'TR290006400000112345678901',
      'tr33 0006 1005 1978 6457 8413 26',
    ]) {
      test('geçerli: $iban', () => expect(isValidIban(iban), isTrue));
    }

    for (final iban in [
      'TR12 0006 4000 0011 2345 6789 01',
      'TR29 0006 4000 0011 2345 6789 0',
      'TR29 0006 4000 0011 2345 6789 012',
      'DE89 3704 0044 0532 0130 00',
      'TR29 0006 4000 0011 2345 6789 0A',
      '',
    ]) {
      test('geçersiz: "$iban"', () => expect(isValidIban(iban), isFalse));
    }
  });

  test('Türkçe büyük harf ve baş harfler', () {
    expect(toUpperTr('Sıradaki'), 'SIRADAKİ');
    expect(initials('Kuaför Elif'), 'KE');
    expect(initials('ilker şahin'), 'İŞ');
  });
}
