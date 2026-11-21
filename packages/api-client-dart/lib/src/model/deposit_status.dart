//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

/// Randevu kaporasının durumu. Awaiting = havale bekleniyor; CustomerReported = müşteri \"Havaleyi yaptım\" dedi (\"Havale bildirildi · kontrol edin\"); Received = salon \"Kapora geldi\" dedi; Refunded = salon iade etti olarak işaretledi (\"Salon iade etti\"); Waived = salon \"Kaporasız onayla\" dedi, kapora istenmiyor. İstemciler bilinmeyen değeri genel durum olarak ele alır. 
enum DepositStatus {
  @JsonValue(r'Awaiting')
  awaiting(r'Awaiting'),
  @JsonValue(r'CustomerReported')
  customerReported(r'CustomerReported'),
  @JsonValue(r'Received')
  received(r'Received'),
  @JsonValue(r'Refunded')
  refunded(r'Refunded'),
  @JsonValue(r'Waived')
  waived(r'Waived'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const DepositStatus(this.value);

  final String value;

  @override
  String toString() => value;
}
