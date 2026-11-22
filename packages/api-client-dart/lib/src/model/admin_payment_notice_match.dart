//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_payment_notice_match.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminPaymentNoticeMatch {
  /// Returns a new [AdminPaymentNoticeMatch] instance.
  AdminPaymentNoticeMatch({

    required  this.bankTransactionId,
  });

      /// Seçili banka hareketi
  @JsonKey(
    
    name: r'bankTransactionId',
    required: true,
    includeIfNull: false,
  )


  final String bankTransactionId;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminPaymentNoticeMatch &&
      other.bankTransactionId == bankTransactionId;

    @override
    int get hashCode =>
        bankTransactionId.hashCode;

  factory AdminPaymentNoticeMatch.fromJson(Map<String, dynamic> json) => _$AdminPaymentNoticeMatchFromJson(json);

  Map<String, dynamic> toJson() => _$AdminPaymentNoticeMatchToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

