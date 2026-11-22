//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/money.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'bank_transaction.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class BankTransaction {
  /// Returns a new [BankTransaction] instance.
  BankTransaction({

    required  this.id,

    required  this.valueDate,

    required  this.receivedUtc,

    required  this.amount,

    required  this.senderName,

    required  this.description,

    required  this.bankReference,

    required  this.source_,

    required  this.matchedNoticeId,

    required  this.matchedBrandName,
  });

      /// Hareket
  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



      /// Değer tarihi
  @JsonKey(
    
    name: r'valueDate',
    required: true,
    includeIfNull: false,
  )
  @DateOnlyConverter()


  final DateTime valueDate;



      /// Dökümde saat varsa hareketin anı
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
    required: true,
    includeIfNull: true,
  )


  final String? description;



      /// Bankanın hareket referansı (benzersiz)
  @JsonKey(
    
    name: r'bankReference',
    required: true,
    includeIfNull: false,
  )


  final String bankReference;



      /// CSV ile içe aktarıldı veya elle girildi
  @JsonKey(
    
    name: r'source',
    required: true,
    includeIfNull: false,
  unknownEnumValue: BankTransactionSource_Enum.unknownDefaultOpenApi,
  )


  final BankTransactionSource_Enum source_;



      /// Eşleştiği bildirim
  @JsonKey(
    
    name: r'matchedNoticeId',
    required: true,
    includeIfNull: true,
  )


  final String? matchedNoticeId;



      /// Eşleştiği marka
  @JsonKey(
    
    name: r'matchedBrandName',
    required: true,
    includeIfNull: true,
  )


  final String? matchedBrandName;





    @override
    bool operator ==(Object other) => identical(this, other) || other is BankTransaction &&
      other.id == id &&
      other.valueDate == valueDate &&
      other.receivedUtc == receivedUtc &&
      other.amount == amount &&
      other.senderName == senderName &&
      other.description == description &&
      other.bankReference == bankReference &&
      other.source_ == source_ &&
      other.matchedNoticeId == matchedNoticeId &&
      other.matchedBrandName == matchedBrandName;

    @override
    int get hashCode =>
        id.hashCode +
        valueDate.hashCode +
        (receivedUtc == null ? 0 : receivedUtc.hashCode) +
        amount.hashCode +
        senderName.hashCode +
        (description == null ? 0 : description.hashCode) +
        bankReference.hashCode +
        source_.hashCode +
        (matchedNoticeId == null ? 0 : matchedNoticeId.hashCode) +
        (matchedBrandName == null ? 0 : matchedBrandName.hashCode);

  factory BankTransaction.fromJson(Map<String, dynamic> json) => _$BankTransactionFromJson(json);

  Map<String, dynamic> toJson() => _$BankTransactionToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

/// CSV ile içe aktarıldı veya elle girildi
enum BankTransactionSource_Enum {
@JsonValue(r'Import')
import_(r'Import'),
@JsonValue(r'Manual')
manual(r'Manual'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const BankTransactionSource_Enum(this.value);

final String value;

@override
String toString() => value;
}


