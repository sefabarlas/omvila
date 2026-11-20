//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/public_brand_rating.dart';
import 'package:omvila_api/src/model/public_brand_branches_inner.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'public_brand.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PublicBrand {
  /// Returns a new [PublicBrand] instance.
  PublicBrand({

    required  this.id,

    required  this.name,

    required  this.slug,

     this.shortDescription,

     this.logoUrl,

     this.industryLabel,

    required  this.coverColor,

    required  this.showPrices,

    required  this.bookingEnabled,

     this.rating,

    required  this.branches,
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
    
    name: r'slug',
    required: true,
    includeIfNull: false,
  )


  final String slug;



  @JsonKey(
    
    name: r'shortDescription',
    required: false,
    includeIfNull: false,
  )


  final String? shortDescription;



  @JsonKey(
    
    name: r'logoUrl',
    required: false,
    includeIfNull: false,
  )


  final String? logoUrl;



  @JsonKey(
    
    name: r'industryLabel',
    required: false,
    includeIfNull: false,
  )


  final String? industryLabel;



  @JsonKey(
    
    name: r'coverColor',
    required: true,
    includeIfNull: false,
  unknownEnumValue: PublicBrandCoverColorEnum.unknownDefaultOpenApi,
  )


  final PublicBrandCoverColorEnum coverColor;



  @JsonKey(
    
    name: r'showPrices',
    required: true,
    includeIfNull: false,
  )


  final bool showPrices;



  @JsonKey(
    
    name: r'bookingEnabled',
    required: true,
    includeIfNull: false,
  )


  final bool bookingEnabled;



  @JsonKey(
    
    name: r'rating',
    required: false,
    includeIfNull: false,
  )


  final PublicBrandRating? rating;



  @JsonKey(
    
    name: r'branches',
    required: true,
    includeIfNull: false,
  )


  final List<PublicBrandBranchesInner> branches;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PublicBrand &&
      other.id == id &&
      other.name == name &&
      other.slug == slug &&
      other.shortDescription == shortDescription &&
      other.logoUrl == logoUrl &&
      other.industryLabel == industryLabel &&
      other.coverColor == coverColor &&
      other.showPrices == showPrices &&
      other.bookingEnabled == bookingEnabled &&
      other.rating == rating &&
      other.branches == branches;

    @override
    int get hashCode =>
        id.hashCode +
        name.hashCode +
        slug.hashCode +
        (shortDescription == null ? 0 : shortDescription.hashCode) +
        (logoUrl == null ? 0 : logoUrl.hashCode) +
        industryLabel.hashCode +
        coverColor.hashCode +
        showPrices.hashCode +
        bookingEnabled.hashCode +
        (rating == null ? 0 : rating.hashCode) +
        branches.hashCode;

  factory PublicBrand.fromJson(Map<String, dynamic> json) => _$PublicBrandFromJson(json);

  Map<String, dynamic> toJson() => _$PublicBrandToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum PublicBrandCoverColorEnum {
@JsonValue(r'petrol')
petrol(r'petrol'),
@JsonValue(r'mercan')
mercan(r'mercan'),
@JsonValue(r'kum')
kum(r'kum'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const PublicBrandCoverColorEnum(this.value);

final String value;

@override
String toString() => value;
}


