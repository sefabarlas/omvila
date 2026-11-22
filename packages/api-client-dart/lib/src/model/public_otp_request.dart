//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'public_otp_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PublicOtpRequest {
  /// Returns a new [PublicOtpRequest] instance.
  PublicOtpRequest({

    required  this.phone,

    required  this.branchId,
  });

      /// Serbest biçimli telefon girişi; sunucu E.164'e çevirir (\"0532 418 77 05\", \"5324187705\", \"+90 532 418 7705\")
  @JsonKey(
    
    name: r'phone',
    required: true,
    includeIfNull: false,
  )


  final String phone;



  @JsonKey(
    
    name: r'branchId',
    required: true,
    includeIfNull: false,
  )


  final String branchId;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PublicOtpRequest &&
      other.phone == phone &&
      other.branchId == branchId;

    @override
    int get hashCode =>
        phone.hashCode +
        branchId.hashCode;

  factory PublicOtpRequest.fromJson(Map<String, dynamic> json) => _$PublicOtpRequestFromJson(json);

  Map<String, dynamic> toJson() => _$PublicOtpRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

