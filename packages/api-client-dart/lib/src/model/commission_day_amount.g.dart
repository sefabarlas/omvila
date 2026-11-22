// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'commission_day_amount.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CommissionDayAmountCWProxy {
  CommissionDayAmount date(DateTime date);

  CommissionDayAmount amountMinor(int amountMinor);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CommissionDayAmount(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CommissionDayAmount(...).copyWith(id: 12, name: "My name")
  /// ````
  CommissionDayAmount call({DateTime date, int amountMinor});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCommissionDayAmount.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCommissionDayAmount.copyWith.fieldName(...)`
class _$CommissionDayAmountCWProxyImpl implements _$CommissionDayAmountCWProxy {
  const _$CommissionDayAmountCWProxyImpl(this._value);

  final CommissionDayAmount _value;

  @override
  CommissionDayAmount date(DateTime date) => this(date: date);

  @override
  CommissionDayAmount amountMinor(int amountMinor) =>
      this(amountMinor: amountMinor);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CommissionDayAmount(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CommissionDayAmount(...).copyWith(id: 12, name: "My name")
  /// ````
  CommissionDayAmount call({
    Object? date = const $CopyWithPlaceholder(),
    Object? amountMinor = const $CopyWithPlaceholder(),
  }) {
    return CommissionDayAmount(
      date: date == const $CopyWithPlaceholder()
          ? _value.date
          // ignore: cast_nullable_to_non_nullable
          : date as DateTime,
      amountMinor: amountMinor == const $CopyWithPlaceholder()
          ? _value.amountMinor
          // ignore: cast_nullable_to_non_nullable
          : amountMinor as int,
    );
  }
}

extension $CommissionDayAmountCopyWith on CommissionDayAmount {
  /// Returns a callable class that can be used as follows: `instanceOfCommissionDayAmount.copyWith(...)` or like so:`instanceOfCommissionDayAmount.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CommissionDayAmountCWProxy get copyWith =>
      _$CommissionDayAmountCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CommissionDayAmount _$CommissionDayAmountFromJson(Map<String, dynamic> json) =>
    $checkedCreate('CommissionDayAmount', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['date', 'amountMinor']);
      final val = CommissionDayAmount(
        date: $checkedConvert(
          'date',
          (v) => const DateOnlyConverter().fromJson(v as String),
        ),
        amountMinor: $checkedConvert('amountMinor', (v) => (v as num).toInt()),
      );
      return val;
    });

Map<String, dynamic> _$CommissionDayAmountToJson(
  CommissionDayAmount instance,
) => <String, dynamic>{
  'date': const DateOnlyConverter().toJson(instance.date),
  'amountMinor': instance.amountMinor,
};
