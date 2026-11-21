//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

/// docs/sozlesme/api-kurallari.md § Hata kodu kataloğu. İstemciler bilinmeyen değeri genel hata olarak ele alır.
enum ErrorCode {
  @JsonValue(r'validation_failed')
  validationFailed(r'validation_failed'),
  @JsonValue(r'unauthorized')
  unauthorized(r'unauthorized'),
  @JsonValue(r'token_expired')
  tokenExpired(r'token_expired'),
  @JsonValue(r'forbidden')
  forbidden(r'forbidden'),
  @JsonValue(r'booking_disabled')
  bookingDisabled(r'booking_disabled'),
  @JsonValue(r'subscription_read_only')
  subscriptionReadOnly(r'subscription_read_only'),
  @JsonValue(r'customer_blocked')
  customerBlocked(r'customer_blocked'),
  @JsonValue(r'not_found')
  notFound(r'not_found'),
  @JsonValue(r'access_code_invalid')
  accessCodeInvalid(r'access_code_invalid'),
  @JsonValue(r'slot_taken')
  slotTaken(r'slot_taken'),
  @JsonValue(r'invalid_state_transition')
  invalidStateTransition(r'invalid_state_transition'),
  @JsonValue(r'cutoff_passed')
  cutoffPassed(r'cutoff_passed'),
  @JsonValue(r'hold_expired')
  holdExpired(r'hold_expired'),
  @JsonValue(r'already_reviewed')
  alreadyReviewed(r'already_reviewed'),
  @JsonValue(r'phone_already_exists')
  phoneAlreadyExists(r'phone_already_exists'),
  @JsonValue(r'slug_unavailable')
  slugUnavailable(r'slug_unavailable'),
  @JsonValue(r'deposit_unavailable_no_iban')
  depositUnavailableNoIban(r'deposit_unavailable_no_iban'),
  @JsonValue(r'deposit_required')
  depositRequired(r'deposit_required'),
  @JsonValue(r'deposit_reminder_already_sent')
  depositReminderAlreadySent(r'deposit_reminder_already_sent'),
  @JsonValue(r'payment_total_mismatch')
  paymentTotalMismatch(r'payment_total_mismatch'),
  @JsonValue(r'package_session_unavailable')
  packageSessionUnavailable(r'package_session_unavailable'),
  @JsonValue(r'cash_day_already_closed')
  cashDayAlreadyClosed(r'cash_day_already_closed'),
  @JsonValue(r'outside_working_hours')
  outsideWorkingHours(r'outside_working_hours'),
  @JsonValue(r'staff_on_time_off')
  staffOnTimeOff(r'staff_on_time_off'),
  @JsonValue(r'insufficient_credits')
  insufficientCredits(r'insufficient_credits'),
  @JsonValue(r'concurrency_conflict')
  concurrencyConflict(r'concurrency_conflict'),
  @JsonValue(r'idempotency_key_reused')
  idempotencyKeyReused(r'idempotency_key_reused'),
  @JsonValue(r'otp_invalid')
  otpInvalid(r'otp_invalid'),
  @JsonValue(r'otp_expired')
  otpExpired(r'otp_expired'),
  @JsonValue(r'precondition_required')
  preconditionRequired(r'precondition_required'),
  @JsonValue(r'otp_attempts_exceeded')
  otpAttemptsExceeded(r'otp_attempts_exceeded'),
  @JsonValue(r'otp_resend_too_soon')
  otpResendTooSoon(r'otp_resend_too_soon'),
  @JsonValue(r'rate_limited')
  rateLimited(r'rate_limited'),
  @JsonValue(r'try_again')
  tryAgain(r'try_again'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const ErrorCode(this.value);

  final String value;

  @override
  String toString() => value;
}
