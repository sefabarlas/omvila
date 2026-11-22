//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

/// Uygulama ve bildirim dili. v1'de yalnız tr (docs/acik-sorular.md T-101); istemciler bilinmeyen değeri tr sayar
enum MeLanguage {
  @JsonValue(r'tr')
  tr(r'tr'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const MeLanguage(this.value);

  final String value;

  @override
  String toString() => value;
}
