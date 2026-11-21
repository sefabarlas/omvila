//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

/// Recorded = kayıtlı; PartiallyRefunded / Refunded = \"Salon iade etti\" (para hareketi yok, yalnız işaret); Voided = yanlış giriş iptali
enum PaymentStatus {
  @JsonValue(r'Recorded')
  recorded(r'Recorded'),
  @JsonValue(r'PartiallyRefunded')
  partiallyRefunded(r'PartiallyRefunded'),
  @JsonValue(r'Refunded')
  refunded(r'Refunded'),
  @JsonValue(r'Voided')
  voided(r'Voided'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const PaymentStatus(this.value);

  final String value;

  @override
  String toString() => value;
}
