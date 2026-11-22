//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

/// CodeMatches = \"Kod tutuyor\" (kod var ve tutar tutuyor); CheckRequired = \"Kontrol edin\" (kod yok veya tutar farklı)
enum MatchVerdict {
  @JsonValue(r'CodeMatches')
  codeMatches(r'CodeMatches'),
  @JsonValue(r'CheckRequired')
  checkRequired(r'CheckRequired'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const MatchVerdict(this.value);

  final String value;

  @override
  String toString() => value;
}
