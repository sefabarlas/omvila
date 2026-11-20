//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/money.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'deposit_instructions.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class DepositInstructions {
  /// Returns a new [DepositInstructions] instance.
  DepositInstructions({

    required  this.accountHolderName,

     this.bankName,

    required  this.iban,

    required  this.amount,

    required  this.transferCode,

    required  this.dueUtc,
  });

  @JsonKey(
    
    name: r'accountHolderName',
    required: true,
    includeIfNull: false,
  )


  final String accountHolderName;



  @JsonKey(
    
    name: r'bankName',
    required: false,
    includeIfNull: false,
  )


  final String? bankName;



      /// Boşluklu biçimde
  @JsonKey(
    
    name: r'iban',
    required: true,
    includeIfNull: false,
  )


  final String iban;



  @JsonKey(
    
    name: r'amount',
    required: true,
    includeIfNull: false,
  )


  final Money amount;



  @JsonKey(
    
    name: r'transferCode',
    required: true,
    includeIfNull: false,
  )


  final String transferCode;



  @JsonKey(
    
    name: r'dueUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime dueUtc;





    @override
    bool operator ==(Object other) => identical(this, other) || other is DepositInstructions &&
      other.accountHolderName == accountHolderName &&
      other.bankName == bankName &&
      other.iban == iban &&
      other.amount == amount &&
      other.transferCode == transferCode &&
      other.dueUtc == dueUtc;

    @override
    int get hashCode =>
        accountHolderName.hashCode +
        (bankName == null ? 0 : bankName.hashCode) +
        iban.hashCode +
        amount.hashCode +
        transferCode.hashCode +
        dueUtc.hashCode;

  factory DepositInstructions.fromJson(Map<String, dynamic> json) => _$DepositInstructionsFromJson(json);

  Map<String, dynamic> toJson() => _$DepositInstructionsToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

