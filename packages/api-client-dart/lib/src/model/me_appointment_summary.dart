//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/me_brand_ref.dart';
import 'package:omvila_api/src/model/me_appointment_item.dart';
import 'package:omvila_api/src/model/money.dart';
import 'package:omvila_api/src/model/appointment_status.dart';
import 'package:omvila_api/src/model/deposit_status.dart';
import 'package:omvila_api/src/model/me_branch_ref.dart';
import 'package:omvila_api/src/model/display_status.dart';
import 'package:omvila_api/src/model/me_appointment_review.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'me_appointment_summary.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MeAppointmentSummary {
  /// Returns a new [MeAppointmentSummary] instance.
  MeAppointmentSummary({

    required  this.id,

    required  this.brand,

    required  this.branch,

    required  this.timeZone,

    required  this.startUtc,

    required  this.endUtc,

    required  this.status,

    required  this.displayStatus,

     this.holdExpiresUtc,

     this.depositStatus,

    required  this.items,

    required  this.total,

    required  this.amountDue,

    required  this.review,

    required  this.canReview,

    required  this.canRebook,
  });

  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



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


  final MeBranchRef branch;



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



      /// Yalnız PendingApproval
  @JsonKey(
    
    name: r'holdExpiresUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? holdExpiresUtc;



      /// Yalnız kapora istenmiş randevuda bulunur
  @JsonKey(
    
    name: r'depositStatus',
    required: false,
    includeIfNull: false,
  unknownEnumValue: DepositStatus.unknownDefaultOpenApi,
  )


  final DepositStatus? depositStatus;



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



      /// \"350 ₺ salonda\"
  @JsonKey(
    
    name: r'amountDue',
    required: true,
    includeIfNull: false,
  )


  final Money amountDue;



      /// Gönderilmiş değerlendirme; yoksa null
  @JsonKey(
    
    name: r'review',
    required: true,
    includeIfNull: true,
  )


  final MeAppointmentReview? review;



      /// Completed ve değerlendirilmemiş (\"Değerlendir\")
  @JsonKey(
    
    name: r'canReview',
    required: true,
    includeIfNull: false,
  )


  final bool canReview;



      /// \"Aynısını tekrar al\": salon online randevu alıyor ve kalemlerin hizmetleri şubede online açık
  @JsonKey(
    
    name: r'canRebook',
    required: true,
    includeIfNull: false,
  )


  final bool canRebook;





    @override
    bool operator ==(Object other) => identical(this, other) || other is MeAppointmentSummary &&
      other.id == id &&
      other.brand == brand &&
      other.branch == branch &&
      other.timeZone == timeZone &&
      other.startUtc == startUtc &&
      other.endUtc == endUtc &&
      other.status == status &&
      other.displayStatus == displayStatus &&
      other.holdExpiresUtc == holdExpiresUtc &&
      other.depositStatus == depositStatus &&
      other.items == items &&
      other.total == total &&
      other.amountDue == amountDue &&
      other.review == review &&
      other.canReview == canReview &&
      other.canRebook == canRebook;

    @override
    int get hashCode =>
        id.hashCode +
        brand.hashCode +
        branch.hashCode +
        timeZone.hashCode +
        startUtc.hashCode +
        endUtc.hashCode +
        status.hashCode +
        displayStatus.hashCode +
        (holdExpiresUtc == null ? 0 : holdExpiresUtc.hashCode) +
        depositStatus.hashCode +
        items.hashCode +
        total.hashCode +
        amountDue.hashCode +
        (review == null ? 0 : review.hashCode) +
        canReview.hashCode +
        canRebook.hashCode;

  factory MeAppointmentSummary.fromJson(Map<String, dynamic> json) => _$MeAppointmentSummaryFromJson(json);

  Map<String, dynamic> toJson() => _$MeAppointmentSummaryToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

