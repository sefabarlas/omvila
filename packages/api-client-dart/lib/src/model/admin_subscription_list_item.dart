//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/subscription_status.dart';
import 'package:omvila_api/src/model/admin_subscription_list_item_next_step.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_subscription_list_item.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminSubscriptionListItem {
  /// Returns a new [AdminSubscriptionListItem] instance.
  AdminSubscriptionListItem({

    required  this.brandId,

    required  this.brandName,

    required  this.planName,

    required  this.status,

    required  this.pastDueDays,

    required  this.trialDaysLeft,

    required  this.creditBalance,

    required  this.creditsLow,

    required  this.nextStep,
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



      /// Plan adı
  @JsonKey(
    
    name: r'planName',
    required: true,
    includeIfNull: false,
  )


  final String planName;



  @JsonKey(
    
    name: r'status',
    required: true,
    includeIfNull: false,
  unknownEnumValue: SubscriptionStatus.unknownDefaultOpenApi,
  )


  final SubscriptionStatus status;



      /// \"Ödeme gecikti · 6. gün\"
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



      /// \"Kontör\"
          // minimum: 0
  @JsonKey(
    
    name: r'creditBalance',
    required: true,
    includeIfNull: false,
  )


  final int creditBalance;



      /// Kontör eşiğin altında (\"Aktif · kontör bitti\" bakiye 0 iken)
  @JsonKey(
    
    name: r'creditsLow',
    required: true,
    includeIfNull: false,
  )


  final bool creditsLow;



  @JsonKey(
    
    name: r'nextStep',
    required: true,
    includeIfNull: false,
  )


  final AdminSubscriptionListItemNextStep nextStep;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminSubscriptionListItem &&
      other.brandId == brandId &&
      other.brandName == brandName &&
      other.planName == planName &&
      other.status == status &&
      other.pastDueDays == pastDueDays &&
      other.trialDaysLeft == trialDaysLeft &&
      other.creditBalance == creditBalance &&
      other.creditsLow == creditsLow &&
      other.nextStep == nextStep;

    @override
    int get hashCode =>
        brandId.hashCode +
        brandName.hashCode +
        planName.hashCode +
        status.hashCode +
        (pastDueDays == null ? 0 : pastDueDays.hashCode) +
        (trialDaysLeft == null ? 0 : trialDaysLeft.hashCode) +
        creditBalance.hashCode +
        creditsLow.hashCode +
        nextStep.hashCode;

  factory AdminSubscriptionListItem.fromJson(Map<String, dynamic> json) => _$AdminSubscriptionListItemFromJson(json);

  Map<String, dynamic> toJson() => _$AdminSubscriptionListItemToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

