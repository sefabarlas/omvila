//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

/// Tahsilatın hedefi (PaymentAllocation.TargetType). v1'de bir tahsilat tek hedefe tahsis edilir
enum PaymentPurpose {
  @JsonValue(r'Appointment')
  appointment(r'Appointment'),
  @JsonValue(r'CustomerPackage')
  customerPackage(r'CustomerPackage'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const PaymentPurpose(this.value);

  final String value;

  @override
  String toString() => value;
}
