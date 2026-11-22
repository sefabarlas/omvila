//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

/// İşletme olay kodu (docs/standartlar/bildirimler.md § İşletme olayları). Olay kodları katalogdaki biçimiyle (nokta ve alt çizgi) yazılır, PascalCase'e çevrilmez; tek kaynak olay kataloğudur. İstemciler bilinmeyen değeri genel bildirim olarak gösterir. 
enum StaffNotificationEvent {
  @JsonValue(r'staff.approval_needed')
  staffPeriodApprovalNeeded(r'staff.approval_needed'),
  @JsonValue(r'staff.appointment_created')
  staffPeriodAppointmentCreated(r'staff.appointment_created'),
  @JsonValue(r'staff.appointment_changed')
  staffPeriodAppointmentChanged(r'staff.appointment_changed'),
  @JsonValue(r'staff.customer_checked_in')
  staffPeriodCustomerCheckedIn(r'staff.customer_checked_in'),
  @JsonValue(r'staff.deposit_reported')
  staffPeriodDepositReported(r'staff.deposit_reported'),
  @JsonValue(r'staff.review_received')
  staffPeriodReviewReceived(r'staff.review_received'),
  @JsonValue(r'staff.walkin_queue')
  staffPeriodWalkinQueue(r'staff.walkin_queue'),
  @JsonValue(r'staff.daily_summary')
  staffPeriodDailySummary(r'staff.daily_summary'),
  @JsonValue(r'owner.credits_low')
  ownerPeriodCreditsLow(r'owner.credits_low'),
  @JsonValue(r'owner.subscription_payment_failed')
  ownerPeriodSubscriptionPaymentFailed(r'owner.subscription_payment_failed'),
  @JsonValue(r'owner.trial_ending')
  ownerPeriodTrialEnding(r'owner.trial_ending'),
  @JsonValue(r'owner.payment_notice_matched')
  ownerPeriodPaymentNoticeMatched(r'owner.payment_notice_matched'),
  @JsonValue(r'owner.payment_notice_not_found')
  ownerPeriodPaymentNoticeNotFound(r'owner.payment_notice_not_found'),
  @JsonValue(r'owner.payment_notice_rejected')
  ownerPeriodPaymentNoticeRejected(r'owner.payment_notice_rejected'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const StaffNotificationEvent(this.value);

  final String value;

  @override
  String toString() => value;
}
