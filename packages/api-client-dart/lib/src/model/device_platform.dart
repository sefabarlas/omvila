//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

/// Cihaz platformu (alan modeli Device.Platform)
enum DevicePlatform {
  @JsonValue(r'iOS')
  iOS(r'iOS'),
  @JsonValue(r'Android')
  android(r'Android'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const DevicePlatform(this.value);

  final String value;

  @override
  String toString() => value;
}
