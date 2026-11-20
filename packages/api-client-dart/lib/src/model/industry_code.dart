//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

enum IndustryCode {
  @JsonValue(r'barber')
  barber(r'barber'),
  @JsonValue(r'hair_salon')
  hairSalon(r'hair_salon'),
  @JsonValue(r'beauty')
  beauty(r'beauty'),
  @JsonValue(r'spa')
  spa(r'spa'),
  @JsonValue(r'nail')
  nail(r'nail'),
  @JsonValue(r'dietitian')
  dietitian(r'dietitian'),
  @JsonValue(r'other')
  other(r'other'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const IndustryCode(this.value);

  final String value;

  @override
  String toString() => value;
}
