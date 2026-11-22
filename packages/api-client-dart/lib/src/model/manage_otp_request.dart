//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'manage_otp_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageOtpRequest {
  /// Returns a new [ManageOtpRequest] instance.
  ManageOtpRequest({

    required  this.phone,

    required  this.purpose,
  });

      /// Serbest biçimli telefon girişi; sunucu E.164'e çevirir (\"0532 418 77 05\", \"5324187705\", \"+90 532 418 7705\")
  @JsonKey(
    
    name: r'phone',
    required: true,
    includeIfNull: false,
  )


  final String phone;



  @JsonKey(
    
    name: r'purpose',
    required: true,
    includeIfNull: false,
  unknownEnumValue: ManageOtpRequestPurposeEnum.unknownDefaultOpenApi,
  )


  final ManageOtpRequestPurposeEnum purpose;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageOtpRequest &&
      other.phone == phone &&
      other.purpose == purpose;

    @override
    int get hashCode =>
        phone.hashCode +
        purpose.hashCode;

  factory ManageOtpRequest.fromJson(Map<String, dynamic> json) => _$ManageOtpRequestFromJson(json);

  Map<String, dynamic> toJson() => _$ManageOtpRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum ManageOtpRequestPurposeEnum {
@JsonValue(r'Login')
login(r'Login'),
@JsonValue(r'Signup')
signup(r'Signup'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const ManageOtpRequestPurposeEnum(this.value);

final String value;

@override
String toString() => value;
}


