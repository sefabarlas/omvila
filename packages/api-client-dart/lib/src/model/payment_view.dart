//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/payment_status.dart';
import 'package:omvila_api/src/model/money.dart';
import 'package:omvila_api/src/model/customer_ref.dart';
import 'package:omvila_api/src/model/payment_method.dart';
import 'package:omvila_api/src/model/payment_purpose.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'payment_view.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PaymentView {
  /// Returns a new [PaymentView] instance.
  PaymentView({

    required  this.id,

    required  this.branchId,

    required  this.method,

    required  this.amount,

    required  this.isDeposit,

    required  this.status,

     this.refundedAmount,

    required  this.paidUtc,

     this.collectedByName,

     this.reference,

    required  this.purpose,

     this.customer,

     this.appointmentId,

     this.appointmentStartUtc,

     this.serviceNames,

     this.isBalanceAfterDeposit,

     this.staffName,

     this.customerPackageId,

     this.packageName,

     this.packageSessionCount,

     this.recordedAfterDayClose,
  });

  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



  @JsonKey(
    
    name: r'branchId',
    required: true,
    includeIfNull: false,
  )


  final String branchId;



  @JsonKey(
    
    name: r'method',
    required: true,
    includeIfNull: false,
  unknownEnumValue: PaymentMethod.unknownDefaultOpenApi,
  )


  final PaymentMethod method;



  @JsonKey(
    
    name: r'amount',
    required: true,
    includeIfNull: false,
  )


  final Money amount;



      /// Kapora havalesi (\"Kapora geldi\" ile yazılan kayıt)
  @JsonKey(
    
    name: r'isDeposit',
    required: true,
    includeIfNull: false,
  )


  final bool isDeposit;



  @JsonKey(
    
    name: r'status',
    required: true,
    includeIfNull: false,
  unknownEnumValue: PaymentStatus.unknownDefaultOpenApi,
  )


  final PaymentStatus status;



      /// \"Salon iade etti\" ile işaretlenen tutar; yalnız iade varsa bulunur
  @JsonKey(
    
    name: r'refundedAmount',
    required: false,
    includeIfNull: false,
  )


  final Money? refundedAmount;



  @JsonKey(
    
    name: r'paidUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime paidUtc;



      /// Tahsilatı kaydeden kullanıcı
  @JsonKey(
    
    name: r'collectedByName',
    required: false,
    includeIfNull: false,
  )


  final String? collectedByName;



      /// Havale açıklama kodu vb.
  @JsonKey(
    
    name: r'reference',
    required: false,
    includeIfNull: false,
  )


  final String? reference;



  @JsonKey(
    
    name: r'purpose',
    required: true,
    includeIfNull: false,
  unknownEnumValue: PaymentPurpose.unknownDefaultOpenApi,
  )


  final PaymentPurpose purpose;



  @JsonKey(
    
    name: r'customer',
    required: false,
    includeIfNull: false,
  )


  final CustomerRef? customer;



  @JsonKey(
    
    name: r'appointmentId',
    required: false,
    includeIfNull: false,
  )


  final String? appointmentId;



  @JsonKey(
    
    name: r'appointmentStartUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? appointmentStartUtc;



      /// Randevu tahsilatında randevunun hizmetleri
  @JsonKey(
    
    name: r'serviceNames',
    required: false,
    includeIfNull: false,
  )


  final List<String>? serviceNames;



      /// Randevuda önceden kapora alınmıştı; bu tahsilat kalan tutardır (\"kalan\")
  @JsonKey(
    
    name: r'isBalanceAfterDeposit',
    required: false,
    includeIfNull: false,
  )


  final bool? isBalanceAfterDeposit;



      /// Randevuda hizmeti veren (birden çok personelde ilk kalemin personeli), paket satışında satan personel
  @JsonKey(
    
    name: r'staffName',
    required: false,
    includeIfNull: false,
  )


  final String? staffName;



  @JsonKey(
    
    name: r'customerPackageId',
    required: false,
    includeIfNull: false,
  )


  final String? customerPackageId;



  @JsonKey(
    
    name: r'packageName',
    required: false,
    includeIfNull: false,
  )


  final String? packageName;



      /// Paketteki toplam seans
  @JsonKey(
    
    name: r'packageSessionCount',
    required: false,
    includeIfNull: false,
  )


  final int? packageSessionCount;



      /// Gün kapatıldıktan sonra yazıldı (\"kapanıştan sonra\")
  @JsonKey(
    
    name: r'recordedAfterDayClose',
    required: false,
    includeIfNull: false,
  )


  final bool? recordedAfterDayClose;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PaymentView &&
      other.id == id &&
      other.branchId == branchId &&
      other.method == method &&
      other.amount == amount &&
      other.isDeposit == isDeposit &&
      other.status == status &&
      other.refundedAmount == refundedAmount &&
      other.paidUtc == paidUtc &&
      other.collectedByName == collectedByName &&
      other.reference == reference &&
      other.purpose == purpose &&
      other.customer == customer &&
      other.appointmentId == appointmentId &&
      other.appointmentStartUtc == appointmentStartUtc &&
      other.serviceNames == serviceNames &&
      other.isBalanceAfterDeposit == isBalanceAfterDeposit &&
      other.staffName == staffName &&
      other.customerPackageId == customerPackageId &&
      other.packageName == packageName &&
      other.packageSessionCount == packageSessionCount &&
      other.recordedAfterDayClose == recordedAfterDayClose;

    @override
    int get hashCode =>
        id.hashCode +
        branchId.hashCode +
        method.hashCode +
        amount.hashCode +
        isDeposit.hashCode +
        status.hashCode +
        refundedAmount.hashCode +
        paidUtc.hashCode +
        (collectedByName == null ? 0 : collectedByName.hashCode) +
        (reference == null ? 0 : reference.hashCode) +
        purpose.hashCode +
        customer.hashCode +
        (appointmentId == null ? 0 : appointmentId.hashCode) +
        (appointmentStartUtc == null ? 0 : appointmentStartUtc.hashCode) +
        serviceNames.hashCode +
        isBalanceAfterDeposit.hashCode +
        (staffName == null ? 0 : staffName.hashCode) +
        (customerPackageId == null ? 0 : customerPackageId.hashCode) +
        (packageName == null ? 0 : packageName.hashCode) +
        (packageSessionCount == null ? 0 : packageSessionCount.hashCode) +
        recordedAfterDayClose.hashCode;

  factory PaymentView.fromJson(Map<String, dynamic> json) => _$PaymentViewFromJson(json);

  Map<String, dynamic> toJson() => _$PaymentViewToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

