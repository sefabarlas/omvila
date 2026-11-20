// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_otp_request_result.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PublicOtpRequestResultCWProxy {
  PublicOtpRequestResult status(PublicOtpRequestResultStatusEnum status);

  PublicOtpRequestResult challenge(OtpChallengeResponse? challenge);

  PublicOtpRequestResult booking(BookingTokenResult? booking);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicOtpRequestResult(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicOtpRequestResult(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicOtpRequestResult call({
    PublicOtpRequestResultStatusEnum status,
    OtpChallengeResponse? challenge,
    BookingTokenResult? booking,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPublicOtpRequestResult.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPublicOtpRequestResult.copyWith.fieldName(...)`
class _$PublicOtpRequestResultCWProxyImpl
    implements _$PublicOtpRequestResultCWProxy {
  const _$PublicOtpRequestResultCWProxyImpl(this._value);

  final PublicOtpRequestResult _value;

  @override
  PublicOtpRequestResult status(PublicOtpRequestResultStatusEnum status) =>
      this(status: status);

  @override
  PublicOtpRequestResult challenge(OtpChallengeResponse? challenge) =>
      this(challenge: challenge);

  @override
  PublicOtpRequestResult booking(BookingTokenResult? booking) =>
      this(booking: booking);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicOtpRequestResult(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicOtpRequestResult(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicOtpRequestResult call({
    Object? status = const $CopyWithPlaceholder(),
    Object? challenge = const $CopyWithPlaceholder(),
    Object? booking = const $CopyWithPlaceholder(),
  }) {
    return PublicOtpRequestResult(
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as PublicOtpRequestResultStatusEnum,
      challenge: challenge == const $CopyWithPlaceholder()
          ? _value.challenge
          // ignore: cast_nullable_to_non_nullable
          : challenge as OtpChallengeResponse?,
      booking: booking == const $CopyWithPlaceholder()
          ? _value.booking
          // ignore: cast_nullable_to_non_nullable
          : booking as BookingTokenResult?,
    );
  }
}

extension $PublicOtpRequestResultCopyWith on PublicOtpRequestResult {
  /// Returns a callable class that can be used as follows: `instanceOfPublicOtpRequestResult.copyWith(...)` or like so:`instanceOfPublicOtpRequestResult.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PublicOtpRequestResultCWProxy get copyWith =>
      _$PublicOtpRequestResultCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PublicOtpRequestResult _$PublicOtpRequestResultFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('PublicOtpRequestResult', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['status']);
  final val = PublicOtpRequestResult(
    status: $checkedConvert(
      'status',
      (v) => $enumDecode(
        _$PublicOtpRequestResultStatusEnumEnumMap,
        v,
        unknownValue: PublicOtpRequestResultStatusEnum.unknownDefaultOpenApi,
      ),
    ),
    challenge: $checkedConvert(
      'challenge',
      (v) => v == null
          ? null
          : OtpChallengeResponse.fromJson(v as Map<String, dynamic>),
    ),
    booking: $checkedConvert(
      'booking',
      (v) => v == null
          ? null
          : BookingTokenResult.fromJson(v as Map<String, dynamic>),
    ),
  );
  return val;
});

Map<String, dynamic> _$PublicOtpRequestResultToJson(
  PublicOtpRequestResult instance,
) => <String, dynamic>{
  'status': _$PublicOtpRequestResultStatusEnumEnumMap[instance.status]!,
  'challenge': ?instance.challenge?.toJson(),
  'booking': ?instance.booking?.toJson(),
};

const _$PublicOtpRequestResultStatusEnumEnumMap = {
  PublicOtpRequestResultStatusEnum.codeSent: 'CodeSent',
  PublicOtpRequestResultStatusEnum.alreadyVerified: 'AlreadyVerified',
  PublicOtpRequestResultStatusEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
