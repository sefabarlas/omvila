//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/calendar_appointment.dart';
import 'package:omvila_api/src/model/time_off.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'time_off_created.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class TimeOffCreated {
  /// Returns a new [TimeOffCreated] instance.
  TimeOffCreated({

    required  this.timeOff,

    required  this.affectedAppointments,
  });

  @JsonKey(
    
    name: r'timeOff',
    required: true,
    includeIfNull: false,
  )


  final TimeOff timeOff;



  @JsonKey(
    
    name: r'affectedAppointments',
    required: true,
    includeIfNull: false,
  )


  final List<CalendarAppointment> affectedAppointments;





    @override
    bool operator ==(Object other) => identical(this, other) || other is TimeOffCreated &&
      other.timeOff == timeOff &&
      other.affectedAppointments == affectedAppointments;

    @override
    int get hashCode =>
        timeOff.hashCode +
        affectedAppointments.hashCode;

  factory TimeOffCreated.fromJson(Map<String, dynamic> json) => _$TimeOffCreatedFromJson(json);

  Map<String, dynamic> toJson() => _$TimeOffCreatedToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

