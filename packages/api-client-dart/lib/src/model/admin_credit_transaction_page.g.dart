// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_credit_transaction_page.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminCreditTransactionPageCWProxy {
  AdminCreditTransactionPage items(List<AdminCreditTransaction> items);

  AdminCreditTransactionPage nextCursor(String? nextCursor);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminCreditTransactionPage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminCreditTransactionPage(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminCreditTransactionPage call({
    List<AdminCreditTransaction> items,
    String? nextCursor,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminCreditTransactionPage.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminCreditTransactionPage.copyWith.fieldName(...)`
class _$AdminCreditTransactionPageCWProxyImpl
    implements _$AdminCreditTransactionPageCWProxy {
  const _$AdminCreditTransactionPageCWProxyImpl(this._value);

  final AdminCreditTransactionPage _value;

  @override
  AdminCreditTransactionPage items(List<AdminCreditTransaction> items) =>
      this(items: items);

  @override
  AdminCreditTransactionPage nextCursor(String? nextCursor) =>
      this(nextCursor: nextCursor);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminCreditTransactionPage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminCreditTransactionPage(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminCreditTransactionPage call({
    Object? items = const $CopyWithPlaceholder(),
    Object? nextCursor = const $CopyWithPlaceholder(),
  }) {
    return AdminCreditTransactionPage(
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<AdminCreditTransaction>,
      nextCursor: nextCursor == const $CopyWithPlaceholder()
          ? _value.nextCursor
          // ignore: cast_nullable_to_non_nullable
          : nextCursor as String?,
    );
  }
}

extension $AdminCreditTransactionPageCopyWith on AdminCreditTransactionPage {
  /// Returns a callable class that can be used as follows: `instanceOfAdminCreditTransactionPage.copyWith(...)` or like so:`instanceOfAdminCreditTransactionPage.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminCreditTransactionPageCWProxy get copyWith =>
      _$AdminCreditTransactionPageCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminCreditTransactionPage _$AdminCreditTransactionPageFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AdminCreditTransactionPage', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['items', 'nextCursor']);
  final val = AdminCreditTransactionPage(
    items: $checkedConvert(
      'items',
      (v) => (v as List<dynamic>)
          .map(
            (e) => AdminCreditTransaction.fromJson(e as Map<String, dynamic>),
          )
          .toList(),
    ),
    nextCursor: $checkedConvert('nextCursor', (v) => v as String?),
  );
  return val;
});

Map<String, dynamic> _$AdminCreditTransactionPageToJson(
  AdminCreditTransactionPage instance,
) => <String, dynamic>{
  'items': instance.items.map((e) => e.toJson()).toList(),
  'nextCursor': instance.nextCursor,
};
