// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'bank_transaction_create.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$BankTransactionCreateCWProxy {
  BankTransactionCreate valueDate(DateTime valueDate);

  BankTransactionCreate amount(Money amount);

  BankTransactionCreate senderName(String senderName);

  BankTransactionCreate description(String? description);

  BankTransactionCreate bankReference(String bankReference);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `BankTransactionCreate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// BankTransactionCreate(...).copyWith(id: 12, name: "My name")
  /// ````
  BankTransactionCreate call({
    DateTime valueDate,
    Money amount,
    String senderName,
    String? description,
    String bankReference,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfBankTransactionCreate.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfBankTransactionCreate.copyWith.fieldName(...)`
class _$BankTransactionCreateCWProxyImpl
    implements _$BankTransactionCreateCWProxy {
  const _$BankTransactionCreateCWProxyImpl(this._value);

  final BankTransactionCreate _value;

  @override
  BankTransactionCreate valueDate(DateTime valueDate) =>
      this(valueDate: valueDate);

  @override
  BankTransactionCreate amount(Money amount) => this(amount: amount);

  @override
  BankTransactionCreate senderName(String senderName) =>
      this(senderName: senderName);

  @override
  BankTransactionCreate description(String? description) =>
      this(description: description);

  @override
  BankTransactionCreate bankReference(String bankReference) =>
      this(bankReference: bankReference);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `BankTransactionCreate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// BankTransactionCreate(...).copyWith(id: 12, name: "My name")
  /// ````
  BankTransactionCreate call({
    Object? valueDate = const $CopyWithPlaceholder(),
    Object? amount = const $CopyWithPlaceholder(),
    Object? senderName = const $CopyWithPlaceholder(),
    Object? description = const $CopyWithPlaceholder(),
    Object? bankReference = const $CopyWithPlaceholder(),
  }) {
    return BankTransactionCreate(
      valueDate: valueDate == const $CopyWithPlaceholder()
          ? _value.valueDate
          // ignore: cast_nullable_to_non_nullable
          : valueDate as DateTime,
      amount: amount == const $CopyWithPlaceholder()
          ? _value.amount
          // ignore: cast_nullable_to_non_nullable
          : amount as Money,
      senderName: senderName == const $CopyWithPlaceholder()
          ? _value.senderName
          // ignore: cast_nullable_to_non_nullable
          : senderName as String,
      description: description == const $CopyWithPlaceholder()
          ? _value.description
          // ignore: cast_nullable_to_non_nullable
          : description as String?,
      bankReference: bankReference == const $CopyWithPlaceholder()
          ? _value.bankReference
          // ignore: cast_nullable_to_non_nullable
          : bankReference as String,
    );
  }
}

extension $BankTransactionCreateCopyWith on BankTransactionCreate {
  /// Returns a callable class that can be used as follows: `instanceOfBankTransactionCreate.copyWith(...)` or like so:`instanceOfBankTransactionCreate.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$BankTransactionCreateCWProxy get copyWith =>
      _$BankTransactionCreateCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

BankTransactionCreate _$BankTransactionCreateFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('BankTransactionCreate', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const ['valueDate', 'amount', 'senderName', 'bankReference'],
  );
  final val = BankTransactionCreate(
    valueDate: $checkedConvert(
      'valueDate',
      (v) => const DateOnlyConverter().fromJson(v as String),
    ),
    amount: $checkedConvert(
      'amount',
      (v) => Money.fromJson(v as Map<String, dynamic>),
    ),
    senderName: $checkedConvert('senderName', (v) => v as String),
    description: $checkedConvert('description', (v) => v as String?),
    bankReference: $checkedConvert('bankReference', (v) => v as String),
  );
  return val;
});

Map<String, dynamic> _$BankTransactionCreateToJson(
  BankTransactionCreate instance,
) => <String, dynamic>{
  'valueDate': const DateOnlyConverter().toJson(instance.valueDate),
  'amount': instance.amount.toJson(),
  'senderName': instance.senderName,
  'description': instance.description,
  'bankReference': instance.bankReference,
};
