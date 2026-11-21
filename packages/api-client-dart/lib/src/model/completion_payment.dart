//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/collected_payment_method.dart';
import 'package:omvila_api/src/model/money.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'completion_payment.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class CompletionPayment {
  /// Returns a new [CompletionPayment] instance.
  CompletionPayment({

    required  this.method,

    required  this.amount,
  });

  @JsonKey(
    
    name: r'method',
    required: true,
    includeIfNull: false,
  unknownEnumValue: CollectedPaymentMethod.unknownDefaultOpenApi,
  )


  final CollectedPaymentMethod method;



      /// 0'dan büyük olmalı
  @JsonKey(
    
    name: r'amount',
    required: true,
    includeIfNull: false,
  )


  final Money amount;





    @override
    bool operator ==(Object other) => identical(this, other) || other is CompletionPayment &&
      other.method == method &&
      other.amount == amount;

    @override
    int get hashCode =>
        method.hashCode +
        amount.hashCode;

  factory CompletionPayment.fromJson(Map<String, dynamic> json) => _$CompletionPaymentFromJson(json);

  Map<String, dynamic> toJson() => _$CompletionPaymentToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

