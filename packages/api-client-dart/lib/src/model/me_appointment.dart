//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/me_brand_ref.dart';
import 'package:omvila_api/src/model/me_appointment_item.dart';
import 'package:omvila_api/src/model/me_appointment_deposit.dart';
import 'package:omvila_api/src/model/money.dart';
import 'package:omvila_api/src/model/appointment_status.dart';
import 'package:omvila_api/src/model/me_appointment_branch.dart';
import 'package:omvila_api/src/model/public_appointment_cancellation.dart';
import 'package:omvila_api/src/model/display_status.dart';
import 'package:omvila_api/src/model/me_appointment_review.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'me_appointment.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MeAppointment {
  /// Returns a new [MeAppointment] instance.
  MeAppointment({

    required  this.id,

     this.reference,

    required  this.brand,

    required  this.branch,

    required  this.timeZone,

    required  this.startUtc,

    required  this.endUtc,

    required  this.status,

    required  this.displayStatus,

     this.holdExpiresUtc,

    required  this.items,

    required  this.total,

    required  this.amountDue,

    required  this.deposit,

    required  this.canCancel,

    required  this.cancelDeadlineUtc,

    required  this.canReschedule,

    required  this.rescheduleDeadlineUtc,

    required  this.canReview,

    required  this.canRebook,

    required  this.review,

     this.preAppointmentNote,

     this.customerNote,

    required  this.cancellation,

    required  this.version,
  });

  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



      /// Kısa görünen numara
  @JsonKey(
    
    name: r'reference',
    required: false,
    includeIfNull: false,
  )


  final String? reference;



  @JsonKey(
    
    name: r'brand',
    required: true,
    includeIfNull: false,
  )


  final MeBrandRef brand;



  @JsonKey(
    
    name: r'branch',
    required: true,
    includeIfNull: false,
  )


  final MeAppointmentBranch branch;



      /// IANA saat dilimi
  @JsonKey(
    
    name: r'timeZone',
    required: true,
    includeIfNull: false,
  )


  final String timeZone;



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


  final List<MeAppointmentItem> items;



  @JsonKey(
    
    name: r'total',
    required: true,
    includeIfNull: false,
  )


  final Money total;



      /// \"Salonda ödenecek\"
  @JsonKey(
    
    name: r'amountDue',
    required: true,
    includeIfNull: false,
  )


  final Money amountDue;



  @JsonKey(
    
    name: r'deposit',
    required: true,
    includeIfNull: true,
  )


  final MeAppointmentDeposit? deposit;



  @JsonKey(
    
    name: r'canCancel',
    required: true,
    includeIfNull: false,
  )


  final bool canCancel;



  @JsonKey(
    
    name: r'cancelDeadlineUtc',
    required: true,
    includeIfNull: true,
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
    required: true,
    includeIfNull: true,
  )


  final DateTime? rescheduleDeadlineUtc;



  @JsonKey(
    
    name: r'canReview',
    required: true,
    includeIfNull: false,
  )


  final bool canReview;



  @JsonKey(
    
    name: r'canRebook',
    required: true,
    includeIfNull: false,
  )


  final bool canRebook;



  @JsonKey(
    
    name: r'review',
    required: true,
    includeIfNull: true,
  )


  final MeAppointmentReview? review;



      /// Salonun randevu öncesi notu
  @JsonKey(
    
    name: r'preAppointmentNote',
    required: false,
    includeIfNull: false,
  )


  final String? preAppointmentNote;



      /// \"Salona notunuz\"
  @JsonKey(
    
    name: r'customerNote',
    required: false,
    includeIfNull: false,
  )


  final String? customerNote;



  @JsonKey(
    
    name: r'cancellation',
    required: true,
    includeIfNull: true,
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
    bool operator ==(Object other) => identical(this, other) || other is MeAppointment &&
      other.id == id &&
      other.reference == reference &&
      other.brand == brand &&
      other.branch == branch &&
      other.timeZone == timeZone &&
      other.startUtc == startUtc &&
      other.endUtc == endUtc &&
      other.status == status &&
      other.displayStatus == displayStatus &&
      other.holdExpiresUtc == holdExpiresUtc &&
      other.items == items &&
      other.total == total &&
      other.amountDue == amountDue &&
      other.deposit == deposit &&
      other.canCancel == canCancel &&
      other.cancelDeadlineUtc == cancelDeadlineUtc &&
      other.canReschedule == canReschedule &&
      other.rescheduleDeadlineUtc == rescheduleDeadlineUtc &&
      other.canReview == canReview &&
      other.canRebook == canRebook &&
      other.review == review &&
      other.preAppointmentNote == preAppointmentNote &&
      other.customerNote == customerNote &&
      other.cancellation == cancellation &&
      other.version == version;

    @override
    int get hashCode =>
        id.hashCode +
        reference.hashCode +
        brand.hashCode +
        branch.hashCode +
        timeZone.hashCode +
        startUtc.hashCode +
        endUtc.hashCode +
        status.hashCode +
        displayStatus.hashCode +
        (holdExpiresUtc == null ? 0 : holdExpiresUtc.hashCode) +
        items.hashCode +
        total.hashCode +
        amountDue.hashCode +
        (deposit == null ? 0 : deposit.hashCode) +
        canCancel.hashCode +
        (cancelDeadlineUtc == null ? 0 : cancelDeadlineUtc.hashCode) +
        canReschedule.hashCode +
        (rescheduleDeadlineUtc == null ? 0 : rescheduleDeadlineUtc.hashCode) +
        canReview.hashCode +
        canRebook.hashCode +
        (review == null ? 0 : review.hashCode) +
        (preAppointmentNote == null ? 0 : preAppointmentNote.hashCode) +
        (customerNote == null ? 0 : customerNote.hashCode) +
        (cancellation == null ? 0 : cancellation.hashCode) +
        version.hashCode;

  factory MeAppointment.fromJson(Map<String, dynamic> json) => _$MeAppointmentFromJson(json);

  Map<String, dynamic> toJson() => _$MeAppointmentToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

