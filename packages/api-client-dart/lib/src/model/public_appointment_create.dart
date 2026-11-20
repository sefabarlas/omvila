//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/appointment_item_request.dart';
import 'package:omvila_api/src/model/public_appointment_create_use_package_items_inner.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'public_appointment_create.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PublicAppointmentCreate {
  /// Returns a new [PublicAppointmentCreate] instance.
  PublicAppointmentCreate({

    required  this.items,

     this.sameStaffForAll = true,

    required  this.startUtc,

    required  this.fullName,

     this.customerNote,

     this.usePackageItems,

    required  this.privacyNoticeAcknowledged,

    required  this.privacyNoticeVersion,

     this.marketingConsent = false,
  });

  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<AppointmentItemRequest> items;



  @JsonKey(
    defaultValue: true,
    name: r'sameStaffForAll',
    required: false,
    includeIfNull: false,
  )


  final bool? sameStaffForAll;



  @JsonKey(
    
    name: r'startUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime startUtc;



  @JsonKey(
    
    name: r'fullName',
    required: true,
    includeIfNull: false,
  )


  final String fullName;



      /// \"Salona notunuz\"
  @JsonKey(
    
    name: r'customerNote',
    required: false,
    includeIfNull: false,
  )


  final String? customerNote;



      /// Paketten düşülecek kalemler
  @JsonKey(
    
    name: r'usePackageItems',
    required: false,
    includeIfNull: false,
  )


  final List<PublicAppointmentCreateUsePackageItemsInner>? usePackageItems;



      /// Aydınlatma metni okundu
  @JsonKey(
    
    name: r'privacyNoticeAcknowledged',
    required: true,
    includeIfNull: false,
  )


  final bool privacyNoticeAcknowledged;



  @JsonKey(
    
    name: r'privacyNoticeVersion',
    required: true,
    includeIfNull: false,
  )


  final String privacyNoticeVersion;



      /// Salonun kampanyalarından SMS ve e-posta (isteğe bağlı)
  @JsonKey(
    defaultValue: false,
    name: r'marketingConsent',
    required: false,
    includeIfNull: false,
  )


  final bool? marketingConsent;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PublicAppointmentCreate &&
      other.items == items &&
      other.sameStaffForAll == sameStaffForAll &&
      other.startUtc == startUtc &&
      other.fullName == fullName &&
      other.customerNote == customerNote &&
      other.usePackageItems == usePackageItems &&
      other.privacyNoticeAcknowledged == privacyNoticeAcknowledged &&
      other.privacyNoticeVersion == privacyNoticeVersion &&
      other.marketingConsent == marketingConsent;

    @override
    int get hashCode =>
        items.hashCode +
        sameStaffForAll.hashCode +
        startUtc.hashCode +
        fullName.hashCode +
        (customerNote == null ? 0 : customerNote.hashCode) +
        usePackageItems.hashCode +
        privacyNoticeAcknowledged.hashCode +
        privacyNoticeVersion.hashCode +
        marketingConsent.hashCode;

  factory PublicAppointmentCreate.fromJson(Map<String, dynamic> json) => _$PublicAppointmentCreateFromJson(json);

  Map<String, dynamic> toJson() => _$PublicAppointmentCreateToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

