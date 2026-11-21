//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/money.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'cash_day_transfer.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class CashDayTransfer {
  /// Returns a new [CashDayTransfer] instance.
  CashDayTransfer({

    required  this.amount,

    required  this.count,

    required  this.depositAmount,

    required  this.otherAmount,
  });

  @JsonKey(
    
    name: r'amount',
    required: true,
    includeIfNull: false,
  )


  final Money amount;



      /// Ödeme sayısı
  @JsonKey(
    
    name: r'count',
    required: true,
    includeIfNull: false,
  )


  final int count;



      /// \"600 kapora\"
  @JsonKey(
    
    name: r'depositAmount',
    required: true,
    includeIfNull: false,
  )


  final Money depositAmount;



      /// \"500 ödeme\"
  @JsonKey(
    
    name: r'otherAmount',
    required: true,
    includeIfNull: false,
  )


  final Money otherAmount;





    @override
    bool operator ==(Object other) => identical(this, other) || other is CashDayTransfer &&
      other.amount == amount &&
      other.count == count &&
      other.depositAmount == depositAmount &&
      other.otherAmount == otherAmount;

    @override
    int get hashCode =>
        amount.hashCode +
        count.hashCode +
        depositAmount.hashCode +
        otherAmount.hashCode;

  factory CashDayTransfer.fromJson(Map<String, dynamic> json) => _$CashDayTransferFromJson(json);

  Map<String, dynamic> toJson() => _$CashDayTransferToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

