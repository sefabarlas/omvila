//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/staff_service_assignment.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'manage_set_staff_services_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageSetStaffServicesRequest {
  /// Returns a new [ManageSetStaffServicesRequest] instance.
  ManageSetStaffServicesRequest({

    required  this.items,
  });

  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<StaffServiceAssignment> items;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageSetStaffServicesRequest &&
      other.items == items;

    @override
    int get hashCode =>
        items.hashCode;

  factory ManageSetStaffServicesRequest.fromJson(Map<String, dynamic> json) => _$ManageSetStaffServicesRequestFromJson(json);

  Map<String, dynamic> toJson() => _$ManageSetStaffServicesRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

