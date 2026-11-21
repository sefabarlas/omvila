//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/money.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'cash_day_closing.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class CashDayClosing {
  /// Returns a new [CashDayClosing] instance.
  CashDayClosing({

    required  this.expectedCash,

    required  this.countedCash,

    required  this.differenceMinor,

    required  this.difference,

     this.differenceNote,

     this.closedByName,

    required  this.closedUtc,
  });

  @JsonKey(
    
    name: r'expectedCash',
    required: true,
    includeIfNull: false,
  )


  final Money expectedCash;



  @JsonKey(
    
    name: r'countedCash',
    required: true,
    includeIfNull: false,
  )


  final Money countedCash;



      /// Sayılan − beklenen, kuruş; eksi olabilir (Money eksi taşımadığı için tamsayı)
  @JsonKey(
    
    name: r'differenceMinor',
    required: true,
    includeIfNull: false,
  )


  final int differenceMinor;



      /// None = fark yok, Shortage = kasada eksik, Surplus = kasada fazla
  @JsonKey(
    
    name: r'difference',
    required: true,
    includeIfNull: false,
  unknownEnumValue: CashDayClosingDifferenceEnum.unknownDefaultOpenApi,
  )


  final CashDayClosingDifferenceEnum difference;



  @JsonKey(
    
    name: r'differenceNote',
    required: false,
    includeIfNull: false,
  )


  final String? differenceNote;



  @JsonKey(
    
    name: r'closedByName',
    required: false,
    includeIfNull: false,
  )


  final String? closedByName;



  @JsonKey(
    
    name: r'closedUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime closedUtc;





    @override
    bool operator ==(Object other) => identical(this, other) || other is CashDayClosing &&
      other.expectedCash == expectedCash &&
      other.countedCash == countedCash &&
      other.differenceMinor == differenceMinor &&
      other.difference == difference &&
      other.differenceNote == differenceNote &&
      other.closedByName == closedByName &&
      other.closedUtc == closedUtc;

    @override
    int get hashCode =>
        expectedCash.hashCode +
        countedCash.hashCode +
        differenceMinor.hashCode +
        difference.hashCode +
        (differenceNote == null ? 0 : differenceNote.hashCode) +
        (closedByName == null ? 0 : closedByName.hashCode) +
        closedUtc.hashCode;

  factory CashDayClosing.fromJson(Map<String, dynamic> json) => _$CashDayClosingFromJson(json);

  Map<String, dynamic> toJson() => _$CashDayClosingToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

/// None = fark yok, Shortage = kasada eksik, Surplus = kasada fazla
enum CashDayClosingDifferenceEnum {
@JsonValue(r'None')
none(r'None'),
@JsonValue(r'Shortage')
shortage(r'Shortage'),
@JsonValue(r'Surplus')
surplus(r'Surplus'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const CashDayClosingDifferenceEnum(this.value);

final String value;

@override
String toString() => value;
}


