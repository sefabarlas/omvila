//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/money.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'report_overview_no_show.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ReportOverviewNoShow {
  /// Returns a new [ReportOverviewNoShow] instance.
  ReportOverviewNoShow({

    required  this.count,

    required  this.percent,

    required  this.lostRevenue,
  });

      /// Gelmedi sayısı
          // minimum: 0
  @JsonKey(
    
    name: r'count',
    required: true,
    includeIfNull: false,
  )


  final int count;



      /// Sonuçlanan randevulara oranı; sonuçlanan yoksa null
          // minimum: 0
          // maximum: 100
  @JsonKey(
    
    name: r'percent',
    required: true,
    includeIfNull: true,
  )


  final num? percent;



      /// Kayıp (hizmet tutarı − alınmış kapora)
  @JsonKey(
    
    name: r'lostRevenue',
    required: true,
    includeIfNull: false,
  )


  final Money lostRevenue;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ReportOverviewNoShow &&
      other.count == count &&
      other.percent == percent &&
      other.lostRevenue == lostRevenue;

    @override
    int get hashCode =>
        count.hashCode +
        (percent == null ? 0 : percent.hashCode) +
        lostRevenue.hashCode;

  factory ReportOverviewNoShow.fromJson(Map<String, dynamic> json) => _$ReportOverviewNoShowFromJson(json);

  Map<String, dynamic> toJson() => _$ReportOverviewNoShowToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

