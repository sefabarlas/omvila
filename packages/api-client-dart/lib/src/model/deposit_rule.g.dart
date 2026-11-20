// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'deposit_rule.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$DepositRuleCWProxy {
  DepositRule mode(DepositRuleModeEnum mode);

  DepositRule amount(Money? amount);

  DepositRule percent(num? percent);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `DepositRule(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// DepositRule(...).copyWith(id: 12, name: "My name")
  /// ````
  DepositRule call({DepositRuleModeEnum mode, Money? amount, num? percent});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfDepositRule.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfDepositRule.copyWith.fieldName(...)`
class _$DepositRuleCWProxyImpl implements _$DepositRuleCWProxy {
  const _$DepositRuleCWProxyImpl(this._value);

  final DepositRule _value;

  @override
  DepositRule mode(DepositRuleModeEnum mode) => this(mode: mode);

  @override
  DepositRule amount(Money? amount) => this(amount: amount);

  @override
  DepositRule percent(num? percent) => this(percent: percent);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `DepositRule(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// DepositRule(...).copyWith(id: 12, name: "My name")
  /// ````
  DepositRule call({
    Object? mode = const $CopyWithPlaceholder(),
    Object? amount = const $CopyWithPlaceholder(),
    Object? percent = const $CopyWithPlaceholder(),
  }) {
    return DepositRule(
      mode: mode == const $CopyWithPlaceholder()
          ? _value.mode
          // ignore: cast_nullable_to_non_nullable
          : mode as DepositRuleModeEnum,
      amount: amount == const $CopyWithPlaceholder()
          ? _value.amount
          // ignore: cast_nullable_to_non_nullable
          : amount as Money?,
      percent: percent == const $CopyWithPlaceholder()
          ? _value.percent
          // ignore: cast_nullable_to_non_nullable
          : percent as num?,
    );
  }
}

extension $DepositRuleCopyWith on DepositRule {
  /// Returns a callable class that can be used as follows: `instanceOfDepositRule.copyWith(...)` or like so:`instanceOfDepositRule.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$DepositRuleCWProxy get copyWith => _$DepositRuleCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

DepositRule _$DepositRuleFromJson(Map<String, dynamic> json) =>
    $checkedCreate('DepositRule', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['mode']);
      final val = DepositRule(
        mode: $checkedConvert(
          'mode',
          (v) => $enumDecode(
            _$DepositRuleModeEnumEnumMap,
            v,
            unknownValue: DepositRuleModeEnum.unknownDefaultOpenApi,
          ),
        ),
        amount: $checkedConvert(
          'amount',
          (v) => v == null ? null : Money.fromJson(v as Map<String, dynamic>),
        ),
        percent: $checkedConvert('percent', (v) => v as num?),
      );
      return val;
    });

Map<String, dynamic> _$DepositRuleToJson(DepositRule instance) =>
    <String, dynamic>{
      'mode': _$DepositRuleModeEnumEnumMap[instance.mode]!,
      'amount': ?instance.amount?.toJson(),
      'percent': ?instance.percent,
    };

const _$DepositRuleModeEnumEnumMap = {
  DepositRuleModeEnum.inherit: 'Inherit',
  DepositRuleModeEnum.always: 'Always',
  DepositRuleModeEnum.never: 'Never',
  DepositRuleModeEnum.unknownDefaultOpenApi: 'unknown_default_open_api',
};
