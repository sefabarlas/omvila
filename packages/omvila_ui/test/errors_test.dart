import 'dart:io';

import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_ui/errors/tr.dart';

// Kural: packages/omvila_ui/AGENTS.md — sözleşmedeki her ErrorCode için girdi;
// metinler web'deki packages/ui-web/src/errors/tr.ts ile birebir aynı.
// Testler paket klasöründen çalışır (flutter test, melos).
void main() {
  final contractCodes = ErrorCode.values
      .where((c) => c != ErrorCode.unknownDefaultOpenApi)
      .toList();

  test('sözleşmedeki kodların hepsi', () {
    expect(contractCodes.length, greaterThan(20));
    expect(errorMessages.keys.toSet(), contractCodes.toSet());
  });

  test('web mesajlarıyla aynı kodlar ve metinler', () {
    final source = File('../ui-web/src/errors/tr.ts').readAsStringSync();
    final block = RegExp(
      r'errorMessages[^{]*\{([\s\S]*?)\n\};',
    ).firstMatch(source)!.group(1)!;
    final web = {
      for (final m in RegExp(
        r"^\s*(\w+):\s*'((?:[^'\\]|\\.)*)'",
        multiLine: true,
      ).allMatches(block))
        m.group(1)!: m.group(2)!,
    };
    final dart = {
      for (final entry in errorMessages.entries) entry.key.value: entry.value,
    };
    expect(web.length, contractCodes.length);
    expect(dart, web);
    expect(
      RegExp("unknownErrorMessage = '([^']*)'").firstMatch(source)!.group(1),
      unknownErrorMessage,
    );
  });

  test('Dilim 4 kapora kodları', () {
    expect(errorMessage('deposit_required'), contains('Kapora henüz gelmedi'));
    expect(
      errorMessage('deposit_reminder_already_sent'),
      'Bu randevu için hatırlatma zaten gönderildi.',
    );
  });

  test('ses tonu: ünlem, "lütfen", "başarıyla" yok', () {
    for (final message in errorMessages.values) {
      expect(
        message,
        isNot(
          matches(
            RegExp('!|lütfen|başarıyla|kolayca|sadece', caseSensitive: false),
          ),
        ),
      );
    }
  });

  test('bilinmeyen kod genel mesaj', () {
    expect(errorMessage('yeni_kod'), unknownErrorMessage);
    expect(errorMessage(null), unknownErrorMessage);
    expect(errorMessage('slot_taken'), errorMessages[ErrorCode.slotTaken]);
  });
}
