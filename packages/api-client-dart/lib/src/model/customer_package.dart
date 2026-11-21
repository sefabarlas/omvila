//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/customer_package_status.dart';
import 'package:omvila_api/src/model/money.dart';
import 'package:omvila_api/src/model/customer_ref.dart';
import 'package:omvila_api/src/model/package_session_usage_view.dart';
import 'package:omvila_api/src/model/customer_package_item_view.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'customer_package.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class CustomerPackage {
  /// Returns a new [CustomerPackage] instance.
  CustomerPackage({

    required  this.id,

    required  this.customer,

    required  this.packageId,

    required  this.packageName,

    required  this.branchId,

    required  this.isAvailableAllBranches,

    required  this.price,

    required  this.purchasedUtc,

    required  this.expiresOn,

     this.soldByStaffId,

     this.soldByStaffName,

    required  this.status,

    required  this.items,

    required  this.totalSessions,

    required  this.remainingSessions,

     this.refundedAmount,

     this.refundedUtc,

     this.usages,

    required  this.version,
  });

  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



  @JsonKey(
    
    name: r'customer',
    required: true,
    includeIfNull: false,
  )


  final CustomerRef customer;



  @JsonKey(
    
    name: r'packageId',
    required: true,
    includeIfNull: false,
  )


  final String packageId;



  @JsonKey(
    
    name: r'packageName',
    required: true,
    includeIfNull: false,
  )


  final String packageName;



      /// Satıldığı şube
  @JsonKey(
    
    name: r'branchId',
    required: true,
    includeIfNull: false,
  )


  final String branchId;



  @JsonKey(
    
    name: r'isAvailableAllBranches',
    required: true,
    includeIfNull: false,
  )


  final bool isAvailableAllBranches;



      /// Ödenen tutar
  @JsonKey(
    
    name: r'price',
    required: true,
    includeIfNull: false,
  )


  final Money price;



  @JsonKey(
    
    name: r'purchasedUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime purchasedUtc;



      /// Son kullanım günü (şubenin yerel takvimi)
  @JsonKey(
    
    name: r'expiresOn',
    required: true,
    includeIfNull: false,
  )


  final DateTime expiresOn;



  @JsonKey(
    
    name: r'soldByStaffId',
    required: false,
    includeIfNull: false,
  )


  final String? soldByStaffId;



  @JsonKey(
    
    name: r'soldByStaffName',
    required: false,
    includeIfNull: false,
  )


  final String? soldByStaffName;



  @JsonKey(
    
    name: r'status',
    required: true,
    includeIfNull: false,
  unknownEnumValue: CustomerPackageStatus.unknownDefaultOpenApi,
  )


  final CustomerPackageStatus status;



  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<CustomerPackageItemView> items;



  @JsonKey(
    
    name: r'totalSessions',
    required: true,
    includeIfNull: false,
  )


  final int totalSessions;



  @JsonKey(
    
    name: r'remainingSessions',
    required: true,
    includeIfNull: false,
  )


  final int remainingSessions;



      /// Yalnız iade edildiyse
  @JsonKey(
    
    name: r'refundedAmount',
    required: false,
    includeIfNull: false,
  )


  final Money? refundedAmount;



  @JsonKey(
    
    name: r'refundedUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? refundedUtc;



      /// Yalnız müşteri kartı Paketler sekmesinde döner (yeniden eskiye)
  @JsonKey(
    
    name: r'usages',
    required: false,
    includeIfNull: false,
  )


  final List<PackageSessionUsageView>? usages;



      /// Kaynak sürümü (ETag ile aynı değer)
  @JsonKey(
    
    name: r'version',
    required: true,
    includeIfNull: false,
  )


  final String version;





    @override
    bool operator ==(Object other) => identical(this, other) || other is CustomerPackage &&
      other.id == id &&
      other.customer == customer &&
      other.packageId == packageId &&
      other.packageName == packageName &&
      other.branchId == branchId &&
      other.isAvailableAllBranches == isAvailableAllBranches &&
      other.price == price &&
      other.purchasedUtc == purchasedUtc &&
      other.expiresOn == expiresOn &&
      other.soldByStaffId == soldByStaffId &&
      other.soldByStaffName == soldByStaffName &&
      other.status == status &&
      other.items == items &&
      other.totalSessions == totalSessions &&
      other.remainingSessions == remainingSessions &&
      other.refundedAmount == refundedAmount &&
      other.refundedUtc == refundedUtc &&
      other.usages == usages &&
      other.version == version;

    @override
    int get hashCode =>
        id.hashCode +
        customer.hashCode +
        packageId.hashCode +
        packageName.hashCode +
        branchId.hashCode +
        isAvailableAllBranches.hashCode +
        price.hashCode +
        purchasedUtc.hashCode +
        expiresOn.hashCode +
        (soldByStaffId == null ? 0 : soldByStaffId.hashCode) +
        (soldByStaffName == null ? 0 : soldByStaffName.hashCode) +
        status.hashCode +
        items.hashCode +
        totalSessions.hashCode +
        remainingSessions.hashCode +
        refundedAmount.hashCode +
        (refundedUtc == null ? 0 : refundedUtc.hashCode) +
        usages.hashCode +
        version.hashCode;

  factory CustomerPackage.fromJson(Map<String, dynamic> json) => _$CustomerPackageFromJson(json);

  Map<String, dynamic> toJson() => _$CustomerPackageToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

