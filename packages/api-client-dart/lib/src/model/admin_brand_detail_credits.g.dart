// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_brand_detail_credits.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminBrandDetailCreditsCWProxy {
  AdminBrandDetailCredits balance(int balance);

  AdminBrandDetailCredits estimatedDaysLeft(int? estimatedDaysLeft);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminBrandDetailCredits(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminBrandDetailCredits(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminBrandDetailCredits call({int balance, int? estimatedDaysLeft});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminBrandDetailCredits.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminBrandDetailCredits.copyWith.fieldName(...)`
class _$AdminBrandDetailCreditsCWProxyImpl
    implements _$AdminBrandDetailCreditsCWProxy {
  const _$AdminBrandDetailCreditsCWProxyImpl(this._value);

  final AdminBrandDetailCredits _value;

  @override
  AdminBrandDetailCredits balance(int balance) => this(balance: balance);

  @override
  AdminBrandDetailCredits estimatedDaysLeft(int? estimatedDaysLeft) =>
      this(estimatedDaysLeft: estimatedDaysLeft);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminBrandDetailCredits(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminBrandDetailCredits(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminBrandDetailCredits call({
    Object? balance = const $CopyWithPlaceholder(),
    Object? estimatedDaysLeft = const $CopyWithPlaceholder(),
  }) {
    return AdminBrandDetailCredits(
      balance: balance == const $CopyWithPlaceholder()
          ? _value.balance
          // ignore: cast_nullable_to_non_nullable
          : balance as int,
      estimatedDaysLeft: estimatedDaysLeft == const $CopyWithPlaceholder()
          ? _value.estimatedDaysLeft
          // ignore: cast_nullable_to_non_nullable
          : estimatedDaysLeft as int?,
    );
  }
}

extension $AdminBrandDetailCreditsCopyWith on AdminBrandDetailCredits {
  /// Returns a callable class that can be used as follows: `instanceOfAdminBrandDetailCredits.copyWith(...)` or like so:`instanceOfAdminBrandDetailCredits.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminBrandDetailCreditsCWProxy get copyWith =>
      _$AdminBrandDetailCreditsCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminBrandDetailCredits _$AdminBrandDetailCreditsFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AdminBrandDetailCredits', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['balance', 'estimatedDaysLeft']);
  final val = AdminBrandDetailCredits(
    balance: $checkedConvert('balance', (v) => (v as num).toInt()),
    estimatedDaysLeft: $checkedConvert(
      'estimatedDaysLeft',
      (v) => (v as num?)?.toInt(),
    ),
  );
  return val;
});

Map<String, dynamic> _$AdminBrandDetailCreditsToJson(
  AdminBrandDetailCredits instance,
) => <String, dynamic>{
  'balance': instance.balance,
  'estimatedDaysLeft': instance.estimatedDaysLeft,
};
