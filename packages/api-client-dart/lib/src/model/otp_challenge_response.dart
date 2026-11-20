//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'otp_challenge_response.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class OtpChallengeResponse {
  /// Returns a new [OtpChallengeResponse] instance.
  OtpChallengeResponse({

    required  this.otpId,

    required  this.expiresInSeconds,

    required  this.resendAfterSeconds,

    required  this.phoneMasked,
  });

  @JsonKey(
    
    name: r'otpId',
    required: true,
    includeIfNull: false,
  )


  final String otpId;



  @JsonKey(
    
    name: r'expiresInSeconds',
    required: true,
    includeIfNull: false,
  )


  final int expiresInSeconds;



  @JsonKey(
    
    name: r'resendAfterSeconds',
    required: true,
    includeIfNull: false,
  )


  final int resendAfterSeconds;



  @JsonKey(
    
    name: r'phoneMasked',
    required: true,
    includeIfNull: false,
  )


  final String phoneMasked;





    @override
    bool operator ==(Object other) => identical(this, other) || other is OtpChallengeResponse &&
      other.otpId == otpId &&
      other.expiresInSeconds == expiresInSeconds &&
      other.resendAfterSeconds == resendAfterSeconds &&
      other.phoneMasked == phoneMasked;

    @override
    int get hashCode =>
        otpId.hashCode +
        expiresInSeconds.hashCode +
        resendAfterSeconds.hashCode +
        phoneMasked.hashCode;

  factory OtpChallengeResponse.fromJson(Map<String, dynamic> json) => _$OtpChallengeResponseFromJson(json);

  Map<String, dynamic> toJson() => _$OtpChallengeResponseToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

