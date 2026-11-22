//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'public_catalog_categories_inner.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PublicCatalogCategoriesInner {
  /// Returns a new [PublicCatalogCategoriesInner] instance.
  PublicCatalogCategoriesInner({

    required  this.id,

    required  this.name,
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





    @override
    bool operator ==(Object other) => identical(this, other) || other is PublicCatalogCategoriesInner &&
      other.id == id &&
      other.name == name;

    @override
    int get hashCode =>
        id.hashCode +
        name.hashCode;

  factory PublicCatalogCategoriesInner.fromJson(Map<String, dynamic> json) => _$PublicCatalogCategoriesInnerFromJson(json);

  Map<String, dynamic> toJson() => _$PublicCatalogCategoriesInnerToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

