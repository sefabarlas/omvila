//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/billing_cycle.dart';
import 'package:omvila_api/src/model/subscription_status.dart';
import 'package:omvila_api/src/model/subscription_payment_method.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_brand_detail_subscription.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminBrandDetailSubscription {
  /// Returns a new [AdminBrandDetailSubscription] instance.
  AdminBrandDetailSubscription({

    required  this.status,

    required  this.planCode,

    required  this.planName,

    required  this.billingCycle,

    required  this.paymentMethod,

    required  this.trialEndsOn,

    required  this.trialDaysLeft,

    required  this.pastDueSince,

    required  this.readOnlyOn,

    required  this.transferCode,

    required  this.pendingNoticeId,

    required  this.pendingNoticeNotifiedUtc,
  });

  @JsonKey(
    
    name: r'status',
    required: true,
    includeIfNull: false,
  unknownEnumValue: SubscriptionStatus.unknownDefaultOpenApi,
  )


  final SubscriptionStatus status;



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
    
    name: r'billingCycle',
    required: true,
    includeIfNull: false,
  unknownEnumValue: BillingCycle.unknownDefaultOpenApi,
  )


  final BillingCycle billingCycle;



      /// Ödeme yolu; henüz yoksa null
  @JsonKey(
    
    name: r'paymentMethod',
    required: true,
    includeIfNull: true,
  unknownEnumValue: SubscriptionPaymentMethod.unknownDefaultOpenApi,
  )


  final SubscriptionPaymentMethod? paymentMethod;



      /// Deneme bitişi
  @JsonKey(
    
    name: r'trialEndsOn',
    required: true,
    includeIfNull: true,
  )
  @DateOnlyConverter()


  final DateTime? trialEndsOn;



      /// \"9 gün kaldı\"
          // minimum: 0
  @JsonKey(
    
    name: r'trialDaysLeft',
    required: true,
    includeIfNull: true,
  )


  final int? trialDaysLeft;



      /// Ödeme gecikti başlangıcı
  @JsonKey(
    
    name: r'pastDueSince',
    required: true,
    includeIfNull: true,
  )
  @DateOnlyConverter()


  final DateTime? pastDueSince;



      /// Salt okunura geçeceği gün
  @JsonKey(
    
    name: r'readOnlyOn',
    required: true,
    includeIfNull: true,
  )
  @DateOnlyConverter()


  final DateTime? readOnlyOn;



      /// Abonelik havale kodu
  @JsonKey(
    
    name: r'transferCode',
    required: true,
    includeIfNull: false,
  )


  final String transferCode;



      /// \"Kasım bildirildi · eşleştirme bekliyor\"
  @JsonKey(
    
    name: r'pendingNoticeId',
    required: true,
    includeIfNull: true,
  )


  final String? pendingNoticeId;



      /// Bekleyen bildirimin anı
  @JsonKey(
    
    name: r'pendingNoticeNotifiedUtc',
    required: true,
    includeIfNull: true,
  )


  final DateTime? pendingNoticeNotifiedUtc;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminBrandDetailSubscription &&
      other.status == status &&
      other.planCode == planCode &&
      other.planName == planName &&
      other.billingCycle == billingCycle &&
      other.paymentMethod == paymentMethod &&
      other.trialEndsOn == trialEndsOn &&
      other.trialDaysLeft == trialDaysLeft &&
      other.pastDueSince == pastDueSince &&
      other.readOnlyOn == readOnlyOn &&
      other.transferCode == transferCode &&
      other.pendingNoticeId == pendingNoticeId &&
      other.pendingNoticeNotifiedUtc == pendingNoticeNotifiedUtc;

    @override
    int get hashCode =>
        status.hashCode +
        planCode.hashCode +
        planName.hashCode +
        billingCycle.hashCode +
        (paymentMethod == null ? 0 : paymentMethod.hashCode) +
        (trialEndsOn == null ? 0 : trialEndsOn.hashCode) +
        (trialDaysLeft == null ? 0 : trialDaysLeft.hashCode) +
        (pastDueSince == null ? 0 : pastDueSince.hashCode) +
        (readOnlyOn == null ? 0 : readOnlyOn.hashCode) +
        transferCode.hashCode +
        (pendingNoticeId == null ? 0 : pendingNoticeId.hashCode) +
        (pendingNoticeNotifiedUtc == null ? 0 : pendingNoticeNotifiedUtc.hashCode);

  factory AdminBrandDetailSubscription.fromJson(Map<String, dynamic> json) => _$AdminBrandDetailSubscriptionFromJson(json);

  Map<String, dynamic> toJson() => _$AdminBrandDetailSubscriptionToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

