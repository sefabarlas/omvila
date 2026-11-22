//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/working_hours_override.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'manage_list_branch_overrides200_response.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageListBranchOverrides200Response {
  /// Returns a new [ManageListBranchOverrides200Response] instance.
  ManageListBranchOverrides200Response({

    required  this.items,
  });

  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<WorkingHoursOverride> items;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageListBranchOverrides200Response &&
      other.items == items;

    @override
    int get hashCode =>
        items.hashCode;

  factory ManageListBranchOverrides200Response.fromJson(Map<String, dynamic> json) => _$ManageListBranchOverrides200ResponseFromJson(json);

  Map<String, dynamic> toJson() => _$ManageListBranchOverrides200ResponseToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

