//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/appointment_channel.dart';
import 'package:omvila_api/src/model/appointment_item_view.dart';
import 'package:omvila_api/src/model/money.dart';
import 'package:omvila_api/src/model/appointment_detail_customer.dart';
import 'package:omvila_api/src/model/appointment_history_entry.dart';
import 'package:omvila_api/src/model/appointment_status.dart';
import 'package:omvila_api/src/model/appointment_detail_cancellation.dart';
import 'package:omvila_api/src/model/deposit_view.dart';
import 'package:omvila_api/src/model/payment_view.dart';
import 'package:omvila_api/src/model/display_status.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'appointment_detail.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AppointmentDetail {
  /// Returns a new [AppointmentDetail] instance.
  AppointmentDetail({

    required  this.id,

     this.reference,

    required  this.branchId,

    required  this.status,

    required  this.displayStatus,

    required  this.channel,

    required  this.startUtc,

    required  this.endUtc,

    required  this.timeZone,

     this.holdExpiresUtc,

    required  this.customer,

    required  this.items,

     this.deposit,

    required  this.total,

    required  this.amountDue,

     this.customerNote,

     this.staffNote,

    required  this.isRecurring,

     this.seriesId,

     this.noShowAtUtc,

     this.cancellation,

     this.completedUtc,

     this.payments,

    required  this.history,

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
    
    name: r'branchId',
    required: true,
    includeIfNull: false,
  )


  final String branchId;



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
    
    name: r'channel',
    required: true,
    includeIfNull: false,
  unknownEnumValue: AppointmentChannel.unknownDefaultOpenApi,
  )


  final AppointmentChannel channel;



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
    
    name: r'customer',
    required: true,
    includeIfNull: false,
  )


  final AppointmentDetailCustomer customer;



  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<AppointmentItemView> items;



  @JsonKey(
    
    name: r'deposit',
    required: false,
    includeIfNull: false,
  )


  final DepositView? deposit;



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
    
    name: r'customerNote',
    required: false,
    includeIfNull: false,
  )


  final String? customerNote;



  @JsonKey(
    
    name: r'staffNote',
    required: false,
    includeIfNull: false,
  )


  final String? staffNote;



  @JsonKey(
    
    name: r'isRecurring',
    required: true,
    includeIfNull: false,
  )


  final bool isRecurring;



  @JsonKey(
    
    name: r'seriesId',
    required: false,
    includeIfNull: false,
  )


  final String? seriesId;



      /// \"Gelmezse 13:15'te otomatik Gelmedi olur\"
  @JsonKey(
    
    name: r'noShowAtUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? noShowAtUtc;



  @JsonKey(
    
    name: r'cancellation',
    required: false,
    includeIfNull: false,
  )


  final AppointmentDetailCancellation? cancellation;



      /// \"Tamamla ve ödeme al\" anı
  @JsonKey(
    
    name: r'completedUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? completedUtc;



      /// Randevuya tahsis edilen bütün tahsilatlar (kapora havalesi, salonda alınanlar, paketten düşülen kalemlerin 0 TL kaydı), eskiden yeniye. Yetkisi olmayan (payment.collect yok) kullanıcıda dönmez
  @JsonKey(
    
    name: r'payments',
    required: false,
    includeIfNull: false,
  )


  final List<PaymentView>? payments;



  @JsonKey(
    
    name: r'history',
    required: true,
    includeIfNull: false,
  )


  final List<AppointmentHistoryEntry> history;



      /// Kaynak sürümü (ETag ile aynı değer)
  @JsonKey(
    
    name: r'version',
    required: true,
    includeIfNull: false,
  )


  final String version;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AppointmentDetail &&
      other.id == id &&
      other.reference == reference &&
      other.branchId == branchId &&
      other.status == status &&
      other.displayStatus == displayStatus &&
      other.channel == channel &&
      other.startUtc == startUtc &&
      other.endUtc == endUtc &&
      other.timeZone == timeZone &&
      other.holdExpiresUtc == holdExpiresUtc &&
      other.customer == customer &&
      other.items == items &&
      other.deposit == deposit &&
      other.total == total &&
      other.amountDue == amountDue &&
      other.customerNote == customerNote &&
      other.staffNote == staffNote &&
      other.isRecurring == isRecurring &&
      other.seriesId == seriesId &&
      other.noShowAtUtc == noShowAtUtc &&
      other.cancellation == cancellation &&
      other.completedUtc == completedUtc &&
      other.payments == payments &&
      other.history == history &&
      other.version == version;

    @override
    int get hashCode =>
        id.hashCode +
        reference.hashCode +
        branchId.hashCode +
        status.hashCode +
        displayStatus.hashCode +
        channel.hashCode +
        startUtc.hashCode +
        endUtc.hashCode +
        timeZone.hashCode +
        (holdExpiresUtc == null ? 0 : holdExpiresUtc.hashCode) +
        customer.hashCode +
        items.hashCode +
        (deposit == null ? 0 : deposit.hashCode) +
        total.hashCode +
        amountDue.hashCode +
        (customerNote == null ? 0 : customerNote.hashCode) +
        (staffNote == null ? 0 : staffNote.hashCode) +
        isRecurring.hashCode +
        (seriesId == null ? 0 : seriesId.hashCode) +
        (noShowAtUtc == null ? 0 : noShowAtUtc.hashCode) +
        (cancellation == null ? 0 : cancellation.hashCode) +
        (completedUtc == null ? 0 : completedUtc.hashCode) +
        payments.hashCode +
        history.hashCode +
        version.hashCode;

  factory AppointmentDetail.fromJson(Map<String, dynamic> json) => _$AppointmentDetailFromJson(json);

  Map<String, dynamic> toJson() => _$AppointmentDetailToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

