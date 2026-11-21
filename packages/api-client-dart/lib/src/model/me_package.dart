//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/me_brand_ref.dart';
import 'package:omvila_api/src/model/customer_package_status.dart';
import 'package:omvila_api/src/model/customer_package_item_view.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'me_package.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MePackage {
  /// Returns a new [MePackage] instance.
  MePackage({

    required  this.id,

    required  this.brand,

    required  this.branchName,

    required  this.isAvailableAllBranches,

    required  this.packageName,

    required  this.status,

    required  this.items,

    required  this.totalSessions,

    required  this.remainingSessions,

    required  this.expiresOn,

    required  this.isUsable,

    required  this.isRunningLow,

    required  this.finishedOn,
  });

      /// CustomerPackage
  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



  @JsonKey(
    
    name: r'brand',
    required: true,
    includeIfNull: false,
  )


  final MeBrandRef brand;



      /// Yalnız satıldığı şubede geçerliyse şube adı; bütün şubelerde geçerliyse null
  @JsonKey(
    
    name: r'branchName',
    required: true,
    includeIfNull: true,
  )


  final String? branchName;



  @JsonKey(
    
    name: r'isAvailableAllBranches',
    required: true,
    includeIfNull: false,
  )


  final bool isAvailableAllBranches;



  @JsonKey(
    
    name: r'packageName',
    required: true,
    includeIfNull: false,
  )


  final String packageName;



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



      /// Son kullanım günü
  @JsonKey(
    
    name: r'expiresOn',
    required: true,
    includeIfNull: false,
  )


  final DateTime expiresOn;



      /// Active ve kalan seansı var (üstteki kartlar); false ise \"Biten paketler\"
  @JsonKey(
    
    name: r'isUsable',
    required: true,
    includeIfNull: false,
  )


  final bool isUsable;



      /// \"Az kaldı\": kullanılabilir ve kalan seans PackageLowSessionThreshold'u geçmiyor veya son kullanım PackageExpiringSoonDays içinde
  @JsonKey(
    
    name: r'isRunningLow',
    required: true,
    includeIfNull: false,
  )


  final bool isRunningLow;



      /// \"Eylül'de bitti\": son seansın, son kullanım gününün veya iadenin yerel günü; kullanılabilirse null
  @JsonKey(
    
    name: r'finishedOn',
    required: true,
    includeIfNull: true,
  )


  final DateTime? finishedOn;





    @override
    bool operator ==(Object other) => identical(this, other) || other is MePackage &&
      other.id == id &&
      other.brand == brand &&
      other.branchName == branchName &&
      other.isAvailableAllBranches == isAvailableAllBranches &&
      other.packageName == packageName &&
      other.status == status &&
      other.items == items &&
      other.totalSessions == totalSessions &&
      other.remainingSessions == remainingSessions &&
      other.expiresOn == expiresOn &&
      other.isUsable == isUsable &&
      other.isRunningLow == isRunningLow &&
      other.finishedOn == finishedOn;

    @override
    int get hashCode =>
        id.hashCode +
        brand.hashCode +
        (branchName == null ? 0 : branchName.hashCode) +
        isAvailableAllBranches.hashCode +
        packageName.hashCode +
        status.hashCode +
        items.hashCode +
        totalSessions.hashCode +
        remainingSessions.hashCode +
        expiresOn.hashCode +
        isUsable.hashCode +
        isRunningLow.hashCode +
        (finishedOn == null ? 0 : finishedOn.hashCode);

  factory MePackage.fromJson(Map<String, dynamic> json) => _$MePackageFromJson(json);

  Map<String, dynamic> toJson() => _$MePackageToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

