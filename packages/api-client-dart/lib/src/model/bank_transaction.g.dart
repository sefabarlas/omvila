// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'bank_transaction.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$BankTransactionCWProxy {
  BankTransaction id(String id);

  BankTransaction valueDate(DateTime valueDate);

  BankTransaction receivedUtc(DateTime? receivedUtc);

  BankTransaction amount(Money amount);

  BankTransaction senderName(String senderName);

  BankTransaction description(String? description);

  BankTransaction bankReference(String bankReference);

  BankTransaction source_(BankTransactionSource_Enum source_);

  BankTransaction matchedNoticeId(String? matchedNoticeId);

  BankTransaction matchedBrandName(String? matchedBrandName);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `BankTransaction(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// BankTransaction(...).copyWith(id: 12, name: "My name")
  /// ````
  BankTransaction call({
    String id,
    DateTime valueDate,
    DateTime? receivedUtc,
    Money amount,
    String senderName,
    String? description,
    String bankReference,
    BankTransactionSource_Enum source_,
    String? matchedNoticeId,
    String? matchedBrandName,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfBankTransaction.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfBankTransaction.copyWith.fieldName(...)`
class _$BankTransactionCWProxyImpl implements _$BankTransactionCWProxy {
  const _$BankTransactionCWProxyImpl(this._value);

  final BankTransaction _value;

  @override
  BankTransaction id(String id) => this(id: id);

  @override
  BankTransaction valueDate(DateTime valueDate) => this(valueDate: valueDate);

  @override
  BankTransaction receivedUtc(DateTime? receivedUtc) =>
      this(receivedUtc: receivedUtc);

  @override
  BankTransaction amount(Money amount) => this(amount: amount);

  @override
  BankTransaction senderName(String senderName) => this(senderName: senderName);

  @override
  BankTransaction description(String? description) =>
      this(description: description);

  @override
  BankTransaction bankReference(String bankReference) =>
      this(bankReference: bankReference);

  @override
  BankTransaction source_(BankTransactionSource_Enum source_) =>
      this(source_: source_);

  @override
  BankTransaction matchedNoticeId(String? matchedNoticeId) =>
      this(matchedNoticeId: matchedNoticeId);

  @override
  BankTransaction matchedBrandName(String? matchedBrandName) =>
      this(matchedBrandName: matchedBrandName);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `BankTransaction(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// BankTransaction(...).copyWith(id: 12, name: "My name")
  /// ````
  BankTransaction call({
    Object? id = const $CopyWithPlaceholder(),
    Object? valueDate = const $CopyWithPlaceholder(),
    Object? receivedUtc = const $CopyWithPlaceholder(),
    Object? amount = const $CopyWithPlaceholder(),
    Object? senderName = const $CopyWithPlaceholder(),
    Object? description = const $CopyWithPlaceholder(),
    Object? bankReference = const $CopyWithPlaceholder(),
    Object? source_ = const $CopyWithPlaceholder(),
    Object? matchedNoticeId = const $CopyWithPlaceholder(),
    Object? matchedBrandName = const $CopyWithPlaceholder(),
  }) {
    return BankTransaction(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
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
      bankReference: bankReference == const $CopyWithPlaceholder()
          ? _value.bankReference
          // ignore: cast_nullable_to_non_nullable
          : bankReference as String,
      source_: source_ == const $CopyWithPlaceholder()
          ? _value.source_
          // ignore: cast_nullable_to_non_nullable
          : source_ as BankTransactionSource_Enum,
      matchedNoticeId: matchedNoticeId == const $CopyWithPlaceholder()
          ? _value.matchedNoticeId
          // ignore: cast_nullable_to_non_nullable
          : matchedNoticeId as String?,
      matchedBrandName: matchedBrandName == const $CopyWithPlaceholder()
          ? _value.matchedBrandName
          // ignore: cast_nullable_to_non_nullable
          : matchedBrandName as String?,
    );
  }
}

extension $BankTransactionCopyWith on BankTransaction {
  /// Returns a callable class that can be used as follows: `instanceOfBankTransaction.copyWith(...)` or like so:`instanceOfBankTransaction.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$BankTransactionCWProxy get copyWith => _$BankTransactionCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

BankTransaction _$BankTransactionFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('BankTransaction', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'id',
      'valueDate',
      'receivedUtc',
      'amount',
      'senderName',
      'description',
      'bankReference',
      'source',
      'matchedNoticeId',
      'matchedBrandName',
    ],
  );
  final val = BankTransaction(
    id: $checkedConvert('id', (v) => v as String),
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
    bankReference: $checkedConvert('bankReference', (v) => v as String),
    source_: $checkedConvert(
      'source',
      (v) => $enumDecode(
        _$BankTransactionSource_EnumEnumMap,
        v,
        unknownValue: BankTransactionSource_Enum.unknownDefaultOpenApi,
      ),
    ),
    matchedNoticeId: $checkedConvert('matchedNoticeId', (v) => v as String?),
    matchedBrandName: $checkedConvert('matchedBrandName', (v) => v as String?),
  );
  return val;
}, fieldKeyMap: const {'source_': 'source'});

Map<String, dynamic> _$BankTransactionToJson(BankTransaction instance) =>
    <String, dynamic>{
      'id': instance.id,
      'valueDate': const DateOnlyConverter().toJson(instance.valueDate),
      'receivedUtc': instance.receivedUtc?.toIso8601String(),
      'amount': instance.amount.toJson(),
      'senderName': instance.senderName,
      'description': instance.description,
      'bankReference': instance.bankReference,
      'source': _$BankTransactionSource_EnumEnumMap[instance.source_]!,
      'matchedNoticeId': instance.matchedNoticeId,
      'matchedBrandName': instance.matchedBrandName,
    };

const _$BankTransactionSource_EnumEnumMap = {
  BankTransactionSource_Enum.import_: 'Import',
  BankTransactionSource_Enum.manual: 'Manual',
  BankTransactionSource_Enum.unknownDefaultOpenApi: 'unknown_default_open_api',
};
