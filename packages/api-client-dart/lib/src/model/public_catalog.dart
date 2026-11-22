//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/public_catalog_services_inner.dart';
import 'package:omvila_api/src/model/public_catalog_categories_inner.dart';
import 'package:omvila_api/src/model/public_catalog_staff_inner.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'public_catalog.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PublicCatalog {
  /// Returns a new [PublicCatalog] instance.
  PublicCatalog({

    required  this.branchId,

    required  this.timeZone,

    required  this.allowStaffSelection,

    required  this.allowAnyStaffOption,

     this.priceNote,

    required  this.categories,

    required  this.services,

    required  this.staff,
  });

  @JsonKey(
    
    name: r'branchId',
    required: true,
    includeIfNull: false,
  )


  final String branchId;



      /// IANA saat dilimi
  @JsonKey(
    
    name: r'timeZone',
    required: true,
    includeIfNull: false,
  )


  final String timeZone;



  @JsonKey(
    
    name: r'allowStaffSelection',
    required: true,
    includeIfNull: false,
  )


  final bool allowStaffSelection;



  @JsonKey(
    
    name: r'allowAnyStaffOption',
    required: true,
    includeIfNull: false,
  )


  final bool allowAnyStaffOption;



  @JsonKey(
    
    name: r'priceNote',
    required: false,
    includeIfNull: false,
  )


  final String? priceNote;



  @JsonKey(
    
    name: r'categories',
    required: true,
    includeIfNull: false,
  )


  final List<PublicCatalogCategoriesInner> categories;



  @JsonKey(
    
    name: r'services',
    required: true,
    includeIfNull: false,
  )


  final List<PublicCatalogServicesInner> services;



  @JsonKey(
    
    name: r'staff',
    required: true,
    includeIfNull: false,
  )


  final List<PublicCatalogStaffInner> staff;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PublicCatalog &&
      other.branchId == branchId &&
      other.timeZone == timeZone &&
      other.allowStaffSelection == allowStaffSelection &&
      other.allowAnyStaffOption == allowAnyStaffOption &&
      other.priceNote == priceNote &&
      other.categories == categories &&
      other.services == services &&
      other.staff == staff;

    @override
    int get hashCode =>
        branchId.hashCode +
        timeZone.hashCode +
        allowStaffSelection.hashCode +
        allowAnyStaffOption.hashCode +
        (priceNote == null ? 0 : priceNote.hashCode) +
        categories.hashCode +
        services.hashCode +
        staff.hashCode;

  factory PublicCatalog.fromJson(Map<String, dynamic> json) => _$PublicCatalogFromJson(json);

  Map<String, dynamic> toJson() => _$PublicCatalogToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

