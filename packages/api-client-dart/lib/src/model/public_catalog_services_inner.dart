//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/money.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'public_catalog_services_inner.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PublicCatalogServicesInner {
  /// Returns a new [PublicCatalogServicesInner] instance.
  PublicCatalogServicesInner({

    required  this.id,

    required  this.categoryId,

    required  this.name,

     this.description,

    required  this.durationMinutes,

     this.price,

    required  this.priceDisplay,

    required  this.hasDeposit,

     this.depositHint,

    required  this.staffIds,
  });

  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



  @JsonKey(
    
    name: r'categoryId',
    required: true,
    includeIfNull: false,
  )


  final String categoryId;



  @JsonKey(
    
    name: r'name',
    required: true,
    includeIfNull: false,
  )


  final String name;



  @JsonKey(
    
    name: r'description',
    required: false,
    includeIfNull: false,
  )


  final String? description;



  @JsonKey(
    
    name: r'durationMinutes',
    required: true,
    includeIfNull: false,
  )


  final int durationMinutes;



      /// showPrices false ise null
  @JsonKey(
    
    name: r'price',
    required: false,
    includeIfNull: false,
  )


  final Money? price;



  @JsonKey(
    
    name: r'priceDisplay',
    required: true,
    includeIfNull: false,
  unknownEnumValue: PublicCatalogServicesInnerPriceDisplayEnum.unknownDefaultOpenApi,
  )


  final PublicCatalogServicesInnerPriceDisplayEnum priceDisplay;



      /// Bilgi amaçlı (\"Kapora 300 ₺\"); kesin kapora oluşturmada müşteriye göre hesaplanır
  @JsonKey(
    
    name: r'hasDeposit',
    required: true,
    includeIfNull: false,
  )


  final bool hasDeposit;



  @JsonKey(
    
    name: r'depositHint',
    required: false,
    includeIfNull: false,
  )


  final Money? depositHint;



  @JsonKey(
    
    name: r'staffIds',
    required: true,
    includeIfNull: false,
  )


  final List<String> staffIds;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PublicCatalogServicesInner &&
      other.id == id &&
      other.categoryId == categoryId &&
      other.name == name &&
      other.description == description &&
      other.durationMinutes == durationMinutes &&
      other.price == price &&
      other.priceDisplay == priceDisplay &&
      other.hasDeposit == hasDeposit &&
      other.depositHint == depositHint &&
      other.staffIds == staffIds;

    @override
    int get hashCode =>
        id.hashCode +
        categoryId.hashCode +
        name.hashCode +
        (description == null ? 0 : description.hashCode) +
        durationMinutes.hashCode +
        (price == null ? 0 : price.hashCode) +
        priceDisplay.hashCode +
        hasDeposit.hashCode +
        (depositHint == null ? 0 : depositHint.hashCode) +
        staffIds.hashCode;

  factory PublicCatalogServicesInner.fromJson(Map<String, dynamic> json) => _$PublicCatalogServicesInnerFromJson(json);

  Map<String, dynamic> toJson() => _$PublicCatalogServicesInnerToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum PublicCatalogServicesInnerPriceDisplayEnum {
@JsonValue(r'Fixed')
fixed(r'Fixed'),
@JsonValue(r'StartingFrom')
startingFrom(r'StartingFrom'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const PublicCatalogServicesInnerPriceDisplayEnum(this.value);

final String value;

@override
String toString() => value;
}


