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

part 'admin_plan_update.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminPlanUpdate {
  /// Returns a new [AdminPlanUpdate] instance.
  AdminPlanUpdate({

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
  });

      /// Düzenlenen plan
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



      /// Kısa açıklama
  @JsonKey(
    
    name: r'description',
    required: true,
    includeIfNull: true,
  )


  final String? description;



      /// Aylık fiyat; null = belirlenmedi
  @JsonKey(
    
    name: r'monthlyPrice',
    required: true,
    includeIfNull: true,
  )


  final Money? monthlyPrice;



      /// Yıllık fiyat; null = yok
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



      /// Ek şube ücreti; null = ek şube yok
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



      /// Dahil SMS
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



      /// Tanıtım sitesinde gösterilir
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





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminPlanUpdate &&
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
      other.isActive == isActive;

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
        isActive.hashCode;

  factory AdminPlanUpdate.fromJson(Map<String, dynamic> json) => _$AdminPlanUpdateFromJson(json);

  Map<String, dynamic> toJson() => _$AdminPlanUpdateToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

