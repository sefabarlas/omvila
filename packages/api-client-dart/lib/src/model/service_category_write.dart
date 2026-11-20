//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'service_category_write.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ServiceCategoryWrite {
  /// Returns a new [ServiceCategoryWrite] instance.
  ServiceCategoryWrite({

    required  this.name,

     this.sortOrder,

     this.isActive,
  });

  @JsonKey(
    
    name: r'name',
    required: true,
    includeIfNull: false,
  )


  final String name;



  @JsonKey(
    
    name: r'sortOrder',
    required: false,
    includeIfNull: false,
  )


  final int? sortOrder;



  @JsonKey(
    
    name: r'isActive',
    required: false,
    includeIfNull: false,
  )


  final bool? isActive;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ServiceCategoryWrite &&
      other.name == name &&
      other.sortOrder == sortOrder &&
      other.isActive == isActive;

    @override
    int get hashCode =>
        name.hashCode +
        sortOrder.hashCode +
        isActive.hashCode;

  factory ServiceCategoryWrite.fromJson(Map<String, dynamic> json) => _$ServiceCategoryWriteFromJson(json);

  Map<String, dynamic> toJson() => _$ServiceCategoryWriteToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

