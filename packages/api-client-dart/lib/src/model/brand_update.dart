//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/industry_code.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'brand_update.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class BrandUpdate {
  /// Returns a new [BrandUpdate] instance.
  BrandUpdate({

     this.name,

     this.industryCode,

     this.shortDescription,

     this.phone,

     this.email,

     this.instagram,

     this.website,

     this.legalName,

     this.taxOffice,

     this.taxNumber,
  });

  @JsonKey(
    
    name: r'name',
    required: false,
    includeIfNull: false,
  )


  final String? name;



  @JsonKey(
    
    name: r'industryCode',
    required: false,
    includeIfNull: false,
  unknownEnumValue: IndustryCode.unknownDefaultOpenApi,
  )


  final IndustryCode? industryCode;



  @JsonKey(
    
    name: r'shortDescription',
    required: false,
    includeIfNull: false,
  )


  final String? shortDescription;



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





    @override
    bool operator ==(Object other) => identical(this, other) || other is BrandUpdate &&
      other.name == name &&
      other.industryCode == industryCode &&
      other.shortDescription == shortDescription &&
      other.phone == phone &&
      other.email == email &&
      other.instagram == instagram &&
      other.website == website &&
      other.legalName == legalName &&
      other.taxOffice == taxOffice &&
      other.taxNumber == taxNumber;

    @override
    int get hashCode =>
        name.hashCode +
        industryCode.hashCode +
        (shortDescription == null ? 0 : shortDescription.hashCode) +
        (phone == null ? 0 : phone.hashCode) +
        (email == null ? 0 : email.hashCode) +
        (instagram == null ? 0 : instagram.hashCode) +
        (website == null ? 0 : website.hashCode) +
        (legalName == null ? 0 : legalName.hashCode) +
        (taxOffice == null ? 0 : taxOffice.hashCode) +
        (taxNumber == null ? 0 : taxNumber.hashCode);

  factory BrandUpdate.fromJson(Map<String, dynamic> json) => _$BrandUpdateFromJson(json);

  Map<String, dynamic> toJson() => _$BrandUpdateToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

