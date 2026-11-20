// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'onboarding_checklist.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$OnboardingChecklistCWProxy {
  OnboardingChecklist completedCount(int completedCount);

  OnboardingChecklist totalCount(int totalCount);

  OnboardingChecklist steps(List<OnboardingChecklistStepsInner> steps);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `OnboardingChecklist(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// OnboardingChecklist(...).copyWith(id: 12, name: "My name")
  /// ````
  OnboardingChecklist call({
    int completedCount,
    int totalCount,
    List<OnboardingChecklistStepsInner> steps,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfOnboardingChecklist.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfOnboardingChecklist.copyWith.fieldName(...)`
class _$OnboardingChecklistCWProxyImpl implements _$OnboardingChecklistCWProxy {
  const _$OnboardingChecklistCWProxyImpl(this._value);

  final OnboardingChecklist _value;

  @override
  OnboardingChecklist completedCount(int completedCount) =>
      this(completedCount: completedCount);

  @override
  OnboardingChecklist totalCount(int totalCount) =>
      this(totalCount: totalCount);

  @override
  OnboardingChecklist steps(List<OnboardingChecklistStepsInner> steps) =>
      this(steps: steps);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `OnboardingChecklist(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// OnboardingChecklist(...).copyWith(id: 12, name: "My name")
  /// ````
  OnboardingChecklist call({
    Object? completedCount = const $CopyWithPlaceholder(),
    Object? totalCount = const $CopyWithPlaceholder(),
    Object? steps = const $CopyWithPlaceholder(),
  }) {
    return OnboardingChecklist(
      completedCount: completedCount == const $CopyWithPlaceholder()
          ? _value.completedCount
          // ignore: cast_nullable_to_non_nullable
          : completedCount as int,
      totalCount: totalCount == const $CopyWithPlaceholder()
          ? _value.totalCount
          // ignore: cast_nullable_to_non_nullable
          : totalCount as int,
      steps: steps == const $CopyWithPlaceholder()
          ? _value.steps
          // ignore: cast_nullable_to_non_nullable
          : steps as List<OnboardingChecklistStepsInner>,
    );
  }
}

extension $OnboardingChecklistCopyWith on OnboardingChecklist {
  /// Returns a callable class that can be used as follows: `instanceOfOnboardingChecklist.copyWith(...)` or like so:`instanceOfOnboardingChecklist.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$OnboardingChecklistCWProxy get copyWith =>
      _$OnboardingChecklistCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

OnboardingChecklist _$OnboardingChecklistFromJson(Map<String, dynamic> json) =>
    $checkedCreate('OnboardingChecklist', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const ['completedCount', 'totalCount', 'steps'],
      );
      final val = OnboardingChecklist(
        completedCount: $checkedConvert(
          'completedCount',
          (v) => (v as num).toInt(),
        ),
        totalCount: $checkedConvert('totalCount', (v) => (v as num).toInt()),
        steps: $checkedConvert(
          'steps',
          (v) => (v as List<dynamic>)
              .map(
                (e) => OnboardingChecklistStepsInner.fromJson(
                  e as Map<String, dynamic>,
                ),
              )
              .toList(),
        ),
      );
      return val;
    });

Map<String, dynamic> _$OnboardingChecklistToJson(
  OnboardingChecklist instance,
) => <String, dynamic>{
  'completedCount': instance.completedCount,
  'totalCount': instance.totalCount,
  'steps': instance.steps.map((e) => e.toJson()).toList(),
};
