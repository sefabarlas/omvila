// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'cash_method_total.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CashMethodTotalCWProxy {
  CashMethodTotal amount(Money amount);

  CashMethodTotal count(int count);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CashMethodTotal(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CashMethodTotal(...).copyWith(id: 12, name: "My name")
  /// ````
  CashMethodTotal call({Money amount, int count});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCashMethodTotal.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCashMethodTotal.copyWith.fieldName(...)`
class _$CashMethodTotalCWProxyImpl implements _$CashMethodTotalCWProxy {
  const _$CashMethodTotalCWProxyImpl(this._value);

  final CashMethodTotal _value;

  @override
  CashMethodTotal amount(Money amount) => this(amount: amount);

  @override
  CashMethodTotal count(int count) => this(count: count);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CashMethodTotal(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CashMethodTotal(...).copyWith(id: 12, name: "My name")
  /// ````
  CashMethodTotal call({
    Object? amount = const $CopyWithPlaceholder(),
    Object? count = const $CopyWithPlaceholder(),
  }) {
    return CashMethodTotal(
      amount: amount == const $CopyWithPlaceholder()
          ? _value.amount
          // ignore: cast_nullable_to_non_nullable
          : amount as Money,
      count: count == const $CopyWithPlaceholder()
          ? _value.count
          // ignore: cast_nullable_to_non_nullable
          : count as int,
    );
  }
}

extension $CashMethodTotalCopyWith on CashMethodTotal {
  /// Returns a callable class that can be used as follows: `instanceOfCashMethodTotal.copyWith(...)` or like so:`instanceOfCashMethodTotal.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CashMethodTotalCWProxy get copyWith => _$CashMethodTotalCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CashMethodTotal _$CashMethodTotalFromJson(Map<String, dynamic> json) =>
    $checkedCreate('CashMethodTotal', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['amount', 'count']);
      final val = CashMethodTotal(
        amount: $checkedConvert(
          'amount',
          (v) => Money.fromJson(v as Map<String, dynamic>),
        ),
        count: $checkedConvert('count', (v) => (v as num).toInt()),
      );
      return val;
    });

Map<String, dynamic> _$CashMethodTotalToJson(CashMethodTotal instance) =>
    <String, dynamic>{
      'amount': instance.amount.toJson(),
      'count': instance.count,
    };
