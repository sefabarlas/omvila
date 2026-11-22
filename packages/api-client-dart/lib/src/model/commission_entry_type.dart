//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

/// Service = hizmet primi (randevu tamamlanınca), Sale = paket satış primi
enum CommissionEntryType {
  @JsonValue(r'Service')
  service(r'Service'),
  @JsonValue(r'Sale')
  sale(r'Sale'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const CommissionEntryType(this.value);

  final String value;

  @override
  String toString() => value;
}
