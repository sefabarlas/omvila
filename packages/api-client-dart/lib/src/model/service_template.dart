//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'service_template.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ServiceTemplate {
  /// Returns a new [ServiceTemplate] instance.
  ServiceTemplate({

    required  this.id,

    required  this.categoryName,

    required  this.name,

    required  this.defaultDurationMinutes,
  });

  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



  @JsonKey(
    
    name: r'categoryName',
    required: true,
    includeIfNull: false,
  )


  final String categoryName;



  @JsonKey(
    
    name: r'name',
    required: true,
    includeIfNull: false,
  )


  final String name;



  @JsonKey(
    
    name: r'defaultDurationMinutes',
    required: true,
    includeIfNull: false,
  )


  final int defaultDurationMinutes;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ServiceTemplate &&
      other.id == id &&
      other.categoryName == categoryName &&
      other.name == name &&
      other.defaultDurationMinutes == defaultDurationMinutes;

    @override
    int get hashCode =>
        id.hashCode +
        categoryName.hashCode +
        name.hashCode +
        defaultDurationMinutes.hashCode;

  factory ServiceTemplate.fromJson(Map<String, dynamic> json) => _$ServiceTemplateFromJson(json);

  Map<String, dynamic> toJson() => _$ServiceTemplateToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

