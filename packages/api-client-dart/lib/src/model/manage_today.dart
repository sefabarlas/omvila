//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/today_commission.dart';
import 'package:omvila_api/src/model/today_walk_in_summary.dart';
import 'package:omvila_api/src/model/today_appointment.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'manage_today.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageToday {
  /// Returns a new [ManageToday] instance.
  ManageToday({

    required  this.branchId,

    required  this.branchName,

    required  this.timeZone,

    required  this.date,

    required  this.generatedUtc,

    required  this.next,

    required  this.pendingApprovalCount,

    required  this.walkIns,

    required  this.commission,

    required  this.completedCount,

    required  this.totalCount,

    required  this.items,
  });

      /// Şube
  @JsonKey(
    
    name: r'branchId',
    required: true,
    includeIfNull: false,
  )


  final String branchId;



      /// Başlıktaki şube adı (\"Salı 25 Kasım · Kadıköy\")
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


  final DateTime date;



      /// Sunucunun \"şimdi\"si; \"50 dk sonra\" buna göre sayılır
  @JsonKey(
    
    name: r'generatedUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime generatedUtc;



      /// \"Sıradaki\": başlangıcı gelmemiş ilk Confirmed veya PendingApproval randevu; yoksa null
  @JsonKey(
    
    name: r'next',
    required: true,
    includeIfNull: true,
  )


  final TodayAppointment? next;



      /// \"Onay bekliyor\" (şubedeki PendingApproval talepler); appointment.approve izni yoksa null
          // minimum: 0
  @JsonKey(
    
    name: r'pendingApprovalCount',
    required: true,
    includeIfNull: true,
  )


  final int? pendingApprovalCount;



      /// Kapıda sıra özeti; walkin.manage izni yoksa null
  @JsonKey(
    
    name: r'walkIns',
    required: true,
    includeIfNull: true,
  )


  final TodayWalkInSummary? walkIns;



      /// Kişisel prim kartı; commission.view izni veya personel kaydı yoksa null
  @JsonKey(
    
    name: r'commission',
    required: true,
    includeIfNull: true,
  )


  final TodayCommission? commission;



      /// \"4/12 bitti\": tamamlanan
          // minimum: 0
  @JsonKey(
    
    name: r'completedCount',
    required: true,
    includeIfNull: false,
  )


  final int completedCount;



      /// \"4/12 bitti\": iptal edilmemiş bütün randevular
          // minimum: 0
  @JsonKey(
    
    name: r'totalCount',
    required: true,
    includeIfNull: false,
  )


  final int totalCount;



      /// Gün akışı, başlangıca göre artan; iptal edilenler yok. Boşsa MainBos
  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<TodayAppointment> items;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageToday &&
      other.branchId == branchId &&
      other.branchName == branchName &&
      other.timeZone == timeZone &&
      other.date == date &&
      other.generatedUtc == generatedUtc &&
      other.next == next &&
      other.pendingApprovalCount == pendingApprovalCount &&
      other.walkIns == walkIns &&
      other.commission == commission &&
      other.completedCount == completedCount &&
      other.totalCount == totalCount &&
      other.items == items;

    @override
    int get hashCode =>
        branchId.hashCode +
        branchName.hashCode +
        timeZone.hashCode +
        date.hashCode +
        generatedUtc.hashCode +
        (next == null ? 0 : next.hashCode) +
        (pendingApprovalCount == null ? 0 : pendingApprovalCount.hashCode) +
        (walkIns == null ? 0 : walkIns.hashCode) +
        (commission == null ? 0 : commission.hashCode) +
        completedCount.hashCode +
        totalCount.hashCode +
        items.hashCode;

  factory ManageToday.fromJson(Map<String, dynamic> json) => _$ManageTodayFromJson(json);

  Map<String, dynamic> toJson() => _$ManageTodayToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

