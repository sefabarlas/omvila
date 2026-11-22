//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/industry_code.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_attention_brand.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminAttentionBrand {
  /// Returns a new [AdminAttentionBrand] instance.
  AdminAttentionBrand({

    required  this.brandId,

    required  this.brandName,

    required  this.industryCode,

    required  this.city,

    required  this.planName,

    required  this.reason,

    required  this.pastDueDays,

    required  this.trialDaysLeft,

    required  this.branchCount,

    required  this.appointmentsThisMonth,
  });

      /// Marka
  @JsonKey(
    
    name: r'brandId',
    required: true,
    includeIfNull: false,
  )


  final String brandId;



      /// Marka adı
  @JsonKey(
    
    name: r'brandName',
    required: true,
    includeIfNull: false,
  )


  final String brandName;



  @JsonKey(
    
    name: r'industryCode',
    required: true,
    includeIfNull: false,
  unknownEnumValue: IndustryCode.unknownDefaultOpenApi,
  )


  final IndustryCode industryCode;



      /// Ana şubenin ili
  @JsonKey(
    
    name: r'city',
    required: true,
    includeIfNull: true,
  )


  final String? city;



      /// Plan adı
  @JsonKey(
    
    name: r'planName',
    required: true,
    includeIfNull: false,
  )


  final String planName;



      /// \"Ödeme gecikti · 6 gün\", \"Kontör bitti\", \"Deneme · 2 gün\"
  @JsonKey(
    
    name: r'reason',
    required: true,
    includeIfNull: false,
  unknownEnumValue: AdminAttentionBrandReasonEnum.unknownDefaultOpenApi,
  )


  final AdminAttentionBrandReasonEnum reason;



      /// Ödeme gecikti gün sayısı
          // minimum: 0
  @JsonKey(
    
    name: r'pastDueDays',
    required: true,
    includeIfNull: true,
  )


  final int? pastDueDays;



      /// Denemenin bitmesine kalan gün
          // minimum: 0
  @JsonKey(
    
    name: r'trialDaysLeft',
    required: true,
    includeIfNull: true,
  )


  final int? trialDaysLeft;



      /// Şube sayısı
          // minimum: 0
  @JsonKey(
    
    name: r'branchCount',
    required: true,
    includeIfNull: false,
  )


  final int branchCount;



      /// Bu ay randevu
          // minimum: 0
  @JsonKey(
    
    name: r'appointmentsThisMonth',
    required: true,
    includeIfNull: false,
  )


  final int appointmentsThisMonth;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminAttentionBrand &&
      other.brandId == brandId &&
      other.brandName == brandName &&
      other.industryCode == industryCode &&
      other.city == city &&
      other.planName == planName &&
      other.reason == reason &&
      other.pastDueDays == pastDueDays &&
      other.trialDaysLeft == trialDaysLeft &&
      other.branchCount == branchCount &&
      other.appointmentsThisMonth == appointmentsThisMonth;

    @override
    int get hashCode =>
        brandId.hashCode +
        brandName.hashCode +
        industryCode.hashCode +
        (city == null ? 0 : city.hashCode) +
        planName.hashCode +
        reason.hashCode +
        (pastDueDays == null ? 0 : pastDueDays.hashCode) +
        (trialDaysLeft == null ? 0 : trialDaysLeft.hashCode) +
        branchCount.hashCode +
        appointmentsThisMonth.hashCode;

  factory AdminAttentionBrand.fromJson(Map<String, dynamic> json) => _$AdminAttentionBrandFromJson(json);

  Map<String, dynamic> toJson() => _$AdminAttentionBrandToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

/// \"Ödeme gecikti · 6 gün\", \"Kontör bitti\", \"Deneme · 2 gün\"
enum AdminAttentionBrandReasonEnum {
@JsonValue(r'PastDue')
pastDue(r'PastDue'),
@JsonValue(r'CreditsDepleted')
creditsDepleted(r'CreditsDepleted'),
@JsonValue(r'TrialEnding')
trialEnding(r'TrialEnding'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const AdminAttentionBrandReasonEnum(this.value);

final String value;

@override
String toString() => value;
}


