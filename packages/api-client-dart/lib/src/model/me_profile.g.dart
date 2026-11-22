// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'me_profile.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MeProfileCWProxy {
  MeProfile id(String id);

  MeProfile fullName(String? fullName);

  MeProfile reviewerDisplayName(String? reviewerDisplayName);

  MeProfile phone(String phone);

  MeProfile email(String? email);

  MeProfile language(MeLanguage language);

  MeProfile version(String version);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeProfile(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeProfile(...).copyWith(id: 12, name: "My name")
  /// ````
  MeProfile call({
    String id,
    String? fullName,
    String? reviewerDisplayName,
    String phone,
    String? email,
    MeLanguage language,
    String version,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMeProfile.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMeProfile.copyWith.fieldName(...)`
class _$MeProfileCWProxyImpl implements _$MeProfileCWProxy {
  const _$MeProfileCWProxyImpl(this._value);

  final MeProfile _value;

  @override
  MeProfile id(String id) => this(id: id);

  @override
  MeProfile fullName(String? fullName) => this(fullName: fullName);

  @override
  MeProfile reviewerDisplayName(String? reviewerDisplayName) =>
      this(reviewerDisplayName: reviewerDisplayName);

  @override
  MeProfile phone(String phone) => this(phone: phone);

  @override
  MeProfile email(String? email) => this(email: email);

  @override
  MeProfile language(MeLanguage language) => this(language: language);

  @override
  MeProfile version(String version) => this(version: version);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeProfile(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeProfile(...).copyWith(id: 12, name: "My name")
  /// ````
  MeProfile call({
    Object? id = const $CopyWithPlaceholder(),
    Object? fullName = const $CopyWithPlaceholder(),
    Object? reviewerDisplayName = const $CopyWithPlaceholder(),
    Object? phone = const $CopyWithPlaceholder(),
    Object? email = const $CopyWithPlaceholder(),
    Object? language = const $CopyWithPlaceholder(),
    Object? version = const $CopyWithPlaceholder(),
  }) {
    return MeProfile(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      fullName: fullName == const $CopyWithPlaceholder()
          ? _value.fullName
          // ignore: cast_nullable_to_non_nullable
          : fullName as String?,
      reviewerDisplayName: reviewerDisplayName == const $CopyWithPlaceholder()
          ? _value.reviewerDisplayName
          // ignore: cast_nullable_to_non_nullable
          : reviewerDisplayName as String?,
      phone: phone == const $CopyWithPlaceholder()
          ? _value.phone
          // ignore: cast_nullable_to_non_nullable
          : phone as String,
      email: email == const $CopyWithPlaceholder()
          ? _value.email
          // ignore: cast_nullable_to_non_nullable
          : email as String?,
      language: language == const $CopyWithPlaceholder()
          ? _value.language
          // ignore: cast_nullable_to_non_nullable
          : language as MeLanguage,
      version: version == const $CopyWithPlaceholder()
          ? _value.version
          // ignore: cast_nullable_to_non_nullable
          : version as String,
    );
  }
}

extension $MeProfileCopyWith on MeProfile {
  /// Returns a callable class that can be used as follows: `instanceOfMeProfile.copyWith(...)` or like so:`instanceOfMeProfile.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MeProfileCWProxy get copyWith => _$MeProfileCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MeProfile _$MeProfileFromJson(Map<String, dynamic> json) =>
    $checkedCreate('MeProfile', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'id',
          'fullName',
          'reviewerDisplayName',
          'phone',
          'email',
          'language',
          'version',
        ],
      );
      final val = MeProfile(
        id: $checkedConvert('id', (v) => v as String),
        fullName: $checkedConvert('fullName', (v) => v as String?),
        reviewerDisplayName: $checkedConvert(
          'reviewerDisplayName',
          (v) => v as String?,
        ),
        phone: $checkedConvert('phone', (v) => v as String),
        email: $checkedConvert('email', (v) => v as String?),
        language: $checkedConvert(
          'language',
          (v) => $enumDecode(
            _$MeLanguageEnumMap,
            v,
            unknownValue: MeLanguage.unknownDefaultOpenApi,
          ),
        ),
        version: $checkedConvert('version', (v) => v as String),
      );
      return val;
    });

Map<String, dynamic> _$MeProfileToJson(MeProfile instance) => <String, dynamic>{
  'id': instance.id,
  'fullName': instance.fullName,
  'reviewerDisplayName': instance.reviewerDisplayName,
  'phone': instance.phone,
  'email': instance.email,
  'language': _$MeLanguageEnumMap[instance.language]!,
  'version': instance.version,
};

const _$MeLanguageEnumMap = {
  MeLanguage.tr: 'tr',
  MeLanguage.unknownDefaultOpenApi: 'unknown_default_open_api',
};
