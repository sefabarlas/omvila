// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'subscription_invoice_page.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$SubscriptionInvoicePageCWProxy {
  SubscriptionInvoicePage items(List<SubscriptionInvoice> items);

  SubscriptionInvoicePage nextCursor(String? nextCursor);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `SubscriptionInvoicePage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// SubscriptionInvoicePage(...).copyWith(id: 12, name: "My name")
  /// ````
  SubscriptionInvoicePage call({
    List<SubscriptionInvoice> items,
    String? nextCursor,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfSubscriptionInvoicePage.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfSubscriptionInvoicePage.copyWith.fieldName(...)`
class _$SubscriptionInvoicePageCWProxyImpl
    implements _$SubscriptionInvoicePageCWProxy {
  const _$SubscriptionInvoicePageCWProxyImpl(this._value);

  final SubscriptionInvoicePage _value;

  @override
  SubscriptionInvoicePage items(List<SubscriptionInvoice> items) =>
      this(items: items);

  @override
  SubscriptionInvoicePage nextCursor(String? nextCursor) =>
      this(nextCursor: nextCursor);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `SubscriptionInvoicePage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// SubscriptionInvoicePage(...).copyWith(id: 12, name: "My name")
  /// ````
  SubscriptionInvoicePage call({
    Object? items = const $CopyWithPlaceholder(),
    Object? nextCursor = const $CopyWithPlaceholder(),
  }) {
    return SubscriptionInvoicePage(
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<SubscriptionInvoice>,
      nextCursor: nextCursor == const $CopyWithPlaceholder()
          ? _value.nextCursor
          // ignore: cast_nullable_to_non_nullable
          : nextCursor as String?,
    );
  }
}

extension $SubscriptionInvoicePageCopyWith on SubscriptionInvoicePage {
  /// Returns a callable class that can be used as follows: `instanceOfSubscriptionInvoicePage.copyWith(...)` or like so:`instanceOfSubscriptionInvoicePage.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$SubscriptionInvoicePageCWProxy get copyWith =>
      _$SubscriptionInvoicePageCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

SubscriptionInvoicePage _$SubscriptionInvoicePageFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('SubscriptionInvoicePage', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['items', 'nextCursor']);
  final val = SubscriptionInvoicePage(
    items: $checkedConvert(
      'items',
      (v) => (v as List<dynamic>)
          .map((e) => SubscriptionInvoice.fromJson(e as Map<String, dynamic>))
          .toList(),
    ),
    nextCursor: $checkedConvert('nextCursor', (v) => v as String?),
  );
  return val;
});

Map<String, dynamic> _$SubscriptionInvoicePageToJson(
  SubscriptionInvoicePage instance,
) => <String, dynamic>{
  'items': instance.items.map((e) => e.toJson()).toList(),
  'nextCursor': instance.nextCursor,
};
