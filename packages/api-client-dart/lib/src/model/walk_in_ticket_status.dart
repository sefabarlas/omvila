//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

/// Waiting = sırada; Called = çağrıldı; Served = randevuya çevrildi; Left = \"Sıradan çıkar\"
enum WalkInTicketStatus {
  @JsonValue(r'Waiting')
  waiting(r'Waiting'),
  @JsonValue(r'Called')
  called(r'Called'),
  @JsonValue(r'Served')
  served(r'Served'),
  @JsonValue(r'Left')
  left(r'Left'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const WalkInTicketStatus(this.value);

  final String value;

  @override
  String toString() => value;
}
