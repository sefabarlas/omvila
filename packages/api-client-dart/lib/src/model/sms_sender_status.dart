//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

/// Gönderen adı durumu; None = hiç başvuru yok
enum SmsSenderStatus {
  @JsonValue(r'None')
  none(r'None'),
  @JsonValue(r'Pending')
  pending(r'Pending'),
  @JsonValue(r'Approved')
  approved(r'Approved'),
  @JsonValue(r'Rejected')
  rejected(r'Rejected'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const SmsSenderStatus(this.value);

  final String value;

  @override
  String toString() => value;
}
