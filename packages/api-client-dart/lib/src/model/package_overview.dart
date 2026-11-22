//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/money.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'package_overview.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PackageOverview {
  /// Returns a new [PackageOverview] instance.
  PackageOverview({

    required  this.activeCustomerPackages,

    required  this.remainingSessions,

    required  this.soldThisMonth,

    required  this.soldThisMonthAmount,

    required  this.expiringSoon,

     this.usageRatePercent,
  });

      /// \"Aktif müşteri paketi\"
  @JsonKey(
    
    name: r'activeCustomerPackages',
    required: true,
    includeIfNull: false,
  )


  final int activeCustomerPackages;



      /// \"kalan 182 seans\"
  @JsonKey(
    
    name: r'remainingSessions',
    required: true,
    includeIfNull: false,
  )


  final int remainingSessions;



      /// \"Bu ay satılan\" (şubenin yerel takvim ayı)
  @JsonKey(
    
    name: r'soldThisMonth',
    required: true,
    includeIfNull: false,
  )


  final int soldThisMonth;



  @JsonKey(
    
    name: r'soldThisMonthAmount',
    required: true,
    includeIfNull: false,
  )


  final Money soldThisMonthAmount;



      /// \"Süresi yaklaşan\": 30 gün içinde biten Active paketler
  @JsonKey(
    
    name: r'expiringSoon',
    required: true,
    includeIfNull: false,
  )


  final int expiringSoon;



      /// \"Kullanım oranı\": süresi biten paketlerde kullanılan seans / toplam seans; süresi biten paket yoksa null
  @JsonKey(
    
    name: r'usageRatePercent',
    required: false,
    includeIfNull: false,
  )


  final num? usageRatePercent;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PackageOverview &&
      other.activeCustomerPackages == activeCustomerPackages &&
      other.remainingSessions == remainingSessions &&
      other.soldThisMonth == soldThisMonth &&
      other.soldThisMonthAmount == soldThisMonthAmount &&
      other.expiringSoon == expiringSoon &&
      other.usageRatePercent == usageRatePercent;

    @override
    int get hashCode =>
        activeCustomerPackages.hashCode +
        remainingSessions.hashCode +
        soldThisMonth.hashCode +
        soldThisMonthAmount.hashCode +
        expiringSoon.hashCode +
        (usageRatePercent == null ? 0 : usageRatePercent.hashCode);

  factory PackageOverview.fromJson(Map<String, dynamic> json) => _$PackageOverviewFromJson(json);

  Map<String, dynamic> toJson() => _$PackageOverviewToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

