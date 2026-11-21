//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

/// Salonun metnini ezebileceği müşteri olay kodları (docs/standartlar/bildirimler.md § Müşteri olayları). otp.code ve deposit.* bilerek dışarıdadır: kod ve IBAN/tutar içeren mesajlar yalnız sistem metniyle gider. Hangi şablonların ezilebildiğini GET yanıtındaki templates listesi söyler. 
enum CustomerNotificationEvent {
  @JsonValue(r'appointment.confirmed')
  appointmentPeriodConfirmed(r'appointment.confirmed'),
  @JsonValue(r'appointment.requested')
  appointmentPeriodRequested(r'appointment.requested'),
  @JsonValue(r'appointment.approved')
  appointmentPeriodApproved(r'appointment.approved'),
  @JsonValue(r'appointment.approval_expired')
  appointmentPeriodApprovalExpired(r'appointment.approval_expired'),
  @JsonValue(r'appointment.declined')
  appointmentPeriodDeclined(r'appointment.declined'),
  @JsonValue(r'appointment.reminder')
  appointmentPeriodReminder(r'appointment.reminder'),
  @JsonValue(r'appointment.rescheduled')
  appointmentPeriodRescheduled(r'appointment.rescheduled'),
  @JsonValue(r'appointment.cancelled_by_salon')
  appointmentPeriodCancelledBySalon(r'appointment.cancelled_by_salon'),
  @JsonValue(r'waitlist.offer')
  waitlistPeriodOffer(r'waitlist.offer'),
  @JsonValue(r'walkin.almost_turn')
  walkinPeriodAlmostTurn(r'walkin.almost_turn'),
  @JsonValue(r'review.invite')
  reviewPeriodInvite(r'review.invite'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const CustomerNotificationEvent(this.value);

  final String value;

  @override
  String toString() => value;
}
