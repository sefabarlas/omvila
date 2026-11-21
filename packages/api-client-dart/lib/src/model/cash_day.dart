//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/money.dart';
import 'package:omvila_api/src/model/payment_view.dart';
import 'package:omvila_api/src/model/cash_day_closing.dart';
import 'package:omvila_api/src/model/cash_day_transfer.dart';
import 'package:omvila_api/src/model/cash_method_total.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'cash_day.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class CashDay {
  /// Returns a new [CashDay] instance.
  CashDay({

    required  this.branchId,

    required  this.date,

    required  this.timeZone,

    required  this.status,

    required  this.total,

    required  this.cash,

    required  this.card,

    required  this.transfer,

     this.cashRefunded,

    required  this.expectedCash,

    required  this.payments,

    required  this.closing,
  });

  @JsonKey(
    
    name: r'branchId',
    required: true,
    includeIfNull: false,
  )


  final String branchId;



  @JsonKey(
    
    name: r'date',
    required: true,
    includeIfNull: false,
  )


  final DateTime date;



      /// IANA saat dilimi
  @JsonKey(
    
    name: r'timeZone',
    required: true,
    includeIfNull: false,
  )


  final String timeZone;



      /// \"gün açık\" / gün kapatıldı
  @JsonKey(
    
    name: r'status',
    required: true,
    includeIfNull: false,
  unknownEnumValue: CashDayStatusEnum.unknownDefaultOpenApi,
  )


  final CashDayStatusEnum status;



      /// \"Bugün tahsil edilen\" (PackageRedemption hariç tutar; sayı tüm kayıtlar)
  @JsonKey(
    
    name: r'total',
    required: true,
    includeIfNull: false,
  )


  final CashMethodTotal total;



  @JsonKey(
    
    name: r'cash',
    required: true,
    includeIfNull: false,
  )


  final CashMethodTotal cash;



  @JsonKey(
    
    name: r'card',
    required: true,
    includeIfNull: false,
  )


  final CashMethodTotal card;



  @JsonKey(
    
    name: r'transfer',
    required: true,
    includeIfNull: false,
  )


  final CashDayTransfer transfer;



      /// O gün işaretlenen nakit iadeler
  @JsonKey(
    
    name: r'cashRefunded',
    required: false,
    includeIfNull: false,
  )


  final Money? cashRefunded;



      /// \"kasada olmalı\" / \"Beklenen\"
  @JsonKey(
    
    name: r'expectedCash',
    required: true,
    includeIfNull: false,
  )


  final Money expectedCash;



      /// Yeniden eskiye; sayfalanmaz (bir gün)
  @JsonKey(
    
    name: r'payments',
    required: true,
    includeIfNull: false,
  )


  final List<PaymentView> payments;



  @JsonKey(
    
    name: r'closing',
    required: true,
    includeIfNull: true,
  )


  final CashDayClosing? closing;





    @override
    bool operator ==(Object other) => identical(this, other) || other is CashDay &&
      other.branchId == branchId &&
      other.date == date &&
      other.timeZone == timeZone &&
      other.status == status &&
      other.total == total &&
      other.cash == cash &&
      other.card == card &&
      other.transfer == transfer &&
      other.cashRefunded == cashRefunded &&
      other.expectedCash == expectedCash &&
      other.payments == payments &&
      other.closing == closing;

    @override
    int get hashCode =>
        branchId.hashCode +
        date.hashCode +
        timeZone.hashCode +
        status.hashCode +
        total.hashCode +
        cash.hashCode +
        card.hashCode +
        transfer.hashCode +
        cashRefunded.hashCode +
        expectedCash.hashCode +
        payments.hashCode +
        (closing == null ? 0 : closing.hashCode);

  factory CashDay.fromJson(Map<String, dynamic> json) => _$CashDayFromJson(json);

  Map<String, dynamic> toJson() => _$CashDayToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

/// \"gün açık\" / gün kapatıldı
enum CashDayStatusEnum {
@JsonValue(r'Open')
open(r'Open'),
@JsonValue(r'Closed')
closed(r'Closed'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const CashDayStatusEnum(this.value);

final String value;

@override
String toString() => value;
}


