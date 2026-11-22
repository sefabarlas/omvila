//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'phone_reveal_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PhoneRevealRequest {
  /// Returns a new [PhoneRevealRequest] instance.
  PhoneRevealRequest({

    required  this.purpose,
  });

      /// Call = \"Ara\"; Sms = \"SMS\"
  @JsonKey(
    
    name: r'purpose',
    required: true,
    includeIfNull: false,
  unknownEnumValue: PhoneRevealRequestPurposeEnum.unknownDefaultOpenApi,
  )


  final PhoneRevealRequestPurposeEnum purpose;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PhoneRevealRequest &&
      other.purpose == purpose;

    @override
    int get hashCode =>
        purpose.hashCode;

  factory PhoneRevealRequest.fromJson(Map<String, dynamic> json) => _$PhoneRevealRequestFromJson(json);

  Map<String, dynamic> toJson() => _$PhoneRevealRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

/// Call = \"Ara\"; Sms = \"SMS\"
enum PhoneRevealRequestPurposeEnum {
@JsonValue(r'Call')
call(r'Call'),
@JsonValue(r'Sms')
sms(r'Sms'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const PhoneRevealRequestPurposeEnum(this.value);

final String value;

@override
String toString() => value;
}


