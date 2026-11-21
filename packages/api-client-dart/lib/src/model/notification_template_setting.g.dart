// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'notification_template_setting.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$NotificationTemplateSettingCWProxy {
  NotificationTemplateSetting eventCode(CustomerNotificationEvent eventCode);

  NotificationTemplateSetting variant(String? variant);

  NotificationTemplateSetting channel(
    NotificationTemplateSettingChannelEnum channel,
  );

  NotificationTemplateSetting systemBody(String systemBody);

  NotificationTemplateSetting customBody(String? customBody);

  NotificationTemplateSetting variables(List<String> variables);

  NotificationTemplateSetting requiredVariables(List<String> requiredVariables);

  NotificationTemplateSetting maxLength(int maxLength);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `NotificationTemplateSetting(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// NotificationTemplateSetting(...).copyWith(id: 12, name: "My name")
  /// ````
  NotificationTemplateSetting call({
    CustomerNotificationEvent eventCode,
    String? variant,
    NotificationTemplateSettingChannelEnum channel,
    String systemBody,
    String? customBody,
    List<String> variables,
    List<String> requiredVariables,
    int maxLength,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfNotificationTemplateSetting.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfNotificationTemplateSetting.copyWith.fieldName(...)`
class _$NotificationTemplateSettingCWProxyImpl
    implements _$NotificationTemplateSettingCWProxy {
  const _$NotificationTemplateSettingCWProxyImpl(this._value);

  final NotificationTemplateSetting _value;

  @override
  NotificationTemplateSetting eventCode(CustomerNotificationEvent eventCode) =>
      this(eventCode: eventCode);

  @override
  NotificationTemplateSetting variant(String? variant) =>
      this(variant: variant);

  @override
  NotificationTemplateSetting channel(
    NotificationTemplateSettingChannelEnum channel,
  ) => this(channel: channel);

  @override
  NotificationTemplateSetting systemBody(String systemBody) =>
      this(systemBody: systemBody);

  @override
  NotificationTemplateSetting customBody(String? customBody) =>
      this(customBody: customBody);

  @override
  NotificationTemplateSetting variables(List<String> variables) =>
      this(variables: variables);

  @override
  NotificationTemplateSetting requiredVariables(
    List<String> requiredVariables,
  ) => this(requiredVariables: requiredVariables);

  @override
  NotificationTemplateSetting maxLength(int maxLength) =>
      this(maxLength: maxLength);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `NotificationTemplateSetting(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// NotificationTemplateSetting(...).copyWith(id: 12, name: "My name")
  /// ````
  NotificationTemplateSetting call({
    Object? eventCode = const $CopyWithPlaceholder(),
    Object? variant = const $CopyWithPlaceholder(),
    Object? channel = const $CopyWithPlaceholder(),
    Object? systemBody = const $CopyWithPlaceholder(),
    Object? customBody = const $CopyWithPlaceholder(),
    Object? variables = const $CopyWithPlaceholder(),
    Object? requiredVariables = const $CopyWithPlaceholder(),
    Object? maxLength = const $CopyWithPlaceholder(),
  }) {
    return NotificationTemplateSetting(
      eventCode: eventCode == const $CopyWithPlaceholder()
          ? _value.eventCode
          // ignore: cast_nullable_to_non_nullable
          : eventCode as CustomerNotificationEvent,
      variant: variant == const $CopyWithPlaceholder()
          ? _value.variant
          // ignore: cast_nullable_to_non_nullable
          : variant as String?,
      channel: channel == const $CopyWithPlaceholder()
          ? _value.channel
          // ignore: cast_nullable_to_non_nullable
          : channel as NotificationTemplateSettingChannelEnum,
      systemBody: systemBody == const $CopyWithPlaceholder()
          ? _value.systemBody
          // ignore: cast_nullable_to_non_nullable
          : systemBody as String,
      customBody: customBody == const $CopyWithPlaceholder()
          ? _value.customBody
          // ignore: cast_nullable_to_non_nullable
          : customBody as String?,
      variables: variables == const $CopyWithPlaceholder()
          ? _value.variables
          // ignore: cast_nullable_to_non_nullable
          : variables as List<String>,
      requiredVariables: requiredVariables == const $CopyWithPlaceholder()
          ? _value.requiredVariables
          // ignore: cast_nullable_to_non_nullable
          : requiredVariables as List<String>,
      maxLength: maxLength == const $CopyWithPlaceholder()
          ? _value.maxLength
          // ignore: cast_nullable_to_non_nullable
          : maxLength as int,
    );
  }
}

extension $NotificationTemplateSettingCopyWith on NotificationTemplateSetting {
  /// Returns a callable class that can be used as follows: `instanceOfNotificationTemplateSetting.copyWith(...)` or like so:`instanceOfNotificationTemplateSetting.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$NotificationTemplateSettingCWProxy get copyWith =>
      _$NotificationTemplateSettingCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

NotificationTemplateSetting _$NotificationTemplateSettingFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('NotificationTemplateSetting', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'eventCode',
      'variant',
      'channel',
      'systemBody',
      'customBody',
      'variables',
      'requiredVariables',
      'maxLength',
    ],
  );
  final val = NotificationTemplateSetting(
    eventCode: $checkedConvert(
      'eventCode',
      (v) => $enumDecode(
        _$CustomerNotificationEventEnumMap,
        v,
        unknownValue: CustomerNotificationEvent.unknownDefaultOpenApi,
      ),
    ),
    variant: $checkedConvert('variant', (v) => v as String?),
    channel: $checkedConvert(
      'channel',
      (v) => $enumDecode(
        _$NotificationTemplateSettingChannelEnumEnumMap,
        v,
        unknownValue:
            NotificationTemplateSettingChannelEnum.unknownDefaultOpenApi,
      ),
    ),
    systemBody: $checkedConvert('systemBody', (v) => v as String),
    customBody: $checkedConvert('customBody', (v) => v as String?),
    variables: $checkedConvert(
      'variables',
      (v) => (v as List<dynamic>).map((e) => e as String).toList(),
    ),
    requiredVariables: $checkedConvert(
      'requiredVariables',
      (v) => (v as List<dynamic>).map((e) => e as String).toList(),
    ),
    maxLength: $checkedConvert('maxLength', (v) => (v as num).toInt()),
  );
  return val;
});

Map<String, dynamic> _$NotificationTemplateSettingToJson(
  NotificationTemplateSetting instance,
) => <String, dynamic>{
  'eventCode': _$CustomerNotificationEventEnumMap[instance.eventCode]!,
  'variant': instance.variant,
  'channel': _$NotificationTemplateSettingChannelEnumEnumMap[instance.channel]!,
  'systemBody': instance.systemBody,
  'customBody': instance.customBody,
  'variables': instance.variables,
  'requiredVariables': instance.requiredVariables,
  'maxLength': instance.maxLength,
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
  CustomerNotificationEvent.reviewPeriodInvite: 'review.invite',
  CustomerNotificationEvent.unknownDefaultOpenApi: 'unknown_default_open_api',
};

const _$NotificationTemplateSettingChannelEnumEnumMap = {
  NotificationTemplateSettingChannelEnum.sms: 'Sms',
  NotificationTemplateSettingChannelEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
