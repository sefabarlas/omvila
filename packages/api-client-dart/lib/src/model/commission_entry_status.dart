//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

/// docs/alan/durum-makinesi.md § Prim. Reversed = iptal veya iade ters kaydı (eksi tutar). İstemciler bilinmeyen değeri genel durum olarak gösterir
enum CommissionEntryStatus {
  @JsonValue(r'Accrued')
  accrued(r'Accrued'),
  @JsonValue(r'Approved')
  approved(r'Approved'),
  @JsonValue(r'Paid')
  paid(r'Paid'),
  @JsonValue(r'Reversed')
  reversed(r'Reversed'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const CommissionEntryStatus(this.value);

  final String value;

  @override
  String toString() => value;
}
