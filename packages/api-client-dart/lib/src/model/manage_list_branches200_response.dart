//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/branch.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'manage_list_branches200_response.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageListBranches200Response {
  /// Returns a new [ManageListBranches200Response] instance.
  ManageListBranches200Response({

    required  this.items,
  });

  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<Branch> items;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageListBranches200Response &&
      other.items == items;

    @override
    int get hashCode =>
        items.hashCode;

  factory ManageListBranches200Response.fromJson(Map<String, dynamic> json) => _$ManageListBranches200ResponseFromJson(json);

  Map<String, dynamic> toJson() => _$ManageListBranches200ResponseToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

