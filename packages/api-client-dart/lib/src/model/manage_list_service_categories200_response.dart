//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/service_category.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'manage_list_service_categories200_response.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageListServiceCategories200Response {
  /// Returns a new [ManageListServiceCategories200Response] instance.
  ManageListServiceCategories200Response({

    required  this.items,
  });

  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<ServiceCategory> items;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageListServiceCategories200Response &&
      other.items == items;

    @override
    int get hashCode =>
        items.hashCode;

  factory ManageListServiceCategories200Response.fromJson(Map<String, dynamic> json) => _$ManageListServiceCategories200ResponseFromJson(json);

  Map<String, dynamic> toJson() => _$ManageListServiceCategories200ResponseToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

