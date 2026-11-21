// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'notification_settings.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$NotificationSettingsCWProxy {
  NotificationSettings reminderOffsetsMinutes(Set<int> reminderOffsetsMinutes);

  NotificationSettings reviewInviteEnabled(bool reviewInviteEnabled);

  NotificationSettings templates(List<NotificationTemplateSetting> templates);

  NotificationSettings version(String version);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `NotificationSettings(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// NotificationSettings(...).copyWith(id: 12, name: "My name")
  /// ````
  NotificationSettings call({
    Set<int> reminderOffsetsMinutes,
    bool reviewInviteEnabled,
    List<NotificationTemplateSetting> templates,
    String version,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfNotificationSettings.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfNotificationSettings.copyWith.fieldName(...)`
class _$NotificationSettingsCWProxyImpl
    implements _$NotificationSettingsCWProxy {
  const _$NotificationSettingsCWProxyImpl(this._value);

  final NotificationSettings _value;

  @override
  NotificationSettings reminderOffsetsMinutes(
    Set<int> reminderOffsetsMinutes,
  ) => this(reminderOffsetsMinutes: reminderOffsetsMinutes);

  @override
  NotificationSettings reviewInviteEnabled(bool reviewInviteEnabled) =>
      this(reviewInviteEnabled: reviewInviteEnabled);

  @override
  NotificationSettings templates(List<NotificationTemplateSetting> templates) =>
      this(templates: templates);

  @override
  NotificationSettings version(String version) => this(version: version);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `NotificationSettings(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// NotificationSettings(...).copyWith(id: 12, name: "My name")
  /// ````
  NotificationSettings call({
    Object? reminderOffsetsMinutes = const $CopyWithPlaceholder(),
    Object? reviewInviteEnabled = const $CopyWithPlaceholder(),
    Object? templates = const $CopyWithPlaceholder(),
    Object? version = const $CopyWithPlaceholder(),
  }) {
    return NotificationSettings(
      reminderOffsetsMinutes:
          reminderOffsetsMinutes == const $CopyWithPlaceholder()
          ? _value.reminderOffsetsMinutes
          // ignore: cast_nullable_to_non_nullable
          : reminderOffsetsMinutes as Set<int>,
      reviewInviteEnabled: reviewInviteEnabled == const $CopyWithPlaceholder()
          ? _value.reviewInviteEnabled
          // ignore: cast_nullable_to_non_nullable
          : reviewInviteEnabled as bool,
      templates: templates == const $CopyWithPlaceholder()
          ? _value.templates
          // ignore: cast_nullable_to_non_nullable
          : templates as List<NotificationTemplateSetting>,
      version: version == const $CopyWithPlaceholder()
          ? _value.version
          // ignore: cast_nullable_to_non_nullable
          : version as String,
    );
  }
}

extension $NotificationSettingsCopyWith on NotificationSettings {
  /// Returns a callable class that can be used as follows: `instanceOfNotificationSettings.copyWith(...)` or like so:`instanceOfNotificationSettings.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$NotificationSettingsCWProxy get copyWith =>
      _$NotificationSettingsCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

NotificationSettings _$NotificationSettingsFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('NotificationSettings', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'reminderOffsetsMinutes',
      'reviewInviteEnabled',
      'templates',
      'version',
    ],
  );
  final val = NotificationSettings(
    reminderOffsetsMinutes: $checkedConvert(
      'reminderOffsetsMinutes',
      (v) => (v as List<dynamic>).map((e) => (e as num).toInt()).toSet(),
    ),
    reviewInviteEnabled: $checkedConvert(
      'reviewInviteEnabled',
      (v) => v as bool,
    ),
    templates: $checkedConvert(
      'templates',
      (v) => (v as List<dynamic>)
          .map(
            (e) =>
                NotificationTemplateSetting.fromJson(e as Map<String, dynamic>),
          )
          .toList(),
    ),
    version: $checkedConvert('version', (v) => v as String),
  );
  return val;
});

Map<String, dynamic> _$NotificationSettingsToJson(
  NotificationSettings instance,
) => <String, dynamic>{
  'reminderOffsetsMinutes': instance.reminderOffsetsMinutes.toList(),
  'reviewInviteEnabled': instance.reviewInviteEnabled,
  'templates': instance.templates.map((e) => e.toJson()).toList(),
  'version': instance.version,
};
