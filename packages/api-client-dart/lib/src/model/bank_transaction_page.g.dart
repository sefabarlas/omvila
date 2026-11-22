// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'bank_transaction_page.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$BankTransactionPageCWProxy {
  BankTransactionPage items(List<BankTransaction> items);

  BankTransactionPage nextCursor(String? nextCursor);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `BankTransactionPage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// BankTransactionPage(...).copyWith(id: 12, name: "My name")
  /// ````
  BankTransactionPage call({List<BankTransaction> items, String? nextCursor});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfBankTransactionPage.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfBankTransactionPage.copyWith.fieldName(...)`
class _$BankTransactionPageCWProxyImpl implements _$BankTransactionPageCWProxy {
  const _$BankTransactionPageCWProxyImpl(this._value);

  final BankTransactionPage _value;

  @override
  BankTransactionPage items(List<BankTransaction> items) => this(items: items);

  @override
  BankTransactionPage nextCursor(String? nextCursor) =>
      this(nextCursor: nextCursor);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `BankTransactionPage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// BankTransactionPage(...).copyWith(id: 12, name: "My name")
  /// ````
  BankTransactionPage call({
    Object? items = const $CopyWithPlaceholder(),
    Object? nextCursor = const $CopyWithPlaceholder(),
  }) {
    return BankTransactionPage(
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<BankTransaction>,
      nextCursor: nextCursor == const $CopyWithPlaceholder()
          ? _value.nextCursor
          // ignore: cast_nullable_to_non_nullable
          : nextCursor as String?,
    );
  }
}

extension $BankTransactionPageCopyWith on BankTransactionPage {
  /// Returns a callable class that can be used as follows: `instanceOfBankTransactionPage.copyWith(...)` or like so:`instanceOfBankTransactionPage.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$BankTransactionPageCWProxy get copyWith =>
      _$BankTransactionPageCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

BankTransactionPage _$BankTransactionPageFromJson(Map<String, dynamic> json) =>
    $checkedCreate('BankTransactionPage', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['items', 'nextCursor']);
      final val = BankTransactionPage(
        items: $checkedConvert(
          'items',
          (v) => (v as List<dynamic>)
              .map((e) => BankTransaction.fromJson(e as Map<String, dynamic>))
              .toList(),
        ),
        nextCursor: $checkedConvert('nextCursor', (v) => v as String?),
      );
      return val;
    });

Map<String, dynamic> _$BankTransactionPageToJson(
  BankTransactionPage instance,
) => <String, dynamic>{
  'items': instance.items.map((e) => e.toJson()).toList(),
  'nextCursor': instance.nextCursor,
};
