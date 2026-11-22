//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/calendar_appointment.dart';
import 'package:omvila_api/src/model/calendar_view_staff_inner.dart';
import 'package:omvila_api/src/model/calendar_view_days_inner.dart';
import 'package:omvila_api/src/model/time_off.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'calendar_view.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class CalendarView {
  /// Returns a new [CalendarView] instance.
  CalendarView({

    required  this.branchId,

    required  this.timeZone,

    required  this.from,

    required  this.to,

    required  this.staff,

    required  this.days,

    required  this.appointments,

    required  this.timeOffs,
  });

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
    
    name: r'from',
    required: true,
    includeIfNull: false,
  )
  @DateOnlyConverter()


  final DateTime from;



  @JsonKey(
    
    name: r'to',
    required: true,
    includeIfNull: false,
  )
  @DateOnlyConverter()


  final DateTime to;



  @JsonKey(
    
    name: r'staff',
    required: true,
    includeIfNull: false,
  )


  final List<CalendarViewStaffInner> staff;



  @JsonKey(
    
    name: r'days',
    required: true,
    includeIfNull: false,
  )


  final List<CalendarViewDaysInner> days;



  @JsonKey(
    
    name: r'appointments',
    required: true,
    includeIfNull: false,
  )


  final List<CalendarAppointment> appointments;



  @JsonKey(
    
    name: r'timeOffs',
    required: true,
    includeIfNull: false,
  )


  final List<TimeOff> timeOffs;





    @override
    bool operator ==(Object other) => identical(this, other) || other is CalendarView &&
      other.branchId == branchId &&
      other.timeZone == timeZone &&
      other.from == from &&
      other.to == to &&
      other.staff == staff &&
      other.days == days &&
      other.appointments == appointments &&
      other.timeOffs == timeOffs;

    @override
    int get hashCode =>
        branchId.hashCode +
        timeZone.hashCode +
        from.hashCode +
        to.hashCode +
        staff.hashCode +
        days.hashCode +
        appointments.hashCode +
        timeOffs.hashCode;

  factory CalendarView.fromJson(Map<String, dynamic> json) => _$CalendarViewFromJson(json);

  Map<String, dynamic> toJson() => _$CalendarViewToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

