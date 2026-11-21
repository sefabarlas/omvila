// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'customer_payment_page.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CustomerPaymentPageCWProxy {
  CustomerPaymentPage items(List<PaymentView> items);

  CustomerPaymentPage nextCursor(String? nextCursor);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerPaymentPage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerPaymentPage(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerPaymentPage call({List<PaymentView> items, String? nextCursor});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCustomerPaymentPage.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCustomerPaymentPage.copyWith.fieldName(...)`
class _$CustomerPaymentPageCWProxyImpl implements _$CustomerPaymentPageCWProxy {
  const _$CustomerPaymentPageCWProxyImpl(this._value);

  final CustomerPaymentPage _value;

  @override
  CustomerPaymentPage items(List<PaymentView> items) => this(items: items);

  @override
  CustomerPaymentPage nextCursor(String? nextCursor) =>
      this(nextCursor: nextCursor);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerPaymentPage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerPaymentPage(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerPaymentPage call({
    Object? items = const $CopyWithPlaceholder(),
    Object? nextCursor = const $CopyWithPlaceholder(),
  }) {
    return CustomerPaymentPage(
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<PaymentView>,
      nextCursor: nextCursor == const $CopyWithPlaceholder()
          ? _value.nextCursor
          // ignore: cast_nullable_to_non_nullable
          : nextCursor as String?,
    );
  }
}

extension $CustomerPaymentPageCopyWith on CustomerPaymentPage {
  /// Returns a callable class that can be used as follows: `instanceOfCustomerPaymentPage.copyWith(...)` or like so:`instanceOfCustomerPaymentPage.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CustomerPaymentPageCWProxy get copyWith =>
      _$CustomerPaymentPageCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CustomerPaymentPage _$CustomerPaymentPageFromJson(Map<String, dynamic> json) =>
    $checkedCreate('CustomerPaymentPage', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['items', 'nextCursor']);
      final val = CustomerPaymentPage(
        items: $checkedConvert(
          'items',
          (v) => (v as List<dynamic>)
              .map((e) => PaymentView.fromJson(e as Map<String, dynamic>))
              .toList(),
        ),
        nextCursor: $checkedConvert('nextCursor', (v) => v as String?),
      );
      return val;
    });

Map<String, dynamic> _$CustomerPaymentPageToJson(
  CustomerPaymentPage instance,
) => <String, dynamic>{
  'items': instance.items.map((e) => e.toJson()).toList(),
  'nextCursor': instance.nextCursor,
};
