// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'notification_settings_update.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$NotificationSettingsUpdateCWProxy {
  NotificationSettingsUpdate reminderOffsetsMinutes(
    Set<int> reminderOffsetsMinutes,
  );

  NotificationSettingsUpdate reviewInviteEnabled(bool reviewInviteEnabled);

  NotificationSettingsUpdate templates(
    List<NotificationSettingsUpdateTemplatesInner>? templates,
  );

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `NotificationSettingsUpdate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// NotificationSettingsUpdate(...).copyWith(id: 12, name: "My name")
  /// ````
  NotificationSettingsUpdate call({
    Set<int> reminderOffsetsMinutes,
    bool reviewInviteEnabled,
    List<NotificationSettingsUpdateTemplatesInner>? templates,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfNotificationSettingsUpdate.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfNotificationSettingsUpdate.copyWith.fieldName(...)`
class _$NotificationSettingsUpdateCWProxyImpl
    implements _$NotificationSettingsUpdateCWProxy {
  const _$NotificationSettingsUpdateCWProxyImpl(this._value);

  final NotificationSettingsUpdate _value;

  @override
  NotificationSettingsUpdate reminderOffsetsMinutes(
    Set<int> reminderOffsetsMinutes,
  ) => this(reminderOffsetsMinutes: reminderOffsetsMinutes);

  @override
  NotificationSettingsUpdate reviewInviteEnabled(bool reviewInviteEnabled) =>
      this(reviewInviteEnabled: reviewInviteEnabled);

  @override
  NotificationSettingsUpdate templates(
    List<NotificationSettingsUpdateTemplatesInner>? templates,
  ) => this(templates: templates);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `NotificationSettingsUpdate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// NotificationSettingsUpdate(...).copyWith(id: 12, name: "My name")
  /// ````
  NotificationSettingsUpdate call({
    Object? reminderOffsetsMinutes = const $CopyWithPlaceholder(),
    Object? reviewInviteEnabled = const $CopyWithPlaceholder(),
    Object? templates = const $CopyWithPlaceholder(),
  }) {
    return NotificationSettingsUpdate(
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
          : templates as List<NotificationSettingsUpdateTemplatesInner>?,
    );
  }
}

extension $NotificationSettingsUpdateCopyWith on NotificationSettingsUpdate {
  /// Returns a callable class that can be used as follows: `instanceOfNotificationSettingsUpdate.copyWith(...)` or like so:`instanceOfNotificationSettingsUpdate.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$NotificationSettingsUpdateCWProxy get copyWith =>
      _$NotificationSettingsUpdateCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

NotificationSettingsUpdate _$NotificationSettingsUpdateFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('NotificationSettingsUpdate', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const ['reminderOffsetsMinutes', 'reviewInviteEnabled'],
  );
  final val = NotificationSettingsUpdate(
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
      (v) => (v as List<dynamic>?)
          ?.map(
            (e) => NotificationSettingsUpdateTemplatesInner.fromJson(
              e as Map<String, dynamic>,
            ),
          )
          .toList(),
    ),
  );
  return val;
});

Map<String, dynamic> _$NotificationSettingsUpdateToJson(
  NotificationSettingsUpdate instance,
) => <String, dynamic>{
  'reminderOffsetsMinutes': instance.reminderOffsetsMinutes.toList(),
  'reviewInviteEnabled': instance.reviewInviteEnabled,
  'templates': ?instance.templates?.map((e) => e.toJson()).toList(),
};
