// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_overview_walk_ins.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageOverviewWalkInsCWProxy {
  ManageOverviewWalkIns waitingCount(int waitingCount);

  ManageOverviewWalkIns estimatedWaitMinutesForNew(
    int? estimatedWaitMinutesForNew,
  );

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageOverviewWalkIns(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageOverviewWalkIns(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageOverviewWalkIns call({
    int waitingCount,
    int? estimatedWaitMinutesForNew,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageOverviewWalkIns.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageOverviewWalkIns.copyWith.fieldName(...)`
class _$ManageOverviewWalkInsCWProxyImpl
    implements _$ManageOverviewWalkInsCWProxy {
  const _$ManageOverviewWalkInsCWProxyImpl(this._value);

  final ManageOverviewWalkIns _value;

  @override
  ManageOverviewWalkIns waitingCount(int waitingCount) =>
      this(waitingCount: waitingCount);

  @override
  ManageOverviewWalkIns estimatedWaitMinutesForNew(
    int? estimatedWaitMinutesForNew,
  ) => this(estimatedWaitMinutesForNew: estimatedWaitMinutesForNew);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageOverviewWalkIns(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageOverviewWalkIns(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageOverviewWalkIns call({
    Object? waitingCount = const $CopyWithPlaceholder(),
    Object? estimatedWaitMinutesForNew = const $CopyWithPlaceholder(),
  }) {
    return ManageOverviewWalkIns(
      waitingCount: waitingCount == const $CopyWithPlaceholder()
          ? _value.waitingCount
          // ignore: cast_nullable_to_non_nullable
          : waitingCount as int,
      estimatedWaitMinutesForNew:
          estimatedWaitMinutesForNew == const $CopyWithPlaceholder()
          ? _value.estimatedWaitMinutesForNew
          // ignore: cast_nullable_to_non_nullable
          : estimatedWaitMinutesForNew as int?,
    );
  }
}

extension $ManageOverviewWalkInsCopyWith on ManageOverviewWalkIns {
  /// Returns a callable class that can be used as follows: `instanceOfManageOverviewWalkIns.copyWith(...)` or like so:`instanceOfManageOverviewWalkIns.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageOverviewWalkInsCWProxy get copyWith =>
      _$ManageOverviewWalkInsCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageOverviewWalkIns _$ManageOverviewWalkInsFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('ManageOverviewWalkIns', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const ['waitingCount', 'estimatedWaitMinutesForNew'],
  );
  final val = ManageOverviewWalkIns(
    waitingCount: $checkedConvert('waitingCount', (v) => (v as num).toInt()),
    estimatedWaitMinutesForNew: $checkedConvert(
      'estimatedWaitMinutesForNew',
      (v) => (v as num?)?.toInt(),
    ),
  );
  return val;
});

Map<String, dynamic> _$ManageOverviewWalkInsToJson(
  ManageOverviewWalkIns instance,
) => <String, dynamic>{
  'waitingCount': instance.waitingCount,
  'estimatedWaitMinutesForNew': instance.estimatedWaitMinutesForNew,
};
