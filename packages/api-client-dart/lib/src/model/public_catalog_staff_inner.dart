//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/public_catalog_staff_inner_surcharges_inner.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'public_catalog_staff_inner.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PublicCatalogStaffInner {
  /// Returns a new [PublicCatalogStaffInner] instance.
  PublicCatalogStaffInner({

    required  this.id,

    required  this.displayName,

     this.title,

     this.photoUrl,

     this.surcharges,
  });

  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



  @JsonKey(
    
    name: r'displayName',
    required: true,
    includeIfNull: false,
  )


  final String displayName;



  @JsonKey(
    
    name: r'title',
    required: false,
    includeIfNull: false,
  )


  final String? title;



  @JsonKey(
    
    name: r'photoUrl',
    required: false,
    includeIfNull: false,
  )


  final String? photoUrl;



      /// Hizmet bazında ek ücret (\"+50 ₺\")
  @JsonKey(
    
    name: r'surcharges',
    required: false,
    includeIfNull: false,
  )


  final List<PublicCatalogStaffInnerSurchargesInner>? surcharges;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PublicCatalogStaffInner &&
      other.id == id &&
      other.displayName == displayName &&
      other.title == title &&
      other.photoUrl == photoUrl &&
      other.surcharges == surcharges;

    @override
    int get hashCode =>
        id.hashCode +
        displayName.hashCode +
        (title == null ? 0 : title.hashCode) +
        (photoUrl == null ? 0 : photoUrl.hashCode) +
        surcharges.hashCode;

  factory PublicCatalogStaffInner.fromJson(Map<String, dynamic> json) => _$PublicCatalogStaffInnerFromJson(json);

  Map<String, dynamic> toJson() => _$PublicCatalogStaffInnerToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

