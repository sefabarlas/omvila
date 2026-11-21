// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'me_notification.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MeNotificationCWProxy {
  MeNotification id(String id);

  MeNotification event(MeNotificationEvent event);

  MeNotification title(String title);

  MeNotification body(String body);

  MeNotification createdUtc(DateTime createdUtc);

  MeNotification readUtc(DateTime? readUtc);

  MeNotification brand(MeBrandRef? brand);

  MeNotification target(MeNotificationTarget target);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeNotification(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeNotification(...).copyWith(id: 12, name: "My name")
  /// ````
  MeNotification call({
    String id,
    MeNotificationEvent event,
    String title,
    String body,
    DateTime createdUtc,
    DateTime? readUtc,
    MeBrandRef? brand,
    MeNotificationTarget target,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMeNotification.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMeNotification.copyWith.fieldName(...)`
class _$MeNotificationCWProxyImpl implements _$MeNotificationCWProxy {
  const _$MeNotificationCWProxyImpl(this._value);

  final MeNotification _value;

  @override
  MeNotification id(String id) => this(id: id);

  @override
  MeNotification event(MeNotificationEvent event) => this(event: event);

  @override
  MeNotification title(String title) => this(title: title);

  @override
  MeNotification body(String body) => this(body: body);

  @override
  MeNotification createdUtc(DateTime createdUtc) =>
      this(createdUtc: createdUtc);

  @override
  MeNotification readUtc(DateTime? readUtc) => this(readUtc: readUtc);

  @override
  MeNotification brand(MeBrandRef? brand) => this(brand: brand);

  @override
  MeNotification target(MeNotificationTarget target) => this(target: target);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeNotification(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeNotification(...).copyWith(id: 12, name: "My name")
  /// ````
  MeNotification call({
    Object? id = const $CopyWithPlaceholder(),
    Object? event = const $CopyWithPlaceholder(),
    Object? title = const $CopyWithPlaceholder(),
    Object? body = const $CopyWithPlaceholder(),
    Object? createdUtc = const $CopyWithPlaceholder(),
    Object? readUtc = const $CopyWithPlaceholder(),
    Object? brand = const $CopyWithPlaceholder(),
    Object? target = const $CopyWithPlaceholder(),
  }) {
    return MeNotification(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      event: event == const $CopyWithPlaceholder()
          ? _value.event
          // ignore: cast_nullable_to_non_nullable
          : event as MeNotificationEvent,
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
      brand: brand == const $CopyWithPlaceholder()
          ? _value.brand
          // ignore: cast_nullable_to_non_nullable
          : brand as MeBrandRef?,
      target: target == const $CopyWithPlaceholder()
          ? _value.target
          // ignore: cast_nullable_to_non_nullable
          : target as MeNotificationTarget,
    );
  }
}

extension $MeNotificationCopyWith on MeNotification {
  /// Returns a callable class that can be used as follows: `instanceOfMeNotification.copyWith(...)` or like so:`instanceOfMeNotification.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MeNotificationCWProxy get copyWith => _$MeNotificationCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MeNotification _$MeNotificationFromJson(Map<String, dynamic> json) =>
    $checkedCreate('MeNotification', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'id',
          'event',
          'title',
          'body',
          'createdUtc',
          'readUtc',
          'brand',
          'target',
        ],
      );
      final val = MeNotification(
        id: $checkedConvert('id', (v) => v as String),
        event: $checkedConvert(
          'event',
          (v) => $enumDecode(
            _$MeNotificationEventEnumMap,
            v,
            unknownValue: MeNotificationEvent.unknownDefaultOpenApi,
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
        brand: $checkedConvert(
          'brand',
          (v) =>
              v == null ? null : MeBrandRef.fromJson(v as Map<String, dynamic>),
        ),
        target: $checkedConvert(
          'target',
          (v) => MeNotificationTarget.fromJson(v as Map<String, dynamic>),
        ),
      );
      return val;
    });

Map<String, dynamic> _$MeNotificationToJson(MeNotification instance) =>
    <String, dynamic>{
      'id': instance.id,
      'event': _$MeNotificationEventEnumMap[instance.event]!,
      'title': instance.title,
      'body': instance.body,
      'createdUtc': instance.createdUtc.toIso8601String(),
      'readUtc': instance.readUtc?.toIso8601String(),
      'brand': instance.brand?.toJson(),
      'target': instance.target.toJson(),
    };

const _$MeNotificationEventEnumMap = {
  MeNotificationEvent.appointmentPeriodConfirmed: 'appointment.confirmed',
  MeNotificationEvent.appointmentPeriodRequested: 'appointment.requested',
  MeNotificationEvent.appointmentPeriodApproved: 'appointment.approved',
  MeNotificationEvent.appointmentPeriodApprovalExpired:
      'appointment.approval_expired',
  MeNotificationEvent.appointmentPeriodDeclined: 'appointment.declined',
  MeNotificationEvent.appointmentPeriodReminder: 'appointment.reminder',
  MeNotificationEvent.appointmentPeriodRescheduled: 'appointment.rescheduled',
  MeNotificationEvent.appointmentPeriodCancelledBySalon:
      'appointment.cancelled_by_salon',
  MeNotificationEvent.depositPeriodReceived: 'deposit.received',
  MeNotificationEvent.waitlistPeriodOffer: 'waitlist.offer',
  MeNotificationEvent.reviewPeriodInvite: 'review.invite',
  MeNotificationEvent.unknownDefaultOpenApi: 'unknown_default_open_api',
};
