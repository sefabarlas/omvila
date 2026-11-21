//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/waitlist_time_window.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'waitlist_preferences.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class WaitlistPreferences {
  /// Returns a new [WaitlistPreferences] instance.
  WaitlistPreferences({

    required  this.serviceIds,

     this.preferredStaffId,

    required  this.acceptOtherStaff,

    required  this.dates,

    required  this.timeWindows,
  });

  @JsonKey(
    
    name: r'serviceIds',
    required: true,
    includeIfNull: false,
  )


  final List<String> serviceIds;



      /// null = fark etmez
  @JsonKey(
    
    name: r'preferredStaffId',
    required: false,
    includeIfNull: false,
  )


  final String? preferredStaffId;



      /// \"Başka personel de olur\"
  @JsonKey(
    
    name: r'acceptOtherStaff',
    required: true,
    includeIfNull: false,
  )


  final bool acceptOtherStaff;



      /// İstenen yerel günler
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
    bool operator ==(Object other) => identical(this, other) || other is WaitlistPreferences &&
      other.serviceIds == serviceIds &&
      other.preferredStaffId == preferredStaffId &&
      other.acceptOtherStaff == acceptOtherStaff &&
      other.dates == dates &&
      other.timeWindows == timeWindows;

    @override
    int get hashCode =>
        serviceIds.hashCode +
        (preferredStaffId == null ? 0 : preferredStaffId.hashCode) +
        acceptOtherStaff.hashCode +
        dates.hashCode +
        timeWindows.hashCode;

  factory WaitlistPreferences.fromJson(Map<String, dynamic> json) => _$WaitlistPreferencesFromJson(json);

  Map<String, dynamic> toJson() => _$WaitlistPreferencesToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

