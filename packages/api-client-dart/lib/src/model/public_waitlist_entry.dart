//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/waitlist_entry_status.dart';
import 'package:omvila_api/src/model/waitlist_time_window.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'public_waitlist_entry.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PublicWaitlistEntry {
  /// Returns a new [PublicWaitlistEntry] instance.
  PublicWaitlistEntry({

    required  this.id,

    required  this.accessCode,

    required  this.status,

    required  this.branchId,

    required  this.timeZone,

    required  this.serviceNames,

     this.preferredStaffName,

    required  this.acceptOtherStaff,

    required  this.dates,

    required  this.timeWindows,
  });

  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



      /// \"Listeden çık\" bağlantısının kodu
  @JsonKey(
    
    name: r'accessCode',
    required: true,
    includeIfNull: false,
  )


  final String accessCode;



  @JsonKey(
    
    name: r'status',
    required: true,
    includeIfNull: false,
  unknownEnumValue: WaitlistEntryStatus.unknownDefaultOpenApi,
  )


  final WaitlistEntryStatus status;



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
    
    name: r'serviceNames',
    required: true,
    includeIfNull: false,
  )


  final List<String> serviceNames;



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





    @override
    bool operator ==(Object other) => identical(this, other) || other is PublicWaitlistEntry &&
      other.id == id &&
      other.accessCode == accessCode &&
      other.status == status &&
      other.branchId == branchId &&
      other.timeZone == timeZone &&
      other.serviceNames == serviceNames &&
      other.preferredStaffName == preferredStaffName &&
      other.acceptOtherStaff == acceptOtherStaff &&
      other.dates == dates &&
      other.timeWindows == timeWindows;

    @override
    int get hashCode =>
        id.hashCode +
        accessCode.hashCode +
        status.hashCode +
        branchId.hashCode +
        timeZone.hashCode +
        serviceNames.hashCode +
        (preferredStaffName == null ? 0 : preferredStaffName.hashCode) +
        acceptOtherStaff.hashCode +
        dates.hashCode +
        timeWindows.hashCode;

  factory PublicWaitlistEntry.fromJson(Map<String, dynamic> json) => _$PublicWaitlistEntryFromJson(json);

  Map<String, dynamic> toJson() => _$PublicWaitlistEntryToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

