//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'manage_overview_weekly_occupancy_staff_inner.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageOverviewWeeklyOccupancyStaffInner {
  /// Returns a new [ManageOverviewWeeklyOccupancyStaffInner] instance.
  ManageOverviewWeeklyOccupancyStaffInner({

    required  this.staffId,

    required  this.staffName,

    required  this.calendarColor,

    required  this.percent,
  });

      /// Personel
  @JsonKey(
    
    name: r'staffId',
    required: true,
    includeIfNull: false,
  )


  final String staffId;



      /// Personel adı
  @JsonKey(
    
    name: r'staffName',
    required: true,
    includeIfNull: false,
  )


  final String staffName;



      /// Takvim rengi (satırdaki nokta)
  @JsonKey(
    
    name: r'calendarColor',
    required: true,
    includeIfNull: false,
  )


  final String calendarColor;



      /// Personelin doluluk yüzdesi
          // minimum: 0
          // maximum: 100
  @JsonKey(
    
    name: r'percent',
    required: true,
    includeIfNull: false,
  )


  final int percent;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageOverviewWeeklyOccupancyStaffInner &&
      other.staffId == staffId &&
      other.staffName == staffName &&
      other.calendarColor == calendarColor &&
      other.percent == percent;

    @override
    int get hashCode =>
        staffId.hashCode +
        staffName.hashCode +
        calendarColor.hashCode +
        percent.hashCode;

  factory ManageOverviewWeeklyOccupancyStaffInner.fromJson(Map<String, dynamic> json) => _$ManageOverviewWeeklyOccupancyStaffInnerFromJson(json);

  Map<String, dynamic> toJson() => _$ManageOverviewWeeklyOccupancyStaffInnerToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

