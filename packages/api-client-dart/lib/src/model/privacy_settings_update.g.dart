// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'privacy_settings_update.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PrivacySettingsUpdateCWProxy {
  PrivacySettingsUpdate noticeMode(PrivacyNoticeMode noticeMode);

  PrivacySettingsUpdate customNoticeText(String? customNoticeText);

  PrivacySettingsUpdate transactionRetentionYears(
    int transactionRetentionYears,
  );

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PrivacySettingsUpdate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PrivacySettingsUpdate(...).copyWith(id: 12, name: "My name")
  /// ````
  PrivacySettingsUpdate call({
    PrivacyNoticeMode noticeMode,
    String? customNoticeText,
    int transactionRetentionYears,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPrivacySettingsUpdate.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPrivacySettingsUpdate.copyWith.fieldName(...)`
class _$PrivacySettingsUpdateCWProxyImpl
    implements _$PrivacySettingsUpdateCWProxy {
  const _$PrivacySettingsUpdateCWProxyImpl(this._value);

  final PrivacySettingsUpdate _value;

  @override
  PrivacySettingsUpdate noticeMode(PrivacyNoticeMode noticeMode) =>
      this(noticeMode: noticeMode);

  @override
  PrivacySettingsUpdate customNoticeText(String? customNoticeText) =>
      this(customNoticeText: customNoticeText);

  @override
  PrivacySettingsUpdate transactionRetentionYears(
    int transactionRetentionYears,
  ) => this(transactionRetentionYears: transactionRetentionYears);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PrivacySettingsUpdate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PrivacySettingsUpdate(...).copyWith(id: 12, name: "My name")
  /// ````
  PrivacySettingsUpdate call({
    Object? noticeMode = const $CopyWithPlaceholder(),
    Object? customNoticeText = const $CopyWithPlaceholder(),
    Object? transactionRetentionYears = const $CopyWithPlaceholder(),
  }) {
    return PrivacySettingsUpdate(
      noticeMode: noticeMode == const $CopyWithPlaceholder()
          ? _value.noticeMode
          // ignore: cast_nullable_to_non_nullable
          : noticeMode as PrivacyNoticeMode,
      customNoticeText: customNoticeText == const $CopyWithPlaceholder()
          ? _value.customNoticeText
          // ignore: cast_nullable_to_non_nullable
          : customNoticeText as String?,
      transactionRetentionYears:
          transactionRetentionYears == const $CopyWithPlaceholder()
          ? _value.transactionRetentionYears
          // ignore: cast_nullable_to_non_nullable
          : transactionRetentionYears as int,
    );
  }
}

extension $PrivacySettingsUpdateCopyWith on PrivacySettingsUpdate {
  /// Returns a callable class that can be used as follows: `instanceOfPrivacySettingsUpdate.copyWith(...)` or like so:`instanceOfPrivacySettingsUpdate.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PrivacySettingsUpdateCWProxy get copyWith =>
      _$PrivacySettingsUpdateCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PrivacySettingsUpdate _$PrivacySettingsUpdateFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('PrivacySettingsUpdate', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const ['noticeMode', 'transactionRetentionYears'],
  );
  final val = PrivacySettingsUpdate(
    noticeMode: $checkedConvert(
      'noticeMode',
      (v) => $enumDecode(
        _$PrivacyNoticeModeEnumMap,
        v,
        unknownValue: PrivacyNoticeMode.unknownDefaultOpenApi,
      ),
    ),
    customNoticeText: $checkedConvert('customNoticeText', (v) => v as String?),
    transactionRetentionYears: $checkedConvert(
      'transactionRetentionYears',
      (v) => (v as num).toInt(),
    ),
  );
  return val;
});

Map<String, dynamic> _$PrivacySettingsUpdateToJson(
  PrivacySettingsUpdate instance,
) => <String, dynamic>{
  'noticeMode': _$PrivacyNoticeModeEnumMap[instance.noticeMode]!,
  'customNoticeText': instance.customNoticeText,
  'transactionRetentionYears': instance.transactionRetentionYears,
};

const _$PrivacyNoticeModeEnumMap = {
  PrivacyNoticeMode.standard: 'Standard',
  PrivacyNoticeMode.custom: 'Custom',
  PrivacyNoticeMode.unknownDefaultOpenApi: 'unknown_default_open_api',
};
