//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/admin_plan_update.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_plan_catalog_update.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminPlanCatalogUpdate {
  /// Returns a new [AdminPlanCatalogUpdate] instance.
  AdminPlanCatalogUpdate({

    required  this.plans,
  });

      /// Düzenlenen planlar
  @JsonKey(
    
    name: r'plans',
    required: true,
    includeIfNull: false,
  )


  final List<AdminPlanUpdate> plans;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminPlanCatalogUpdate &&
      other.plans == plans;

    @override
    int get hashCode =>
        plans.hashCode;

  factory AdminPlanCatalogUpdate.fromJson(Map<String, dynamic> json) => _$AdminPlanCatalogUpdateFromJson(json);

  Map<String, dynamic> toJson() => _$AdminPlanCatalogUpdateToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

