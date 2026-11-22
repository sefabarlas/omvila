//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/money.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_credit_package.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminCreditPackage {
  /// Returns a new [AdminCreditPackage] instance.
  AdminCreditPackage({

    required  this.code,

    required  this.name,

    required  this.credits,

    required  this.price,

    required  this.isActive,
  });

      /// Paket kodu
  @JsonKey(
    
    name: r'code',
    required: true,
    includeIfNull: false,
  )


  final String code;



      /// \"Paket\"
  @JsonKey(
    
    name: r'name',
    required: true,
    includeIfNull: false,
  )


  final String name;



      /// \"SMS\"
          // minimum: 1
  @JsonKey(
    
    name: r'credits',
    required: true,
    includeIfNull: false,
  )


  final int credits;



      /// \"Fiyat\"; null = belirlenmedi
  @JsonKey(
    
    name: r'price',
    required: true,
    includeIfNull: true,
  )


  final Money? price;



      /// Satışta
  @JsonKey(
    
    name: r'isActive',
    required: true,
    includeIfNull: false,
  )


  final bool isActive;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminCreditPackage &&
      other.code == code &&
      other.name == name &&
      other.credits == credits &&
      other.price == price &&
      other.isActive == isActive;

    @override
    int get hashCode =>
        code.hashCode +
        name.hashCode +
        credits.hashCode +
        (price == null ? 0 : price.hashCode) +
        isActive.hashCode;

  factory AdminCreditPackage.fromJson(Map<String, dynamic> json) => _$AdminCreditPackageFromJson(json);

  Map<String, dynamic> toJson() => _$AdminCreditPackageToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

