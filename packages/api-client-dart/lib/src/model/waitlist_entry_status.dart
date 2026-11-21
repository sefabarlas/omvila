//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

/// Waiting = bekliyor; Converted = teklif kabul edildi, randevu oluştu; Cancelled = \"Listeden çık\"; Expired = istenen günlerin hepsi geçti
enum WaitlistEntryStatus {
  @JsonValue(r'Waiting')
  waiting(r'Waiting'),
  @JsonValue(r'Converted')
  converted(r'Converted'),
  @JsonValue(r'Cancelled')
  cancelled(r'Cancelled'),
  @JsonValue(r'Expired')
  expired(r'Expired'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const WaitlistEntryStatus(this.value);

  final String value;

  @override
  String toString() => value;
}
