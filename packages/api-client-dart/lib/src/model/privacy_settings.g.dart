// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'privacy_settings.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PrivacySettingsCWProxy {
  PrivacySettings noticeMode(PrivacyNoticeMode noticeMode);

  PrivacySettings standardNotice(PrivacySettingsStandardNotice standardNotice);

  PrivacySettings customNoticeText(String? customNoticeText);

  PrivacySettings customNoticeUpdatedUtc(DateTime? customNoticeUpdatedUtc);

  PrivacySettings transactionRetentionYears(int transactionRetentionYears);

  PrivacySettings allowedRetentionYears(List<int> allowedRetentionYears);

  PrivacySettings version(String version);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PrivacySettings(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PrivacySettings(...).copyWith(id: 12, name: "My name")
  /// ````
  PrivacySettings call({
    PrivacyNoticeMode noticeMode,
    PrivacySettingsStandardNotice standardNotice,
    String? customNoticeText,
    DateTime? customNoticeUpdatedUtc,
    int transactionRetentionYears,
    List<int> allowedRetentionYears,
    String version,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPrivacySettings.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPrivacySettings.copyWith.fieldName(...)`
class _$PrivacySettingsCWProxyImpl implements _$PrivacySettingsCWProxy {
  const _$PrivacySettingsCWProxyImpl(this._value);

  final PrivacySettings _value;

  @override
  PrivacySettings noticeMode(PrivacyNoticeMode noticeMode) =>
      this(noticeMode: noticeMode);

  @override
  PrivacySettings standardNotice(
    PrivacySettingsStandardNotice standardNotice,
  ) => this(standardNotice: standardNotice);

  @override
  PrivacySettings customNoticeText(String? customNoticeText) =>
      this(customNoticeText: customNoticeText);

  @override
  PrivacySettings customNoticeUpdatedUtc(DateTime? customNoticeUpdatedUtc) =>
      this(customNoticeUpdatedUtc: customNoticeUpdatedUtc);

  @override
  PrivacySettings transactionRetentionYears(int transactionRetentionYears) =>
      this(transactionRetentionYears: transactionRetentionYears);

  @override
  PrivacySettings allowedRetentionYears(List<int> allowedRetentionYears) =>
      this(allowedRetentionYears: allowedRetentionYears);

  @override
  PrivacySettings version(String version) => this(version: version);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PrivacySettings(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PrivacySettings(...).copyWith(id: 12, name: "My name")
  /// ````
  PrivacySettings call({
    Object? noticeMode = const $CopyWithPlaceholder(),
    Object? standardNotice = const $CopyWithPlaceholder(),
    Object? customNoticeText = const $CopyWithPlaceholder(),
    Object? customNoticeUpdatedUtc = const $CopyWithPlaceholder(),
    Object? transactionRetentionYears = const $CopyWithPlaceholder(),
    Object? allowedRetentionYears = const $CopyWithPlaceholder(),
    Object? version = const $CopyWithPlaceholder(),
  }) {
    return PrivacySettings(
      noticeMode: noticeMode == const $CopyWithPlaceholder()
          ? _value.noticeMode
          // ignore: cast_nullable_to_non_nullable
          : noticeMode as PrivacyNoticeMode,
      standardNotice: standardNotice == const $CopyWithPlaceholder()
          ? _value.standardNotice
          // ignore: cast_nullable_to_non_nullable
          : standardNotice as PrivacySettingsStandardNotice,
      customNoticeText: customNoticeText == const $CopyWithPlaceholder()
          ? _value.customNoticeText
          // ignore: cast_nullable_to_non_nullable
          : customNoticeText as String?,
      customNoticeUpdatedUtc:
          customNoticeUpdatedUtc == const $CopyWithPlaceholder()
          ? _value.customNoticeUpdatedUtc
          // ignore: cast_nullable_to_non_nullable
          : customNoticeUpdatedUtc as DateTime?,
      transactionRetentionYears:
          transactionRetentionYears == const $CopyWithPlaceholder()
          ? _value.transactionRetentionYears
          // ignore: cast_nullable_to_non_nullable
          : transactionRetentionYears as int,
      allowedRetentionYears:
          allowedRetentionYears == const $CopyWithPlaceholder()
          ? _value.allowedRetentionYears
          // ignore: cast_nullable_to_non_nullable
          : allowedRetentionYears as List<int>,
      version: version == const $CopyWithPlaceholder()
          ? _value.version
          // ignore: cast_nullable_to_non_nullable
          : version as String,
    );
  }
}

extension $PrivacySettingsCopyWith on PrivacySettings {
  /// Returns a callable class that can be used as follows: `instanceOfPrivacySettings.copyWith(...)` or like so:`instanceOfPrivacySettings.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PrivacySettingsCWProxy get copyWith => _$PrivacySettingsCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PrivacySettings _$PrivacySettingsFromJson(Map<String, dynamic> json) =>
    $checkedCreate('PrivacySettings', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'noticeMode',
          'standardNotice',
          'customNoticeText',
          'customNoticeUpdatedUtc',
          'transactionRetentionYears',
          'allowedRetentionYears',
          'version',
        ],
      );
      final val = PrivacySettings(
        noticeMode: $checkedConvert(
          'noticeMode',
          (v) => $enumDecode(
            _$PrivacyNoticeModeEnumMap,
            v,
            unknownValue: PrivacyNoticeMode.unknownDefaultOpenApi,
          ),
        ),
        standardNotice: $checkedConvert(
          'standardNotice',
          (v) =>
              PrivacySettingsStandardNotice.fromJson(v as Map<String, dynamic>),
        ),
        customNoticeText: $checkedConvert(
          'customNoticeText',
          (v) => v as String?,
        ),
        customNoticeUpdatedUtc: $checkedConvert(
          'customNoticeUpdatedUtc',
          (v) => v == null ? null : DateTime.parse(v as String),
        ),
        transactionRetentionYears: $checkedConvert(
          'transactionRetentionYears',
          (v) => (v as num).toInt(),
        ),
        allowedRetentionYears: $checkedConvert(
          'allowedRetentionYears',
          (v) => (v as List<dynamic>).map((e) => (e as num).toInt()).toList(),
        ),
        version: $checkedConvert('version', (v) => v as String),
      );
      return val;
    });

Map<String, dynamic> _$PrivacySettingsToJson(
  PrivacySettings instance,
) => <String, dynamic>{
  'noticeMode': _$PrivacyNoticeModeEnumMap[instance.noticeMode]!,
  'standardNotice': instance.standardNotice.toJson(),
  'customNoticeText': instance.customNoticeText,
  'customNoticeUpdatedUtc': instance.customNoticeUpdatedUtc?.toIso8601String(),
  'transactionRetentionYears': instance.transactionRetentionYears,
  'allowedRetentionYears': instance.allowedRetentionYears,
  'version': instance.version,
};

const _$PrivacyNoticeModeEnumMap = {
  PrivacyNoticeMode.standard: 'Standard',
  PrivacyNoticeMode.custom: 'Custom',
  PrivacyNoticeMode.unknownDefaultOpenApi: 'unknown_default_open_api',
};
