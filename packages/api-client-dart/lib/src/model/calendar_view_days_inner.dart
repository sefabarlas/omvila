//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/calendar_view_days_inner_staff_hours_inner.dart';
import 'package:omvila_api/src/model/time_interval.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'calendar_view_days_inner.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class CalendarViewDaysInner {
  /// Returns a new [CalendarViewDaysInner] instance.
  CalendarViewDaysInner({

    required  this.date,

    required  this.isClosed,

     this.closureNote,

     this.branchIntervals,

    required  this.staffHours,
  });

  @JsonKey(
    
    name: r'date',
    required: true,
    includeIfNull: false,
  )


  final DateTime date;



  @JsonKey(
    
    name: r'isClosed',
    required: true,
    includeIfNull: false,
  )


  final bool isClosed;



  @JsonKey(
    
    name: r'closureNote',
    required: false,
    includeIfNull: false,
  )


  final String? closureNote;



  @JsonKey(
    
    name: r'branchIntervals',
    required: false,
    includeIfNull: false,
  )


  final List<TimeInterval>? branchIntervals;



  @JsonKey(
    
    name: r'staffHours',
    required: true,
    includeIfNull: false,
  )


  final List<CalendarViewDaysInnerStaffHoursInner> staffHours;





    @override
    bool operator ==(Object other) => identical(this, other) || other is CalendarViewDaysInner &&
      other.date == date &&
      other.isClosed == isClosed &&
      other.closureNote == closureNote &&
      other.branchIntervals == branchIntervals &&
      other.staffHours == staffHours;

    @override
    int get hashCode =>
        date.hashCode +
        isClosed.hashCode +
        (closureNote == null ? 0 : closureNote.hashCode) +
        branchIntervals.hashCode +
        staffHours.hashCode;

  factory CalendarViewDaysInner.fromJson(Map<String, dynamic> json) => _$CalendarViewDaysInnerFromJson(json);

  Map<String, dynamic> toJson() => _$CalendarViewDaysInnerToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

