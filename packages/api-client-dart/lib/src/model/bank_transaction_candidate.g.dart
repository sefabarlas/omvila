// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'bank_transaction_candidate.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$BankTransactionCandidateCWProxy {
  BankTransactionCandidate bankTransactionId(String bankTransactionId);

  BankTransactionCandidate valueDate(DateTime valueDate);

  BankTransactionCandidate receivedUtc(DateTime? receivedUtc);

  BankTransactionCandidate amount(Money amount);

  BankTransactionCandidate senderName(String senderName);

  BankTransactionCandidate description(String? description);

  BankTransactionCandidate codeFound(bool codeFound);

  BankTransactionCandidate amountMatches(bool amountMatches);

  BankTransactionCandidate verdict(MatchVerdict verdict);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `BankTransactionCandidate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// BankTransactionCandidate(...).copyWith(id: 12, name: "My name")
  /// ````
  BankTransactionCandidate call({
    String bankTransactionId,
    DateTime valueDate,
    DateTime? receivedUtc,
    Money amount,
    String senderName,
    String? description,
    bool codeFound,
    bool amountMatches,
    MatchVerdict verdict,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfBankTransactionCandidate.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfBankTransactionCandidate.copyWith.fieldName(...)`
class _$BankTransactionCandidateCWProxyImpl
    implements _$BankTransactionCandidateCWProxy {
  const _$BankTransactionCandidateCWProxyImpl(this._value);

  final BankTransactionCandidate _value;

  @override
  BankTransactionCandidate bankTransactionId(String bankTransactionId) =>
      this(bankTransactionId: bankTransactionId);

  @override
  BankTransactionCandidate valueDate(DateTime valueDate) =>
      this(valueDate: valueDate);

  @override
  BankTransactionCandidate receivedUtc(DateTime? receivedUtc) =>
      this(receivedUtc: receivedUtc);

  @override
  BankTransactionCandidate amount(Money amount) => this(amount: amount);

  @override
  BankTransactionCandidate senderName(String senderName) =>
      this(senderName: senderName);

  @override
  BankTransactionCandidate description(String? description) =>
      this(description: description);

  @override
  BankTransactionCandidate codeFound(bool codeFound) =>
      this(codeFound: codeFound);

  @override
  BankTransactionCandidate amountMatches(bool amountMatches) =>
      this(amountMatches: amountMatches);

  @override
  BankTransactionCandidate verdict(MatchVerdict verdict) =>
      this(verdict: verdict);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `BankTransactionCandidate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// BankTransactionCandidate(...).copyWith(id: 12, name: "My name")
  /// ````
  BankTransactionCandidate call({
    Object? bankTransactionId = const $CopyWithPlaceholder(),
    Object? valueDate = const $CopyWithPlaceholder(),
    Object? receivedUtc = const $CopyWithPlaceholder(),
    Object? amount = const $CopyWithPlaceholder(),
    Object? senderName = const $CopyWithPlaceholder(),
    Object? description = const $CopyWithPlaceholder(),
    Object? codeFound = const $CopyWithPlaceholder(),
    Object? amountMatches = const $CopyWithPlaceholder(),
    Object? verdict = const $CopyWithPlaceholder(),
  }) {
    return BankTransactionCandidate(
      bankTransactionId: bankTransactionId == const $CopyWithPlaceholder()
          ? _value.bankTransactionId
          // ignore: cast_nullable_to_non_nullable
          : bankTransactionId as String,
      valueDate: valueDate == const $CopyWithPlaceholder()
          ? _value.valueDate
          // ignore: cast_nullable_to_non_nullable
          : valueDate as DateTime,
      receivedUtc: receivedUtc == const $CopyWithPlaceholder()
          ? _value.receivedUtc
          // ignore: cast_nullable_to_non_nullable
          : receivedUtc as DateTime?,
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
      codeFound: codeFound == const $CopyWithPlaceholder()
          ? _value.codeFound
          // ignore: cast_nullable_to_non_nullable
          : codeFound as bool,
      amountMatches: amountMatches == const $CopyWithPlaceholder()
          ? _value.amountMatches
          // ignore: cast_nullable_to_non_nullable
          : amountMatches as bool,
      verdict: verdict == const $CopyWithPlaceholder()
          ? _value.verdict
          // ignore: cast_nullable_to_non_nullable
          : verdict as MatchVerdict,
    );
  }
}

extension $BankTransactionCandidateCopyWith on BankTransactionCandidate {
  /// Returns a callable class that can be used as follows: `instanceOfBankTransactionCandidate.copyWith(...)` or like so:`instanceOfBankTransactionCandidate.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$BankTransactionCandidateCWProxy get copyWith =>
      _$BankTransactionCandidateCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

BankTransactionCandidate _$BankTransactionCandidateFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('BankTransactionCandidate', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'bankTransactionId',
      'valueDate',
      'receivedUtc',
      'amount',
      'senderName',
      'description',
      'codeFound',
      'amountMatches',
      'verdict',
    ],
  );
  final val = BankTransactionCandidate(
    bankTransactionId: $checkedConvert('bankTransactionId', (v) => v as String),
    valueDate: $checkedConvert(
      'valueDate',
      (v) => const DateOnlyConverter().fromJson(v as String),
    ),
    receivedUtc: $checkedConvert(
      'receivedUtc',
      (v) => v == null ? null : DateTime.parse(v as String),
    ),
    amount: $checkedConvert(
      'amount',
      (v) => Money.fromJson(v as Map<String, dynamic>),
    ),
    senderName: $checkedConvert('senderName', (v) => v as String),
    description: $checkedConvert('description', (v) => v as String?),
    codeFound: $checkedConvert('codeFound', (v) => v as bool),
    amountMatches: $checkedConvert('amountMatches', (v) => v as bool),
    verdict: $checkedConvert(
      'verdict',
      (v) => $enumDecode(
        _$MatchVerdictEnumMap,
        v,
        unknownValue: MatchVerdict.unknownDefaultOpenApi,
      ),
    ),
  );
  return val;
});

Map<String, dynamic> _$BankTransactionCandidateToJson(
  BankTransactionCandidate instance,
) => <String, dynamic>{
  'bankTransactionId': instance.bankTransactionId,
  'valueDate': const DateOnlyConverter().toJson(instance.valueDate),
  'receivedUtc': instance.receivedUtc?.toIso8601String(),
  'amount': instance.amount.toJson(),
  'senderName': instance.senderName,
  'description': instance.description,
  'codeFound': instance.codeFound,
  'amountMatches': instance.amountMatches,
  'verdict': _$MatchVerdictEnumMap[instance.verdict]!,
};

const _$MatchVerdictEnumMap = {
  MatchVerdict.codeMatches: 'CodeMatches',
  MatchVerdict.checkRequired: 'CheckRequired',
  MatchVerdict.unknownDefaultOpenApi: 'unknown_default_open_api',
};
