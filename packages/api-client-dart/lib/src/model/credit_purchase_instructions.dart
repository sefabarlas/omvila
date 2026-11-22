//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/money.dart';
import 'package:omvila_api/src/model/omvila_bank_account.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'credit_purchase_instructions.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class CreditPurchaseInstructions {
  /// Returns a new [CreditPurchaseInstructions] instance.
  CreditPurchaseInstructions({

    required  this.creditPackageId,

    required  this.credits,

    required  this.amount,

    required  this.transferCode,

    required  this.omvilaBankAccount,
  });

      /// Seçilen paket
  @JsonKey(
    
    name: r'creditPackageId',
    required: true,
    includeIfNull: false,
  )


  final String creditPackageId;



      /// Paketin kontörü
          // minimum: 1
  @JsonKey(
    
    name: r'credits',
    required: true,
    includeIfNull: false,
  )


  final int credits;



      /// Havale tutarı; fiyat tanımlı değilse null
  @JsonKey(
    
    name: r'amount',
    required: true,
    includeIfNull: true,
  )


  final Money? amount;



      /// Markanın sabit kontör havale kodu (açıklamaya yazılır)
  @JsonKey(
    
    name: r'transferCode',
    required: true,
    includeIfNull: false,
  )


  final String transferCode;



      /// Alıcı ve IBAN; yapılandırma yoksa null
  @JsonKey(
    
    name: r'omvilaBankAccount',
    required: true,
    includeIfNull: true,
  )


  final OmvilaBankAccount? omvilaBankAccount;





    @override
    bool operator ==(Object other) => identical(this, other) || other is CreditPurchaseInstructions &&
      other.creditPackageId == creditPackageId &&
      other.credits == credits &&
      other.amount == amount &&
      other.transferCode == transferCode &&
      other.omvilaBankAccount == omvilaBankAccount;

    @override
    int get hashCode =>
        creditPackageId.hashCode +
        credits.hashCode +
        (amount == null ? 0 : amount.hashCode) +
        transferCode.hashCode +
        (omvilaBankAccount == null ? 0 : omvilaBankAccount.hashCode);

  factory CreditPurchaseInstructions.fromJson(Map<String, dynamic> json) => _$CreditPurchaseInstructionsFromJson(json);

  Map<String, dynamic> toJson() => _$CreditPurchaseInstructionsToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

