//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/industry_code.dart';
import 'package:omvila_api/src/model/subscription_status.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_brand_list_item.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminBrandListItem {
  /// Returns a new [AdminBrandListItem] instance.
  AdminBrandListItem({

    required  this.id,

    required  this.name,

    required  this.industryCode,

    required  this.city,

    required  this.brandStatus,

    required  this.planCode,

    required  this.planName,

    required  this.subscriptionStatus,

    required  this.trialDaysLeft,

    required  this.pastDueDays,

    required  this.branchCount,

    required  this.appointmentsThisMonth,

    required  this.creditBalance,

    required  this.depositEnabled,
  });

      /// Marka
  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



      /// Marka adı
  @JsonKey(
    
    name: r'name',
    required: true,
    includeIfNull: false,
  )


  final String name;



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



      /// Marka durumu
  @JsonKey(
    
    name: r'brandStatus',
    required: true,
    includeIfNull: false,
  unknownEnumValue: AdminBrandListItemBrandStatusEnum.unknownDefaultOpenApi,
  )


  final AdminBrandListItemBrandStatusEnum brandStatus;



      /// Plan kodu
  @JsonKey(
    
    name: r'planCode',
    required: true,
    includeIfNull: false,
  )


  final String planCode;



      /// Plan adı
  @JsonKey(
    
    name: r'planName',
    required: true,
    includeIfNull: false,
  )


  final String planName;



  @JsonKey(
    
    name: r'subscriptionStatus',
    required: true,
    includeIfNull: false,
  unknownEnumValue: SubscriptionStatus.unknownDefaultOpenApi,
  )


  final SubscriptionStatus subscriptionStatus;



      /// \"Deneme · 9 gün\"
          // minimum: 0
  @JsonKey(
    
    name: r'trialDaysLeft',
    required: true,
    includeIfNull: true,
  )


  final int? trialDaysLeft;



      /// Ödeme gecikti gün sayısı
          // minimum: 0
  @JsonKey(
    
    name: r'pastDueDays',
    required: true,
    includeIfNull: true,
  )


  final int? pastDueDays;



      /// \"Şube\"
          // minimum: 0
  @JsonKey(
    
    name: r'branchCount',
    required: true,
    includeIfNull: false,
  )


  final int branchCount;



      /// \"Bu ay randevu\"
          // minimum: 0
  @JsonKey(
    
    name: r'appointmentsThisMonth',
    required: true,
    includeIfNull: false,
  )


  final int appointmentsThisMonth;



      /// Kontör bakiyesi
          // minimum: 0
  @JsonKey(
    
    name: r'creditBalance',
    required: true,
    includeIfNull: false,
  )


  final int creditBalance;



      /// \"Kapora açık\": etkin kapora IBAN hesabı var
  @JsonKey(
    
    name: r'depositEnabled',
    required: true,
    includeIfNull: false,
  )


  final bool depositEnabled;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminBrandListItem &&
      other.id == id &&
      other.name == name &&
      other.industryCode == industryCode &&
      other.city == city &&
      other.brandStatus == brandStatus &&
      other.planCode == planCode &&
      other.planName == planName &&
      other.subscriptionStatus == subscriptionStatus &&
      other.trialDaysLeft == trialDaysLeft &&
      other.pastDueDays == pastDueDays &&
      other.branchCount == branchCount &&
      other.appointmentsThisMonth == appointmentsThisMonth &&
      other.creditBalance == creditBalance &&
      other.depositEnabled == depositEnabled;

    @override
    int get hashCode =>
        id.hashCode +
        name.hashCode +
        industryCode.hashCode +
        (city == null ? 0 : city.hashCode) +
        brandStatus.hashCode +
        planCode.hashCode +
        planName.hashCode +
        subscriptionStatus.hashCode +
        (trialDaysLeft == null ? 0 : trialDaysLeft.hashCode) +
        (pastDueDays == null ? 0 : pastDueDays.hashCode) +
        branchCount.hashCode +
        appointmentsThisMonth.hashCode +
        creditBalance.hashCode +
        depositEnabled.hashCode;

  factory AdminBrandListItem.fromJson(Map<String, dynamic> json) => _$AdminBrandListItemFromJson(json);

  Map<String, dynamic> toJson() => _$AdminBrandListItemToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

/// Marka durumu
enum AdminBrandListItemBrandStatusEnum {
@JsonValue(r'Onboarding')
onboarding(r'Onboarding'),
@JsonValue(r'Active')
active(r'Active'),
@JsonValue(r'Suspended')
suspended(r'Suspended'),
@JsonValue(r'Archived')
archived(r'Archived'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const AdminBrandListItemBrandStatusEnum(this.value);

final String value;

@override
String toString() => value;
}


