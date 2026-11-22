//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/credit_transaction_type.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_credit_transaction.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminCreditTransaction {
  /// Returns a new [AdminCreditTransaction] instance.
  AdminCreditTransaction({

    required  this.id,

    required  this.occurredUtc,

    required  this.type,

    required  this.credits,

    required  this.balanceAfter,

    required  this.note,
  });

      /// Hareket (günlük toplanmış gönderim satırında günün ilk hareketi)
  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



      /// \"Tarih\"
  @JsonKey(
    
    name: r'occurredUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime occurredUtc;



  @JsonKey(
    
    name: r'type',
    required: true,
    includeIfNull: false,
  unknownEnumValue: CreditTransactionType.unknownDefaultOpenApi,
  )


  final CreditTransactionType type;



      /// \"Miktar\" (artı ekleme, eksi tüketim)
  @JsonKey(
    
    name: r'credits',
    required: true,
    includeIfNull: false,
  )


  final int credits;



      /// \"Bakiye\"
  @JsonKey(
    
    name: r'balanceAfter',
    required: true,
    includeIfNull: false,
  )


  final int balanceAfter;



      /// Platform eklemesinin nedeni; diğerlerinde null
  @JsonKey(
    
    name: r'note',
    required: true,
    includeIfNull: true,
  )


  final String? note;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminCreditTransaction &&
      other.id == id &&
      other.occurredUtc == occurredUtc &&
      other.type == type &&
      other.credits == credits &&
      other.balanceAfter == balanceAfter &&
      other.note == note;

    @override
    int get hashCode =>
        id.hashCode +
        occurredUtc.hashCode +
        type.hashCode +
        credits.hashCode +
        balanceAfter.hashCode +
        (note == null ? 0 : note.hashCode);

  factory AdminCreditTransaction.fromJson(Map<String, dynamic> json) => _$AdminCreditTransactionFromJson(json);

  Map<String, dynamic> toJson() => _$AdminCreditTransactionToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

