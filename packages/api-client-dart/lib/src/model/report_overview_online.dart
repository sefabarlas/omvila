//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'report_overview_online.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ReportOverviewOnline {
  /// Returns a new [ReportOverviewOnline] instance.
  ReportOverviewOnline({

    required  this.appointmentCount,

    required  this.percent,
  });

      /// Online kanaldan gelen randevu
          // minimum: 0
  @JsonKey(
    
    name: r'appointmentCount',
    required: true,
    includeIfNull: false,
  )


  final int appointmentCount;



      /// Online oranı; randevu yoksa null
          // minimum: 0
          // maximum: 100
  @JsonKey(
    
    name: r'percent',
    required: true,
    includeIfNull: true,
  )


  final num? percent;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ReportOverviewOnline &&
      other.appointmentCount == appointmentCount &&
      other.percent == percent;

    @override
    int get hashCode =>
        appointmentCount.hashCode +
        (percent == null ? 0 : percent.hashCode);

  factory ReportOverviewOnline.fromJson(Map<String, dynamic> json) => _$ReportOverviewOnlineFromJson(json);

  Map<String, dynamic> toJson() => _$ReportOverviewOnlineToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

