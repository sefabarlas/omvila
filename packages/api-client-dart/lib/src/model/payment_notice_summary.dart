//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/payment_notice_purpose.dart';
import 'package:omvila_api/src/model/money.dart';
import 'package:omvila_api/src/model/payment_notice_status.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'payment_notice_summary.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PaymentNoticeSummary {
  /// Returns a new [PaymentNoticeSummary] instance.
  PaymentNoticeSummary({

    required  this.id,

    required  this.purpose,

    required  this.transferCode,

    required  this.expectedAmount,

    required  this.credits,

    required  this.senderName,

    required  this.hasReceipt,

    required  this.status,

    required  this.notifiedUtc,

     this.resolvedUtc,
  });

      /// Bildirim kimliği
  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



  @JsonKey(
    
    name: r'purpose',
    required: true,
    includeIfNull: false,
  unknownEnumValue: PaymentNoticePurpose.unknownDefaultOpenApi,
  )


  final PaymentNoticePurpose purpose;



      /// Beklenen açıklama kodu (ABN-… veya KNT-…)
  @JsonKey(
    
    name: r'transferCode',
    required: true,
    includeIfNull: false,
  )


  final String transferCode;



      /// Beklenen tutar; fiyat tanımlı değilse null
  @JsonKey(
    
    name: r'expectedAmount',
    required: true,
    includeIfNull: true,
  )


  final Money? expectedAmount;



      /// Kontör bildiriminde paketin kontörü; abonelikte null
          // minimum: 1
  @JsonKey(
    
    name: r'credits',
    required: true,
    includeIfNull: true,
  )


  final int? credits;



      /// Gönderen adı
  @JsonKey(
    
    name: r'senderName',
    required: true,
    includeIfNull: true,
  )


  final String? senderName;



      /// Dekont yüklendi mi
  @JsonKey(
    
    name: r'hasReceipt',
    required: true,
    includeIfNull: false,
  )


  final bool hasReceipt;



  @JsonKey(
    
    name: r'status',
    required: true,
    includeIfNull: false,
  unknownEnumValue: PaymentNoticeStatus.unknownDefaultOpenApi,
  )


  final PaymentNoticeStatus status;



      /// Bildirim anı
  @JsonKey(
    
    name: r'notifiedUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime notifiedUtc;



      /// Sonuçlandığı an (eşleşti, bulunamadı, reddedildi)
  @JsonKey(
    
    name: r'resolvedUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? resolvedUtc;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PaymentNoticeSummary &&
      other.id == id &&
      other.purpose == purpose &&
      other.transferCode == transferCode &&
      other.expectedAmount == expectedAmount &&
      other.credits == credits &&
      other.senderName == senderName &&
      other.hasReceipt == hasReceipt &&
      other.status == status &&
      other.notifiedUtc == notifiedUtc &&
      other.resolvedUtc == resolvedUtc;

    @override
    int get hashCode =>
        id.hashCode +
        purpose.hashCode +
        transferCode.hashCode +
        (expectedAmount == null ? 0 : expectedAmount.hashCode) +
        (credits == null ? 0 : credits.hashCode) +
        (senderName == null ? 0 : senderName.hashCode) +
        hasReceipt.hashCode +
        status.hashCode +
        notifiedUtc.hashCode +
        (resolvedUtc == null ? 0 : resolvedUtc.hashCode);

  factory PaymentNoticeSummary.fromJson(Map<String, dynamic> json) => _$PaymentNoticeSummaryFromJson(json);

  Map<String, dynamic> toJson() => _$PaymentNoticeSummaryToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

