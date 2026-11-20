//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'onboarding_checklist_steps_inner.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class OnboardingChecklistStepsInner {
  /// Returns a new [OnboardingChecklistStepsInner] instance.
  OnboardingChecklistStepsInner({

    required  this.key,

    required  this.done,
  });

  @JsonKey(
    
    name: r'key',
    required: true,
    includeIfNull: false,
  unknownEnumValue: OnboardingChecklistStepsInnerKeyEnum.unknownDefaultOpenApi,
  )


  final OnboardingChecklistStepsInnerKeyEnum key;



  @JsonKey(
    
    name: r'done',
    required: true,
    includeIfNull: false,
  )


  final bool done;





    @override
    bool operator ==(Object other) => identical(this, other) || other is OnboardingChecklistStepsInner &&
      other.key == key &&
      other.done == done;

    @override
    int get hashCode =>
        key.hashCode +
        done.hashCode;

  factory OnboardingChecklistStepsInner.fromJson(Map<String, dynamic> json) => _$OnboardingChecklistStepsInnerFromJson(json);

  Map<String, dynamic> toJson() => _$OnboardingChecklistStepsInnerToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum OnboardingChecklistStepsInnerKeyEnum {
@JsonValue(r'BusinessCreated')
businessCreated(r'BusinessCreated'),
@JsonValue(r'WorkingHours')
workingHours(r'WorkingHours'),
@JsonValue(r'Services')
services(r'Services'),
@JsonValue(r'Staff')
staff(r'Staff'),
@JsonValue(r'ShareBookingLink')
shareBookingLink(r'ShareBookingLink'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const OnboardingChecklistStepsInnerKeyEnum(this.value);

final String value;

@override
String toString() => value;
}


