//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/onboarding_checklist_steps_inner.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'onboarding_checklist.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class OnboardingChecklist {
  /// Returns a new [OnboardingChecklist] instance.
  OnboardingChecklist({

    required  this.completedCount,

    required  this.totalCount,

    required  this.steps,
  });

  @JsonKey(
    
    name: r'completedCount',
    required: true,
    includeIfNull: false,
  )


  final int completedCount;



  @JsonKey(
    
    name: r'totalCount',
    required: true,
    includeIfNull: false,
  )


  final int totalCount;



  @JsonKey(
    
    name: r'steps',
    required: true,
    includeIfNull: false,
  )


  final List<OnboardingChecklistStepsInner> steps;





    @override
    bool operator ==(Object other) => identical(this, other) || other is OnboardingChecklist &&
      other.completedCount == completedCount &&
      other.totalCount == totalCount &&
      other.steps == steps;

    @override
    int get hashCode =>
        completedCount.hashCode +
        totalCount.hashCode +
        steps.hashCode;

  factory OnboardingChecklist.fromJson(Map<String, dynamic> json) => _$OnboardingChecklistFromJson(json);

  Map<String, dynamic> toJson() => _$OnboardingChecklistToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

