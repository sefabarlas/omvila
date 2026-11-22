import 'package:flutter/foundation.dart';
import 'package:sms_autofill/sms_autofill.dart';

/// Gelen SMS'ten kodu okur (kodlama-flutter.md § Metin: Android SMS
/// Retriever). iOS'ta klavye önerisi (`AutofillHints.oneTimeCode`) yeterli;
/// orada akış boştur.
abstract interface class SmsCodeRetriever {
  /// Gelen kodlar.
  Stream<String> codes();

  /// Dinlemeyi bırakır.
  Future<void> stop();
}

/// Platforma göre: Android'de `sms_autofill`, diğerlerinde boş.
SmsCodeRetriever platformSmsCodeRetriever() =>
    defaultTargetPlatform == TargetPlatform.android
    ? AndroidSmsCodeRetriever()
    : const NoSmsCodeRetriever();

/// Android SMS Retriever (izin istemez; SMS uygulama imzasını taşır).
final class AndroidSmsCodeRetriever implements SmsCodeRetriever {
  /// Android.
  AndroidSmsCodeRetriever([SmsAutoFill? autoFill])
    : _autoFill = autoFill ?? SmsAutoFill();

  final SmsAutoFill _autoFill;

  @override
  Stream<String> codes() {
    _autoFill.listenForCode(smsCodeRegexPattern: r'\d{6}');
    return _autoFill.code;
  }

  @override
  Future<void> stop() => _autoFill.unregisterListener();
}

/// Otomatik okuma yok.
final class NoSmsCodeRetriever implements SmsCodeRetriever {
  /// Boş.
  const NoSmsCodeRetriever();

  @override
  Stream<String> codes() => const Stream.empty();

  @override
  Future<void> stop() async {}
}
