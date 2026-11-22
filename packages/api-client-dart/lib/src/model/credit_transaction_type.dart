//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

/// Kontör hareketi türü (paket satın alındı, gönderim, deneme hediyesi, platform ekleme veya düzeltme, iade)
enum CreditTransactionType {
  @JsonValue(r'Purchase')
  purchase(r'Purchase'),
  @JsonValue(r'Usage')
  usage(r'Usage'),
  @JsonValue(r'TrialGift')
  trialGift(r'TrialGift'),
  @JsonValue(r'Grant')
  grant(r'Grant'),
  @JsonValue(r'Refund')
  refund(r'Refund'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const CreditTransactionType(this.value);

  final String value;

  @override
  String toString() => value;
}
