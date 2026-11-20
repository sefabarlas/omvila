//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/staff_summary.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'manage_list_staff200_response.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageListStaff200Response {
  /// Returns a new [ManageListStaff200Response] instance.
  ManageListStaff200Response({

    required  this.items,
  });

  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<StaffSummary> items;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageListStaff200Response &&
      other.items == items;

    @override
    int get hashCode =>
        items.hashCode;

  factory ManageListStaff200Response.fromJson(Map<String, dynamic> json) => _$ManageListStaff200ResponseFromJson(json);

  Map<String, dynamic> toJson() => _$ManageListStaff200ResponseToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

