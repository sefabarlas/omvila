//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/commission_report_row.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'commission_report.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class CommissionReport {
  /// Returns a new [CommissionReport] instance.
  CommissionReport({

    required  this.branchId,

    required  this.from,

    required  this.to,

    required  this.currency,

    required  this.items,

    required  this.totalCommissionMinor,
  });

      /// Şube; null = erişilebilen bütün şubeler
  @JsonKey(
    
    name: r'branchId',
    required: true,
    includeIfNull: true,
  )


  final String? branchId;



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



      /// Tutarların para birimi
  @JsonKey(
    
    name: r'currency',
    required: true,
    includeIfNull: false,
  unknownEnumValue: CommissionReportCurrencyEnum.unknownDefaultOpenApi,
  )


  final CommissionReportCurrencyEnum currency;



      /// Toplam prime göre azalan
  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<CommissionReportRow> items;



      /// Tablonun alt toplamı (\"15.665 ₺\")
  @JsonKey(
    
    name: r'totalCommissionMinor',
    required: true,
    includeIfNull: false,
  )


  final int totalCommissionMinor;





    @override
    bool operator ==(Object other) => identical(this, other) || other is CommissionReport &&
      other.branchId == branchId &&
      other.from == from &&
      other.to == to &&
      other.currency == currency &&
      other.items == items &&
      other.totalCommissionMinor == totalCommissionMinor;

    @override
    int get hashCode =>
        (branchId == null ? 0 : branchId.hashCode) +
        from.hashCode +
        to.hashCode +
        currency.hashCode +
        items.hashCode +
        totalCommissionMinor.hashCode;

  factory CommissionReport.fromJson(Map<String, dynamic> json) => _$CommissionReportFromJson(json);

  Map<String, dynamic> toJson() => _$CommissionReportToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

/// Tutarların para birimi
enum CommissionReportCurrencyEnum {
@JsonValue(r'TRY')
TRY(r'TRY'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const CommissionReportCurrencyEnum(this.value);

final String value;

@override
String toString() => value;
}


