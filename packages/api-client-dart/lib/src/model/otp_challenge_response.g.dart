// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'otp_challenge_response.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$OtpChallengeResponseCWProxy {
  OtpChallengeResponse otpId(String otpId);

  OtpChallengeResponse expiresInSeconds(int expiresInSeconds);

  OtpChallengeResponse resendAfterSeconds(int resendAfterSeconds);

  OtpChallengeResponse phoneMasked(String phoneMasked);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `OtpChallengeResponse(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// OtpChallengeResponse(...).copyWith(id: 12, name: "My name")
  /// ````
  OtpChallengeResponse call({
    String otpId,
    int expiresInSeconds,
    int resendAfterSeconds,
    String phoneMasked,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfOtpChallengeResponse.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfOtpChallengeResponse.copyWith.fieldName(...)`
class _$OtpChallengeResponseCWProxyImpl
    implements _$OtpChallengeResponseCWProxy {
  const _$OtpChallengeResponseCWProxyImpl(this._value);

  final OtpChallengeResponse _value;

  @override
  OtpChallengeResponse otpId(String otpId) => this(otpId: otpId);

  @override
  OtpChallengeResponse expiresInSeconds(int expiresInSeconds) =>
      this(expiresInSeconds: expiresInSeconds);

  @override
  OtpChallengeResponse resendAfterSeconds(int resendAfterSeconds) =>
      this(resendAfterSeconds: resendAfterSeconds);

  @override
  OtpChallengeResponse phoneMasked(String phoneMasked) =>
      this(phoneMasked: phoneMasked);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `OtpChallengeResponse(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// OtpChallengeResponse(...).copyWith(id: 12, name: "My name")
  /// ````
  OtpChallengeResponse call({
    Object? otpId = const $CopyWithPlaceholder(),
    Object? expiresInSeconds = const $CopyWithPlaceholder(),
    Object? resendAfterSeconds = const $CopyWithPlaceholder(),
    Object? phoneMasked = const $CopyWithPlaceholder(),
  }) {
    return OtpChallengeResponse(
      otpId: otpId == const $CopyWithPlaceholder()
          ? _value.otpId
          // ignore: cast_nullable_to_non_nullable
          : otpId as String,
      expiresInSeconds: expiresInSeconds == const $CopyWithPlaceholder()
          ? _value.expiresInSeconds
          // ignore: cast_nullable_to_non_nullable
          : expiresInSeconds as int,
      resendAfterSeconds: resendAfterSeconds == const $CopyWithPlaceholder()
          ? _value.resendAfterSeconds
          // ignore: cast_nullable_to_non_nullable
          : resendAfterSeconds as int,
      phoneMasked: phoneMasked == const $CopyWithPlaceholder()
          ? _value.phoneMasked
          // ignore: cast_nullable_to_non_nullable
          : phoneMasked as String,
    );
  }
}

extension $OtpChallengeResponseCopyWith on OtpChallengeResponse {
  /// Returns a callable class that can be used as follows: `instanceOfOtpChallengeResponse.copyWith(...)` or like so:`instanceOfOtpChallengeResponse.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$OtpChallengeResponseCWProxy get copyWith =>
      _$OtpChallengeResponseCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

OtpChallengeResponse _$OtpChallengeResponseFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('OtpChallengeResponse', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'otpId',
      'expiresInSeconds',
      'resendAfterSeconds',
      'phoneMasked',
    ],
  );
  final val = OtpChallengeResponse(
    otpId: $checkedConvert('otpId', (v) => v as String),
    expiresInSeconds: $checkedConvert(
      'expiresInSeconds',
      (v) => (v as num).toInt(),
    ),
    resendAfterSeconds: $checkedConvert(
      'resendAfterSeconds',
      (v) => (v as num).toInt(),
    ),
    phoneMasked: $checkedConvert('phoneMasked', (v) => v as String),
  );
  return val;
});

Map<String, dynamic> _$OtpChallengeResponseToJson(
  OtpChallengeResponse instance,
) => <String, dynamic>{
  'otpId': instance.otpId,
  'expiresInSeconds': instance.expiresInSeconds,
  'resendAfterSeconds': instance.resendAfterSeconds,
  'phoneMasked': instance.phoneMasked,
};
