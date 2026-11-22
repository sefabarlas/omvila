//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/manage_overview_weekly_occupancy_staff_inner.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'manage_overview_weekly_occupancy.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageOverviewWeeklyOccupancy {
  /// Returns a new [ManageOverviewWeeklyOccupancy] instance.
  ManageOverviewWeeklyOccupancy({

    required  this.weekStart,

    required  this.percent,

    required  this.previousWeekPercent,

    required  this.staff,
  });

      /// Haftanın Pazartesi'si (yerel)
  @JsonKey(
    
    name: r'weekStart',
    required: true,
    includeIfNull: false,
  )
  @DateOnlyConverter()


  final DateTime weekStart;



      /// Şube doluluk yüzdesi; çalışma dakikası yoksa null
          // minimum: 0
          // maximum: 100
  @JsonKey(
    
    name: r'percent',
    required: true,
    includeIfNull: true,
  )


  final int? percent;



      /// \"geçen hafta %68\"
          // minimum: 0
          // maximum: 100
  @JsonKey(
    
    name: r'previousWeekPercent',
    required: true,
    includeIfNull: true,
  )


  final int? previousWeekPercent;



      /// Doluluğa göre azalan
  @JsonKey(
    
    name: r'staff',
    required: true,
    includeIfNull: false,
  )


  final List<ManageOverviewWeeklyOccupancyStaffInner> staff;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageOverviewWeeklyOccupancy &&
      other.weekStart == weekStart &&
      other.percent == percent &&
      other.previousWeekPercent == previousWeekPercent &&
      other.staff == staff;

    @override
    int get hashCode =>
        weekStart.hashCode +
        (percent == null ? 0 : percent.hashCode) +
        (previousWeekPercent == null ? 0 : previousWeekPercent.hashCode) +
        staff.hashCode;

  factory ManageOverviewWeeklyOccupancy.fromJson(Map<String, dynamic> json) => _$ManageOverviewWeeklyOccupancyFromJson(json);

  Map<String, dynamic> toJson() => _$ManageOverviewWeeklyOccupancyToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

