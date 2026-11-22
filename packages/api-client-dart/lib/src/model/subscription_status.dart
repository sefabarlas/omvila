//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

/// Abonelik durumu (docs/alan/durum-makinesi.md § 8): Deneme, Aktif, Ödeme gecikti, Salt okunur, İptal edildi
enum SubscriptionStatus {
  @JsonValue(r'Trialing')
  trialing(r'Trialing'),
  @JsonValue(r'Active')
  active(r'Active'),
  @JsonValue(r'PastDue')
  pastDue(r'PastDue'),
  @JsonValue(r'ReadOnly')
  readOnly(r'ReadOnly'),
  @JsonValue(r'Cancelled')
  cancelled(r'Cancelled'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const SubscriptionStatus(this.value);

  final String value;

  @override
  String toString() => value;
}
