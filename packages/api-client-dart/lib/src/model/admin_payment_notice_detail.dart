//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/payment_notice_purpose.dart';
import 'package:omvila_api/src/model/admin_payment_notice_detail_candidate_window.dart';
import 'package:omvila_api/src/model/admin_payment_notice_detail_resolution.dart';
import 'package:omvila_api/src/model/money.dart';
import 'package:omvila_api/src/model/admin_payment_notice_detail_receipt.dart';
import 'package:omvila_api/src/model/bank_transaction_candidate.dart';
import 'package:omvila_api/src/model/payment_notice_status.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_payment_notice_detail.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminPaymentNoticeDetail {
  /// Returns a new [AdminPaymentNoticeDetail] instance.
  AdminPaymentNoticeDetail({

    required  this.id,

    required  this.brandId,

    required  this.brandName,

    required  this.purpose,

    required  this.planName,

    required  this.periodStart,

    required  this.periodEnd,

    required  this.credits,

    required  this.expectedAmount,

    required  this.transferCode,

    required  this.senderName,

    required  this.notifiedUtc,

    required  this.channel,

    required  this.status,

    required  this.receipt,

    required  this.resolution,

    required  this.candidateWindow,

    required  this.candidates,

    required  this.version,
  });

      /// Bildirim
  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



      /// Marka
  @JsonKey(
    
    name: r'brandId',
    required: true,
    includeIfNull: false,
  )


  final String brandId;



      /// Marka adı
  @JsonKey(
    
    name: r'brandName',
    required: true,
    includeIfNull: false,
  )


  final String brandName;



  @JsonKey(
    
    name: r'purpose',
    required: true,
    includeIfNull: false,
  unknownEnumValue: PaymentNoticePurpose.unknownDefaultOpenApi,
  )


  final PaymentNoticePurpose purpose;



      /// Abonelikte plan adı (\"Salon planı · Kasım 2026\"); kontörde null
  @JsonKey(
    
    name: r'planName',
    required: true,
    includeIfNull: true,
  )


  final String? planName;



      /// Abonelikte dönem başı
  @JsonKey(
    
    name: r'periodStart',
    required: true,
    includeIfNull: true,
  )
  @DateOnlyConverter()


  final DateTime? periodStart;



      /// Abonelikte dönem sonu
  @JsonKey(
    
    name: r'periodEnd',
    required: true,
    includeIfNull: true,
  )
  @DateOnlyConverter()


  final DateTime? periodEnd;



      /// Kontörde paket kontörü
          // minimum: 1
  @JsonKey(
    
    name: r'credits',
    required: true,
    includeIfNull: true,
  )


  final int? credits;



      /// \"Beklenen tutar\"; tanımlı değilse null
  @JsonKey(
    
    name: r'expectedAmount',
    required: true,
    includeIfNull: true,
  )


  final Money? expectedAmount;



      /// \"Açıklama kodu\"
  @JsonKey(
    
    name: r'transferCode',
    required: true,
    includeIfNull: false,
  )


  final String transferCode;



      /// \"Gönderen\"
  @JsonKey(
    
    name: r'senderName',
    required: true,
    includeIfNull: true,
  )


  final String? senderName;



      /// \"Bildirim\"
  @JsonKey(
    
    name: r'notifiedUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime notifiedUtc;



      /// \"panelden\" / uygulamadan
  @JsonKey(
    
    name: r'channel',
    required: true,
    includeIfNull: false,
  unknownEnumValue: AdminPaymentNoticeDetailChannelEnum.unknownDefaultOpenApi,
  )


  final AdminPaymentNoticeDetailChannelEnum channel;



  @JsonKey(
    
    name: r'status',
    required: true,
    includeIfNull: false,
  unknownEnumValue: PaymentNoticeStatus.unknownDefaultOpenApi,
  )


  final PaymentNoticeStatus status;



  @JsonKey(
    
    name: r'receipt',
    required: true,
    includeIfNull: true,
  )


  final AdminPaymentNoticeDetailReceipt? receipt;



  @JsonKey(
    
    name: r'resolution',
    required: true,
    includeIfNull: true,
  )


  final AdminPaymentNoticeDetailResolution? resolution;



  @JsonKey(
    
    name: r'candidateWindow',
    required: true,
    includeIfNull: false,
  )


  final AdminPaymentNoticeDetailCandidateWindow candidateWindow;



      /// Eşleşmemiş adaylar; en olası önce. Bildirim sonuçlandıysa boş
  @JsonKey(
    
    name: r'candidates',
    required: true,
    includeIfNull: false,
  )


  final List<BankTransactionCandidate> candidates;



      /// Kaynak sürümü (ETag ile aynı değer)
  @JsonKey(
    
    name: r'version',
    required: true,
    includeIfNull: false,
  )


  final String version;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminPaymentNoticeDetail &&
      other.id == id &&
      other.brandId == brandId &&
      other.brandName == brandName &&
      other.purpose == purpose &&
      other.planName == planName &&
      other.periodStart == periodStart &&
      other.periodEnd == periodEnd &&
      other.credits == credits &&
      other.expectedAmount == expectedAmount &&
      other.transferCode == transferCode &&
      other.senderName == senderName &&
      other.notifiedUtc == notifiedUtc &&
      other.channel == channel &&
      other.status == status &&
      other.receipt == receipt &&
      other.resolution == resolution &&
      other.candidateWindow == candidateWindow &&
      other.candidates == candidates &&
      other.version == version;

    @override
    int get hashCode =>
        id.hashCode +
        brandId.hashCode +
        brandName.hashCode +
        purpose.hashCode +
        (planName == null ? 0 : planName.hashCode) +
        (periodStart == null ? 0 : periodStart.hashCode) +
        (periodEnd == null ? 0 : periodEnd.hashCode) +
        (credits == null ? 0 : credits.hashCode) +
        (expectedAmount == null ? 0 : expectedAmount.hashCode) +
        transferCode.hashCode +
        (senderName == null ? 0 : senderName.hashCode) +
        notifiedUtc.hashCode +
        channel.hashCode +
        status.hashCode +
        (receipt == null ? 0 : receipt.hashCode) +
        (resolution == null ? 0 : resolution.hashCode) +
        candidateWindow.hashCode +
        candidates.hashCode +
        version.hashCode;

  factory AdminPaymentNoticeDetail.fromJson(Map<String, dynamic> json) => _$AdminPaymentNoticeDetailFromJson(json);

  Map<String, dynamic> toJson() => _$AdminPaymentNoticeDetailToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

/// \"panelden\" / uygulamadan
enum AdminPaymentNoticeDetailChannelEnum {
@JsonValue(r'Panel')
panel(r'Panel'),
@JsonValue(r'App')
app(r'App'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const AdminPaymentNoticeDetailChannelEnum(this.value);

final String value;

@override
String toString() => value;
}


