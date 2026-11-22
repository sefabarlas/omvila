//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

/// Fatura durumu (kesildi, ödendi, ödenemedi, iptal)
enum SubscriptionInvoiceStatus {
  @JsonValue(r'Issued')
  issued(r'Issued'),
  @JsonValue(r'Paid')
  paid(r'Paid'),
  @JsonValue(r'Failed')
  failed(r'Failed'),
  @JsonValue(r'Void')
  void_(r'Void'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const SubscriptionInvoiceStatus(this.value);

  final String value;

  @override
  String toString() => value;
}
