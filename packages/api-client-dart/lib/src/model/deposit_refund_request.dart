//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/money.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'deposit_refund_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class DepositRefundRequest {
  /// Returns a new [DepositRefundRequest] instance.
  DepositRefundRequest({

    required  this.amount,

     this.note,
  });

      /// İade edilen tutar; 0'dan büyük ve alınan kaporadan küçük veya eşit
  @JsonKey(
    
    name: r'amount',
    required: true,
    includeIfNull: false,
  )


  final Money amount;



      /// Salon içi not (geçmişte görünür, müşteriye gösterilmez)
  @JsonKey(
    
    name: r'note',
    required: false,
    includeIfNull: false,
  )


  final String? note;





    @override
    bool operator ==(Object other) => identical(this, other) || other is DepositRefundRequest &&
      other.amount == amount &&
      other.note == note;

    @override
    int get hashCode =>
        amount.hashCode +
        (note == null ? 0 : note.hashCode);

  factory DepositRefundRequest.fromJson(Map<String, dynamic> json) => _$DepositRefundRequestFromJson(json);

  Map<String, dynamic> toJson() => _$DepositRefundRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

