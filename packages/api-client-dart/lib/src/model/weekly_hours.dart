//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/weekly_hours_days_inner.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'weekly_hours.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class WeeklyHours {
  /// Returns a new [WeeklyHours] instance.
  WeeklyHours({

    required  this.days,

     this.version,
  });

  @JsonKey(
    
    name: r'days',
    required: true,
    includeIfNull: false,
  )


  final List<WeeklyHoursDaysInner> days;



      /// Kaynak sürümü (ETag ile aynı değer)
  @JsonKey(
    
    name: r'version',
    required: false,
    includeIfNull: false,
  )


  final String? version;





    @override
    bool operator ==(Object other) => identical(this, other) || other is WeeklyHours &&
      other.days == days &&
      other.version == version;

    @override
    int get hashCode =>
        days.hashCode +
        version.hashCode;

  factory WeeklyHours.fromJson(Map<String, dynamic> json) => _$WeeklyHoursFromJson(json);

  Map<String, dynamic> toJson() => _$WeeklyHoursToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

