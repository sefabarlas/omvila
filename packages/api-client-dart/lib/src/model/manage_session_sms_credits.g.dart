// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_session_sms_credits.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageSessionSmsCreditsCWProxy {
  ManageSessionSmsCredits balance(int balance);

  ManageSessionSmsCredits isLow(bool isLow);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageSessionSmsCredits(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageSessionSmsCredits(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageSessionSmsCredits call({int balance, bool isLow});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageSessionSmsCredits.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageSessionSmsCredits.copyWith.fieldName(...)`
class _$ManageSessionSmsCreditsCWProxyImpl
    implements _$ManageSessionSmsCreditsCWProxy {
  const _$ManageSessionSmsCreditsCWProxyImpl(this._value);

  final ManageSessionSmsCredits _value;

  @override
  ManageSessionSmsCredits balance(int balance) => this(balance: balance);

  @override
  ManageSessionSmsCredits isLow(bool isLow) => this(isLow: isLow);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageSessionSmsCredits(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageSessionSmsCredits(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageSessionSmsCredits call({
    Object? balance = const $CopyWithPlaceholder(),
    Object? isLow = const $CopyWithPlaceholder(),
  }) {
    return ManageSessionSmsCredits(
      balance: balance == const $CopyWithPlaceholder()
          ? _value.balance
          // ignore: cast_nullable_to_non_nullable
          : balance as int,
      isLow: isLow == const $CopyWithPlaceholder()
          ? _value.isLow
          // ignore: cast_nullable_to_non_nullable
          : isLow as bool,
    );
  }
}

extension $ManageSessionSmsCreditsCopyWith on ManageSessionSmsCredits {
  /// Returns a callable class that can be used as follows: `instanceOfManageSessionSmsCredits.copyWith(...)` or like so:`instanceOfManageSessionSmsCredits.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageSessionSmsCreditsCWProxy get copyWith =>
      _$ManageSessionSmsCreditsCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageSessionSmsCredits _$ManageSessionSmsCreditsFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('ManageSessionSmsCredits', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['balance', 'isLow']);
  final val = ManageSessionSmsCredits(
    balance: $checkedConvert('balance', (v) => (v as num).toInt()),
    isLow: $checkedConvert('isLow', (v) => v as bool),
  );
  return val;
});

Map<String, dynamic> _$ManageSessionSmsCreditsToJson(
  ManageSessionSmsCredits instance,
) => <String, dynamic>{'balance': instance.balance, 'isLow': instance.isLow};
