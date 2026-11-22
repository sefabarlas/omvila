// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'me_reschedule_slot_search.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MeRescheduleSlotSearchCWProxy {
  MeRescheduleSlotSearch date(DateTime date);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeRescheduleSlotSearch(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeRescheduleSlotSearch(...).copyWith(id: 12, name: "My name")
  /// ````
  MeRescheduleSlotSearch call({DateTime date});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMeRescheduleSlotSearch.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMeRescheduleSlotSearch.copyWith.fieldName(...)`
class _$MeRescheduleSlotSearchCWProxyImpl
    implements _$MeRescheduleSlotSearchCWProxy {
  const _$MeRescheduleSlotSearchCWProxyImpl(this._value);

  final MeRescheduleSlotSearch _value;

  @override
  MeRescheduleSlotSearch date(DateTime date) => this(date: date);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeRescheduleSlotSearch(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeRescheduleSlotSearch(...).copyWith(id: 12, name: "My name")
  /// ````
  MeRescheduleSlotSearch call({Object? date = const $CopyWithPlaceholder()}) {
    return MeRescheduleSlotSearch(
      date: date == const $CopyWithPlaceholder()
          ? _value.date
          // ignore: cast_nullable_to_non_nullable
          : date as DateTime,
    );
  }
}

extension $MeRescheduleSlotSearchCopyWith on MeRescheduleSlotSearch {
  /// Returns a callable class that can be used as follows: `instanceOfMeRescheduleSlotSearch.copyWith(...)` or like so:`instanceOfMeRescheduleSlotSearch.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MeRescheduleSlotSearchCWProxy get copyWith =>
      _$MeRescheduleSlotSearchCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MeRescheduleSlotSearch _$MeRescheduleSlotSearchFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('MeRescheduleSlotSearch', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['date']);
  final val = MeRescheduleSlotSearch(
    date: $checkedConvert(
      'date',
      (v) => const DateOnlyConverter().fromJson(v as String),
    ),
  );
  return val;
});

Map<String, dynamic> _$MeRescheduleSlotSearchToJson(
  MeRescheduleSlotSearch instance,
) => <String, dynamic>{'date': const DateOnlyConverter().toJson(instance.date)};
