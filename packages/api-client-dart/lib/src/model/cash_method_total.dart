//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/money.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'cash_method_total.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class CashMethodTotal {
  /// Returns a new [CashMethodTotal] instance.
  CashMethodTotal({

    required  this.amount,

    required  this.count,
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





    @override
    bool operator ==(Object other) => identical(this, other) || other is CashMethodTotal &&
      other.amount == amount &&
      other.count == count;

    @override
    int get hashCode =>
        amount.hashCode +
        count.hashCode;

  factory CashMethodTotal.fromJson(Map<String, dynamic> json) => _$CashMethodTotalFromJson(json);

  Map<String, dynamic> toJson() => _$CashMethodTotalToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

