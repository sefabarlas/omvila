//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/public_appointment_branch.dart';
import 'package:omvila_api/src/model/public_appointment_brand.dart';
import 'package:omvila_api/src/model/money.dart';
import 'package:omvila_api/src/model/appointment_status.dart';
import 'package:omvila_api/src/model/public_appointment_items_inner.dart';
import 'package:omvila_api/src/model/public_appointment_cancellation.dart';
import 'package:omvila_api/src/model/public_appointment_deposit.dart';
import 'package:omvila_api/src/model/display_status.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'public_appointment.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PublicAppointment {
  /// Returns a new [PublicAppointment] instance.
  PublicAppointment({

    required  this.id,

    required  this.accessCode,

    required  this.manageUrl,

    required  this.status,

    required  this.displayStatus,

    required  this.brand,

    required  this.branch,

    required  this.startUtc,

    required  this.endUtc,

    required  this.timeZone,

     this.holdExpiresUtc,

    required  this.items,

    required  this.total,

    required  this.amountDue,

     this.deposit,

    required  this.canCancel,

     this.cancelDeadlineUtc,

    required  this.canReschedule,

     this.rescheduleDeadlineUtc,

     this.preAppointmentNote,

     this.cancellation,

    required  this.version,
  });

  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



  @JsonKey(
    
    name: r'accessCode',
    required: true,
    includeIfNull: false,
  )


  final String accessCode;



  @JsonKey(
    
    name: r'manageUrl',
    required: true,
    includeIfNull: false,
  )


  final String manageUrl;



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
    
    name: r'brand',
    required: true,
    includeIfNull: false,
  )


  final PublicAppointmentBrand brand;



  @JsonKey(
    
    name: r'branch',
    required: true,
    includeIfNull: false,
  )


  final PublicAppointmentBranch branch;



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



      /// IANA saat dilimi
  @JsonKey(
    
    name: r'timeZone',
    required: true,
    includeIfNull: false,
  )


  final String timeZone;



  @JsonKey(
    
    name: r'holdExpiresUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? holdExpiresUtc;



  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<PublicAppointmentItemsInner> items;



  @JsonKey(
    
    name: r'total',
    required: true,
    includeIfNull: false,
  )


  final Money total;



  @JsonKey(
    
    name: r'amountDue',
    required: true,
    includeIfNull: false,
  )


  final Money amountDue;



  @JsonKey(
    
    name: r'deposit',
    required: false,
    includeIfNull: false,
  )


  final PublicAppointmentDeposit? deposit;



  @JsonKey(
    
    name: r'canCancel',
    required: true,
    includeIfNull: false,
  )


  final bool canCancel;



  @JsonKey(
    
    name: r'cancelDeadlineUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? cancelDeadlineUtc;



  @JsonKey(
    
    name: r'canReschedule',
    required: true,
    includeIfNull: false,
  )


  final bool canReschedule;



  @JsonKey(
    
    name: r'rescheduleDeadlineUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? rescheduleDeadlineUtc;



  @JsonKey(
    
    name: r'preAppointmentNote',
    required: false,
    includeIfNull: false,
  )


  final String? preAppointmentNote;



  @JsonKey(
    
    name: r'cancellation',
    required: false,
    includeIfNull: false,
  )


  final PublicAppointmentCancellation? cancellation;



      /// Kaynak sürümü (ETag ile aynı değer)
  @JsonKey(
    
    name: r'version',
    required: true,
    includeIfNull: false,
  )


  final String version;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PublicAppointment &&
      other.id == id &&
      other.accessCode == accessCode &&
      other.manageUrl == manageUrl &&
      other.status == status &&
      other.displayStatus == displayStatus &&
      other.brand == brand &&
      other.branch == branch &&
      other.startUtc == startUtc &&
      other.endUtc == endUtc &&
      other.timeZone == timeZone &&
      other.holdExpiresUtc == holdExpiresUtc &&
      other.items == items &&
      other.total == total &&
      other.amountDue == amountDue &&
      other.deposit == deposit &&
      other.canCancel == canCancel &&
      other.cancelDeadlineUtc == cancelDeadlineUtc &&
      other.canReschedule == canReschedule &&
      other.rescheduleDeadlineUtc == rescheduleDeadlineUtc &&
      other.preAppointmentNote == preAppointmentNote &&
      other.cancellation == cancellation &&
      other.version == version;

    @override
    int get hashCode =>
        id.hashCode +
        accessCode.hashCode +
        manageUrl.hashCode +
        status.hashCode +
        displayStatus.hashCode +
        brand.hashCode +
        branch.hashCode +
        startUtc.hashCode +
        endUtc.hashCode +
        timeZone.hashCode +
        (holdExpiresUtc == null ? 0 : holdExpiresUtc.hashCode) +
        items.hashCode +
        total.hashCode +
        amountDue.hashCode +
        (deposit == null ? 0 : deposit.hashCode) +
        canCancel.hashCode +
        (cancelDeadlineUtc == null ? 0 : cancelDeadlineUtc.hashCode) +
        canReschedule.hashCode +
        (rescheduleDeadlineUtc == null ? 0 : rescheduleDeadlineUtc.hashCode) +
        (preAppointmentNote == null ? 0 : preAppointmentNote.hashCode) +
        (cancellation == null ? 0 : cancellation.hashCode) +
        version.hashCode;

  factory PublicAppointment.fromJson(Map<String, dynamic> json) => _$PublicAppointmentFromJson(json);

  Map<String, dynamic> toJson() => _$PublicAppointmentToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

