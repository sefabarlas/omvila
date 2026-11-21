// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'bank_account.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$BankAccountCWProxy {
  BankAccount configured(bool configured);

  BankAccount isActive(bool isActive);

  BankAccount accountHolderName(String? accountHolderName);

  BankAccount bankName(String? bankName);

  BankAccount ibanMasked(String? ibanMasked);

  BankAccount ibanLast4(String? ibanLast4);

  BankAccount transferCodePrefix(String transferCodePrefix);

  BankAccount transferWindowMinutes(
    BankAccountTransferWindowMinutesEnum transferWindowMinutes,
  );

  BankAccount updatedUtc(DateTime? updatedUtc);

  BankAccount version(String version);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `BankAccount(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// BankAccount(...).copyWith(id: 12, name: "My name")
  /// ````
  BankAccount call({
    bool configured,
    bool isActive,
    String? accountHolderName,
    String? bankName,
    String? ibanMasked,
    String? ibanLast4,
    String transferCodePrefix,
    BankAccountTransferWindowMinutesEnum transferWindowMinutes,
    DateTime? updatedUtc,
    String version,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfBankAccount.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfBankAccount.copyWith.fieldName(...)`
class _$BankAccountCWProxyImpl implements _$BankAccountCWProxy {
  const _$BankAccountCWProxyImpl(this._value);

  final BankAccount _value;

  @override
  BankAccount configured(bool configured) => this(configured: configured);

  @override
  BankAccount isActive(bool isActive) => this(isActive: isActive);

  @override
  BankAccount accountHolderName(String? accountHolderName) =>
      this(accountHolderName: accountHolderName);

  @override
  BankAccount bankName(String? bankName) => this(bankName: bankName);

  @override
  BankAccount ibanMasked(String? ibanMasked) => this(ibanMasked: ibanMasked);

  @override
  BankAccount ibanLast4(String? ibanLast4) => this(ibanLast4: ibanLast4);

  @override
  BankAccount transferCodePrefix(String transferCodePrefix) =>
      this(transferCodePrefix: transferCodePrefix);

  @override
  BankAccount transferWindowMinutes(
    BankAccountTransferWindowMinutesEnum transferWindowMinutes,
  ) => this(transferWindowMinutes: transferWindowMinutes);

  @override
  BankAccount updatedUtc(DateTime? updatedUtc) => this(updatedUtc: updatedUtc);

  @override
  BankAccount version(String version) => this(version: version);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `BankAccount(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// BankAccount(...).copyWith(id: 12, name: "My name")
  /// ````
  BankAccount call({
    Object? configured = const $CopyWithPlaceholder(),
    Object? isActive = const $CopyWithPlaceholder(),
    Object? accountHolderName = const $CopyWithPlaceholder(),
    Object? bankName = const $CopyWithPlaceholder(),
    Object? ibanMasked = const $CopyWithPlaceholder(),
    Object? ibanLast4 = const $CopyWithPlaceholder(),
    Object? transferCodePrefix = const $CopyWithPlaceholder(),
    Object? transferWindowMinutes = const $CopyWithPlaceholder(),
    Object? updatedUtc = const $CopyWithPlaceholder(),
    Object? version = const $CopyWithPlaceholder(),
  }) {
    return BankAccount(
      configured: configured == const $CopyWithPlaceholder()
          ? _value.configured
          // ignore: cast_nullable_to_non_nullable
          : configured as bool,
      isActive: isActive == const $CopyWithPlaceholder()
          ? _value.isActive
          // ignore: cast_nullable_to_non_nullable
          : isActive as bool,
      accountHolderName: accountHolderName == const $CopyWithPlaceholder()
          ? _value.accountHolderName
          // ignore: cast_nullable_to_non_nullable
          : accountHolderName as String?,
      bankName: bankName == const $CopyWithPlaceholder()
          ? _value.bankName
          // ignore: cast_nullable_to_non_nullable
          : bankName as String?,
      ibanMasked: ibanMasked == const $CopyWithPlaceholder()
          ? _value.ibanMasked
          // ignore: cast_nullable_to_non_nullable
          : ibanMasked as String?,
      ibanLast4: ibanLast4 == const $CopyWithPlaceholder()
          ? _value.ibanLast4
          // ignore: cast_nullable_to_non_nullable
          : ibanLast4 as String?,
      transferCodePrefix: transferCodePrefix == const $CopyWithPlaceholder()
          ? _value.transferCodePrefix
          // ignore: cast_nullable_to_non_nullable
          : transferCodePrefix as String,
      transferWindowMinutes:
          transferWindowMinutes == const $CopyWithPlaceholder()
          ? _value.transferWindowMinutes
          // ignore: cast_nullable_to_non_nullable
          : transferWindowMinutes as BankAccountTransferWindowMinutesEnum,
      updatedUtc: updatedUtc == const $CopyWithPlaceholder()
          ? _value.updatedUtc
          // ignore: cast_nullable_to_non_nullable
          : updatedUtc as DateTime?,
      version: version == const $CopyWithPlaceholder()
          ? _value.version
          // ignore: cast_nullable_to_non_nullable
          : version as String,
    );
  }
}

extension $BankAccountCopyWith on BankAccount {
  /// Returns a callable class that can be used as follows: `instanceOfBankAccount.copyWith(...)` or like so:`instanceOfBankAccount.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$BankAccountCWProxy get copyWith => _$BankAccountCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

BankAccount _$BankAccountFromJson(Map<String, dynamic> json) =>
    $checkedCreate('BankAccount', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'configured',
          'isActive',
          'transferCodePrefix',
          'transferWindowMinutes',
          'version',
        ],
      );
      final val = BankAccount(
        configured: $checkedConvert('configured', (v) => v as bool),
        isActive: $checkedConvert('isActive', (v) => v as bool),
        accountHolderName: $checkedConvert(
          'accountHolderName',
          (v) => v as String?,
        ),
        bankName: $checkedConvert('bankName', (v) => v as String?),
        ibanMasked: $checkedConvert('ibanMasked', (v) => v as String?),
        ibanLast4: $checkedConvert('ibanLast4', (v) => v as String?),
        transferCodePrefix: $checkedConvert(
          'transferCodePrefix',
          (v) => v as String,
        ),
        transferWindowMinutes: $checkedConvert(
          'transferWindowMinutes',
          (v) => $enumDecode(
            _$BankAccountTransferWindowMinutesEnumEnumMap,
            v,
            unknownValue:
                BankAccountTransferWindowMinutesEnum.unknownDefaultOpenApi,
          ),
        ),
        updatedUtc: $checkedConvert(
          'updatedUtc',
          (v) => v == null ? null : DateTime.parse(v as String),
        ),
        version: $checkedConvert('version', (v) => v as String),
      );
      return val;
    });

Map<String, dynamic> _$BankAccountToJson(BankAccount instance) =>
    <String, dynamic>{
      'configured': instance.configured,
      'isActive': instance.isActive,
      'accountHolderName': ?instance.accountHolderName,
      'bankName': ?instance.bankName,
      'ibanMasked': ?instance.ibanMasked,
      'ibanLast4': ?instance.ibanLast4,
      'transferCodePrefix': instance.transferCodePrefix,
      'transferWindowMinutes':
          _$BankAccountTransferWindowMinutesEnumEnumMap[instance
              .transferWindowMinutes]!,
      'updatedUtc': ?instance.updatedUtc?.toIso8601String(),
      'version': instance.version,
    };

const _$BankAccountTransferWindowMinutesEnumEnumMap = {
  BankAccountTransferWindowMinutesEnum.number60: 60,
  BankAccountTransferWindowMinutesEnum.number120: 120,
  BankAccountTransferWindowMinutesEnum.number240: 240,
  BankAccountTransferWindowMinutesEnum.unknownDefaultOpenApi: 11184809,
};
