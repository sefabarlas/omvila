// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'me_profile_update.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MeProfileUpdateCWProxy {
  MeProfileUpdate fullName(String? fullName);

  MeProfileUpdate email(String? email);

  MeProfileUpdate language(MeLanguage? language);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeProfileUpdate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeProfileUpdate(...).copyWith(id: 12, name: "My name")
  /// ````
  MeProfileUpdate call({String? fullName, String? email, MeLanguage? language});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMeProfileUpdate.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMeProfileUpdate.copyWith.fieldName(...)`
class _$MeProfileUpdateCWProxyImpl implements _$MeProfileUpdateCWProxy {
  const _$MeProfileUpdateCWProxyImpl(this._value);

  final MeProfileUpdate _value;

  @override
  MeProfileUpdate fullName(String? fullName) => this(fullName: fullName);

  @override
  MeProfileUpdate email(String? email) => this(email: email);

  @override
  MeProfileUpdate language(MeLanguage? language) => this(language: language);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeProfileUpdate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeProfileUpdate(...).copyWith(id: 12, name: "My name")
  /// ````
  MeProfileUpdate call({
    Object? fullName = const $CopyWithPlaceholder(),
    Object? email = const $CopyWithPlaceholder(),
    Object? language = const $CopyWithPlaceholder(),
  }) {
    return MeProfileUpdate(
      fullName: fullName == const $CopyWithPlaceholder()
          ? _value.fullName
          // ignore: cast_nullable_to_non_nullable
          : fullName as String?,
      email: email == const $CopyWithPlaceholder()
          ? _value.email
          // ignore: cast_nullable_to_non_nullable
          : email as String?,
      language: language == const $CopyWithPlaceholder()
          ? _value.language
          // ignore: cast_nullable_to_non_nullable
          : language as MeLanguage?,
    );
  }
}

extension $MeProfileUpdateCopyWith on MeProfileUpdate {
  /// Returns a callable class that can be used as follows: `instanceOfMeProfileUpdate.copyWith(...)` or like so:`instanceOfMeProfileUpdate.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MeProfileUpdateCWProxy get copyWith => _$MeProfileUpdateCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MeProfileUpdate _$MeProfileUpdateFromJson(Map<String, dynamic> json) =>
    $checkedCreate('MeProfileUpdate', json, ($checkedConvert) {
      final val = MeProfileUpdate(
        fullName: $checkedConvert('fullName', (v) => v as String?),
        email: $checkedConvert('email', (v) => v as String?),
        language: $checkedConvert(
          'language',
          (v) => $enumDecodeNullable(
            _$MeLanguageEnumMap,
            v,
            unknownValue: MeLanguage.unknownDefaultOpenApi,
          ),
        ),
      );
      return val;
    });

Map<String, dynamic> _$MeProfileUpdateToJson(MeProfileUpdate instance) =>
    <String, dynamic>{
      'fullName': ?instance.fullName,
      'email': ?instance.email,
      'language': ?_$MeLanguageEnumMap[instance.language],
    };

const _$MeLanguageEnumMap = {
  MeLanguage.tr: 'tr',
  MeLanguage.unknownDefaultOpenApi: 'unknown_default_open_api',
};
