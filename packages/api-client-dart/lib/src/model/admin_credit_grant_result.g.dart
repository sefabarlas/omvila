// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_credit_grant_result.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminCreditGrantResultCWProxy {
  AdminCreditGrantResult transaction(AdminCreditTransaction transaction);

  AdminCreditGrantResult balance(int balance);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminCreditGrantResult(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminCreditGrantResult(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminCreditGrantResult call({
    AdminCreditTransaction transaction,
    int balance,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminCreditGrantResult.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminCreditGrantResult.copyWith.fieldName(...)`
class _$AdminCreditGrantResultCWProxyImpl
    implements _$AdminCreditGrantResultCWProxy {
  const _$AdminCreditGrantResultCWProxyImpl(this._value);

  final AdminCreditGrantResult _value;

  @override
  AdminCreditGrantResult transaction(AdminCreditTransaction transaction) =>
      this(transaction: transaction);

  @override
  AdminCreditGrantResult balance(int balance) => this(balance: balance);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminCreditGrantResult(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminCreditGrantResult(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminCreditGrantResult call({
    Object? transaction = const $CopyWithPlaceholder(),
    Object? balance = const $CopyWithPlaceholder(),
  }) {
    return AdminCreditGrantResult(
      transaction: transaction == const $CopyWithPlaceholder()
          ? _value.transaction
          // ignore: cast_nullable_to_non_nullable
          : transaction as AdminCreditTransaction,
      balance: balance == const $CopyWithPlaceholder()
          ? _value.balance
          // ignore: cast_nullable_to_non_nullable
          : balance as int,
    );
  }
}

extension $AdminCreditGrantResultCopyWith on AdminCreditGrantResult {
  /// Returns a callable class that can be used as follows: `instanceOfAdminCreditGrantResult.copyWith(...)` or like so:`instanceOfAdminCreditGrantResult.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminCreditGrantResultCWProxy get copyWith =>
      _$AdminCreditGrantResultCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminCreditGrantResult _$AdminCreditGrantResultFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AdminCreditGrantResult', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['transaction', 'balance']);
  final val = AdminCreditGrantResult(
    transaction: $checkedConvert(
      'transaction',
      (v) => AdminCreditTransaction.fromJson(v as Map<String, dynamic>),
    ),
    balance: $checkedConvert('balance', (v) => (v as num).toInt()),
  );
  return val;
});

Map<String, dynamic> _$AdminCreditGrantResultToJson(
  AdminCreditGrantResult instance,
) => <String, dynamic>{
  'transaction': instance.transaction.toJson(),
  'balance': instance.balance,
};
