//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/commission_week_day.dart';
import 'package:omvila_api/src/model/my_commission_entry.dart';
import 'package:omvila_api/src/model/my_commission_summary_next_accrual.dart';
import 'package:omvila_api/src/model/commission_day_amount.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'my_commission_summary.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MyCommissionSummary {
  /// Returns a new [MyCommissionSummary] instance.
  MyCommissionSummary({

    required  this.staffId,

    required  this.staffName,

    required  this.timeZone,

    required  this.month,

    required  this.from,

    required  this.to,

    required  this.totalMinor,

    required  this.serviceRevenueMinor,

    required  this.currency,

    required  this.appointmentCount,

    required  this.noShowPercent,

    required  this.averageRating,

    required  this.daily,

    required  this.thisWeek,

    required  this.entries,

    required  this.nextAccrual,
  });

      /// Oturumdaki kullanıcının personel kaydı
  @JsonKey(
    
    name: r'staffId',
    required: true,
    includeIfNull: false,
  )


  final String staffId;



  @JsonKey(
    
    name: r'staffName',
    required: true,
    includeIfNull: false,
  )


  final String staffName;



      /// IANA saat dilimi
  @JsonKey(
    
    name: r'timeZone',
    required: true,
    includeIfNull: false,
  )


  final String timeZone;



      /// Yıl-ay
  @JsonKey(
    
    name: r'month',
    required: true,
    includeIfNull: false,
  )


  final String month;



      /// Dönem başı (\"1–25 Kasım\")
  @JsonKey(
    
    name: r'from',
    required: true,
    includeIfNull: false,
  )


  final DateTime from;



      /// Dönem sonu; bu ayda bugün
  @JsonKey(
    
    name: r'to',
    required: true,
    includeIfNull: false,
  )


  final DateTime to;



      /// Ayın kişisel prim toplamı (kuruş; ters kayıtlar düşülmüş, eksi olabilir)
  @JsonKey(
    
    name: r'totalMinor',
    required: true,
    includeIfNull: false,
  )


  final int totalMinor;



      /// \"Hizmet geliri\" (hizmet primi tabanları; ters kayıtlar düşülmüş)
  @JsonKey(
    
    name: r'serviceRevenueMinor',
    required: true,
    includeIfNull: false,
  )


  final int serviceRevenueMinor;



      /// Para birimi
  @JsonKey(
    
    name: r'currency',
    required: true,
    includeIfNull: false,
  unknownEnumValue: MyCommissionSummaryCurrencyEnum.unknownDefaultOpenApi,
  )


  final MyCommissionSummaryCurrencyEnum currency;



      /// \"Randevu\": ayda tamamlanan
          // minimum: 0
  @JsonKey(
    
    name: r'appointmentCount',
    required: true,
    includeIfNull: false,
  )


  final int appointmentCount;



      /// \"Gelmedi %4\": ayın sonuçlanan randevularında Gelmedi oranı; sonuçlanan yoksa null
  @JsonKey(
    
    name: r'noShowPercent',
    required: true,
    includeIfNull: true,
  )


  final num? noShowPercent;



      /// \"Puan\": son 90 gündeki değerlendirmelerin ortalaması; yoksa null
  @JsonKey(
    
    name: r'averageRating',
    required: true,
    includeIfNull: true,
  )


  final num? averageRating;



      /// Ayın prim olan günleri (Bugün kartındaki çizgi)
  @JsonKey(
    
    name: r'daily',
    required: true,
    includeIfNull: false,
  )


  final List<CommissionDayAmount> daily;



      /// Bu haftanın (Pazartesi başlar) bugüne kadarki günleri
  @JsonKey(
    
    name: r'thisWeek',
    required: true,
    includeIfNull: false,
  )


  final List<CommissionWeekDay> thisWeek;



      /// Ayın bütün prim kayıtları, yeniden eskiye (\"Son primler\" ilk satırları, \"Tümü\" hepsi)
  @JsonKey(
    
    name: r'entries',
    required: true,
    includeIfNull: false,
  )


  final List<MyCommissionEntry> entries;



  @JsonKey(
    
    name: r'nextAccrual',
    required: true,
    includeIfNull: true,
  )


  final MyCommissionSummaryNextAccrual? nextAccrual;





    @override
    bool operator ==(Object other) => identical(this, other) || other is MyCommissionSummary &&
      other.staffId == staffId &&
      other.staffName == staffName &&
      other.timeZone == timeZone &&
      other.month == month &&
      other.from == from &&
      other.to == to &&
      other.totalMinor == totalMinor &&
      other.serviceRevenueMinor == serviceRevenueMinor &&
      other.currency == currency &&
      other.appointmentCount == appointmentCount &&
      other.noShowPercent == noShowPercent &&
      other.averageRating == averageRating &&
      other.daily == daily &&
      other.thisWeek == thisWeek &&
      other.entries == entries &&
      other.nextAccrual == nextAccrual;

    @override
    int get hashCode =>
        staffId.hashCode +
        staffName.hashCode +
        timeZone.hashCode +
        month.hashCode +
        from.hashCode +
        to.hashCode +
        totalMinor.hashCode +
        serviceRevenueMinor.hashCode +
        currency.hashCode +
        appointmentCount.hashCode +
        (noShowPercent == null ? 0 : noShowPercent.hashCode) +
        (averageRating == null ? 0 : averageRating.hashCode) +
        daily.hashCode +
        thisWeek.hashCode +
        entries.hashCode +
        (nextAccrual == null ? 0 : nextAccrual.hashCode);

  factory MyCommissionSummary.fromJson(Map<String, dynamic> json) => _$MyCommissionSummaryFromJson(json);

  Map<String, dynamic> toJson() => _$MyCommissionSummaryToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

/// Para birimi
enum MyCommissionSummaryCurrencyEnum {
@JsonValue(r'TRY')
TRY(r'TRY'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const MyCommissionSummaryCurrencyEnum(this.value);

final String value;

@override
String toString() => value;
}


