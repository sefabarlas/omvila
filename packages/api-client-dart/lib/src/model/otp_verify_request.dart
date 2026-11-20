//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'otp_verify_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class OtpVerifyRequest {
  /// Returns a new [OtpVerifyRequest] instance.
  OtpVerifyRequest({

    required  this.otpId,

    required  this.code,

     this.rememberDevice = false,
  });

  @JsonKey(
    
    name: r'otpId',
    required: true,
    includeIfNull: false,
  )


  final String otpId;



  @JsonKey(
    
    name: r'code',
    required: true,
    includeIfNull: false,
  )


  final String code;



      /// Panelde \"Bu bilgisayarda 30 gün hatırla\"; rezervasyon sayfasında güvenilir cihaz çerezi
  @JsonKey(
    defaultValue: false,
    name: r'rememberDevice',
    required: false,
    includeIfNull: false,
  )


  final bool? rememberDevice;





    @override
    bool operator ==(Object other) => identical(this, other) || other is OtpVerifyRequest &&
      other.otpId == otpId &&
      other.code == code &&
      other.rememberDevice == rememberDevice;

    @override
    int get hashCode =>
        otpId.hashCode +
        code.hashCode +
        rememberDevice.hashCode;

  factory OtpVerifyRequest.fromJson(Map<String, dynamic> json) => _$OtpVerifyRequestFromJson(json);

  Map<String, dynamic> toJson() => _$OtpVerifyRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

