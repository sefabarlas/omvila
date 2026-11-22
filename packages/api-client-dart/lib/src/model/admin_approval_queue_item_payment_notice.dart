//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/payment_notice_purpose.dart';
import 'package:omvila_api/src/model/money.dart';
import 'package:omvila_api/src/model/admin_approval_queue_item_payment_notice_candidate.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_approval_queue_item_payment_notice.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminApprovalQueueItemPaymentNotice {
  /// Returns a new [AdminApprovalQueueItemPaymentNotice] instance.
  AdminApprovalQueueItemPaymentNotice({

    required  this.purpose,

    required  this.periodStart,

    required  this.credits,

    required  this.expectedAmount,

    required  this.transferCode,

    required  this.senderName,

    required  this.hasReceipt,

    required  this.version,

    required  this.candidate,
  });

  @JsonKey(
    
    name: r'purpose',
    required: true,
    includeIfNull: false,
  unknownEnumValue: PaymentNoticePurpose.unknownDefaultOpenApi,
  )


  final PaymentNoticePurpose purpose;



      /// Abonelikte ödenen dönemin ilk günü (\"Kasım\"); kontörde null
  @JsonKey(
    
    name: r'periodStart',
    required: true,
    includeIfNull: true,
  )
  @DateOnlyConverter()


  final DateTime? periodStart;



      /// Kontörde paket kontörü (\"5.000\")
          // minimum: 1
  @JsonKey(
    
    name: r'credits',
    required: true,
    includeIfNull: true,
  )


  final int? credits;



      /// Beklenen tutar; tanımlı değilse null
  @JsonKey(
    
    name: r'expectedAmount',
    required: true,
    includeIfNull: true,
  )


  final Money? expectedAmount;



      /// Beklenen açıklama kodu
  @JsonKey(
    
    name: r'transferCode',
    required: true,
    includeIfNull: false,
  )


  final String transferCode;



      /// Gönderen adı
  @JsonKey(
    
    name: r'senderName',
    required: true,
    includeIfNull: true,
  )


  final String? senderName;



      /// \"dekont yüklendi\"
  @JsonKey(
    
    name: r'hasReceipt',
    required: true,
    includeIfNull: false,
  )


  final bool hasReceipt;



      /// Kaynak sürümü (ETag ile aynı değer)
  @JsonKey(
    
    name: r'version',
    required: true,
    includeIfNull: false,
  )


  final String version;



  @JsonKey(
    
    name: r'candidate',
    required: true,
    includeIfNull: true,
  )


  final AdminApprovalQueueItemPaymentNoticeCandidate? candidate;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminApprovalQueueItemPaymentNotice &&
      other.purpose == purpose &&
      other.periodStart == periodStart &&
      other.credits == credits &&
      other.expectedAmount == expectedAmount &&
      other.transferCode == transferCode &&
      other.senderName == senderName &&
      other.hasReceipt == hasReceipt &&
      other.version == version &&
      other.candidate == candidate;

    @override
    int get hashCode =>
        purpose.hashCode +
        (periodStart == null ? 0 : periodStart.hashCode) +
        (credits == null ? 0 : credits.hashCode) +
        (expectedAmount == null ? 0 : expectedAmount.hashCode) +
        transferCode.hashCode +
        (senderName == null ? 0 : senderName.hashCode) +
        hasReceipt.hashCode +
        version.hashCode +
        (candidate == null ? 0 : candidate.hashCode);

  factory AdminApprovalQueueItemPaymentNotice.fromJson(Map<String, dynamic> json) => _$AdminApprovalQueueItemPaymentNoticeFromJson(json);

  Map<String, dynamic> toJson() => _$AdminApprovalQueueItemPaymentNoticeToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

