//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'commission_report_row.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class CommissionReportRow {
  /// Returns a new [CommissionReportRow] instance.
  CommissionReportRow({

    required  this.staffId,

    required  this.staffName,

    required  this.appointmentCount,

    required  this.serviceRevenueMinor,

    required  this.serviceCommissionPercent,

    required  this.serviceCommissionMinor,

    required  this.saleCommissionMinor,

    required  this.totalCommissionMinor,
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



      /// \"Randevu\": tamamlanan randevu
          // minimum: 0
  @JsonKey(
    
    name: r'appointmentCount',
    required: true,
    includeIfNull: false,
  )


  final int appointmentCount;



      /// \"Hizmet cirosu\" (hizmet primi tabanı, kuruş)
  @JsonKey(
    
    name: r'serviceRevenueMinor',
    required: true,
    includeIfNull: false,
  )


  final int serviceRevenueMinor;



      /// \"Oran\": etkin hizmet prim oranı; ciro yoksa null
          // minimum: 0
          // maximum: 100
  @JsonKey(
    
    name: r'serviceCommissionPercent',
    required: true,
    includeIfNull: true,
  )


  final num? serviceCommissionPercent;



      /// Hizmet primi (ters kayıtlar dahil; eksi olabilir)
  @JsonKey(
    
    name: r'serviceCommissionMinor',
    required: true,
    includeIfNull: false,
  )


  final int serviceCommissionMinor;



      /// Paket satış primi (ters kayıtlar dahil; eksi olabilir)
  @JsonKey(
    
    name: r'saleCommissionMinor',
    required: true,
    includeIfNull: false,
  )


  final int saleCommissionMinor;



      /// \"Prim\" toplamı
  @JsonKey(
    
    name: r'totalCommissionMinor',
    required: true,
    includeIfNull: false,
  )


  final int totalCommissionMinor;





    @override
    bool operator ==(Object other) => identical(this, other) || other is CommissionReportRow &&
      other.staffId == staffId &&
      other.staffName == staffName &&
      other.appointmentCount == appointmentCount &&
      other.serviceRevenueMinor == serviceRevenueMinor &&
      other.serviceCommissionPercent == serviceCommissionPercent &&
      other.serviceCommissionMinor == serviceCommissionMinor &&
      other.saleCommissionMinor == saleCommissionMinor &&
      other.totalCommissionMinor == totalCommissionMinor;

    @override
    int get hashCode =>
        staffId.hashCode +
        staffName.hashCode +
        appointmentCount.hashCode +
        serviceRevenueMinor.hashCode +
        (serviceCommissionPercent == null ? 0 : serviceCommissionPercent.hashCode) +
        serviceCommissionMinor.hashCode +
        saleCommissionMinor.hashCode +
        totalCommissionMinor.hashCode;

  factory CommissionReportRow.fromJson(Map<String, dynamic> json) => _$CommissionReportRowFromJson(json);

  Map<String, dynamic> toJson() => _$CommissionReportRowToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

