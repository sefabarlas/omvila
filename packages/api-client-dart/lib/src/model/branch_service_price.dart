//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/money.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'branch_service_price.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class BranchServicePrice {
  /// Returns a new [BranchServicePrice] instance.
  BranchServicePrice({

    required  this.price,

     this.durationOverrideMinutes,

    required  this.isAvailable,
  });

  @JsonKey(
    
    name: r'price',
    required: true,
    includeIfNull: false,
  )


  final Money price;



          // minimum: 5
  @JsonKey(
    
    name: r'durationOverrideMinutes',
    required: false,
    includeIfNull: false,
  )


  final int? durationOverrideMinutes;



      /// false = hizmet bu şubede verilmiyor
  @JsonKey(
    
    name: r'isAvailable',
    required: true,
    includeIfNull: false,
  )


  final bool isAvailable;





    @override
    bool operator ==(Object other) => identical(this, other) || other is BranchServicePrice &&
      other.price == price &&
      other.durationOverrideMinutes == durationOverrideMinutes &&
      other.isAvailable == isAvailable;

    @override
    int get hashCode =>
        price.hashCode +
        (durationOverrideMinutes == null ? 0 : durationOverrideMinutes.hashCode) +
        isAvailable.hashCode;

  factory BranchServicePrice.fromJson(Map<String, dynamic> json) => _$BranchServicePriceFromJson(json);

  Map<String, dynamic> toJson() => _$BranchServicePriceToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

