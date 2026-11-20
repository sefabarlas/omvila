//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/otp_challenge_response.dart';
import 'package:omvila_api/src/model/booking_token_result.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'public_otp_request_result.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PublicOtpRequestResult {
  /// Returns a new [PublicOtpRequestResult] instance.
  PublicOtpRequestResult({

    required  this.status,

     this.challenge,

     this.booking,
  });

  @JsonKey(
    
    name: r'status',
    required: true,
    includeIfNull: false,
  unknownEnumValue: PublicOtpRequestResultStatusEnum.unknownDefaultOpenApi,
  )


  final PublicOtpRequestResultStatusEnum status;



  @JsonKey(
    
    name: r'challenge',
    required: false,
    includeIfNull: false,
  )


  final OtpChallengeResponse? challenge;



  @JsonKey(
    
    name: r'booking',
    required: false,
    includeIfNull: false,
  )


  final BookingTokenResult? booking;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PublicOtpRequestResult &&
      other.status == status &&
      other.challenge == challenge &&
      other.booking == booking;

    @override
    int get hashCode =>
        status.hashCode +
        challenge.hashCode +
        booking.hashCode;

  factory PublicOtpRequestResult.fromJson(Map<String, dynamic> json) => _$PublicOtpRequestResultFromJson(json);

  Map<String, dynamic> toJson() => _$PublicOtpRequestResultToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum PublicOtpRequestResultStatusEnum {
@JsonValue(r'CodeSent')
codeSent(r'CodeSent'),
@JsonValue(r'AlreadyVerified')
alreadyVerified(r'AlreadyVerified'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const PublicOtpRequestResultStatusEnum(this.value);

final String value;

@override
String toString() => value;
}


