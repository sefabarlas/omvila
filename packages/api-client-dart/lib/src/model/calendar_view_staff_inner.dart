//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'calendar_view_staff_inner.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class CalendarViewStaffInner {
  /// Returns a new [CalendarViewStaffInner] instance.
  CalendarViewStaffInner({

    required  this.staffId,

    required  this.fullName,

    required  this.calendarColor,

     this.photoUrl,

    required  this.appointmentCount,

    required  this.hasGapAvailability,
  });

  @JsonKey(
    
    name: r'staffId',
    required: true,
    includeIfNull: false,
  )


  final String staffId;



  @JsonKey(
    
    name: r'fullName',
    required: true,
    includeIfNull: false,
  )


  final String fullName;



  @JsonKey(
    
    name: r'calendarColor',
    required: true,
    includeIfNull: false,
  )


  final String calendarColor;



  @JsonKey(
    
    name: r'photoUrl',
    required: false,
    includeIfNull: false,
  )


  final String? photoUrl;



  @JsonKey(
    
    name: r'appointmentCount',
    required: true,
    includeIfNull: false,
  )


  final int appointmentCount;



      /// \"bekleyen var\" / bekleme diliminde boşluk
  @JsonKey(
    
    name: r'hasGapAvailability',
    required: true,
    includeIfNull: false,
  )


  final bool hasGapAvailability;





    @override
    bool operator ==(Object other) => identical(this, other) || other is CalendarViewStaffInner &&
      other.staffId == staffId &&
      other.fullName == fullName &&
      other.calendarColor == calendarColor &&
      other.photoUrl == photoUrl &&
      other.appointmentCount == appointmentCount &&
      other.hasGapAvailability == hasGapAvailability;

    @override
    int get hashCode =>
        staffId.hashCode +
        fullName.hashCode +
        calendarColor.hashCode +
        (photoUrl == null ? 0 : photoUrl.hashCode) +
        appointmentCount.hashCode +
        hasGapAvailability.hashCode;

  factory CalendarViewStaffInner.fromJson(Map<String, dynamic> json) => _$CalendarViewStaffInnerFromJson(json);

  Map<String, dynamic> toJson() => _$CalendarViewStaffInnerToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

