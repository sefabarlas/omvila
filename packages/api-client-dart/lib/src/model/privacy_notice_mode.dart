//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

/// Aydınlatma metni seçimi (OMVILA standart metni veya salonun kendi metni)
enum PrivacyNoticeMode {
  @JsonValue(r'Standard')
  standard(r'Standard'),
  @JsonValue(r'Custom')
  custom(r'Custom'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const PrivacyNoticeMode(this.value);

  final String value;

  @override
  String toString() => value;
}
