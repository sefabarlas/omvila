//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'manage_auth_result.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageAuthResult {
  /// Returns a new [ManageAuthResult] instance.
  ManageAuthResult({

    required  this.result,

     this.accessToken,

     this.expiresInSeconds,

     this.refreshToken,

     this.signupToken,
  });

  @JsonKey(
    
    name: r'result',
    required: true,
    includeIfNull: false,
  unknownEnumValue: ManageAuthResultResultEnum.unknownDefaultOpenApi,
  )


  final ManageAuthResultResultEnum result;



      /// result = SignedIn
  @JsonKey(
    
    name: r'accessToken',
    required: false,
    includeIfNull: false,
  )


  final String? accessToken;



  @JsonKey(
    
    name: r'expiresInSeconds',
    required: false,
    includeIfNull: false,
  )


  final int? expiresInSeconds;



      /// Yalnız mobil istemcide
  @JsonKey(
    
    name: r'refreshToken',
    required: false,
    includeIfNull: false,
  )


  final String? refreshToken;



      /// result = SignupRequired; 15 dk geçerli
  @JsonKey(
    
    name: r'signupToken',
    required: false,
    includeIfNull: false,
  )


  final String? signupToken;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageAuthResult &&
      other.result == result &&
      other.accessToken == accessToken &&
      other.expiresInSeconds == expiresInSeconds &&
      other.refreshToken == refreshToken &&
      other.signupToken == signupToken;

    @override
    int get hashCode =>
        result.hashCode +
        accessToken.hashCode +
        expiresInSeconds.hashCode +
        refreshToken.hashCode +
        signupToken.hashCode;

  factory ManageAuthResult.fromJson(Map<String, dynamic> json) => _$ManageAuthResultFromJson(json);

  Map<String, dynamic> toJson() => _$ManageAuthResultToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum ManageAuthResultResultEnum {
@JsonValue(r'SignedIn')
signedIn(r'SignedIn'),
@JsonValue(r'SignupRequired')
signupRequired(r'SignupRequired'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const ManageAuthResultResultEnum(this.value);

final String value;

@override
String toString() => value;
}


