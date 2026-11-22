//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/money.dart';
import 'package:omvila_api/src/model/report_overview_daily_collections_inner.dart';
import 'package:omvila_api/src/model/report_overview_online.dart';
import 'package:omvila_api/src/model/report_overview_no_show.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'report_overview.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ReportOverview {
  /// Returns a new [ReportOverview] instance.
  ReportOverview({

    required  this.branchId,

    required  this.timeZone,

    required  this.from,

    required  this.to,

    required  this.earnedRevenue,

    required  this.completedAppointmentCount,

    required  this.earnedRevenueChangePercent,

    required  this.collections,

    required  this.noShow,

    required  this.online,

    required  this.dailyCollections,

    required  this.currency,
  });

      /// Şube; null = erişilebilen bütün şubeler
  @JsonKey(
    
    name: r'branchId',
    required: true,
    includeIfNull: true,
  )


  final String? branchId;



      /// IANA saat dilimi
  @JsonKey(
    
    name: r'timeZone',
    required: true,
    includeIfNull: false,
  )


  final String timeZone;



      /// Aralığın ilk günü
  @JsonKey(
    
    name: r'from',
    required: true,
    includeIfNull: false,
  )
  @DateOnlyConverter()


  final DateTime from;



      /// Aralığın son günü
  @JsonKey(
    
    name: r'to',
    required: true,
    includeIfNull: false,
  )
  @DateOnlyConverter()


  final DateTime to;



      /// \"HAK EDİLEN GELİR\"
  @JsonKey(
    
    name: r'earnedRevenue',
    required: true,
    includeIfNull: false,
  )


  final Money earnedRevenue;



      /// \"tamamlanan 118 randevu\"
          // minimum: 0
  @JsonKey(
    
    name: r'completedAppointmentCount',
    required: true,
    includeIfNull: false,
  )


  final int completedAppointmentCount;



      /// \"+%9\": önceki eşit aralığa göre değişim; önceki aralıkta gelir yoksa null
  @JsonKey(
    
    name: r'earnedRevenueChangePercent',
    required: true,
    includeIfNull: true,
  )


  final num? earnedRevenueChangePercent;



      /// \"TAHSİLAT\" (paket satışları ve kaporalar dahil, iadeler düşülmüş)
  @JsonKey(
    
    name: r'collections',
    required: true,
    includeIfNull: false,
  )


  final Money collections;



  @JsonKey(
    
    name: r'noShow',
    required: true,
    includeIfNull: false,
  )


  final ReportOverviewNoShow noShow;



  @JsonKey(
    
    name: r'online',
    required: true,
    includeIfNull: false,
  )


  final ReportOverviewOnline online;



      /// \"Günlük tahsilat\": aralıktaki her gün için bir satır
  @JsonKey(
    
    name: r'dailyCollections',
    required: true,
    includeIfNull: false,
  )


  final List<ReportOverviewDailyCollectionsInner> dailyCollections;



      /// dailyCollections tutarlarının para birimi
  @JsonKey(
    
    name: r'currency',
    required: true,
    includeIfNull: false,
  unknownEnumValue: ReportOverviewCurrencyEnum.unknownDefaultOpenApi,
  )


  final ReportOverviewCurrencyEnum currency;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ReportOverview &&
      other.branchId == branchId &&
      other.timeZone == timeZone &&
      other.from == from &&
      other.to == to &&
      other.earnedRevenue == earnedRevenue &&
      other.completedAppointmentCount == completedAppointmentCount &&
      other.earnedRevenueChangePercent == earnedRevenueChangePercent &&
      other.collections == collections &&
      other.noShow == noShow &&
      other.online == online &&
      other.dailyCollections == dailyCollections &&
      other.currency == currency;

    @override
    int get hashCode =>
        (branchId == null ? 0 : branchId.hashCode) +
        timeZone.hashCode +
        from.hashCode +
        to.hashCode +
        earnedRevenue.hashCode +
        completedAppointmentCount.hashCode +
        (earnedRevenueChangePercent == null ? 0 : earnedRevenueChangePercent.hashCode) +
        collections.hashCode +
        noShow.hashCode +
        online.hashCode +
        dailyCollections.hashCode +
        currency.hashCode;

  factory ReportOverview.fromJson(Map<String, dynamic> json) => _$ReportOverviewFromJson(json);

  Map<String, dynamic> toJson() => _$ReportOverviewToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

/// dailyCollections tutarlarının para birimi
enum ReportOverviewCurrencyEnum {
@JsonValue(r'TRY')
TRY(r'TRY'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const ReportOverviewCurrencyEnum(this.value);

final String value;

@override
String toString() => value;
}


