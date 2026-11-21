//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/waitlist_entry_status.dart';
import 'package:omvila_api/src/model/customer_ref.dart';
import 'package:omvila_api/src/model/waitlist_offer_view.dart';
import 'package:omvila_api/src/model/waitlist_time_window.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'waitlist_entry_view.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class WaitlistEntryView {
  /// Returns a new [WaitlistEntryView] instance.
  WaitlistEntryView({

    required  this.id,

    required  this.branchId,

    required  this.status,

    required  this.customer,

    required  this.serviceIds,

    required  this.serviceNames,

     this.preferredStaffId,

     this.preferredStaffName,

    required  this.acceptOtherStaff,

    required  this.dates,

    required  this.timeWindows,

     this.note,

    required  this.joinedUtc,

    required  this.source_,

    required  this.pendingOffer,

    required  this.version,
  });

  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



  @JsonKey(
    
    name: r'branchId',
    required: true,
    includeIfNull: false,
  )


  final String branchId;



  @JsonKey(
    
    name: r'status',
    required: true,
    includeIfNull: false,
  unknownEnumValue: WaitlistEntryStatus.unknownDefaultOpenApi,
  )


  final WaitlistEntryStatus status;



  @JsonKey(
    
    name: r'customer',
    required: true,
    includeIfNull: false,
  )


  final CustomerRef customer;



  @JsonKey(
    
    name: r'serviceIds',
    required: true,
    includeIfNull: false,
  )


  final List<String> serviceIds;



  @JsonKey(
    
    name: r'serviceNames',
    required: true,
    includeIfNull: false,
  )


  final List<String> serviceNames;



  @JsonKey(
    
    name: r'preferredStaffId',
    required: false,
    includeIfNull: false,
  )


  final String? preferredStaffId;



      /// null ise \"Fark etmez\"
  @JsonKey(
    
    name: r'preferredStaffName',
    required: false,
    includeIfNull: false,
  )


  final String? preferredStaffName;



  @JsonKey(
    
    name: r'acceptOtherStaff',
    required: true,
    includeIfNull: false,
  )


  final bool acceptOtherStaff;



  @JsonKey(
    
    name: r'dates',
    required: true,
    includeIfNull: false,
  )


  final List<DateTime> dates;



  @JsonKey(
    
    name: r'timeWindows',
    required: true,
    includeIfNull: false,
  unknownEnumValue: WaitlistTimeWindow.unknownDefaultOpenApi,
  )


  final List<WaitlistTimeWindow> timeWindows;



  @JsonKey(
    
    name: r'note',
    required: false,
    includeIfNull: false,
  )


  final String? note;



      /// \"3 gündür bekliyor\"
  @JsonKey(
    
    name: r'joinedUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime joinedUtc;



  @JsonKey(
    
    name: r'source',
    required: true,
    includeIfNull: false,
  unknownEnumValue: WaitlistEntryViewSource_Enum.unknownDefaultOpenApi,
  )


  final WaitlistEntryViewSource_Enum source_;



      /// Bekleyen teklif (\"Teklif gönderildi\"); yoksa null
  @JsonKey(
    
    name: r'pendingOffer',
    required: true,
    includeIfNull: true,
  )


  final WaitlistOfferView? pendingOffer;



      /// Kaynak sürümü (ETag ile aynı değer)
  @JsonKey(
    
    name: r'version',
    required: true,
    includeIfNull: false,
  )


  final String version;





    @override
    bool operator ==(Object other) => identical(this, other) || other is WaitlistEntryView &&
      other.id == id &&
      other.branchId == branchId &&
      other.status == status &&
      other.customer == customer &&
      other.serviceIds == serviceIds &&
      other.serviceNames == serviceNames &&
      other.preferredStaffId == preferredStaffId &&
      other.preferredStaffName == preferredStaffName &&
      other.acceptOtherStaff == acceptOtherStaff &&
      other.dates == dates &&
      other.timeWindows == timeWindows &&
      other.note == note &&
      other.joinedUtc == joinedUtc &&
      other.source_ == source_ &&
      other.pendingOffer == pendingOffer &&
      other.version == version;

    @override
    int get hashCode =>
        id.hashCode +
        branchId.hashCode +
        status.hashCode +
        customer.hashCode +
        serviceIds.hashCode +
        serviceNames.hashCode +
        (preferredStaffId == null ? 0 : preferredStaffId.hashCode) +
        (preferredStaffName == null ? 0 : preferredStaffName.hashCode) +
        acceptOtherStaff.hashCode +
        dates.hashCode +
        timeWindows.hashCode +
        (note == null ? 0 : note.hashCode) +
        joinedUtc.hashCode +
        source_.hashCode +
        (pendingOffer == null ? 0 : pendingOffer.hashCode) +
        version.hashCode;

  factory WaitlistEntryView.fromJson(Map<String, dynamic> json) => _$WaitlistEntryViewFromJson(json);

  Map<String, dynamic> toJson() => _$WaitlistEntryViewToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum WaitlistEntryViewSource_Enum {
@JsonValue(r'Online')
online(r'Online'),
@JsonValue(r'Panel')
panel(r'Panel'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const WaitlistEntryViewSource_Enum(this.value);

final String value;

@override
String toString() => value;
}


