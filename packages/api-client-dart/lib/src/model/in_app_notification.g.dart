// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'in_app_notification.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$InAppNotificationCWProxy {
  InAppNotification id(String id);

  InAppNotification event(StaffNotificationEvent event);

  InAppNotification title(String title);

  InAppNotification body(String body);

  InAppNotification createdUtc(DateTime createdUtc);

  InAppNotification readUtc(DateTime? readUtc);

  InAppNotification branchId(String? branchId);

  InAppNotification target(NotificationTarget target);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `InAppNotification(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// InAppNotification(...).copyWith(id: 12, name: "My name")
  /// ````
  InAppNotification call({
    String id,
    StaffNotificationEvent event,
    String title,
    String body,
    DateTime createdUtc,
    DateTime? readUtc,
    String? branchId,
    NotificationTarget target,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfInAppNotification.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfInAppNotification.copyWith.fieldName(...)`
class _$InAppNotificationCWProxyImpl implements _$InAppNotificationCWProxy {
  const _$InAppNotificationCWProxyImpl(this._value);

  final InAppNotification _value;

  @override
  InAppNotification id(String id) => this(id: id);

  @override
  InAppNotification event(StaffNotificationEvent event) => this(event: event);

  @override
  InAppNotification title(String title) => this(title: title);

  @override
  InAppNotification body(String body) => this(body: body);

  @override
  InAppNotification createdUtc(DateTime createdUtc) =>
      this(createdUtc: createdUtc);

  @override
  InAppNotification readUtc(DateTime? readUtc) => this(readUtc: readUtc);

  @override
  InAppNotification branchId(String? branchId) => this(branchId: branchId);

  @override
  InAppNotification target(NotificationTarget target) => this(target: target);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `InAppNotification(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// InAppNotification(...).copyWith(id: 12, name: "My name")
  /// ````
  InAppNotification call({
    Object? id = const $CopyWithPlaceholder(),
    Object? event = const $CopyWithPlaceholder(),
    Object? title = const $CopyWithPlaceholder(),
    Object? body = const $CopyWithPlaceholder(),
    Object? createdUtc = const $CopyWithPlaceholder(),
    Object? readUtc = const $CopyWithPlaceholder(),
    Object? branchId = const $CopyWithPlaceholder(),
    Object? target = const $CopyWithPlaceholder(),
  }) {
    return InAppNotification(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      event: event == const $CopyWithPlaceholder()
          ? _value.event
          // ignore: cast_nullable_to_non_nullable
          : event as StaffNotificationEvent,
      title: title == const $CopyWithPlaceholder()
          ? _value.title
          // ignore: cast_nullable_to_non_nullable
          : title as String,
      body: body == const $CopyWithPlaceholder()
          ? _value.body
          // ignore: cast_nullable_to_non_nullable
          : body as String,
      createdUtc: createdUtc == const $CopyWithPlaceholder()
          ? _value.createdUtc
          // ignore: cast_nullable_to_non_nullable
          : createdUtc as DateTime,
      readUtc: readUtc == const $CopyWithPlaceholder()
          ? _value.readUtc
          // ignore: cast_nullable_to_non_nullable
          : readUtc as DateTime?,
      branchId: branchId == const $CopyWithPlaceholder()
          ? _value.branchId
          // ignore: cast_nullable_to_non_nullable
          : branchId as String?,
      target: target == const $CopyWithPlaceholder()
          ? _value.target
          // ignore: cast_nullable_to_non_nullable
          : target as NotificationTarget,
    );
  }
}

extension $InAppNotificationCopyWith on InAppNotification {
  /// Returns a callable class that can be used as follows: `instanceOfInAppNotification.copyWith(...)` or like so:`instanceOfInAppNotification.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$InAppNotificationCWProxy get copyWith =>
      _$InAppNotificationCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

InAppNotification _$InAppNotificationFromJson(Map<String, dynamic> json) =>
    $checkedCreate('InAppNotification', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'id',
          'event',
          'title',
          'body',
          'createdUtc',
          'readUtc',
          'target',
        ],
      );
      final val = InAppNotification(
        id: $checkedConvert('id', (v) => v as String),
        event: $checkedConvert(
          'event',
          (v) => $enumDecode(
            _$StaffNotificationEventEnumMap,
            v,
            unknownValue: StaffNotificationEvent.unknownDefaultOpenApi,
          ),
        ),
        title: $checkedConvert('title', (v) => v as String),
        body: $checkedConvert('body', (v) => v as String),
        createdUtc: $checkedConvert(
          'createdUtc',
          (v) => DateTime.parse(v as String),
        ),
        readUtc: $checkedConvert(
          'readUtc',
          (v) => v == null ? null : DateTime.parse(v as String),
        ),
        branchId: $checkedConvert('branchId', (v) => v as String?),
        target: $checkedConvert(
          'target',
          (v) => NotificationTarget.fromJson(v as Map<String, dynamic>),
        ),
      );
      return val;
    });

Map<String, dynamic> _$InAppNotificationToJson(InAppNotification instance) =>
    <String, dynamic>{
      'id': instance.id,
      'event': _$StaffNotificationEventEnumMap[instance.event]!,
      'title': instance.title,
      'body': instance.body,
      'createdUtc': instance.createdUtc.toIso8601String(),
      'readUtc': instance.readUtc?.toIso8601String(),
      'branchId': ?instance.branchId,
      'target': instance.target.toJson(),
    };

const _$StaffNotificationEventEnumMap = {
  StaffNotificationEvent.staffPeriodApprovalNeeded: 'staff.approval_needed',
  StaffNotificationEvent.staffPeriodAppointmentCreated:
      'staff.appointment_created',
  StaffNotificationEvent.staffPeriodAppointmentChanged:
      'staff.appointment_changed',
  StaffNotificationEvent.staffPeriodCustomerCheckedIn:
      'staff.customer_checked_in',
  StaffNotificationEvent.staffPeriodDepositReported: 'staff.deposit_reported',
  StaffNotificationEvent.staffPeriodReviewReceived: 'staff.review_received',
  StaffNotificationEvent.staffPeriodWalkinQueue: 'staff.walkin_queue',
  StaffNotificationEvent.staffPeriodDailySummary: 'staff.daily_summary',
  StaffNotificationEvent.ownerPeriodCreditsLow: 'owner.credits_low',
  StaffNotificationEvent.ownerPeriodSubscriptionPaymentFailed:
      'owner.subscription_payment_failed',
  StaffNotificationEvent.ownerPeriodTrialEnding: 'owner.trial_ending',
  StaffNotificationEvent.ownerPeriodPaymentNoticeMatched:
      'owner.payment_notice_matched',
  StaffNotificationEvent.ownerPeriodPaymentNoticeNotFound:
      'owner.payment_notice_not_found',
  StaffNotificationEvent.ownerPeriodPaymentNoticeRejected:
      'owner.payment_notice_rejected',
  StaffNotificationEvent.ownerPeriodSmsSenderApproved:
      'owner.sms_sender_approved',
  StaffNotificationEvent.ownerPeriodSmsSenderRejected:
      'owner.sms_sender_rejected',
  StaffNotificationEvent.unknownDefaultOpenApi: 'unknown_default_open_api',
};
