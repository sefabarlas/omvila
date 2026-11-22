//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'service_category.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ServiceCategory {
  /// Returns a new [ServiceCategory] instance.
  ServiceCategory({

    required  this.id,

    required  this.name,

    required  this.sortOrder,

    required  this.isActive,

    required  this.serviceCount,
  });

  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



  @JsonKey(
    
    name: r'name',
    required: true,
    includeIfNull: false,
  )


  final String name;



  @JsonKey(
    
    name: r'sortOrder',
    required: true,
    includeIfNull: false,
  )


  final int sortOrder;



  @JsonKey(
    
    name: r'isActive',
    required: true,
    includeIfNull: false,
  )


  final bool isActive;



  @JsonKey(
    
    name: r'serviceCount',
    required: true,
    includeIfNull: false,
  )


  final int serviceCount;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ServiceCategory &&
      other.id == id &&
      other.name == name &&
      other.sortOrder == sortOrder &&
      other.isActive == isActive &&
      other.serviceCount == serviceCount;

    @override
    int get hashCode =>
        id.hashCode +
        name.hashCode +
        sortOrder.hashCode +
        isActive.hashCode +
        serviceCount.hashCode;

  factory ServiceCategory.fromJson(Map<String, dynamic> json) => _$ServiceCategoryFromJson(json);

  Map<String, dynamic> toJson() => _$ServiceCategoryToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

