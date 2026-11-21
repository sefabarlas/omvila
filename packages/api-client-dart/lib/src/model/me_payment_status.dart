//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

/// AwaitingConfirmation = \"Salon onayı bekleniyor\" (havale bildirildi, salon onaylamadı); Recorded = kayıtlı (\"Alındı\"); PartiallyRefunded / Refunded = \"Salon iade etti\"
enum MePaymentStatus {
  @JsonValue(r'AwaitingConfirmation')
  awaitingConfirmation(r'AwaitingConfirmation'),
  @JsonValue(r'Recorded')
  recorded(r'Recorded'),
  @JsonValue(r'PartiallyRefunded')
  partiallyRefunded(r'PartiallyRefunded'),
  @JsonValue(r'Refunded')
  refunded(r'Refunded'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const MePaymentStatus(this.value);

  final String value;

  @override
  String toString() => value;
}
