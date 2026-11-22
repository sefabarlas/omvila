// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'credit_purchase_instructions.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CreditPurchaseInstructionsCWProxy {
  CreditPurchaseInstructions creditPackageId(String creditPackageId);

  CreditPurchaseInstructions credits(int credits);

  CreditPurchaseInstructions amount(Money? amount);

  CreditPurchaseInstructions transferCode(String transferCode);

  CreditPurchaseInstructions omvilaBankAccount(
    OmvilaBankAccount? omvilaBankAccount,
  );

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CreditPurchaseInstructions(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CreditPurchaseInstructions(...).copyWith(id: 12, name: "My name")
  /// ````
  CreditPurchaseInstructions call({
    String creditPackageId,
    int credits,
    Money? amount,
    String transferCode,
    OmvilaBankAccount? omvilaBankAccount,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCreditPurchaseInstructions.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCreditPurchaseInstructions.copyWith.fieldName(...)`
class _$CreditPurchaseInstructionsCWProxyImpl
    implements _$CreditPurchaseInstructionsCWProxy {
  const _$CreditPurchaseInstructionsCWProxyImpl(this._value);

  final CreditPurchaseInstructions _value;

  @override
  CreditPurchaseInstructions creditPackageId(String creditPackageId) =>
      this(creditPackageId: creditPackageId);

  @override
  CreditPurchaseInstructions credits(int credits) => this(credits: credits);

  @override
  CreditPurchaseInstructions amount(Money? amount) => this(amount: amount);

  @override
  CreditPurchaseInstructions transferCode(String transferCode) =>
      this(transferCode: transferCode);

  @override
  CreditPurchaseInstructions omvilaBankAccount(
    OmvilaBankAccount? omvilaBankAccount,
  ) => this(omvilaBankAccount: omvilaBankAccount);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CreditPurchaseInstructions(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CreditPurchaseInstructions(...).copyWith(id: 12, name: "My name")
  /// ````
  CreditPurchaseInstructions call({
    Object? creditPackageId = const $CopyWithPlaceholder(),
    Object? credits = const $CopyWithPlaceholder(),
    Object? amount = const $CopyWithPlaceholder(),
    Object? transferCode = const $CopyWithPlaceholder(),
    Object? omvilaBankAccount = const $CopyWithPlaceholder(),
  }) {
    return CreditPurchaseInstructions(
      creditPackageId: creditPackageId == const $CopyWithPlaceholder()
          ? _value.creditPackageId
          // ignore: cast_nullable_to_non_nullable
          : creditPackageId as String,
      credits: credits == const $CopyWithPlaceholder()
          ? _value.credits
          // ignore: cast_nullable_to_non_nullable
          : credits as int,
      amount: amount == const $CopyWithPlaceholder()
          ? _value.amount
          // ignore: cast_nullable_to_non_nullable
          : amount as Money?,
      transferCode: transferCode == const $CopyWithPlaceholder()
          ? _value.transferCode
          // ignore: cast_nullable_to_non_nullable
          : transferCode as String,
      omvilaBankAccount: omvilaBankAccount == const $CopyWithPlaceholder()
          ? _value.omvilaBankAccount
          // ignore: cast_nullable_to_non_nullable
          : omvilaBankAccount as OmvilaBankAccount?,
    );
  }
}

extension $CreditPurchaseInstructionsCopyWith on CreditPurchaseInstructions {
  /// Returns a callable class that can be used as follows: `instanceOfCreditPurchaseInstructions.copyWith(...)` or like so:`instanceOfCreditPurchaseInstructions.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CreditPurchaseInstructionsCWProxy get copyWith =>
      _$CreditPurchaseInstructionsCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CreditPurchaseInstructions _$CreditPurchaseInstructionsFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('CreditPurchaseInstructions', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'creditPackageId',
      'credits',
      'amount',
      'transferCode',
      'omvilaBankAccount',
    ],
  );
  final val = CreditPurchaseInstructions(
    creditPackageId: $checkedConvert('creditPackageId', (v) => v as String),
    credits: $checkedConvert('credits', (v) => (v as num).toInt()),
    amount: $checkedConvert(
      'amount',
      (v) => v == null ? null : Money.fromJson(v as Map<String, dynamic>),
    ),
    transferCode: $checkedConvert('transferCode', (v) => v as String),
    omvilaBankAccount: $checkedConvert(
      'omvilaBankAccount',
      (v) => v == null
          ? null
          : OmvilaBankAccount.fromJson(v as Map<String, dynamic>),
    ),
  );
  return val;
});

Map<String, dynamic> _$CreditPurchaseInstructionsToJson(
  CreditPurchaseInstructions instance,
) => <String, dynamic>{
  'creditPackageId': instance.creditPackageId,
  'credits': instance.credits,
  'amount': instance.amount?.toJson(),
  'transferCode': instance.transferCode,
  'omvilaBankAccount': instance.omvilaBankAccount?.toJson(),
};
