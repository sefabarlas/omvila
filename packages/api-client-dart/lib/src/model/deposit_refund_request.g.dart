// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'deposit_refund_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$DepositRefundRequestCWProxy {
  DepositRefundRequest amount(Money amount);

  DepositRefundRequest note(String? note);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `DepositRefundRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// DepositRefundRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  DepositRefundRequest call({Money amount, String? note});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfDepositRefundRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfDepositRefundRequest.copyWith.fieldName(...)`
class _$DepositRefundRequestCWProxyImpl
    implements _$DepositRefundRequestCWProxy {
  const _$DepositRefundRequestCWProxyImpl(this._value);

  final DepositRefundRequest _value;

  @override
  DepositRefundRequest amount(Money amount) => this(amount: amount);

  @override
  DepositRefundRequest note(String? note) => this(note: note);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `DepositRefundRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// DepositRefundRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  DepositRefundRequest call({
    Object? amount = const $CopyWithPlaceholder(),
    Object? note = const $CopyWithPlaceholder(),
  }) {
    return DepositRefundRequest(
      amount: amount == const $CopyWithPlaceholder()
          ? _value.amount
          // ignore: cast_nullable_to_non_nullable
          : amount as Money,
      note: note == const $CopyWithPlaceholder()
          ? _value.note
          // ignore: cast_nullable_to_non_nullable
          : note as String?,
    );
  }
}

extension $DepositRefundRequestCopyWith on DepositRefundRequest {
  /// Returns a callable class that can be used as follows: `instanceOfDepositRefundRequest.copyWith(...)` or like so:`instanceOfDepositRefundRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$DepositRefundRequestCWProxy get copyWith =>
      _$DepositRefundRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

DepositRefundRequest _$DepositRefundRequestFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('DepositRefundRequest', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['amount']);
  final val = DepositRefundRequest(
    amount: $checkedConvert(
      'amount',
      (v) => Money.fromJson(v as Map<String, dynamic>),
    ),
    note: $checkedConvert('note', (v) => v as String?),
  );
  return val;
});

Map<String, dynamic> _$DepositRefundRequestToJson(
  DepositRefundRequest instance,
) => <String, dynamic>{
  'amount': instance.amount.toJson(),
  'note': ?instance.note,
};
