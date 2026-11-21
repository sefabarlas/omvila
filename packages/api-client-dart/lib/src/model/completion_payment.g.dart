// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'completion_payment.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CompletionPaymentCWProxy {
  CompletionPayment method(CollectedPaymentMethod method);

  CompletionPayment amount(Money amount);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CompletionPayment(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CompletionPayment(...).copyWith(id: 12, name: "My name")
  /// ````
  CompletionPayment call({CollectedPaymentMethod method, Money amount});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCompletionPayment.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCompletionPayment.copyWith.fieldName(...)`
class _$CompletionPaymentCWProxyImpl implements _$CompletionPaymentCWProxy {
  const _$CompletionPaymentCWProxyImpl(this._value);

  final CompletionPayment _value;

  @override
  CompletionPayment method(CollectedPaymentMethod method) =>
      this(method: method);

  @override
  CompletionPayment amount(Money amount) => this(amount: amount);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CompletionPayment(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CompletionPayment(...).copyWith(id: 12, name: "My name")
  /// ````
  CompletionPayment call({
    Object? method = const $CopyWithPlaceholder(),
    Object? amount = const $CopyWithPlaceholder(),
  }) {
    return CompletionPayment(
      method: method == const $CopyWithPlaceholder()
          ? _value.method
          // ignore: cast_nullable_to_non_nullable
          : method as CollectedPaymentMethod,
      amount: amount == const $CopyWithPlaceholder()
          ? _value.amount
          // ignore: cast_nullable_to_non_nullable
          : amount as Money,
    );
  }
}

extension $CompletionPaymentCopyWith on CompletionPayment {
  /// Returns a callable class that can be used as follows: `instanceOfCompletionPayment.copyWith(...)` or like so:`instanceOfCompletionPayment.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CompletionPaymentCWProxy get copyWith =>
      _$CompletionPaymentCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CompletionPayment _$CompletionPaymentFromJson(Map<String, dynamic> json) =>
    $checkedCreate('CompletionPayment', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['method', 'amount']);
      final val = CompletionPayment(
        method: $checkedConvert(
          'method',
          (v) => $enumDecode(
            _$CollectedPaymentMethodEnumMap,
            v,
            unknownValue: CollectedPaymentMethod.unknownDefaultOpenApi,
          ),
        ),
        amount: $checkedConvert(
          'amount',
          (v) => Money.fromJson(v as Map<String, dynamic>),
        ),
      );
      return val;
    });

Map<String, dynamic> _$CompletionPaymentToJson(CompletionPayment instance) =>
    <String, dynamic>{
      'method': _$CollectedPaymentMethodEnumMap[instance.method]!,
      'amount': instance.amount.toJson(),
    };

const _$CollectedPaymentMethodEnumMap = {
  CollectedPaymentMethod.cash: 'Cash',
  CollectedPaymentMethod.card: 'Card',
  CollectedPaymentMethod.transfer: 'Transfer',
  CollectedPaymentMethod.unknownDefaultOpenApi: 'unknown_default_open_api',
};
