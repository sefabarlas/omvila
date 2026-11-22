//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/billing_cycle.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_plan_change.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminPlanChange {
  /// Returns a new [AdminPlanChange] instance.
  AdminPlanChange({

    required  this.planCode,

     this.billingCycle,

    required  this.reason,
  });

      /// Yeni plan kodu
  @JsonKey(
    
    name: r'planCode',
    required: true,
    includeIfNull: false,
  )


  final String planCode;



  @JsonKey(
    
    name: r'billingCycle',
    required: false,
    includeIfNull: false,
  unknownEnumValue: BillingCycle.unknownDefaultOpenApi,
  )


  final BillingCycle? billingCycle;



      /// Neden; denetime yazılır
  @JsonKey(
    
    name: r'reason',
    required: true,
    includeIfNull: false,
  )


  final String reason;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminPlanChange &&
      other.planCode == planCode &&
      other.billingCycle == billingCycle &&
      other.reason == reason;

    @override
    int get hashCode =>
        planCode.hashCode +
        billingCycle.hashCode +
        reason.hashCode;

  factory AdminPlanChange.fromJson(Map<String, dynamic> json) => _$AdminPlanChangeFromJson(json);

  Map<String, dynamic> toJson() => _$AdminPlanChangeToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

