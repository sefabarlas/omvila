//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

/// Active; Expired (son kullanım tarihi geçti, günlük iş); Refunded (\"İade et\"); Transferred (başka müşteriye devredildi; v1'de devir ucu yok)
enum CustomerPackageStatus {
  @JsonValue(r'Active')
  active(r'Active'),
  @JsonValue(r'Expired')
  expired(r'Expired'),
  @JsonValue(r'Refunded')
  refunded(r'Refunded'),
  @JsonValue(r'Transferred')
  transferred(r'Transferred'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const CustomerPackageStatus(this.value);

  final String value;

  @override
  String toString() => value;
}
