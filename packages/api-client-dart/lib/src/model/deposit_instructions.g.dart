// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'deposit_instructions.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$DepositInstructionsCWProxy {
  DepositInstructions accountHolderName(String accountHolderName);

  DepositInstructions bankName(String? bankName);

  DepositInstructions iban(String iban);

  DepositInstructions amount(Money amount);

  DepositInstructions transferCode(String transferCode);

  DepositInstructions dueUtc(DateTime dueUtc);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `DepositInstructions(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// DepositInstructions(...).copyWith(id: 12, name: "My name")
  /// ````
  DepositInstructions call({
    String accountHolderName,
    String? bankName,
    String iban,
    Money amount,
    String transferCode,
    DateTime dueUtc,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfDepositInstructions.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfDepositInstructions.copyWith.fieldName(...)`
class _$DepositInstructionsCWProxyImpl implements _$DepositInstructionsCWProxy {
  const _$DepositInstructionsCWProxyImpl(this._value);

  final DepositInstructions _value;

  @override
  DepositInstructions accountHolderName(String accountHolderName) =>
      this(accountHolderName: accountHolderName);

  @override
  DepositInstructions bankName(String? bankName) => this(bankName: bankName);

  @override
  DepositInstructions iban(String iban) => this(iban: iban);

  @override
  DepositInstructions amount(Money amount) => this(amount: amount);

  @override
  DepositInstructions transferCode(String transferCode) =>
      this(transferCode: transferCode);

  @override
  DepositInstructions dueUtc(DateTime dueUtc) => this(dueUtc: dueUtc);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `DepositInstructions(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// DepositInstructions(...).copyWith(id: 12, name: "My name")
  /// ````
  DepositInstructions call({
    Object? accountHolderName = const $CopyWithPlaceholder(),
    Object? bankName = const $CopyWithPlaceholder(),
    Object? iban = const $CopyWithPlaceholder(),
    Object? amount = const $CopyWithPlaceholder(),
    Object? transferCode = const $CopyWithPlaceholder(),
    Object? dueUtc = const $CopyWithPlaceholder(),
  }) {
    return DepositInstructions(
      accountHolderName: accountHolderName == const $CopyWithPlaceholder()
          ? _value.accountHolderName
          // ignore: cast_nullable_to_non_nullable
          : accountHolderName as String,
      bankName: bankName == const $CopyWithPlaceholder()
          ? _value.bankName
          // ignore: cast_nullable_to_non_nullable
          : bankName as String?,
      iban: iban == const $CopyWithPlaceholder()
          ? _value.iban
          // ignore: cast_nullable_to_non_nullable
          : iban as String,
      amount: amount == const $CopyWithPlaceholder()
          ? _value.amount
          // ignore: cast_nullable_to_non_nullable
          : amount as Money,
      transferCode: transferCode == const $CopyWithPlaceholder()
          ? _value.transferCode
          // ignore: cast_nullable_to_non_nullable
          : transferCode as String,
      dueUtc: dueUtc == const $CopyWithPlaceholder()
          ? _value.dueUtc
          // ignore: cast_nullable_to_non_nullable
          : dueUtc as DateTime,
    );
  }
}

extension $DepositInstructionsCopyWith on DepositInstructions {
  /// Returns a callable class that can be used as follows: `instanceOfDepositInstructions.copyWith(...)` or like so:`instanceOfDepositInstructions.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$DepositInstructionsCWProxy get copyWith =>
      _$DepositInstructionsCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

DepositInstructions _$DepositInstructionsFromJson(Map<String, dynamic> json) =>
    $checkedCreate('DepositInstructions', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'accountHolderName',
          'iban',
          'amount',
          'transferCode',
          'dueUtc',
        ],
      );
      final val = DepositInstructions(
        accountHolderName: $checkedConvert(
          'accountHolderName',
          (v) => v as String,
        ),
        bankName: $checkedConvert('bankName', (v) => v as String?),
        iban: $checkedConvert('iban', (v) => v as String),
        amount: $checkedConvert(
          'amount',
          (v) => Money.fromJson(v as Map<String, dynamic>),
        ),
        transferCode: $checkedConvert('transferCode', (v) => v as String),
        dueUtc: $checkedConvert('dueUtc', (v) => DateTime.parse(v as String)),
      );
      return val;
    });

Map<String, dynamic> _$DepositInstructionsToJson(
  DepositInstructions instance,
) => <String, dynamic>{
  'accountHolderName': instance.accountHolderName,
  'bankName': ?instance.bankName,
  'iban': instance.iban,
  'amount': instance.amount.toJson(),
  'transferCode': instance.transferCode,
  'dueUtc': instance.dueUtc.toIso8601String(),
};
