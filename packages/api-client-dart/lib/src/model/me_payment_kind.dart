//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

/// Deposit = kapora havalesi, AppointmentPayment = salonda alınan randevu ödemesi, PackagePurchase = paket satın alma. İstemciler bilinmeyen değeri genel satır olarak gösterir
enum MePaymentKind {
  @JsonValue(r'Deposit')
  deposit(r'Deposit'),
  @JsonValue(r'AppointmentPayment')
  appointmentPayment(r'AppointmentPayment'),
  @JsonValue(r'PackagePurchase')
  packagePurchase(r'PackagePurchase'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const MePaymentKind(this.value);

  final String value;

  @override
  String toString() => value;
}
