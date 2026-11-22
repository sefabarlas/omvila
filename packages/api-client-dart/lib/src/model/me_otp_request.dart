//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'me_otp_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MeOtpRequest {
  /// Returns a new [MeOtpRequest] instance.
  MeOtpRequest({

    required  this.phone,
  });

      /// Serbest biçimli telefon girişi; sunucu E.164'e çevirir (\"0532 418 77 05\", \"5324187705\", \"+90 532 418 7705\")
  @JsonKey(
    
    name: r'phone',
    required: true,
    includeIfNull: false,
  )


  final String phone;





    @override
    bool operator ==(Object other) => identical(this, other) || other is MeOtpRequest &&
      other.phone == phone;

    @override
    int get hashCode =>
        phone.hashCode;

  factory MeOtpRequest.fromJson(Map<String, dynamic> json) => _$MeOtpRequestFromJson(json);

  Map<String, dynamic> toJson() => _$MeOtpRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

