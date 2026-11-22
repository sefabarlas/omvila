//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/money.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'bank_transaction_create.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class BankTransactionCreate {
  /// Returns a new [BankTransactionCreate] instance.
  BankTransactionCreate({

    required  this.valueDate,

    required  this.amount,

    required  this.senderName,

     this.description,

    required  this.bankReference,
  });

      /// Değer tarihi
  @JsonKey(
    
    name: r'valueDate',
    required: true,
    includeIfNull: false,
  )
  @DateOnlyConverter()


  final DateTime valueDate;



  @JsonKey(
    
    name: r'amount',
    required: true,
    includeIfNull: false,
  )


  final Money amount;



      /// Gönderen
  @JsonKey(
    
    name: r'senderName',
    required: true,
    includeIfNull: false,
  )


  final String senderName;



      /// Açıklama
  @JsonKey(
    
    name: r'description',
    required: false,
    includeIfNull: true,
  )


  final String? description;



      /// Bankanın hareket referansı
  @JsonKey(
    
    name: r'bankReference',
    required: true,
    includeIfNull: false,
  )


  final String bankReference;





    @override
    bool operator ==(Object other) => identical(this, other) || other is BankTransactionCreate &&
      other.valueDate == valueDate &&
      other.amount == amount &&
      other.senderName == senderName &&
      other.description == description &&
      other.bankReference == bankReference;

    @override
    int get hashCode =>
        valueDate.hashCode +
        amount.hashCode +
        senderName.hashCode +
        (description == null ? 0 : description.hashCode) +
        bankReference.hashCode;

  factory BankTransactionCreate.fromJson(Map<String, dynamic> json) => _$BankTransactionCreateFromJson(json);

  Map<String, dynamic> toJson() => _$BankTransactionCreateToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

