//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

/// Fatura dönemi (aylık, yıllık)
enum BillingCycle {
  @JsonValue(r'Monthly')
  monthly(r'Monthly'),
  @JsonValue(r'Yearly')
  yearly(r'Yearly'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const BillingCycle(this.value);

  final String value;

  @override
  String toString() => value;
}
