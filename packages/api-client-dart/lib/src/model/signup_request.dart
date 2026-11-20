//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/industry_code.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'signup_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class SignupRequest {
  /// Returns a new [SignupRequest] instance.
  SignupRequest({

    required  this.signupToken,

    required  this.businessName,

    required  this.industryCode,

     this.ownerFullName,

    required  this.acceptedTerms,

    required  this.privacyNoticeVersion,
  });

  @JsonKey(
    
    name: r'signupToken',
    required: true,
    includeIfNull: false,
  )


  final String signupToken;



  @JsonKey(
    
    name: r'businessName',
    required: true,
    includeIfNull: false,
  )


  final String businessName;



  @JsonKey(
    
    name: r'industryCode',
    required: true,
    includeIfNull: false,
  unknownEnumValue: IndustryCode.unknownDefaultOpenApi,
  )


  final IndustryCode industryCode;



  @JsonKey(
    
    name: r'ownerFullName',
    required: false,
    includeIfNull: false,
  )


  final String? ownerFullName;



      /// Kullanım koşulları ve aydınlatma metni okundu
  @JsonKey(
    
    name: r'acceptedTerms',
    required: true,
    includeIfNull: false,
  )


  final bool acceptedTerms;



  @JsonKey(
    
    name: r'privacyNoticeVersion',
    required: true,
    includeIfNull: false,
  )


  final String privacyNoticeVersion;





    @override
    bool operator ==(Object other) => identical(this, other) || other is SignupRequest &&
      other.signupToken == signupToken &&
      other.businessName == businessName &&
      other.industryCode == industryCode &&
      other.ownerFullName == ownerFullName &&
      other.acceptedTerms == acceptedTerms &&
      other.privacyNoticeVersion == privacyNoticeVersion;

    @override
    int get hashCode =>
        signupToken.hashCode +
        businessName.hashCode +
        industryCode.hashCode +
        ownerFullName.hashCode +
        acceptedTerms.hashCode +
        privacyNoticeVersion.hashCode;

  factory SignupRequest.fromJson(Map<String, dynamic> json) => _$SignupRequestFromJson(json);

  Map<String, dynamic> toJson() => _$SignupRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

