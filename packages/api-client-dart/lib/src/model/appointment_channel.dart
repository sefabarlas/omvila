//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

enum AppointmentChannel {
  @JsonValue(r'Online')
  online(r'Online'),
  @JsonValue(r'Panel')
  panel(r'Panel'),
  @JsonValue(r'Phone')
  phone(r'Phone'),
  @JsonValue(r'WalkIn')
  walkIn(r'WalkIn'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const AppointmentChannel(this.value);

  final String value;

  @override
  String toString() => value;
}
