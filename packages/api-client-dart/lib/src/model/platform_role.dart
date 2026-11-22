//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

/// Platform rolü (Yönetici, Destek, Finans; K3)
enum PlatformRole {
  @JsonValue(r'PlatformAdmin')
  platformAdmin(r'PlatformAdmin'),
  @JsonValue(r'PlatformSupport')
  platformSupport(r'PlatformSupport'),
  @JsonValue(r'PlatformFinance')
  platformFinance(r'PlatformFinance'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const PlatformRole(this.value);

  final String value;

  @override
  String toString() => value;
}
