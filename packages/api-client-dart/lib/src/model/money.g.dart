// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'money.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MoneyCWProxy {
  Money amountMinor(int amountMinor);

  Money currency(MoneyCurrencyEnum currency);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `Money(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// Money(...).copyWith(id: 12, name: "My name")
  /// ````
  Money call({int amountMinor, MoneyCurrencyEnum currency});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMoney.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMoney.copyWith.fieldName(...)`
class _$MoneyCWProxyImpl implements _$MoneyCWProxy {
  const _$MoneyCWProxyImpl(this._value);

  final Money _value;

  @override
  Money amountMinor(int amountMinor) => this(amountMinor: amountMinor);

  @override
  Money currency(MoneyCurrencyEnum currency) => this(currency: currency);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `Money(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// Money(...).copyWith(id: 12, name: "My name")
  /// ````
  Money call({
    Object? amountMinor = const $CopyWithPlaceholder(),
    Object? currency = const $CopyWithPlaceholder(),
  }) {
    return Money(
      amountMinor: amountMinor == const $CopyWithPlaceholder()
          ? _value.amountMinor
          // ignore: cast_nullable_to_non_nullable
          : amountMinor as int,
      currency: currency == const $CopyWithPlaceholder()
          ? _value.currency
          // ignore: cast_nullable_to_non_nullable
          : currency as MoneyCurrencyEnum,
    );
  }
}

extension $MoneyCopyWith on Money {
  /// Returns a callable class that can be used as follows: `instanceOfMoney.copyWith(...)` or like so:`instanceOfMoney.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MoneyCWProxy get copyWith => _$MoneyCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Money _$MoneyFromJson(Map<String, dynamic> json) =>
    $checkedCreate('Money', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['amountMinor', 'currency']);
      final val = Money(
        amountMinor: $checkedConvert('amountMinor', (v) => (v as num).toInt()),
        currency: $checkedConvert(
          'currency',
          (v) => $enumDecode(
            _$MoneyCurrencyEnumEnumMap,
            v,
            unknownValue: MoneyCurrencyEnum.unknownDefaultOpenApi,
          ),
        ),
      );
      return val;
    });

Map<String, dynamic> _$MoneyToJson(Money instance) => <String, dynamic>{
  'amountMinor': instance.amountMinor,
  'currency': _$MoneyCurrencyEnumEnumMap[instance.currency]!,
};

const _$MoneyCurrencyEnumEnumMap = {
  MoneyCurrencyEnum.TRY: 'TRY',
  MoneyCurrencyEnum.unknownDefaultOpenApi: 'unknown_default_open_api',
};
