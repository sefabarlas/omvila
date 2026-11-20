// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'onboarding_checklist_steps_inner.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$OnboardingChecklistStepsInnerCWProxy {
  OnboardingChecklistStepsInner key(OnboardingChecklistStepsInnerKeyEnum key);

  OnboardingChecklistStepsInner done(bool done);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `OnboardingChecklistStepsInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// OnboardingChecklistStepsInner(...).copyWith(id: 12, name: "My name")
  /// ````
  OnboardingChecklistStepsInner call({
    OnboardingChecklistStepsInnerKeyEnum key,
    bool done,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfOnboardingChecklistStepsInner.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfOnboardingChecklistStepsInner.copyWith.fieldName(...)`
class _$OnboardingChecklistStepsInnerCWProxyImpl
    implements _$OnboardingChecklistStepsInnerCWProxy {
  const _$OnboardingChecklistStepsInnerCWProxyImpl(this._value);

  final OnboardingChecklistStepsInner _value;

  @override
  OnboardingChecklistStepsInner key(OnboardingChecklistStepsInnerKeyEnum key) =>
      this(key: key);

  @override
  OnboardingChecklistStepsInner done(bool done) => this(done: done);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `OnboardingChecklistStepsInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// OnboardingChecklistStepsInner(...).copyWith(id: 12, name: "My name")
  /// ````
  OnboardingChecklistStepsInner call({
    Object? key = const $CopyWithPlaceholder(),
    Object? done = const $CopyWithPlaceholder(),
  }) {
    return OnboardingChecklistStepsInner(
      key: key == const $CopyWithPlaceholder()
          ? _value.key
          // ignore: cast_nullable_to_non_nullable
          : key as OnboardingChecklistStepsInnerKeyEnum,
      done: done == const $CopyWithPlaceholder()
          ? _value.done
          // ignore: cast_nullable_to_non_nullable
          : done as bool,
    );
  }
}

extension $OnboardingChecklistStepsInnerCopyWith
    on OnboardingChecklistStepsInner {
  /// Returns a callable class that can be used as follows: `instanceOfOnboardingChecklistStepsInner.copyWith(...)` or like so:`instanceOfOnboardingChecklistStepsInner.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$OnboardingChecklistStepsInnerCWProxy get copyWith =>
      _$OnboardingChecklistStepsInnerCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

OnboardingChecklistStepsInner _$OnboardingChecklistStepsInnerFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('OnboardingChecklistStepsInner', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['key', 'done']);
  final val = OnboardingChecklistStepsInner(
    key: $checkedConvert(
      'key',
      (v) => $enumDecode(
        _$OnboardingChecklistStepsInnerKeyEnumEnumMap,
        v,
        unknownValue:
            OnboardingChecklistStepsInnerKeyEnum.unknownDefaultOpenApi,
      ),
    ),
    done: $checkedConvert('done', (v) => v as bool),
  );
  return val;
});

Map<String, dynamic> _$OnboardingChecklistStepsInnerToJson(
  OnboardingChecklistStepsInner instance,
) => <String, dynamic>{
  'key': _$OnboardingChecklistStepsInnerKeyEnumEnumMap[instance.key]!,
  'done': instance.done,
};

const _$OnboardingChecklistStepsInnerKeyEnumEnumMap = {
  OnboardingChecklistStepsInnerKeyEnum.businessCreated: 'BusinessCreated',
  OnboardingChecklistStepsInnerKeyEnum.workingHours: 'WorkingHours',
  OnboardingChecklistStepsInnerKeyEnum.services: 'Services',
  OnboardingChecklistStepsInnerKeyEnum.staff: 'Staff',
  OnboardingChecklistStepsInnerKeyEnum.shareBookingLink: 'ShareBookingLink',
  OnboardingChecklistStepsInnerKeyEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
