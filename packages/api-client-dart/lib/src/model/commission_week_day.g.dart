// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'commission_week_day.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CommissionWeekDayCWProxy {
  CommissionWeekDay date(DateTime date);

  CommissionWeekDay amountMinor(int amountMinor);

  CommissionWeekDay appointmentCount(int appointmentCount);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CommissionWeekDay(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CommissionWeekDay(...).copyWith(id: 12, name: "My name")
  /// ````
  CommissionWeekDay call({
    DateTime date,
    int amountMinor,
    int appointmentCount,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCommissionWeekDay.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCommissionWeekDay.copyWith.fieldName(...)`
class _$CommissionWeekDayCWProxyImpl implements _$CommissionWeekDayCWProxy {
  const _$CommissionWeekDayCWProxyImpl(this._value);

  final CommissionWeekDay _value;

  @override
  CommissionWeekDay date(DateTime date) => this(date: date);

  @override
  CommissionWeekDay amountMinor(int amountMinor) =>
      this(amountMinor: amountMinor);

  @override
  CommissionWeekDay appointmentCount(int appointmentCount) =>
      this(appointmentCount: appointmentCount);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CommissionWeekDay(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CommissionWeekDay(...).copyWith(id: 12, name: "My name")
  /// ````
  CommissionWeekDay call({
    Object? date = const $CopyWithPlaceholder(),
    Object? amountMinor = const $CopyWithPlaceholder(),
    Object? appointmentCount = const $CopyWithPlaceholder(),
  }) {
    return CommissionWeekDay(
      date: date == const $CopyWithPlaceholder()
          ? _value.date
          // ignore: cast_nullable_to_non_nullable
          : date as DateTime,
      amountMinor: amountMinor == const $CopyWithPlaceholder()
          ? _value.amountMinor
          // ignore: cast_nullable_to_non_nullable
          : amountMinor as int,
      appointmentCount: appointmentCount == const $CopyWithPlaceholder()
          ? _value.appointmentCount
          // ignore: cast_nullable_to_non_nullable
          : appointmentCount as int,
    );
  }
}

extension $CommissionWeekDayCopyWith on CommissionWeekDay {
  /// Returns a callable class that can be used as follows: `instanceOfCommissionWeekDay.copyWith(...)` or like so:`instanceOfCommissionWeekDay.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CommissionWeekDayCWProxy get copyWith =>
      _$CommissionWeekDayCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CommissionWeekDay _$CommissionWeekDayFromJson(Map<String, dynamic> json) =>
    $checkedCreate('CommissionWeekDay', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const ['date', 'amountMinor', 'appointmentCount'],
      );
      final val = CommissionWeekDay(
        date: $checkedConvert(
          'date',
          (v) => const DateOnlyConverter().fromJson(v as String),
        ),
        amountMinor: $checkedConvert('amountMinor', (v) => (v as num).toInt()),
        appointmentCount: $checkedConvert(
          'appointmentCount',
          (v) => (v as num).toInt(),
        ),
      );
      return val;
    });

Map<String, dynamic> _$CommissionWeekDayToJson(CommissionWeekDay instance) =>
    <String, dynamic>{
      'date': const DateOnlyConverter().toJson(instance.date),
      'amountMinor': instance.amountMinor,
      'appointmentCount': instance.appointmentCount,
    };
