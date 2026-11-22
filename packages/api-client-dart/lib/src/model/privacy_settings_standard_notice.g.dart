// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'privacy_settings_standard_notice.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PrivacySettingsStandardNoticeCWProxy {
  PrivacySettingsStandardNotice version(String version);

  PrivacySettingsStandardNotice updatedOn(DateTime updatedOn);

  PrivacySettingsStandardNotice text(String text);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PrivacySettingsStandardNotice(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PrivacySettingsStandardNotice(...).copyWith(id: 12, name: "My name")
  /// ````
  PrivacySettingsStandardNotice call({
    String version,
    DateTime updatedOn,
    String text,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPrivacySettingsStandardNotice.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPrivacySettingsStandardNotice.copyWith.fieldName(...)`
class _$PrivacySettingsStandardNoticeCWProxyImpl
    implements _$PrivacySettingsStandardNoticeCWProxy {
  const _$PrivacySettingsStandardNoticeCWProxyImpl(this._value);

  final PrivacySettingsStandardNotice _value;

  @override
  PrivacySettingsStandardNotice version(String version) =>
      this(version: version);

  @override
  PrivacySettingsStandardNotice updatedOn(DateTime updatedOn) =>
      this(updatedOn: updatedOn);

  @override
  PrivacySettingsStandardNotice text(String text) => this(text: text);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PrivacySettingsStandardNotice(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PrivacySettingsStandardNotice(...).copyWith(id: 12, name: "My name")
  /// ````
  PrivacySettingsStandardNotice call({
    Object? version = const $CopyWithPlaceholder(),
    Object? updatedOn = const $CopyWithPlaceholder(),
    Object? text = const $CopyWithPlaceholder(),
  }) {
    return PrivacySettingsStandardNotice(
      version: version == const $CopyWithPlaceholder()
          ? _value.version
          // ignore: cast_nullable_to_non_nullable
          : version as String,
      updatedOn: updatedOn == const $CopyWithPlaceholder()
          ? _value.updatedOn
          // ignore: cast_nullable_to_non_nullable
          : updatedOn as DateTime,
      text: text == const $CopyWithPlaceholder()
          ? _value.text
          // ignore: cast_nullable_to_non_nullable
          : text as String,
    );
  }
}

extension $PrivacySettingsStandardNoticeCopyWith
    on PrivacySettingsStandardNotice {
  /// Returns a callable class that can be used as follows: `instanceOfPrivacySettingsStandardNotice.copyWith(...)` or like so:`instanceOfPrivacySettingsStandardNotice.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PrivacySettingsStandardNoticeCWProxy get copyWith =>
      _$PrivacySettingsStandardNoticeCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PrivacySettingsStandardNotice _$PrivacySettingsStandardNoticeFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('PrivacySettingsStandardNotice', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['version', 'updatedOn', 'text']);
  final val = PrivacySettingsStandardNotice(
    version: $checkedConvert('version', (v) => v as String),
    updatedOn: $checkedConvert(
      'updatedOn',
      (v) => const DateOnlyConverter().fromJson(v as String),
    ),
    text: $checkedConvert('text', (v) => v as String),
  );
  return val;
});

Map<String, dynamic> _$PrivacySettingsStandardNoticeToJson(
  PrivacySettingsStandardNotice instance,
) => <String, dynamic>{
  'version': instance.version,
  'updatedOn': const DateOnlyConverter().toJson(instance.updatedOn),
  'text': instance.text,
};
