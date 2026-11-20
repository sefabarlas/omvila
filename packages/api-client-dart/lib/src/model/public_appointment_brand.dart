//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'public_appointment_brand.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PublicAppointmentBrand {
  /// Returns a new [PublicAppointmentBrand] instance.
  PublicAppointmentBrand({

    required  this.name,

    required  this.slug,

     this.logoUrl,
  });

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





    @override
    bool operator ==(Object other) => identical(this, other) || other is PublicAppointmentBrand &&
      other.name == name &&
      other.slug == slug &&
      other.logoUrl == logoUrl;

    @override
    int get hashCode =>
        name.hashCode +
        slug.hashCode +
        (logoUrl == null ? 0 : logoUrl.hashCode);

  factory PublicAppointmentBrand.fromJson(Map<String, dynamic> json) => _$PublicAppointmentBrandFromJson(json);

  Map<String, dynamic> toJson() => _$PublicAppointmentBrandToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

