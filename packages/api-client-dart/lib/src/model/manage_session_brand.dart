//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'manage_session_brand.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageSessionBrand {
  /// Returns a new [ManageSessionBrand] instance.
  ManageSessionBrand({

    required  this.id,

    required  this.name,

    required  this.slug,

     this.logoUrl,

    required  this.status,
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
    
    name: r'logoUrl',
    required: false,
    includeIfNull: false,
  )


  final String? logoUrl;



  @JsonKey(
    
    name: r'status',
    required: true,
    includeIfNull: false,
  unknownEnumValue: ManageSessionBrandStatusEnum.unknownDefaultOpenApi,
  )


  final ManageSessionBrandStatusEnum status;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageSessionBrand &&
      other.id == id &&
      other.name == name &&
      other.slug == slug &&
      other.logoUrl == logoUrl &&
      other.status == status;

    @override
    int get hashCode =>
        id.hashCode +
        name.hashCode +
        slug.hashCode +
        (logoUrl == null ? 0 : logoUrl.hashCode) +
        status.hashCode;

  factory ManageSessionBrand.fromJson(Map<String, dynamic> json) => _$ManageSessionBrandFromJson(json);

  Map<String, dynamic> toJson() => _$ManageSessionBrandToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum ManageSessionBrandStatusEnum {
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

const ManageSessionBrandStatusEnum(this.value);

final String value;

@override
String toString() => value;
}


