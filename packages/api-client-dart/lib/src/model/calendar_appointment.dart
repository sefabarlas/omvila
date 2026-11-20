//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/appointment_status.dart';
import 'package:omvila_api/src/model/segment_view.dart';
import 'package:omvila_api/src/model/display_status.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'calendar_appointment.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class CalendarAppointment {
  /// Returns a new [CalendarAppointment] instance.
  CalendarAppointment({

    required  this.appointmentId,

    required  this.itemId,

    required  this.staffId,

    required  this.startUtc,

    required  this.endUtc,

    required  this.status,

    required  this.displayStatus,

    required  this.customerName,

    required  this.serviceName,

     this.isNewCustomer,

    required  this.isRecurring,

     this.holdExpiresUtc,

    required  this.segments,
  });

  @JsonKey(
    
    name: r'appointmentId',
    required: true,
    includeIfNull: false,
  )


  final String appointmentId;



  @JsonKey(
    
    name: r'itemId',
    required: true,
    includeIfNull: false,
  )


  final String itemId;



  @JsonKey(
    
    name: r'staffId',
    required: true,
    includeIfNull: true,
  )


  final String? staffId;



  @JsonKey(
    
    name: r'startUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime startUtc;



  @JsonKey(
    
    name: r'endUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime endUtc;



  @JsonKey(
    
    name: r'status',
    required: true,
    includeIfNull: false,
  unknownEnumValue: AppointmentStatus.unknownDefaultOpenApi,
  )


  final AppointmentStatus status;



  @JsonKey(
    
    name: r'displayStatus',
    required: true,
    includeIfNull: false,
  unknownEnumValue: DisplayStatus.unknownDefaultOpenApi,
  )


  final DisplayStatus displayStatus;



  @JsonKey(
    
    name: r'customerName',
    required: true,
    includeIfNull: false,
  )


  final String customerName;



  @JsonKey(
    
    name: r'serviceName',
    required: true,
    includeIfNull: false,
  )


  final String serviceName;



  @JsonKey(
    
    name: r'isNewCustomer',
    required: false,
    includeIfNull: false,
  )


  final bool? isNewCustomer;



  @JsonKey(
    
    name: r'isRecurring',
    required: true,
    includeIfNull: false,
  )


  final bool isRecurring;



  @JsonKey(
    
    name: r'holdExpiresUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? holdExpiresUtc;



  @JsonKey(
    
    name: r'segments',
    required: true,
    includeIfNull: false,
  )


  final List<SegmentView> segments;





    @override
    bool operator ==(Object other) => identical(this, other) || other is CalendarAppointment &&
      other.appointmentId == appointmentId &&
      other.itemId == itemId &&
      other.staffId == staffId &&
      other.startUtc == startUtc &&
      other.endUtc == endUtc &&
      other.status == status &&
      other.displayStatus == displayStatus &&
      other.customerName == customerName &&
      other.serviceName == serviceName &&
      other.isNewCustomer == isNewCustomer &&
      other.isRecurring == isRecurring &&
      other.holdExpiresUtc == holdExpiresUtc &&
      other.segments == segments;

    @override
    int get hashCode =>
        appointmentId.hashCode +
        itemId.hashCode +
        (staffId == null ? 0 : staffId.hashCode) +
        startUtc.hashCode +
        endUtc.hashCode +
        status.hashCode +
        displayStatus.hashCode +
        customerName.hashCode +
        serviceName.hashCode +
        isNewCustomer.hashCode +
        isRecurring.hashCode +
        (holdExpiresUtc == null ? 0 : holdExpiresUtc.hashCode) +
        segments.hashCode;

  factory CalendarAppointment.fromJson(Map<String, dynamic> json) => _$CalendarAppointmentFromJson(json);

  Map<String, dynamic> toJson() => _$CalendarAppointmentToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

