// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'plan_features.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PlanFeaturesCWProxy {
  PlanFeatures walkInQueue(bool walkInQueue);

  PlanFeatures waitlist(bool waitlist);

  PlanFeatures packages(bool packages);

  PlanFeatures crossBranchReports(bool crossBranchReports);

  PlanFeatures prioritySupport(bool prioritySupport);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PlanFeatures(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PlanFeatures(...).copyWith(id: 12, name: "My name")
  /// ````
  PlanFeatures call({
    bool walkInQueue,
    bool waitlist,
    bool packages,
    bool crossBranchReports,
    bool prioritySupport,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPlanFeatures.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPlanFeatures.copyWith.fieldName(...)`
class _$PlanFeaturesCWProxyImpl implements _$PlanFeaturesCWProxy {
  const _$PlanFeaturesCWProxyImpl(this._value);

  final PlanFeatures _value;

  @override
  PlanFeatures walkInQueue(bool walkInQueue) => this(walkInQueue: walkInQueue);

  @override
  PlanFeatures waitlist(bool waitlist) => this(waitlist: waitlist);

  @override
  PlanFeatures packages(bool packages) => this(packages: packages);

  @override
  PlanFeatures crossBranchReports(bool crossBranchReports) =>
      this(crossBranchReports: crossBranchReports);

  @override
  PlanFeatures prioritySupport(bool prioritySupport) =>
      this(prioritySupport: prioritySupport);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PlanFeatures(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PlanFeatures(...).copyWith(id: 12, name: "My name")
  /// ````
  PlanFeatures call({
    Object? walkInQueue = const $CopyWithPlaceholder(),
    Object? waitlist = const $CopyWithPlaceholder(),
    Object? packages = const $CopyWithPlaceholder(),
    Object? crossBranchReports = const $CopyWithPlaceholder(),
    Object? prioritySupport = const $CopyWithPlaceholder(),
  }) {
    return PlanFeatures(
      walkInQueue: walkInQueue == const $CopyWithPlaceholder()
          ? _value.walkInQueue
          // ignore: cast_nullable_to_non_nullable
          : walkInQueue as bool,
      waitlist: waitlist == const $CopyWithPlaceholder()
          ? _value.waitlist
          // ignore: cast_nullable_to_non_nullable
          : waitlist as bool,
      packages: packages == const $CopyWithPlaceholder()
          ? _value.packages
          // ignore: cast_nullable_to_non_nullable
          : packages as bool,
      crossBranchReports: crossBranchReports == const $CopyWithPlaceholder()
          ? _value.crossBranchReports
          // ignore: cast_nullable_to_non_nullable
          : crossBranchReports as bool,
      prioritySupport: prioritySupport == const $CopyWithPlaceholder()
          ? _value.prioritySupport
          // ignore: cast_nullable_to_non_nullable
          : prioritySupport as bool,
    );
  }
}

extension $PlanFeaturesCopyWith on PlanFeatures {
  /// Returns a callable class that can be used as follows: `instanceOfPlanFeatures.copyWith(...)` or like so:`instanceOfPlanFeatures.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PlanFeaturesCWProxy get copyWith => _$PlanFeaturesCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PlanFeatures _$PlanFeaturesFromJson(Map<String, dynamic> json) =>
    $checkedCreate('PlanFeatures', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'walkInQueue',
          'waitlist',
          'packages',
          'crossBranchReports',
          'prioritySupport',
        ],
      );
      final val = PlanFeatures(
        walkInQueue: $checkedConvert('walkInQueue', (v) => v as bool),
        waitlist: $checkedConvert('waitlist', (v) => v as bool),
        packages: $checkedConvert('packages', (v) => v as bool),
        crossBranchReports: $checkedConvert(
          'crossBranchReports',
          (v) => v as bool,
        ),
        prioritySupport: $checkedConvert('prioritySupport', (v) => v as bool),
      );
      return val;
    });

Map<String, dynamic> _$PlanFeaturesToJson(PlanFeatures instance) =>
    <String, dynamic>{
      'walkInQueue': instance.walkInQueue,
      'waitlist': instance.waitlist,
      'packages': instance.packages,
      'crossBranchReports': instance.crossBranchReports,
      'prioritySupport': instance.prioritySupport,
    };
