//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'report_overview_daily_collections_inner.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ReportOverviewDailyCollectionsInner {
  /// Returns a new [ReportOverviewDailyCollectionsInner] instance.
  ReportOverviewDailyCollectionsInner({

    required  this.date,

    required  this.amountMinor,

    required  this.isClosed,
  });

      /// Yerel gün
  @JsonKey(
    
    name: r'date',
    required: true,
    includeIfNull: false,
  )
  @DateOnlyConverter()


  final DateTime date;



      /// Günün net tahsilatı (kuruş; iade fazlaysa eksi olabilir)
  @JsonKey(
    
    name: r'amountMinor',
    required: true,
    includeIfNull: false,
  )


  final int amountMinor;



      /// Şube o gün kapalı (\"Pazartesi kapalı\")
  @JsonKey(
    
    name: r'isClosed',
    required: true,
    includeIfNull: false,
  )


  final bool isClosed;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ReportOverviewDailyCollectionsInner &&
      other.date == date &&
      other.amountMinor == amountMinor &&
      other.isClosed == isClosed;

    @override
    int get hashCode =>
        date.hashCode +
        amountMinor.hashCode +
        isClosed.hashCode;

  factory ReportOverviewDailyCollectionsInner.fromJson(Map<String, dynamic> json) => _$ReportOverviewDailyCollectionsInnerFromJson(json);

  Map<String, dynamic> toJson() => _$ReportOverviewDailyCollectionsInnerToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

