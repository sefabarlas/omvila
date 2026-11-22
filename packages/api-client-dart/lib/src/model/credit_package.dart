//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/money.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'credit_package.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class CreditPackage {
  /// Returns a new [CreditPackage] instance.
  CreditPackage({

    required  this.id,

    required  this.code,

    required  this.name,

    required  this.credits,

    required  this.price,
  });

      /// Paket kimliği
  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



      /// Paket kodu
  @JsonKey(
    
    name: r'code',
    required: true,
    includeIfNull: false,
  )


  final String code;



      /// Ekrandaki ad (Başlangıç, Orta, Büyük)
  @JsonKey(
    
    name: r'name',
    required: true,
    includeIfNull: false,
  )


  final String name;



      /// Kontör (SMS) sayısı
          // minimum: 1
  @JsonKey(
    
    name: r'credits',
    required: true,
    includeIfNull: false,
  )


  final int credits;



      /// Fiyat (KDV dahil); tanımlı değilse null
  @JsonKey(
    
    name: r'price',
    required: true,
    includeIfNull: true,
  )


  final Money? price;





    @override
    bool operator ==(Object other) => identical(this, other) || other is CreditPackage &&
      other.id == id &&
      other.code == code &&
      other.name == name &&
      other.credits == credits &&
      other.price == price;

    @override
    int get hashCode =>
        id.hashCode +
        code.hashCode +
        name.hashCode +
        credits.hashCode +
        (price == null ? 0 : price.hashCode);

  factory CreditPackage.fromJson(Map<String, dynamic> json) => _$CreditPackageFromJson(json);

  Map<String, dynamic> toJson() => _$CreditPackageToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

