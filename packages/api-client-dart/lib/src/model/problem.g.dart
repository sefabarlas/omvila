// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'problem.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ProblemCWProxy {
  Problem type(String type);

  Problem title(String title);

  Problem status(int status);

  Problem code(ErrorCode code);

  Problem detail(String? detail);

  Problem traceId(String? traceId);

  Problem errors(Map<String, List<String>>? errors);

  Problem meta(Map<String, Object>? meta);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `Problem(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// Problem(...).copyWith(id: 12, name: "My name")
  /// ````
  Problem call({
    String type,
    String title,
    int status,
    ErrorCode code,
    String? detail,
    String? traceId,
    Map<String, List<String>>? errors,
    Map<String, Object>? meta,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfProblem.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfProblem.copyWith.fieldName(...)`
class _$ProblemCWProxyImpl implements _$ProblemCWProxy {
  const _$ProblemCWProxyImpl(this._value);

  final Problem _value;

  @override
  Problem type(String type) => this(type: type);

  @override
  Problem title(String title) => this(title: title);

  @override
  Problem status(int status) => this(status: status);

  @override
  Problem code(ErrorCode code) => this(code: code);

  @override
  Problem detail(String? detail) => this(detail: detail);

  @override
  Problem traceId(String? traceId) => this(traceId: traceId);

  @override
  Problem errors(Map<String, List<String>>? errors) => this(errors: errors);

  @override
  Problem meta(Map<String, Object>? meta) => this(meta: meta);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `Problem(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// Problem(...).copyWith(id: 12, name: "My name")
  /// ````
  Problem call({
    Object? type = const $CopyWithPlaceholder(),
    Object? title = const $CopyWithPlaceholder(),
    Object? status = const $CopyWithPlaceholder(),
    Object? code = const $CopyWithPlaceholder(),
    Object? detail = const $CopyWithPlaceholder(),
    Object? traceId = const $CopyWithPlaceholder(),
    Object? errors = const $CopyWithPlaceholder(),
    Object? meta = const $CopyWithPlaceholder(),
  }) {
    return Problem(
      type: type == const $CopyWithPlaceholder()
          ? _value.type
          // ignore: cast_nullable_to_non_nullable
          : type as String,
      title: title == const $CopyWithPlaceholder()
          ? _value.title
          // ignore: cast_nullable_to_non_nullable
          : title as String,
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as int,
      code: code == const $CopyWithPlaceholder()
          ? _value.code
          // ignore: cast_nullable_to_non_nullable
          : code as ErrorCode,
      detail: detail == const $CopyWithPlaceholder()
          ? _value.detail
          // ignore: cast_nullable_to_non_nullable
          : detail as String?,
      traceId: traceId == const $CopyWithPlaceholder()
          ? _value.traceId
          // ignore: cast_nullable_to_non_nullable
          : traceId as String?,
      errors: errors == const $CopyWithPlaceholder()
          ? _value.errors
          // ignore: cast_nullable_to_non_nullable
          : errors as Map<String, List<String>>?,
      meta: meta == const $CopyWithPlaceholder()
          ? _value.meta
          // ignore: cast_nullable_to_non_nullable
          : meta as Map<String, Object>?,
    );
  }
}

extension $ProblemCopyWith on Problem {
  /// Returns a callable class that can be used as follows: `instanceOfProblem.copyWith(...)` or like so:`instanceOfProblem.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ProblemCWProxy get copyWith => _$ProblemCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Problem _$ProblemFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('Problem', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['type', 'title', 'status', 'code']);
  final val = Problem(
    type: $checkedConvert('type', (v) => v as String),
    title: $checkedConvert('title', (v) => v as String),
    status: $checkedConvert('status', (v) => (v as num).toInt()),
    code: $checkedConvert(
      'code',
      (v) => $enumDecode(
        _$ErrorCodeEnumMap,
        v,
        unknownValue: ErrorCode.unknownDefaultOpenApi,
      ),
    ),
    detail: $checkedConvert('detail', (v) => v as String?),
    traceId: $checkedConvert('traceId', (v) => v as String?),
    errors: $checkedConvert(
      'errors',
      (v) => (v as Map<String, dynamic>?)?.map(
        (k, e) =>
            MapEntry(k, (e as List<dynamic>).map((e) => e as String).toList()),
      ),
    ),
    meta: $checkedConvert(
      'meta',
      (v) =>
          (v as Map<String, dynamic>?)?.map((k, e) => MapEntry(k, e as Object)),
    ),
  );
  return val;
});

Map<String, dynamic> _$ProblemToJson(Problem instance) => <String, dynamic>{
  'type': instance.type,
  'title': instance.title,
  'status': instance.status,
  'code': _$ErrorCodeEnumMap[instance.code]!,
  'detail': ?instance.detail,
  'traceId': ?instance.traceId,
  'errors': ?instance.errors,
  'meta': ?instance.meta,
};

const _$ErrorCodeEnumMap = {
  ErrorCode.validationFailed: 'validation_failed',
  ErrorCode.unauthorized: 'unauthorized',
  ErrorCode.tokenExpired: 'token_expired',
  ErrorCode.forbidden: 'forbidden',
  ErrorCode.bookingDisabled: 'booking_disabled',
  ErrorCode.subscriptionReadOnly: 'subscription_read_only',
  ErrorCode.customerBlocked: 'customer_blocked',
  ErrorCode.notFound: 'not_found',
  ErrorCode.accessCodeInvalid: 'access_code_invalid',
  ErrorCode.slotTaken: 'slot_taken',
  ErrorCode.invalidStateTransition: 'invalid_state_transition',
  ErrorCode.cutoffPassed: 'cutoff_passed',
  ErrorCode.holdExpired: 'hold_expired',
  ErrorCode.alreadyReviewed: 'already_reviewed',
  ErrorCode.phoneAlreadyExists: 'phone_already_exists',
  ErrorCode.slugUnavailable: 'slug_unavailable',
  ErrorCode.depositUnavailableNoIban: 'deposit_unavailable_no_iban',
  ErrorCode.outsideWorkingHours: 'outside_working_hours',
  ErrorCode.staffOnTimeOff: 'staff_on_time_off',
  ErrorCode.insufficientCredits: 'insufficient_credits',
  ErrorCode.concurrencyConflict: 'concurrency_conflict',
  ErrorCode.idempotencyKeyReused: 'idempotency_key_reused',
  ErrorCode.otpInvalid: 'otp_invalid',
  ErrorCode.otpExpired: 'otp_expired',
  ErrorCode.preconditionRequired: 'precondition_required',
  ErrorCode.otpAttemptsExceeded: 'otp_attempts_exceeded',
  ErrorCode.otpResendTooSoon: 'otp_resend_too_soon',
  ErrorCode.rateLimited: 'rate_limited',
  ErrorCode.tryAgain: 'try_again',
  ErrorCode.unknownDefaultOpenApi: 'unknown_default_open_api',
};
