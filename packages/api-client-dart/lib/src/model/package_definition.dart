//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/package_definition_items_inner.dart';
import 'package:omvila_api/src/model/money.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'package_definition.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PackageDefinition {
  /// Returns a new [PackageDefinition] instance.
  PackageDefinition({

    required  this.id,

    required  this.name,

    required  this.price,

    required  this.items,

    required  this.totalSessions,

    required  this.validityDays,

     this.saleCommissionPercent,

    required  this.isAvailableAllBranches,

    required  this.isActive,

    required  this.soldCount,

    required  this.activeCount,

    required  this.version,
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
    
    name: r'price',
    required: true,
    includeIfNull: false,
  )


  final Money price;



  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<PackageDefinitionItemsInner> items;



      /// \"Seans\" sütunu
  @JsonKey(
    
    name: r'totalSessions',
    required: true,
    includeIfNull: false,
  )


  final int totalSessions;



  @JsonKey(
    
    name: r'validityDays',
    required: true,
    includeIfNull: false,
  )


  final int validityDays;



  @JsonKey(
    
    name: r'saleCommissionPercent',
    required: false,
    includeIfNull: false,
  )


  final num? saleCommissionPercent;



  @JsonKey(
    
    name: r'isAvailableAllBranches',
    required: true,
    includeIfNull: false,
  )


  final bool isAvailableAllBranches;



  @JsonKey(
    
    name: r'isActive',
    required: true,
    includeIfNull: false,
  )


  final bool isActive;



      /// \"Satılan\"
  @JsonKey(
    
    name: r'soldCount',
    required: true,
    includeIfNull: false,
  )


  final int soldCount;



      /// \"Aktif\" (Active müşteri paketi sayısı)
  @JsonKey(
    
    name: r'activeCount',
    required: true,
    includeIfNull: false,
  )


  final int activeCount;



      /// Kaynak sürümü (ETag ile aynı değer)
  @JsonKey(
    
    name: r'version',
    required: true,
    includeIfNull: false,
  )


  final String version;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PackageDefinition &&
      other.id == id &&
      other.name == name &&
      other.price == price &&
      other.items == items &&
      other.totalSessions == totalSessions &&
      other.validityDays == validityDays &&
      other.saleCommissionPercent == saleCommissionPercent &&
      other.isAvailableAllBranches == isAvailableAllBranches &&
      other.isActive == isActive &&
      other.soldCount == soldCount &&
      other.activeCount == activeCount &&
      other.version == version;

    @override
    int get hashCode =>
        id.hashCode +
        name.hashCode +
        price.hashCode +
        items.hashCode +
        totalSessions.hashCode +
        validityDays.hashCode +
        (saleCommissionPercent == null ? 0 : saleCommissionPercent.hashCode) +
        isAvailableAllBranches.hashCode +
        isActive.hashCode +
        soldCount.hashCode +
        activeCount.hashCode +
        version.hashCode;

  factory PackageDefinition.fromJson(Map<String, dynamic> json) => _$PackageDefinitionFromJson(json);

  Map<String, dynamic> toJson() => _$PackageDefinitionToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

