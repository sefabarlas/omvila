//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/money.dart';
import 'package:omvila_api/src/model/appointment_status.dart';
import 'package:omvila_api/src/model/display_status.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'customer_appointment_row.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class CustomerAppointmentRow {
  /// Returns a new [CustomerAppointmentRow] instance.
  CustomerAppointmentRow({

    required  this.appointmentId,

    required  this.branchId,

    required  this.startUtc,

    required  this.status,

    required  this.displayStatus,

    required  this.serviceNames,

    required  this.staffNames,

    required  this.total,
  });

  @JsonKey(
    
    name: r'appointmentId',
    required: true,
    includeIfNull: false,
  )


  final String appointmentId;



  @JsonKey(
    
    name: r'branchId',
    required: true,
    includeIfNull: false,
  )


  final String branchId;



  @JsonKey(
    
    name: r'startUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime startUtc;



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
    
    name: r'serviceNames',
    required: true,
    includeIfNull: false,
  )


  final List<String> serviceNames;



  @JsonKey(
    
    name: r'staffNames',
    required: true,
    includeIfNull: false,
  )


  final List<String> staffNames;



  @JsonKey(
    
    name: r'total',
    required: true,
    includeIfNull: false,
  )


  final Money total;





    @override
    bool operator ==(Object other) => identical(this, other) || other is CustomerAppointmentRow &&
      other.appointmentId == appointmentId &&
      other.branchId == branchId &&
      other.startUtc == startUtc &&
      other.status == status &&
      other.displayStatus == displayStatus &&
      other.serviceNames == serviceNames &&
      other.staffNames == staffNames &&
      other.total == total;

    @override
    int get hashCode =>
        appointmentId.hashCode +
        branchId.hashCode +
        startUtc.hashCode +
        status.hashCode +
        displayStatus.hashCode +
        serviceNames.hashCode +
        staffNames.hashCode +
        total.hashCode;

  factory CustomerAppointmentRow.fromJson(Map<String, dynamic> json) => _$CustomerAppointmentRowFromJson(json);

  Map<String, dynamic> toJson() => _$CustomerAppointmentRowToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

