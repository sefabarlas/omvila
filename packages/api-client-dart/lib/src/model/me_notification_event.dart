//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

/// Müşteri uygulaması bildirim listesindeki olay kodu (docs/standartlar/bildirimler.md § Müşteri olayları; push metni olan olaylar). CustomerNotificationEvent salonun ezebildiği SMS şablonlarıdır; bu enum ondan ayrıdır ve deposit.received'ı da içerir. Yalnız SMS olan olaylar (otp.code, deposit.awaiting, deposit.not_received, walkin.almost_turn) listede yoktur. İstemciler bilinmeyen değeri genel bildirim olarak gösterir. 
enum MeNotificationEvent {
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
  @JsonValue(r'deposit.received')
  depositPeriodReceived(r'deposit.received'),
  @JsonValue(r'waitlist.offer')
  waitlistPeriodOffer(r'waitlist.offer'),
  @JsonValue(r'review.invite')
  reviewPeriodInvite(r'review.invite'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const MeNotificationEvent(this.value);

  final String value;

  @override
  String toString() => value;
}
