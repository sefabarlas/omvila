//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'package_redemption_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PackageRedemptionRequest {
  /// Returns a new [PackageRedemptionRequest] instance.
  PackageRedemptionRequest({

    required  this.appointmentItemId,

    required  this.customerPackageItemId,
  });

  @JsonKey(
    
    name: r'appointmentItemId',
    required: true,
    includeIfNull: false,
  )


  final String appointmentItemId;



  @JsonKey(
    
    name: r'customerPackageItemId',
    required: true,
    includeIfNull: false,
  )


  final String customerPackageItemId;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PackageRedemptionRequest &&
      other.appointmentItemId == appointmentItemId &&
      other.customerPackageItemId == customerPackageItemId;

    @override
    int get hashCode =>
        appointmentItemId.hashCode +
        customerPackageItemId.hashCode;

  factory PackageRedemptionRequest.fromJson(Map<String, dynamic> json) => _$PackageRedemptionRequestFromJson(json);

  Map<String, dynamic> toJson() => _$PackageRedemptionRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

