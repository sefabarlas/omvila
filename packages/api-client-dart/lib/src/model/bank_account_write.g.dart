// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'bank_account_write.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$BankAccountWriteCWProxy {
  BankAccountWrite accountHolderName(String accountHolderName);

  BankAccountWrite bankName(String bankName);

  BankAccountWrite iban(String? iban);

  BankAccountWrite transferCodePrefix(String transferCodePrefix);

  BankAccountWrite transferWindowMinutes(
    BankAccountWriteTransferWindowMinutesEnum transferWindowMinutes,
  );

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `BankAccountWrite(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// BankAccountWrite(...).copyWith(id: 12, name: "My name")
  /// ````
  BankAccountWrite call({
    String accountHolderName,
    String bankName,
    String? iban,
    String transferCodePrefix,
    BankAccountWriteTransferWindowMinutesEnum transferWindowMinutes,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfBankAccountWrite.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfBankAccountWrite.copyWith.fieldName(...)`
class _$BankAccountWriteCWProxyImpl implements _$BankAccountWriteCWProxy {
  const _$BankAccountWriteCWProxyImpl(this._value);

  final BankAccountWrite _value;

  @override
  BankAccountWrite accountHolderName(String accountHolderName) =>
      this(accountHolderName: accountHolderName);

  @override
  BankAccountWrite bankName(String bankName) => this(bankName: bankName);

  @override
  BankAccountWrite iban(String? iban) => this(iban: iban);

  @override
  BankAccountWrite transferCodePrefix(String transferCodePrefix) =>
      this(transferCodePrefix: transferCodePrefix);

  @override
  BankAccountWrite transferWindowMinutes(
    BankAccountWriteTransferWindowMinutesEnum transferWindowMinutes,
  ) => this(transferWindowMinutes: transferWindowMinutes);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `BankAccountWrite(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// BankAccountWrite(...).copyWith(id: 12, name: "My name")
  /// ````
  BankAccountWrite call({
    Object? accountHolderName = const $CopyWithPlaceholder(),
    Object? bankName = const $CopyWithPlaceholder(),
    Object? iban = const $CopyWithPlaceholder(),
    Object? transferCodePrefix = const $CopyWithPlaceholder(),
    Object? transferWindowMinutes = const $CopyWithPlaceholder(),
  }) {
    return BankAccountWrite(
      accountHolderName: accountHolderName == const $CopyWithPlaceholder()
          ? _value.accountHolderName
          // ignore: cast_nullable_to_non_nullable
          : accountHolderName as String,
      bankName: bankName == const $CopyWithPlaceholder()
          ? _value.bankName
          // ignore: cast_nullable_to_non_nullable
          : bankName as String,
      iban: iban == const $CopyWithPlaceholder()
          ? _value.iban
          // ignore: cast_nullable_to_non_nullable
          : iban as String?,
      transferCodePrefix: transferCodePrefix == const $CopyWithPlaceholder()
          ? _value.transferCodePrefix
          // ignore: cast_nullable_to_non_nullable
          : transferCodePrefix as String,
      transferWindowMinutes:
          transferWindowMinutes == const $CopyWithPlaceholder()
          ? _value.transferWindowMinutes
          // ignore: cast_nullable_to_non_nullable
          : transferWindowMinutes as BankAccountWriteTransferWindowMinutesEnum,
    );
  }
}

extension $BankAccountWriteCopyWith on BankAccountWrite {
  /// Returns a callable class that can be used as follows: `instanceOfBankAccountWrite.copyWith(...)` or like so:`instanceOfBankAccountWrite.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$BankAccountWriteCWProxy get copyWith => _$BankAccountWriteCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

BankAccountWrite _$BankAccountWriteFromJson(Map<String, dynamic> json) =>
    $checkedCreate('BankAccountWrite', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'accountHolderName',
          'bankName',
          'transferCodePrefix',
          'transferWindowMinutes',
        ],
      );
      final val = BankAccountWrite(
        accountHolderName: $checkedConvert(
          'accountHolderName',
          (v) => v as String,
        ),
        bankName: $checkedConvert('bankName', (v) => v as String),
        iban: $checkedConvert('iban', (v) => v as String?),
        transferCodePrefix: $checkedConvert(
          'transferCodePrefix',
          (v) => v as String,
        ),
        transferWindowMinutes: $checkedConvert(
          'transferWindowMinutes',
          (v) => $enumDecode(
            _$BankAccountWriteTransferWindowMinutesEnumEnumMap,
            v,
            unknownValue:
                BankAccountWriteTransferWindowMinutesEnum.unknownDefaultOpenApi,
          ),
        ),
      );
      return val;
    });

Map<String, dynamic> _$BankAccountWriteToJson(BankAccountWrite instance) =>
    <String, dynamic>{
      'accountHolderName': instance.accountHolderName,
      'bankName': instance.bankName,
      'iban': ?instance.iban,
      'transferCodePrefix': instance.transferCodePrefix,
      'transferWindowMinutes':
          _$BankAccountWriteTransferWindowMinutesEnumEnumMap[instance
              .transferWindowMinutes]!,
    };

const _$BankAccountWriteTransferWindowMinutesEnumEnumMap = {
  BankAccountWriteTransferWindowMinutesEnum.number60: 60,
  BankAccountWriteTransferWindowMinutesEnum.number120: 120,
  BankAccountWriteTransferWindowMinutesEnum.number240: 240,
  BankAccountWriteTransferWindowMinutesEnum.unknownDefaultOpenApi: 11184809,
};
