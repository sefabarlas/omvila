//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/admin_plan_catalog_rules.dart';
import 'package:omvila_api/src/model/admin_plan.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_plan_catalog.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminPlanCatalog {
  /// Returns a new [AdminPlanCatalog] instance.
  AdminPlanCatalog({

    required  this.plans,

    required  this.rules,

    required  this.version,
  });

      /// Plan kartları (Tekil, Salon, Salon Plus)
  @JsonKey(
    
    name: r'plans',
    required: true,
    includeIfNull: false,
  )


  final List<AdminPlan> plans;



  @JsonKey(
    
    name: r'rules',
    required: true,
    includeIfNull: false,
  )


  final AdminPlanCatalogRules rules;



      /// Kaynak sürümü (ETag ile aynı değer)
  @JsonKey(
    
    name: r'version',
    required: true,
    includeIfNull: false,
  )


  final String version;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminPlanCatalog &&
      other.plans == plans &&
      other.rules == rules &&
      other.version == version;

    @override
    int get hashCode =>
        plans.hashCode +
        rules.hashCode +
        version.hashCode;

  factory AdminPlanCatalog.fromJson(Map<String, dynamic> json) => _$AdminPlanCatalogFromJson(json);

  Map<String, dynamic> toJson() => _$AdminPlanCatalogToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

