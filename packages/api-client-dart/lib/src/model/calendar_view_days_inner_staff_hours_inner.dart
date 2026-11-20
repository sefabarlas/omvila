//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/time_interval.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'calendar_view_days_inner_staff_hours_inner.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class CalendarViewDaysInnerStaffHoursInner {
  /// Returns a new [CalendarViewDaysInnerStaffHoursInner] instance.
  CalendarViewDaysInnerStaffHoursInner({

    required  this.staffId,

    required  this.intervals,
  });

  @JsonKey(
    
    name: r'staffId',
    required: true,
    includeIfNull: false,
  )


  final String staffId;



  @JsonKey(
    
    name: r'intervals',
    required: true,
    includeIfNull: false,
  )


  final List<TimeInterval> intervals;





    @override
    bool operator ==(Object other) => identical(this, other) || other is CalendarViewDaysInnerStaffHoursInner &&
      other.staffId == staffId &&
      other.intervals == intervals;

    @override
    int get hashCode =>
        staffId.hashCode +
        intervals.hashCode;

  factory CalendarViewDaysInnerStaffHoursInner.fromJson(Map<String, dynamic> json) => _$CalendarViewDaysInnerStaffHoursInnerFromJson(json);

  Map<String, dynamic> toJson() => _$CalendarViewDaysInnerStaffHoursInnerToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

