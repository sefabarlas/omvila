//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/manage_overview_pending_approval.dart';
import 'package:omvila_api/src/model/manage_overview_walk_ins.dart';
import 'package:omvila_api/src/model/manage_overview_weekly_occupancy.dart';
import 'package:omvila_api/src/model/money.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'manage_overview.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageOverview {
  /// Returns a new [ManageOverview] instance.
  ManageOverview({

    required  this.branchId,

    required  this.branchName,

    required  this.timeZone,

    required  this.date,

    required  this.generatedUtc,

    required  this.todayRevenue,

    required  this.todayAppointmentCount,

    required  this.pendingApproval,

    required  this.walkIns,

    required  this.weeklyOccupancy,
  });

      /// Şube
  @JsonKey(
    
    name: r'branchId',
    required: true,
    includeIfNull: false,
  )


  final String branchId;



      /// Şube adı
  @JsonKey(
    
    name: r'branchName',
    required: true,
    includeIfNull: false,
  )


  final String branchName;



      /// IANA saat dilimi
  @JsonKey(
    
    name: r'timeZone',
    required: true,
    includeIfNull: false,
  )


  final String timeZone;



      /// Şubenin yerel bugünü
  @JsonKey(
    
    name: r'date',
    required: true,
    includeIfNull: false,
  )
  @DateOnlyConverter()


  final DateTime date;



      /// Yanıtın üretildiği an; kalan süreler buna göre
  @JsonKey(
    
    name: r'generatedUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime generatedUtc;



      /// \"BUGÜNKÜ CİRO\": bugünkü tahsilat toplamı (iade düşülmüş)
  @JsonKey(
    
    name: r'todayRevenue',
    required: true,
    includeIfNull: false,
  )


  final Money todayRevenue;



      /// \"16 randevu\": bugünün iptal ve taslak dışı randevuları
          // minimum: 0
  @JsonKey(
    
    name: r'todayAppointmentCount',
    required: true,
    includeIfNull: false,
  )


  final int todayAppointmentCount;



  @JsonKey(
    
    name: r'pendingApproval',
    required: true,
    includeIfNull: true,
  )


  final ManageOverviewPendingApproval? pendingApproval;



  @JsonKey(
    
    name: r'walkIns',
    required: true,
    includeIfNull: true,
  )


  final ManageOverviewWalkIns? walkIns;



  @JsonKey(
    
    name: r'weeklyOccupancy',
    required: true,
    includeIfNull: false,
  )


  final ManageOverviewWeeklyOccupancy weeklyOccupancy;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageOverview &&
      other.branchId == branchId &&
      other.branchName == branchName &&
      other.timeZone == timeZone &&
      other.date == date &&
      other.generatedUtc == generatedUtc &&
      other.todayRevenue == todayRevenue &&
      other.todayAppointmentCount == todayAppointmentCount &&
      other.pendingApproval == pendingApproval &&
      other.walkIns == walkIns &&
      other.weeklyOccupancy == weeklyOccupancy;

    @override
    int get hashCode =>
        branchId.hashCode +
        branchName.hashCode +
        timeZone.hashCode +
        date.hashCode +
        generatedUtc.hashCode +
        todayRevenue.hashCode +
        todayAppointmentCount.hashCode +
        (pendingApproval == null ? 0 : pendingApproval.hashCode) +
        (walkIns == null ? 0 : walkIns.hashCode) +
        weeklyOccupancy.hashCode;

  factory ManageOverview.fromJson(Map<String, dynamic> json) => _$ManageOverviewFromJson(json);

  Map<String, dynamic> toJson() => _$ManageOverviewToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

