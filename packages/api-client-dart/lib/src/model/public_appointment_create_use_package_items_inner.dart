//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'public_appointment_create_use_package_items_inner.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PublicAppointmentCreateUsePackageItemsInner {
  /// Returns a new [PublicAppointmentCreateUsePackageItemsInner] instance.
  PublicAppointmentCreateUsePackageItemsInner({

    required  this.serviceId,

    required  this.customerPackageItemId,
  });

  @JsonKey(
    
    name: r'serviceId',
    required: true,
    includeIfNull: false,
  )


  final String serviceId;



  @JsonKey(
    
    name: r'customerPackageItemId',
    required: true,
    includeIfNull: false,
  )


  final String customerPackageItemId;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PublicAppointmentCreateUsePackageItemsInner &&
      other.serviceId == serviceId &&
      other.customerPackageItemId == customerPackageItemId;

    @override
    int get hashCode =>
        serviceId.hashCode +
        customerPackageItemId.hashCode;

  factory PublicAppointmentCreateUsePackageItemsInner.fromJson(Map<String, dynamic> json) => _$PublicAppointmentCreateUsePackageItemsInnerFromJson(json);

  Map<String, dynamic> toJson() => _$PublicAppointmentCreateUsePackageItemsInnerToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

