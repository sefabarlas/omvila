//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/money.dart';
import 'package:omvila_api/src/model/plan_features.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_plan.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminPlan {
  /// Returns a new [AdminPlan] instance.
  AdminPlan({

    required  this.code,

    required  this.name,

    required  this.description,

    required  this.monthlyPrice,

    required  this.yearlyPrice,

    required  this.includedBranchCount,

    required  this.extraBranchPrice,

    required  this.staffLimit,

    required  this.includedSmsCredits,

    required  this.features,

    required  this.isPublic,

    required  this.isActive,

    required  this.brandCount,
  });

      /// Plan kodu
  @JsonKey(
    
    name: r'code',
    required: true,
    includeIfNull: false,
  )


  final String code;



      /// Plan adı
  @JsonKey(
    
    name: r'name',
    required: true,
    includeIfNull: false,
  )


  final String name;



      /// \"Küçük ve orta salonlar\"
  @JsonKey(
    
    name: r'description',
    required: true,
    includeIfNull: true,
  )


  final String? description;



      /// Aylık fiyat; tanımlı değilse null
  @JsonKey(
    
    name: r'monthlyPrice',
    required: true,
    includeIfNull: true,
  )


  final Money? monthlyPrice;



      /// Yıllık fiyat; tanımlı değilse null
  @JsonKey(
    
    name: r'yearlyPrice',
    required: true,
    includeIfNull: true,
  )


  final Money? yearlyPrice;



      /// Dahil şube; null = sınırsız
          // minimum: 1
  @JsonKey(
    
    name: r'includedBranchCount',
    required: true,
    includeIfNull: true,
  )


  final int? includedBranchCount;



      /// Ek şube ücreti (aylık); null = ek şube yok veya tanımlı değil
  @JsonKey(
    
    name: r'extraBranchPrice',
    required: true,
    includeIfNull: true,
  )


  final Money? extraBranchPrice;



      /// Personel sınırı; null = sınırsız
          // minimum: 1
  @JsonKey(
    
    name: r'staffLimit',
    required: true,
    includeIfNull: true,
  )


  final int? staffLimit;



      /// Dönem başına dahil SMS; tanımlı değilse null
          // minimum: 0
  @JsonKey(
    
    name: r'includedSmsCredits',
    required: true,
    includeIfNull: true,
  )


  final int? includedSmsCredits;



  @JsonKey(
    
    name: r'features',
    required: true,
    includeIfNull: false,
  )


  final PlanFeatures features;



      /// Tanıtım sitesinde ve kayıtta gösterilir
  @JsonKey(
    
    name: r'isPublic',
    required: true,
    includeIfNull: false,
  )


  final bool isPublic;



      /// Yeni kayıtlarda seçilebilir
  @JsonKey(
    
    name: r'isActive',
    required: true,
    includeIfNull: false,
  )


  final bool isActive;



      /// \"241 marka\": bu plandaki iptal edilmemiş abonelik
          // minimum: 0
  @JsonKey(
    
    name: r'brandCount',
    required: true,
    includeIfNull: false,
  )


  final int brandCount;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminPlan &&
      other.code == code &&
      other.name == name &&
      other.description == description &&
      other.monthlyPrice == monthlyPrice &&
      other.yearlyPrice == yearlyPrice &&
      other.includedBranchCount == includedBranchCount &&
      other.extraBranchPrice == extraBranchPrice &&
      other.staffLimit == staffLimit &&
      other.includedSmsCredits == includedSmsCredits &&
      other.features == features &&
      other.isPublic == isPublic &&
      other.isActive == isActive &&
      other.brandCount == brandCount;

    @override
    int get hashCode =>
        code.hashCode +
        name.hashCode +
        (description == null ? 0 : description.hashCode) +
        (monthlyPrice == null ? 0 : monthlyPrice.hashCode) +
        (yearlyPrice == null ? 0 : yearlyPrice.hashCode) +
        (includedBranchCount == null ? 0 : includedBranchCount.hashCode) +
        (extraBranchPrice == null ? 0 : extraBranchPrice.hashCode) +
        (staffLimit == null ? 0 : staffLimit.hashCode) +
        (includedSmsCredits == null ? 0 : includedSmsCredits.hashCode) +
        features.hashCode +
        isPublic.hashCode +
        isActive.hashCode +
        brandCount.hashCode;

  factory AdminPlan.fromJson(Map<String, dynamic> json) => _$AdminPlanFromJson(json);

  Map<String, dynamic> toJson() => _$AdminPlanToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

