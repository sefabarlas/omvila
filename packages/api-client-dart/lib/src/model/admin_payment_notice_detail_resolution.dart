//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_payment_notice_detail_resolution.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminPaymentNoticeDetailResolution {
  /// Returns a new [AdminPaymentNoticeDetailResolution] instance.
  AdminPaymentNoticeDetailResolution({

    required  this.resolvedUtc,

    required  this.resolvedByName,

    required  this.matchedBankTransactionId,

    required  this.message,
  });

      /// Sonuçlandığı an
  @JsonKey(
    
    name: r'resolvedUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime resolvedUtc;



      /// İşlemi yapan platform kullanıcısı
  @JsonKey(
    
    name: r'resolvedByName',
    required: true,
    includeIfNull: false,
  )


  final String resolvedByName;



      /// Eşleşen banka hareketi (Matched)
  @JsonKey(
    
    name: r'matchedBankTransactionId',
    required: true,
    includeIfNull: true,
  )


  final String? matchedBankTransactionId;



      /// Markaya yazılan not (NotFound, Rejected)
  @JsonKey(
    
    name: r'message',
    required: true,
    includeIfNull: true,
  )


  final String? message;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminPaymentNoticeDetailResolution &&
      other.resolvedUtc == resolvedUtc &&
      other.resolvedByName == resolvedByName &&
      other.matchedBankTransactionId == matchedBankTransactionId &&
      other.message == message;

    @override
    int get hashCode =>
        resolvedUtc.hashCode +
        resolvedByName.hashCode +
        (matchedBankTransactionId == null ? 0 : matchedBankTransactionId.hashCode) +
        (message == null ? 0 : message.hashCode);

  factory AdminPaymentNoticeDetailResolution.fromJson(Map<String, dynamic> json) => _$AdminPaymentNoticeDetailResolutionFromJson(json);

  Map<String, dynamic> toJson() => _$AdminPaymentNoticeDetailResolutionToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

