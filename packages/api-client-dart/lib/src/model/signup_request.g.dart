// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'signup_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$SignupRequestCWProxy {
  SignupRequest signupToken(String signupToken);

  SignupRequest businessName(String businessName);

  SignupRequest industryCode(IndustryCode industryCode);

  SignupRequest ownerFullName(String? ownerFullName);

  SignupRequest acceptedTerms(bool acceptedTerms);

  SignupRequest privacyNoticeVersion(String privacyNoticeVersion);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `SignupRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// SignupRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  SignupRequest call({
    String signupToken,
    String businessName,
    IndustryCode industryCode,
    String? ownerFullName,
    bool acceptedTerms,
    String privacyNoticeVersion,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfSignupRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfSignupRequest.copyWith.fieldName(...)`
class _$SignupRequestCWProxyImpl implements _$SignupRequestCWProxy {
  const _$SignupRequestCWProxyImpl(this._value);

  final SignupRequest _value;

  @override
  SignupRequest signupToken(String signupToken) =>
      this(signupToken: signupToken);

  @override
  SignupRequest businessName(String businessName) =>
      this(businessName: businessName);

  @override
  SignupRequest industryCode(IndustryCode industryCode) =>
      this(industryCode: industryCode);

  @override
  SignupRequest ownerFullName(String? ownerFullName) =>
      this(ownerFullName: ownerFullName);

  @override
  SignupRequest acceptedTerms(bool acceptedTerms) =>
      this(acceptedTerms: acceptedTerms);

  @override
  SignupRequest privacyNoticeVersion(String privacyNoticeVersion) =>
      this(privacyNoticeVersion: privacyNoticeVersion);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `SignupRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// SignupRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  SignupRequest call({
    Object? signupToken = const $CopyWithPlaceholder(),
    Object? businessName = const $CopyWithPlaceholder(),
    Object? industryCode = const $CopyWithPlaceholder(),
    Object? ownerFullName = const $CopyWithPlaceholder(),
    Object? acceptedTerms = const $CopyWithPlaceholder(),
    Object? privacyNoticeVersion = const $CopyWithPlaceholder(),
  }) {
    return SignupRequest(
      signupToken: signupToken == const $CopyWithPlaceholder()
          ? _value.signupToken
          // ignore: cast_nullable_to_non_nullable
          : signupToken as String,
      businessName: businessName == const $CopyWithPlaceholder()
          ? _value.businessName
          // ignore: cast_nullable_to_non_nullable
          : businessName as String,
      industryCode: industryCode == const $CopyWithPlaceholder()
          ? _value.industryCode
          // ignore: cast_nullable_to_non_nullable
          : industryCode as IndustryCode,
      ownerFullName: ownerFullName == const $CopyWithPlaceholder()
          ? _value.ownerFullName
          // ignore: cast_nullable_to_non_nullable
          : ownerFullName as String?,
      acceptedTerms: acceptedTerms == const $CopyWithPlaceholder()
          ? _value.acceptedTerms
          // ignore: cast_nullable_to_non_nullable
          : acceptedTerms as bool,
      privacyNoticeVersion: privacyNoticeVersion == const $CopyWithPlaceholder()
          ? _value.privacyNoticeVersion
          // ignore: cast_nullable_to_non_nullable
          : privacyNoticeVersion as String,
    );
  }
}

extension $SignupRequestCopyWith on SignupRequest {
  /// Returns a callable class that can be used as follows: `instanceOfSignupRequest.copyWith(...)` or like so:`instanceOfSignupRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$SignupRequestCWProxy get copyWith => _$SignupRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

SignupRequest _$SignupRequestFromJson(Map<String, dynamic> json) =>
    $checkedCreate('SignupRequest', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'signupToken',
          'businessName',
          'industryCode',
          'acceptedTerms',
          'privacyNoticeVersion',
        ],
      );
      final val = SignupRequest(
        signupToken: $checkedConvert('signupToken', (v) => v as String),
        businessName: $checkedConvert('businessName', (v) => v as String),
        industryCode: $checkedConvert(
          'industryCode',
          (v) => $enumDecode(
            _$IndustryCodeEnumMap,
            v,
            unknownValue: IndustryCode.unknownDefaultOpenApi,
          ),
        ),
        ownerFullName: $checkedConvert('ownerFullName', (v) => v as String?),
        acceptedTerms: $checkedConvert('acceptedTerms', (v) => v as bool),
        privacyNoticeVersion: $checkedConvert(
          'privacyNoticeVersion',
          (v) => v as String,
        ),
      );
      return val;
    });

Map<String, dynamic> _$SignupRequestToJson(SignupRequest instance) =>
    <String, dynamic>{
      'signupToken': instance.signupToken,
      'businessName': instance.businessName,
      'industryCode': _$IndustryCodeEnumMap[instance.industryCode]!,
      'ownerFullName': ?instance.ownerFullName,
      'acceptedTerms': instance.acceptedTerms,
      'privacyNoticeVersion': instance.privacyNoticeVersion,
    };

const _$IndustryCodeEnumMap = {
  IndustryCode.barber: 'barber',
  IndustryCode.hairSalon: 'hair_salon',
  IndustryCode.beauty: 'beauty',
  IndustryCode.spa: 'spa',
  IndustryCode.nail: 'nail',
  IndustryCode.dietitian: 'dietitian',
  IndustryCode.other: 'other',
  IndustryCode.unknownDefaultOpenApi: 'unknown_default_open_api',
};
