//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/commission_day_amount.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'today_commission.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class TodayCommission {
  /// Returns a new [TodayCommission] instance.
  TodayCommission({

    required  this.month,

    required  this.totalMinor,

    required  this.currency,

    required  this.daily,
  });

      /// Yıl-ay (şubenin yerel takvimi)
  @JsonKey(
    
    name: r'month',
    required: true,
    includeIfNull: false,
  )


  final String month;



      /// Ayın kişisel prim toplamı (kuruş; ters kayıtlar düşülmüş)
  @JsonKey(
    
    name: r'totalMinor',
    required: true,
    includeIfNull: false,
  )


  final int totalMinor;



      /// Para birimi
  @JsonKey(
    
    name: r'currency',
    required: true,
    includeIfNull: false,
  unknownEnumValue: TodayCommissionCurrencyEnum.unknownDefaultOpenApi,
  )


  final TodayCommissionCurrencyEnum currency;



      /// Ayın başından bugüne prim olan günler (grafik istemcide kümülatif çizilir; kaydı olmayan gün listede yoktur)
  @JsonKey(
    
    name: r'daily',
    required: true,
    includeIfNull: false,
  )


  final List<CommissionDayAmount> daily;





    @override
    bool operator ==(Object other) => identical(this, other) || other is TodayCommission &&
      other.month == month &&
      other.totalMinor == totalMinor &&
      other.currency == currency &&
      other.daily == daily;

    @override
    int get hashCode =>
        month.hashCode +
        totalMinor.hashCode +
        currency.hashCode +
        daily.hashCode;

  factory TodayCommission.fromJson(Map<String, dynamic> json) => _$TodayCommissionFromJson(json);

  Map<String, dynamic> toJson() => _$TodayCommissionToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

/// Para birimi
enum TodayCommissionCurrencyEnum {
@JsonValue(r'TRY')
TRY(r'TRY'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const TodayCommissionCurrencyEnum(this.value);

final String value;

@override
String toString() => value;
}


