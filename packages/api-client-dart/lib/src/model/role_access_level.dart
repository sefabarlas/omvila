//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

/// Erişim düzeyi (All = tam, Branch = kendi şube(ler)i, Own = yalnız kendi kayıtları, None = yok)
enum RoleAccessLevel {
  @JsonValue(r'All')
  all(r'All'),
  @JsonValue(r'Branch')
  branch(r'Branch'),
  @JsonValue(r'Own')
  own(r'Own'),
  @JsonValue(r'None')
  none(r'None'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const RoleAccessLevel(this.value);

  final String value;

  @override
  String toString() => value;
}
