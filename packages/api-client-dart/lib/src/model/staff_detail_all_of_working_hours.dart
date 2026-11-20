//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/weekly_hours.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'staff_detail_all_of_working_hours.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class StaffDetailAllOfWorkingHours {
  /// Returns a new [StaffDetailAllOfWorkingHours] instance.
  StaffDetailAllOfWorkingHours({

    required  this.branchId,

    required  this.hours,
  });

  @JsonKey(
    
    name: r'branchId',
    required: true,
    includeIfNull: false,
  )


  final String branchId;



  @JsonKey(
    
    name: r'hours',
    required: true,
    includeIfNull: false,
  )


  final WeeklyHours hours;





    @override
    bool operator ==(Object other) => identical(this, other) || other is StaffDetailAllOfWorkingHours &&
      other.branchId == branchId &&
      other.hours == hours;

    @override
    int get hashCode =>
        branchId.hashCode +
        hours.hashCode;

  factory StaffDetailAllOfWorkingHours.fromJson(Map<String, dynamic> json) => _$StaffDetailAllOfWorkingHoursFromJson(json);

  Map<String, dynamic> toJson() => _$StaffDetailAllOfWorkingHoursToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

