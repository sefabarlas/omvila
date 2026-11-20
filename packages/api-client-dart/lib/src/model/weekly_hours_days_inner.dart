//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/time_interval.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'weekly_hours_days_inner.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class WeeklyHoursDaysInner {
  /// Returns a new [WeeklyHoursDaysInner] instance.
  WeeklyHoursDaysInner({

    required  this.dayOfWeek,

    required  this.intervals,
  });

      /// ISO gün (1 = Pazartesi … 7 = Pazar)
          // minimum: 1
          // maximum: 7
  @JsonKey(
    
    name: r'dayOfWeek',
    required: true,
    includeIfNull: false,
  )


  final int dayOfWeek;



  @JsonKey(
    
    name: r'intervals',
    required: true,
    includeIfNull: false,
  )


  final List<TimeInterval> intervals;





    @override
    bool operator ==(Object other) => identical(this, other) || other is WeeklyHoursDaysInner &&
      other.dayOfWeek == dayOfWeek &&
      other.intervals == intervals;

    @override
    int get hashCode =>
        dayOfWeek.hashCode +
        intervals.hashCode;

  factory WeeklyHoursDaysInner.fromJson(Map<String, dynamic> json) => _$WeeklyHoursDaysInnerFromJson(json);

  Map<String, dynamic> toJson() => _$WeeklyHoursDaysInnerToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

