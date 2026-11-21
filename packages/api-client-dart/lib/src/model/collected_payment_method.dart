//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

/// Salonda elle seçilen tahsilat yöntemi (\"Nakit\", \"Kart\", \"Havale\"); paketten düşüm ayrı alanla istenir
enum CollectedPaymentMethod {
  @JsonValue(r'Cash')
  cash(r'Cash'),
  @JsonValue(r'Card')
  card(r'Card'),
  @JsonValue(r'Transfer')
  transfer(r'Transfer'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const CollectedPaymentMethod(this.value);

  final String value;

  @override
  String toString() => value;
}
