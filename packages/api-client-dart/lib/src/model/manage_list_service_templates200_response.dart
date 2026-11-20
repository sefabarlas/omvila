//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/service_template.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'manage_list_service_templates200_response.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageListServiceTemplates200Response {
  /// Returns a new [ManageListServiceTemplates200Response] instance.
  ManageListServiceTemplates200Response({

    required  this.items,
  });

  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<ServiceTemplate> items;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageListServiceTemplates200Response &&
      other.items == items;

    @override
    int get hashCode =>
        items.hashCode;

  factory ManageListServiceTemplates200Response.fromJson(Map<String, dynamic> json) => _$ManageListServiceTemplates200ResponseFromJson(json);

  Map<String, dynamic> toJson() => _$ManageListServiceTemplates200ResponseToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

