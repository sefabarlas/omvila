//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

/// Abonelik ödeme yolu. Card yalnız kart sağlayıcısı seçilince kullanılır (T-06)
enum SubscriptionPaymentMethod {
  @JsonValue(r'Card')
  card(r'Card'),
  @JsonValue(r'BankTransfer')
  bankTransfer(r'BankTransfer'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const SubscriptionPaymentMethod(this.value);

  final String value;

  @override
  String toString() => value;
}
