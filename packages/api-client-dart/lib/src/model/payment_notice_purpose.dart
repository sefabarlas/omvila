//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

/// Havale bildiriminin amacı (abonelik, SMS kontörü)
enum PaymentNoticePurpose {
  @JsonValue(r'Subscription')
  subscription(r'Subscription'),
  @JsonValue(r'Credits')
  credits(r'Credits'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const PaymentNoticePurpose(this.value);

  final String value;

  @override
  String toString() => value;
}
