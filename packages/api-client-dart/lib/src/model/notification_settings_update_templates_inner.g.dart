// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'notification_settings_update_templates_inner.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$NotificationSettingsUpdateTemplatesInnerCWProxy {
  NotificationSettingsUpdateTemplatesInner eventCode(
    CustomerNotificationEvent eventCode,
  );

  NotificationSettingsUpdateTemplatesInner variant(String? variant);

  NotificationSettingsUpdateTemplatesInner customBody(String? customBody);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `NotificationSettingsUpdateTemplatesInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// NotificationSettingsUpdateTemplatesInner(...).copyWith(id: 12, name: "My name")
  /// ````
  NotificationSettingsUpdateTemplatesInner call({
    CustomerNotificationEvent eventCode,
    String? variant,
    String? customBody,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfNotificationSettingsUpdateTemplatesInner.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfNotificationSettingsUpdateTemplatesInner.copyWith.fieldName(...)`
class _$NotificationSettingsUpdateTemplatesInnerCWProxyImpl
    implements _$NotificationSettingsUpdateTemplatesInnerCWProxy {
  const _$NotificationSettingsUpdateTemplatesInnerCWProxyImpl(this._value);

  final NotificationSettingsUpdateTemplatesInner _value;

  @override
  NotificationSettingsUpdateTemplatesInner eventCode(
    CustomerNotificationEvent eventCode,
  ) => this(eventCode: eventCode);

  @override
  NotificationSettingsUpdateTemplatesInner variant(String? variant) =>
      this(variant: variant);

  @override
  NotificationSettingsUpdateTemplatesInner customBody(String? customBody) =>
      this(customBody: customBody);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `NotificationSettingsUpdateTemplatesInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// NotificationSettingsUpdateTemplatesInner(...).copyWith(id: 12, name: "My name")
  /// ````
  NotificationSettingsUpdateTemplatesInner call({
    Object? eventCode = const $CopyWithPlaceholder(),
    Object? variant = const $CopyWithPlaceholder(),
    Object? customBody = const $CopyWithPlaceholder(),
  }) {
    return NotificationSettingsUpdateTemplatesInner(
      eventCode: eventCode == const $CopyWithPlaceholder()
          ? _value.eventCode
          // ignore: cast_nullable_to_non_nullable
          : eventCode as CustomerNotificationEvent,
      variant: variant == const $CopyWithPlaceholder()
          ? _value.variant
          // ignore: cast_nullable_to_non_nullable
          : variant as String?,
      customBody: customBody == const $CopyWithPlaceholder()
          ? _value.customBody
          // ignore: cast_nullable_to_non_nullable
          : customBody as String?,
    );
  }
}

extension $NotificationSettingsUpdateTemplatesInnerCopyWith
    on NotificationSettingsUpdateTemplatesInner {
  /// Returns a callable class that can be used as follows: `instanceOfNotificationSettingsUpdateTemplatesInner.copyWith(...)` or like so:`instanceOfNotificationSettingsUpdateTemplatesInner.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$NotificationSettingsUpdateTemplatesInnerCWProxy get copyWith =>
      _$NotificationSettingsUpdateTemplatesInnerCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

NotificationSettingsUpdateTemplatesInner
_$NotificationSettingsUpdateTemplatesInnerFromJson(Map<String, dynamic> json) =>
    $checkedCreate('NotificationSettingsUpdateTemplatesInner', json, (
      $checkedConvert,
    ) {
      $checkKeys(json, requiredKeys: const ['eventCode', 'customBody']);
      final val = NotificationSettingsUpdateTemplatesInner(
        eventCode: $checkedConvert(
          'eventCode',
          (v) => $enumDecode(
            _$CustomerNotificationEventEnumMap,
            v,
            unknownValue: CustomerNotificationEvent.unknownDefaultOpenApi,
          ),
        ),
        variant: $checkedConvert('variant', (v) => v as String?),
        customBody: $checkedConvert('customBody', (v) => v as String?),
      );
      return val;
    });

Map<String, dynamic> _$NotificationSettingsUpdateTemplatesInnerToJson(
  NotificationSettingsUpdateTemplatesInner instance,
) => <String, dynamic>{
  'eventCode': _$CustomerNotificationEventEnumMap[instance.eventCode]!,
  'variant': ?instance.variant,
  'customBody': instance.customBody,
};

const _$CustomerNotificationEventEnumMap = {
  CustomerNotificationEvent.appointmentPeriodConfirmed: 'appointment.confirmed',
  CustomerNotificationEvent.appointmentPeriodRequested: 'appointment.requested',
  CustomerNotificationEvent.appointmentPeriodApproved: 'appointment.approved',
  CustomerNotificationEvent.appointmentPeriodApprovalExpired:
      'appointment.approval_expired',
  CustomerNotificationEvent.appointmentPeriodDeclined: 'appointment.declined',
  CustomerNotificationEvent.appointmentPeriodReminder: 'appointment.reminder',
  CustomerNotificationEvent.appointmentPeriodRescheduled:
      'appointment.rescheduled',
  CustomerNotificationEvent.appointmentPeriodCancelledBySalon:
      'appointment.cancelled_by_salon',
  CustomerNotificationEvent.waitlistPeriodOffer: 'waitlist.offer',
  CustomerNotificationEvent.walkinPeriodAlmostTurn: 'walkin.almost_turn',
  CustomerNotificationEvent.reviewPeriodInvite: 'review.invite',
  CustomerNotificationEvent.unknownDefaultOpenApi: 'unknown_default_open_api',
};
