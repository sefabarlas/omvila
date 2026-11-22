// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_credit_transaction.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminCreditTransactionCWProxy {
  AdminCreditTransaction id(String id);

  AdminCreditTransaction occurredUtc(DateTime occurredUtc);

  AdminCreditTransaction type(CreditTransactionType type);

  AdminCreditTransaction credits(int credits);

  AdminCreditTransaction balanceAfter(int balanceAfter);

  AdminCreditTransaction note(String? note);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminCreditTransaction(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminCreditTransaction(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminCreditTransaction call({
    String id,
    DateTime occurredUtc,
    CreditTransactionType type,
    int credits,
    int balanceAfter,
    String? note,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminCreditTransaction.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminCreditTransaction.copyWith.fieldName(...)`
class _$AdminCreditTransactionCWProxyImpl
    implements _$AdminCreditTransactionCWProxy {
  const _$AdminCreditTransactionCWProxyImpl(this._value);

  final AdminCreditTransaction _value;

  @override
  AdminCreditTransaction id(String id) => this(id: id);

  @override
  AdminCreditTransaction occurredUtc(DateTime occurredUtc) =>
      this(occurredUtc: occurredUtc);

  @override
  AdminCreditTransaction type(CreditTransactionType type) => this(type: type);

  @override
  AdminCreditTransaction credits(int credits) => this(credits: credits);

  @override
  AdminCreditTransaction balanceAfter(int balanceAfter) =>
      this(balanceAfter: balanceAfter);

  @override
  AdminCreditTransaction note(String? note) => this(note: note);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminCreditTransaction(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminCreditTransaction(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminCreditTransaction call({
    Object? id = const $CopyWithPlaceholder(),
    Object? occurredUtc = const $CopyWithPlaceholder(),
    Object? type = const $CopyWithPlaceholder(),
    Object? credits = const $CopyWithPlaceholder(),
    Object? balanceAfter = const $CopyWithPlaceholder(),
    Object? note = const $CopyWithPlaceholder(),
  }) {
    return AdminCreditTransaction(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      occurredUtc: occurredUtc == const $CopyWithPlaceholder()
          ? _value.occurredUtc
          // ignore: cast_nullable_to_non_nullable
          : occurredUtc as DateTime,
      type: type == const $CopyWithPlaceholder()
          ? _value.type
          // ignore: cast_nullable_to_non_nullable
          : type as CreditTransactionType,
      credits: credits == const $CopyWithPlaceholder()
          ? _value.credits
          // ignore: cast_nullable_to_non_nullable
          : credits as int,
      balanceAfter: balanceAfter == const $CopyWithPlaceholder()
          ? _value.balanceAfter
          // ignore: cast_nullable_to_non_nullable
          : balanceAfter as int,
      note: note == const $CopyWithPlaceholder()
          ? _value.note
          // ignore: cast_nullable_to_non_nullable
          : note as String?,
    );
  }
}

extension $AdminCreditTransactionCopyWith on AdminCreditTransaction {
  /// Returns a callable class that can be used as follows: `instanceOfAdminCreditTransaction.copyWith(...)` or like so:`instanceOfAdminCreditTransaction.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminCreditTransactionCWProxy get copyWith =>
      _$AdminCreditTransactionCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminCreditTransaction _$AdminCreditTransactionFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AdminCreditTransaction', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'id',
      'occurredUtc',
      'type',
      'credits',
      'balanceAfter',
      'note',
    ],
  );
  final val = AdminCreditTransaction(
    id: $checkedConvert('id', (v) => v as String),
    occurredUtc: $checkedConvert(
      'occurredUtc',
      (v) => DateTime.parse(v as String),
    ),
    type: $checkedConvert(
      'type',
      (v) => $enumDecode(
        _$CreditTransactionTypeEnumMap,
        v,
        unknownValue: CreditTransactionType.unknownDefaultOpenApi,
      ),
    ),
    credits: $checkedConvert('credits', (v) => (v as num).toInt()),
    balanceAfter: $checkedConvert('balanceAfter', (v) => (v as num).toInt()),
    note: $checkedConvert('note', (v) => v as String?),
  );
  return val;
});

Map<String, dynamic> _$AdminCreditTransactionToJson(
  AdminCreditTransaction instance,
) => <String, dynamic>{
  'id': instance.id,
  'occurredUtc': instance.occurredUtc.toIso8601String(),
  'type': _$CreditTransactionTypeEnumMap[instance.type]!,
  'credits': instance.credits,
  'balanceAfter': instance.balanceAfter,
  'note': instance.note,
};

const _$CreditTransactionTypeEnumMap = {
  CreditTransactionType.purchase: 'Purchase',
  CreditTransactionType.usage: 'Usage',
  CreditTransactionType.trialGift: 'TrialGift',
  CreditTransactionType.grant: 'Grant',
  CreditTransactionType.refund: 'Refund',
  CreditTransactionType.unknownDefaultOpenApi: 'unknown_default_open_api',
};
