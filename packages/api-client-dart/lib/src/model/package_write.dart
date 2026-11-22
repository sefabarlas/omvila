//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/money.dart';
import 'package:omvila_api/src/model/package_item_write.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'package_write.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PackageWrite {
  /// Returns a new [PackageWrite] instance.
  PackageWrite({

    required  this.name,

    required  this.price,

    required  this.items,

    required  this.validityDays,

     this.saleCommissionPercent,

    required  this.isAvailableAllBranches,

    required  this.isActive,
  });

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



      /// \"İçerik\"; aynı hizmet iki kez yazılmaz
  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<PackageItemWrite> items;



      /// \"Geçerlilik\", satış tarihinden itibaren gün (3 ay = 90)
          // minimum: 1
          // maximum: 1095
  @JsonKey(
    
    name: r'validityDays',
    required: true,
    includeIfNull: false,
  )


  final int validityDays;



      /// \"Satış primi\" (Package.SaleCommissionPercentOverride); null ise satan personelin SaleCommissionPercent değeri
          // minimum: 0
          // maximum: 100
  @JsonKey(
    
    name: r'saleCommissionPercent',
    required: false,
    includeIfNull: true,
  )


  final num? saleCommissionPercent;



      /// \"Bütün şubelerde kullanılabilir\"; kapalıysa yalnız satıldığı şubede
  @JsonKey(
    
    name: r'isAvailableAllBranches',
    required: true,
    includeIfNull: false,
  )


  final bool isAvailableAllBranches;



      /// \"Satışta\" / \"Satışta değil\"
  @JsonKey(
    
    name: r'isActive',
    required: true,
    includeIfNull: false,
  )


  final bool isActive;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PackageWrite &&
      other.name == name &&
      other.price == price &&
      other.items == items &&
      other.validityDays == validityDays &&
      other.saleCommissionPercent == saleCommissionPercent &&
      other.isAvailableAllBranches == isAvailableAllBranches &&
      other.isActive == isActive;

    @override
    int get hashCode =>
        name.hashCode +
        price.hashCode +
        items.hashCode +
        validityDays.hashCode +
        (saleCommissionPercent == null ? 0 : saleCommissionPercent.hashCode) +
        isAvailableAllBranches.hashCode +
        isActive.hashCode;

  factory PackageWrite.fromJson(Map<String, dynamic> json) => _$PackageWriteFromJson(json);

  Map<String, dynamic> toJson() => _$PackageWriteToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

