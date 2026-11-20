//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/industry_code.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'brand.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class Brand {
  /// Returns a new [Brand] instance.
  Brand({

    required  this.id,

    required  this.name,

    required  this.slug,

    required  this.industryCode,

    required  this.status,

     this.shortDescription,

     this.logoUrl,

     this.phone,

     this.email,

     this.instagram,

     this.website,

     this.legalName,

     this.taxOffice,

     this.taxNumber,

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
    
    name: r'slug',
    required: true,
    includeIfNull: false,
  )


  final String slug;



  @JsonKey(
    
    name: r'industryCode',
    required: true,
    includeIfNull: false,
  unknownEnumValue: IndustryCode.unknownDefaultOpenApi,
  )


  final IndustryCode industryCode;



  @JsonKey(
    
    name: r'status',
    required: true,
    includeIfNull: false,
  unknownEnumValue: BrandStatusEnum.unknownDefaultOpenApi,
  )


  final BrandStatusEnum status;



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
    
    name: r'phone',
    required: false,
    includeIfNull: false,
  )


  final String? phone;



  @JsonKey(
    
    name: r'email',
    required: false,
    includeIfNull: false,
  )


  final String? email;



  @JsonKey(
    
    name: r'instagram',
    required: false,
    includeIfNull: false,
  )


  final String? instagram;



  @JsonKey(
    
    name: r'website',
    required: false,
    includeIfNull: false,
  )


  final String? website;



  @JsonKey(
    
    name: r'legalName',
    required: false,
    includeIfNull: false,
  )


  final String? legalName;



  @JsonKey(
    
    name: r'taxOffice',
    required: false,
    includeIfNull: false,
  )


  final String? taxOffice;



  @JsonKey(
    
    name: r'taxNumber',
    required: false,
    includeIfNull: false,
  )


  final String? taxNumber;



      /// Kaynak sürümü (ETag ile aynı değer)
  @JsonKey(
    
    name: r'version',
    required: true,
    includeIfNull: false,
  )


  final String version;





    @override
    bool operator ==(Object other) => identical(this, other) || other is Brand &&
      other.id == id &&
      other.name == name &&
      other.slug == slug &&
      other.industryCode == industryCode &&
      other.status == status &&
      other.shortDescription == shortDescription &&
      other.logoUrl == logoUrl &&
      other.phone == phone &&
      other.email == email &&
      other.instagram == instagram &&
      other.website == website &&
      other.legalName == legalName &&
      other.taxOffice == taxOffice &&
      other.taxNumber == taxNumber &&
      other.version == version;

    @override
    int get hashCode =>
        id.hashCode +
        name.hashCode +
        slug.hashCode +
        industryCode.hashCode +
        status.hashCode +
        (shortDescription == null ? 0 : shortDescription.hashCode) +
        (logoUrl == null ? 0 : logoUrl.hashCode) +
        (phone == null ? 0 : phone.hashCode) +
        (email == null ? 0 : email.hashCode) +
        (instagram == null ? 0 : instagram.hashCode) +
        (website == null ? 0 : website.hashCode) +
        (legalName == null ? 0 : legalName.hashCode) +
        (taxOffice == null ? 0 : taxOffice.hashCode) +
        (taxNumber == null ? 0 : taxNumber.hashCode) +
        version.hashCode;

  factory Brand.fromJson(Map<String, dynamic> json) => _$BrandFromJson(json);

  Map<String, dynamic> toJson() => _$BrandToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum BrandStatusEnum {
@JsonValue(r'Onboarding')
onboarding(r'Onboarding'),
@JsonValue(r'Active')
active(r'Active'),
@JsonValue(r'Suspended')
suspended(r'Suspended'),
@JsonValue(r'Archived')
archived(r'Archived'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const BrandStatusEnum(this.value);

final String value;

@override
String toString() => value;
}


