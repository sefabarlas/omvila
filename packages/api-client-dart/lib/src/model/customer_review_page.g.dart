// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'customer_review_page.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CustomerReviewPageCWProxy {
  CustomerReviewPage items(List<ReviewView> items);

  CustomerReviewPage nextCursor(String? nextCursor);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerReviewPage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerReviewPage(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerReviewPage call({List<ReviewView> items, String? nextCursor});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCustomerReviewPage.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCustomerReviewPage.copyWith.fieldName(...)`
class _$CustomerReviewPageCWProxyImpl implements _$CustomerReviewPageCWProxy {
  const _$CustomerReviewPageCWProxyImpl(this._value);

  final CustomerReviewPage _value;

  @override
  CustomerReviewPage items(List<ReviewView> items) => this(items: items);

  @override
  CustomerReviewPage nextCursor(String? nextCursor) =>
      this(nextCursor: nextCursor);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerReviewPage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerReviewPage(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerReviewPage call({
    Object? items = const $CopyWithPlaceholder(),
    Object? nextCursor = const $CopyWithPlaceholder(),
  }) {
    return CustomerReviewPage(
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<ReviewView>,
      nextCursor: nextCursor == const $CopyWithPlaceholder()
          ? _value.nextCursor
          // ignore: cast_nullable_to_non_nullable
          : nextCursor as String?,
    );
  }
}

extension $CustomerReviewPageCopyWith on CustomerReviewPage {
  /// Returns a callable class that can be used as follows: `instanceOfCustomerReviewPage.copyWith(...)` or like so:`instanceOfCustomerReviewPage.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CustomerReviewPageCWProxy get copyWith =>
      _$CustomerReviewPageCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CustomerReviewPage _$CustomerReviewPageFromJson(Map<String, dynamic> json) =>
    $checkedCreate('CustomerReviewPage', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['items', 'nextCursor']);
      final val = CustomerReviewPage(
        items: $checkedConvert(
          'items',
          (v) => (v as List<dynamic>)
              .map((e) => ReviewView.fromJson(e as Map<String, dynamic>))
              .toList(),
        ),
        nextCursor: $checkedConvert('nextCursor', (v) => v as String?),
      );
      return val;
    });

Map<String, dynamic> _$CustomerReviewPageToJson(CustomerReviewPage instance) =>
    <String, dynamic>{
      'items': instance.items.map((e) => e.toJson()).toList(),
      'nextCursor': instance.nextCursor,
    };
