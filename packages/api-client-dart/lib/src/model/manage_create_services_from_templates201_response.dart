//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/service.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'manage_create_services_from_templates201_response.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageCreateServicesFromTemplates201Response {
  /// Returns a new [ManageCreateServicesFromTemplates201Response] instance.
  ManageCreateServicesFromTemplates201Response({

    required  this.items,
  });

  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<Service> items;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageCreateServicesFromTemplates201Response &&
      other.items == items;

    @override
    int get hashCode =>
        items.hashCode;

  factory ManageCreateServicesFromTemplates201Response.fromJson(Map<String, dynamic> json) => _$ManageCreateServicesFromTemplates201ResponseFromJson(json);

  Map<String, dynamic> toJson() => _$ManageCreateServicesFromTemplates201ResponseToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

