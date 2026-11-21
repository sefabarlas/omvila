// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'cash_day_transfer.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CashDayTransferCWProxy {
  CashDayTransfer amount(Money amount);

  CashDayTransfer count(int count);

  CashDayTransfer depositAmount(Money depositAmount);

  CashDayTransfer otherAmount(Money otherAmount);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CashDayTransfer(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CashDayTransfer(...).copyWith(id: 12, name: "My name")
  /// ````
  CashDayTransfer call({
    Money amount,
    int count,
    Money depositAmount,
    Money otherAmount,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCashDayTransfer.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCashDayTransfer.copyWith.fieldName(...)`
class _$CashDayTransferCWProxyImpl implements _$CashDayTransferCWProxy {
  const _$CashDayTransferCWProxyImpl(this._value);

  final CashDayTransfer _value;

  @override
  CashDayTransfer amount(Money amount) => this(amount: amount);

  @override
  CashDayTransfer count(int count) => this(count: count);

  @override
  CashDayTransfer depositAmount(Money depositAmount) =>
      this(depositAmount: depositAmount);

  @override
  CashDayTransfer otherAmount(Money otherAmount) =>
      this(otherAmount: otherAmount);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CashDayTransfer(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CashDayTransfer(...).copyWith(id: 12, name: "My name")
  /// ````
  CashDayTransfer call({
    Object? amount = const $CopyWithPlaceholder(),
    Object? count = const $CopyWithPlaceholder(),
    Object? depositAmount = const $CopyWithPlaceholder(),
    Object? otherAmount = const $CopyWithPlaceholder(),
  }) {
    return CashDayTransfer(
      amount: amount == const $CopyWithPlaceholder()
          ? _value.amount
          // ignore: cast_nullable_to_non_nullable
          : amount as Money,
      count: count == const $CopyWithPlaceholder()
          ? _value.count
          // ignore: cast_nullable_to_non_nullable
          : count as int,
      depositAmount: depositAmount == const $CopyWithPlaceholder()
          ? _value.depositAmount
          // ignore: cast_nullable_to_non_nullable
          : depositAmount as Money,
      otherAmount: otherAmount == const $CopyWithPlaceholder()
          ? _value.otherAmount
          // ignore: cast_nullable_to_non_nullable
          : otherAmount as Money,
    );
  }
}

extension $CashDayTransferCopyWith on CashDayTransfer {
  /// Returns a callable class that can be used as follows: `instanceOfCashDayTransfer.copyWith(...)` or like so:`instanceOfCashDayTransfer.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CashDayTransferCWProxy get copyWith => _$CashDayTransferCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CashDayTransfer _$CashDayTransferFromJson(Map<String, dynamic> json) =>
    $checkedCreate('CashDayTransfer', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const ['amount', 'count', 'depositAmount', 'otherAmount'],
      );
      final val = CashDayTransfer(
        amount: $checkedConvert(
          'amount',
          (v) => Money.fromJson(v as Map<String, dynamic>),
        ),
        count: $checkedConvert('count', (v) => (v as num).toInt()),
        depositAmount: $checkedConvert(
          'depositAmount',
          (v) => Money.fromJson(v as Map<String, dynamic>),
        ),
        otherAmount: $checkedConvert(
          'otherAmount',
          (v) => Money.fromJson(v as Map<String, dynamic>),
        ),
      );
      return val;
    });

Map<String, dynamic> _$CashDayTransferToJson(CashDayTransfer instance) =>
    <String, dynamic>{
      'amount': instance.amount.toJson(),
      'count': instance.count,
      'depositAmount': instance.depositAmount.toJson(),
      'otherAmount': instance.otherAmount.toJson(),
    };
