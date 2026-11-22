//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_plan_catalog_rules.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminPlanCatalogRules {
  /// Returns a new [AdminPlanCatalogRules] instance.
  AdminPlanCatalogRules({

    required  this.trialDays,

    required  this.pastDueGraceDays,

    required  this.trialCredits,
  });

      /// \"Deneme süresi\"
          // minimum: 0
  @JsonKey(
    
    name: r'trialDays',
    required: true,
    includeIfNull: false,
  )


  final int trialDays;



      /// \"Ödeme gecikmesinde … gün hatırlatma, sonra salt okunur\"
          // minimum: 0
  @JsonKey(
    
    name: r'pastDueGraceDays',
    required: true,
    includeIfNull: false,
  )


  final int pastDueGraceDays;



      /// \"Deneme hediyesi\" SMS; tanımlı değilse null
          // minimum: 0
  @JsonKey(
    
    name: r'trialCredits',
    required: true,
    includeIfNull: true,
  )


  final int? trialCredits;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminPlanCatalogRules &&
      other.trialDays == trialDays &&
      other.pastDueGraceDays == pastDueGraceDays &&
      other.trialCredits == trialCredits;

    @override
    int get hashCode =>
        trialDays.hashCode +
        pastDueGraceDays.hashCode +
        (trialCredits == null ? 0 : trialCredits.hashCode);

  factory AdminPlanCatalogRules.fromJson(Map<String, dynamic> json) => _$AdminPlanCatalogRulesFromJson(json);

  Map<String, dynamic> toJson() => _$AdminPlanCatalogRulesToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

