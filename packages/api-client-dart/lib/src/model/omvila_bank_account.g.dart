// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'omvila_bank_account.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$OmvilaBankAccountCWProxy {
  OmvilaBankAccount accountHolderName(String accountHolderName);

  OmvilaBankAccount iban(String iban);

  OmvilaBankAccount bankName(String? bankName);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `OmvilaBankAccount(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// OmvilaBankAccount(...).copyWith(id: 12, name: "My name")
  /// ````
  OmvilaBankAccount call({
    String accountHolderName,
    String iban,
    String? bankName,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfOmvilaBankAccount.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfOmvilaBankAccount.copyWith.fieldName(...)`
class _$OmvilaBankAccountCWProxyImpl implements _$OmvilaBankAccountCWProxy {
  const _$OmvilaBankAccountCWProxyImpl(this._value);

  final OmvilaBankAccount _value;

  @override
  OmvilaBankAccount accountHolderName(String accountHolderName) =>
      this(accountHolderName: accountHolderName);

  @override
  OmvilaBankAccount iban(String iban) => this(iban: iban);

  @override
  OmvilaBankAccount bankName(String? bankName) => this(bankName: bankName);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `OmvilaBankAccount(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// OmvilaBankAccount(...).copyWith(id: 12, name: "My name")
  /// ````
  OmvilaBankAccount call({
    Object? accountHolderName = const $CopyWithPlaceholder(),
    Object? iban = const $CopyWithPlaceholder(),
    Object? bankName = const $CopyWithPlaceholder(),
  }) {
    return OmvilaBankAccount(
      accountHolderName: accountHolderName == const $CopyWithPlaceholder()
          ? _value.accountHolderName
          // ignore: cast_nullable_to_non_nullable
          : accountHolderName as String,
      iban: iban == const $CopyWithPlaceholder()
          ? _value.iban
          // ignore: cast_nullable_to_non_nullable
          : iban as String,
      bankName: bankName == const $CopyWithPlaceholder()
          ? _value.bankName
          // ignore: cast_nullable_to_non_nullable
          : bankName as String?,
    );
  }
}

extension $OmvilaBankAccountCopyWith on OmvilaBankAccount {
  /// Returns a callable class that can be used as follows: `instanceOfOmvilaBankAccount.copyWith(...)` or like so:`instanceOfOmvilaBankAccount.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$OmvilaBankAccountCWProxy get copyWith =>
      _$OmvilaBankAccountCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

OmvilaBankAccount _$OmvilaBankAccountFromJson(Map<String, dynamic> json) =>
    $checkedCreate('OmvilaBankAccount', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['accountHolderName', 'iban']);
      final val = OmvilaBankAccount(
        accountHolderName: $checkedConvert(
          'accountHolderName',
          (v) => v as String,
        ),
        iban: $checkedConvert('iban', (v) => v as String),
        bankName: $checkedConvert('bankName', (v) => v as String?),
      );
      return val;
    });

Map<String, dynamic> _$OmvilaBankAccountToJson(OmvilaBankAccount instance) =>
    <String, dynamic>{
      'accountHolderName': instance.accountHolderName,
      'iban': instance.iban,
      'bankName': ?instance.bankName,
    };
