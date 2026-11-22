//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/money.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'staff_service_assignment.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class StaffServiceAssignment {
  /// Returns a new [StaffServiceAssignment] instance.
  StaffServiceAssignment({

    required  this.serviceId,

     this.durationOverrideMinutes,

     this.surcharge,

     this.commissionPercentOverride,
  });

  @JsonKey(
    
    name: r'serviceId',
    required: true,
    includeIfNull: false,
  )


  final String serviceId;



          // minimum: 5
  @JsonKey(
    
    name: r'durationOverrideMinutes',
    required: false,
    includeIfNull: true,
  )


  final int? durationOverrideMinutes;



  @JsonKey(
    
    name: r'surcharge',
    required: false,
    includeIfNull: false,
  )


  final Money? surcharge;



  @JsonKey(
    
    name: r'commissionPercentOverride',
    required: false,
    includeIfNull: true,
  )


  final num? commissionPercentOverride;





    @override
    bool operator ==(Object other) => identical(this, other) || other is StaffServiceAssignment &&
      other.serviceId == serviceId &&
      other.durationOverrideMinutes == durationOverrideMinutes &&
      other.surcharge == surcharge &&
      other.commissionPercentOverride == commissionPercentOverride;

    @override
    int get hashCode =>
        serviceId.hashCode +
        (durationOverrideMinutes == null ? 0 : durationOverrideMinutes.hashCode) +
        surcharge.hashCode +
        (commissionPercentOverride == null ? 0 : commissionPercentOverride.hashCode);

  factory StaffServiceAssignment.fromJson(Map<String, dynamic> json) => _$StaffServiceAssignmentFromJson(json);

  Map<String, dynamic> toJson() => _$StaffServiceAssignmentToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

