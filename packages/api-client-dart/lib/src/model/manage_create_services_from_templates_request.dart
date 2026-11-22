//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/manage_create_services_from_templates_request_items_inner.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'manage_create_services_from_templates_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageCreateServicesFromTemplatesRequest {
  /// Returns a new [ManageCreateServicesFromTemplatesRequest] instance.
  ManageCreateServicesFromTemplatesRequest({

    required  this.items,
  });

  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<ManageCreateServicesFromTemplatesRequestItemsInner> items;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageCreateServicesFromTemplatesRequest &&
      other.items == items;

    @override
    int get hashCode =>
        items.hashCode;

  factory ManageCreateServicesFromTemplatesRequest.fromJson(Map<String, dynamic> json) => _$ManageCreateServicesFromTemplatesRequestFromJson(json);

  Map<String, dynamic> toJson() => _$ManageCreateServicesFromTemplatesRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

