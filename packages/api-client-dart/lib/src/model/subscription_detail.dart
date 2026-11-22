//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/billing_cycle.dart';
import 'package:omvila_api/src/model/subscription_detail_branches.dart';
import 'package:omvila_api/src/model/money.dart';
import 'package:omvila_api/src/model/plan_ref.dart';
import 'package:omvila_api/src/model/subscription_status.dart';
import 'package:omvila_api/src/model/subscription_detail_staff.dart';
import 'package:omvila_api/src/model/omvila_bank_account.dart';
import 'package:omvila_api/src/model/payment_notice_summary.dart';
import 'package:omvila_api/src/model/subscription_payment_method.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'subscription_detail.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class SubscriptionDetail {
  /// Returns a new [SubscriptionDetail] instance.
  SubscriptionDetail({

    required  this.status,

    required  this.plan,

    required  this.billingCycle,

    required  this.paymentMethod,

    required  this.trialEndsOn,

    required  this.currentPeriodStart,

    required  this.currentPeriodEnd,

    required  this.pastDueSince,

    required  this.readOnlyOn,

    required  this.readOnlySince,

    required  this.branches,

    required  this.staff,

    required  this.amountDue,

    required  this.transferCode,

    required  this.omvilaBankAccount,

    required  this.pendingNotice,
  });

  @JsonKey(
    
    name: r'status',
    required: true,
    includeIfNull: false,
  unknownEnumValue: SubscriptionStatus.unknownDefaultOpenApi,
  )


  final SubscriptionStatus status;



  @JsonKey(
    
    name: r'plan',
    required: true,
    includeIfNull: false,
  )


  final PlanRef plan;



  @JsonKey(
    
    name: r'billingCycle',
    required: true,
    includeIfNull: false,
  unknownEnumValue: BillingCycle.unknownDefaultOpenApi,
  )


  final BillingCycle billingCycle;



      /// Son kullanılan ödeme yolu; henüz ödeme yoksa null
  @JsonKey(
    
    name: r'paymentMethod',
    required: true,
    includeIfNull: true,
  unknownEnumValue: SubscriptionPaymentMethod.unknownDefaultOpenApi,
  )


  final SubscriptionPaymentMethod? paymentMethod;



      /// Deneme bitiş günü (Trialing değilse null)
  @JsonKey(
    
    name: r'trialEndsOn',
    required: true,
    includeIfNull: true,
  )
  @DateOnlyConverter()


  final DateTime? trialEndsOn;



      /// Geçerli dönemin ilk günü
  @JsonKey(
    
    name: r'currentPeriodStart',
    required: true,
    includeIfNull: false,
  )
  @DateOnlyConverter()


  final DateTime currentPeriodStart;



      /// Geçerli dönemin son günü
  @JsonKey(
    
    name: r'currentPeriodEnd',
    required: true,
    includeIfNull: false,
  )
  @DateOnlyConverter()


  final DateTime currentPeriodEnd;



      /// Ödeme gecikti başlangıcı
  @JsonKey(
    
    name: r'pastDueSince',
    required: true,
    includeIfNull: true,
  )
  @DateOnlyConverter()


  final DateTime? pastDueSince;



      /// PastDue ise salt okunura geçeceği gün
  @JsonKey(
    
    name: r'readOnlyOn',
    required: true,
    includeIfNull: true,
  )
  @DateOnlyConverter()


  final DateTime? readOnlyOn;



      /// ReadOnly ise salt okunura geçtiği gün
  @JsonKey(
    
    name: r'readOnlySince',
    required: true,
    includeIfNull: true,
  )
  @DateOnlyConverter()


  final DateTime? readOnlySince;



  @JsonKey(
    
    name: r'branches',
    required: true,
    includeIfNull: false,
  )


  final SubscriptionDetailBranches branches;



  @JsonKey(
    
    name: r'staff',
    required: true,
    includeIfNull: false,
  )


  final SubscriptionDetailStaff staff;



      /// Bir dönemin tutarı (plan fiyatı + ek şube ücreti, KDV dahil); fiyat tanımlı değilse null (\"Tutar\")
  @JsonKey(
    
    name: r'amountDue',
    required: true,
    includeIfNull: true,
  )


  final Money? amountDue;



      /// Markanın sabit abonelik havale kodu (\"Açıklama\")
  @JsonKey(
    
    name: r'transferCode',
    required: true,
    includeIfNull: false,
  )


  final String transferCode;



      /// Alıcı ve IBAN; yapılandırma yoksa null
  @JsonKey(
    
    name: r'omvilaBankAccount',
    required: true,
    includeIfNull: true,
  )


  final OmvilaBankAccount? omvilaBankAccount;



      /// Bekleyen abonelik havale bildirimi (\"Havale bildirildi · eşleştirme bekliyor\"); yoksa null
  @JsonKey(
    
    name: r'pendingNotice',
    required: true,
    includeIfNull: true,
  )


  final PaymentNoticeSummary? pendingNotice;





    @override
    bool operator ==(Object other) => identical(this, other) || other is SubscriptionDetail &&
      other.status == status &&
      other.plan == plan &&
      other.billingCycle == billingCycle &&
      other.paymentMethod == paymentMethod &&
      other.trialEndsOn == trialEndsOn &&
      other.currentPeriodStart == currentPeriodStart &&
      other.currentPeriodEnd == currentPeriodEnd &&
      other.pastDueSince == pastDueSince &&
      other.readOnlyOn == readOnlyOn &&
      other.readOnlySince == readOnlySince &&
      other.branches == branches &&
      other.staff == staff &&
      other.amountDue == amountDue &&
      other.transferCode == transferCode &&
      other.omvilaBankAccount == omvilaBankAccount &&
      other.pendingNotice == pendingNotice;

    @override
    int get hashCode =>
        status.hashCode +
        plan.hashCode +
        billingCycle.hashCode +
        (paymentMethod == null ? 0 : paymentMethod.hashCode) +
        (trialEndsOn == null ? 0 : trialEndsOn.hashCode) +
        currentPeriodStart.hashCode +
        currentPeriodEnd.hashCode +
        (pastDueSince == null ? 0 : pastDueSince.hashCode) +
        (readOnlyOn == null ? 0 : readOnlyOn.hashCode) +
        (readOnlySince == null ? 0 : readOnlySince.hashCode) +
        branches.hashCode +
        staff.hashCode +
        (amountDue == null ? 0 : amountDue.hashCode) +
        transferCode.hashCode +
        (omvilaBankAccount == null ? 0 : omvilaBankAccount.hashCode) +
        (pendingNotice == null ? 0 : pendingNotice.hashCode);

  factory SubscriptionDetail.fromJson(Map<String, dynamic> json) => _$SubscriptionDetailFromJson(json);

  Map<String, dynamic> toJson() => _$SubscriptionDetailToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

