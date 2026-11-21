//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

/// \"Hangi saatler size uyar?\" Morning = Sabah, Afternoon = Öğleden sonra, Evening = Akşam (aralıklar şubenin yerel saatiyle sunucuda tanımlıdır)
enum WaitlistTimeWindow {
  @JsonValue(r'Morning')
  morning(r'Morning'),
  @JsonValue(r'Afternoon')
  afternoon(r'Afternoon'),
  @JsonValue(r'Evening')
  evening(r'Evening'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const WaitlistTimeWindow(this.value);

  final String value;

  @override
  String toString() => value;
}
