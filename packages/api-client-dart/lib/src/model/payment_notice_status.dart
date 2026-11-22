//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

/// Havale bildirimi durumu (bekliyor, eşleşti, bulunamadı, reddedildi)
enum PaymentNoticeStatus {
  @JsonValue(r'Pending')
  pending(r'Pending'),
  @JsonValue(r'Matched')
  matched(r'Matched'),
  @JsonValue(r'NotFound')
  notFound(r'NotFound'),
  @JsonValue(r'Rejected')
  rejected(r'Rejected'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const PaymentNoticeStatus(this.value);

  final String value;

  @override
  String toString() => value;
}
