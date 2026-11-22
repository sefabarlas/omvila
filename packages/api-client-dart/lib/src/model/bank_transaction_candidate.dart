//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/money.dart';
import 'package:omvila_api/src/model/match_verdict.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'bank_transaction_candidate.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class BankTransactionCandidate {
  /// Returns a new [BankTransactionCandidate] instance.
  BankTransactionCandidate({

    required  this.bankTransactionId,

    required  this.valueDate,

    required  this.receivedUtc,

    required  this.amount,

    required  this.senderName,

    required  this.description,

    required  this.codeFound,

    required  this.amountMatches,

    required  this.verdict,
  });

      /// Banka hareketi
  @JsonKey(
    
    name: r'bankTransactionId',
    required: true,
    includeIfNull: false,
  )


  final String bankTransactionId;



      /// Değer tarihi
  @JsonKey(
    
    name: r'valueDate',
    required: true,
    includeIfNull: false,
  )
  @DateOnlyConverter()


  final DateTime valueDate;



      /// Dökümde saat varsa hareketin anı (\"18 Kas 14:05\")
  @JsonKey(
    
    name: r'receivedUtc',
    required: true,
    includeIfNull: true,
  )


  final DateTime? receivedUtc;



  @JsonKey(
    
    name: r'amount',
    required: true,
    includeIfNull: false,
  )


  final Money amount;



      /// Gönderen (dökümdeki gibi)
  @JsonKey(
    
    name: r'senderName',
    required: true,
    includeIfNull: false,
  )


  final String senderName;



      /// \"açıklama\"; boşsa null (\"açıklama: —\")
  @JsonKey(
    
    name: r'description',
    required: true,
    includeIfNull: true,
  )


  final String? description;



      /// Açıklamada markanın kodu var
  @JsonKey(
    
    name: r'codeFound',
    required: true,
    includeIfNull: false,
  )


  final bool codeFound;



      /// Tutar beklenene eşit
  @JsonKey(
    
    name: r'amountMatches',
    required: true,
    includeIfNull: false,
  )


  final bool amountMatches;



  @JsonKey(
    
    name: r'verdict',
    required: true,
    includeIfNull: false,
  unknownEnumValue: MatchVerdict.unknownDefaultOpenApi,
  )


  final MatchVerdict verdict;





    @override
    bool operator ==(Object other) => identical(this, other) || other is BankTransactionCandidate &&
      other.bankTransactionId == bankTransactionId &&
      other.valueDate == valueDate &&
      other.receivedUtc == receivedUtc &&
      other.amount == amount &&
      other.senderName == senderName &&
      other.description == description &&
      other.codeFound == codeFound &&
      other.amountMatches == amountMatches &&
      other.verdict == verdict;

    @override
    int get hashCode =>
        bankTransactionId.hashCode +
        valueDate.hashCode +
        (receivedUtc == null ? 0 : receivedUtc.hashCode) +
        amount.hashCode +
        senderName.hashCode +
        (description == null ? 0 : description.hashCode) +
        codeFound.hashCode +
        amountMatches.hashCode +
        verdict.hashCode;

  factory BankTransactionCandidate.fromJson(Map<String, dynamic> json) => _$BankTransactionCandidateFromJson(json);

  Map<String, dynamic> toJson() => _$BankTransactionCandidateToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

