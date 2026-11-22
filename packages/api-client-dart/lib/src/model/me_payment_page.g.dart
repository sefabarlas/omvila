// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'me_payment_page.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MePaymentPageCWProxy {
  MePaymentPage items(List<MePaymentRow> items);

  MePaymentPage nextCursor(String? nextCursor);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MePaymentPage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MePaymentPage(...).copyWith(id: 12, name: "My name")
  /// ````
  MePaymentPage call({List<MePaymentRow> items, String? nextCursor});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMePaymentPage.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMePaymentPage.copyWith.fieldName(...)`
class _$MePaymentPageCWProxyImpl implements _$MePaymentPageCWProxy {
  const _$MePaymentPageCWProxyImpl(this._value);

  final MePaymentPage _value;

  @override
  MePaymentPage items(List<MePaymentRow> items) => this(items: items);

  @override
  MePaymentPage nextCursor(String? nextCursor) => this(nextCursor: nextCursor);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MePaymentPage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MePaymentPage(...).copyWith(id: 12, name: "My name")
  /// ````
  MePaymentPage call({
    Object? items = const $CopyWithPlaceholder(),
    Object? nextCursor = const $CopyWithPlaceholder(),
  }) {
    return MePaymentPage(
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<MePaymentRow>,
      nextCursor: nextCursor == const $CopyWithPlaceholder()
          ? _value.nextCursor
          // ignore: cast_nullable_to_non_nullable
          : nextCursor as String?,
    );
  }
}

extension $MePaymentPageCopyWith on MePaymentPage {
  /// Returns a callable class that can be used as follows: `instanceOfMePaymentPage.copyWith(...)` or like so:`instanceOfMePaymentPage.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MePaymentPageCWProxy get copyWith => _$MePaymentPageCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MePaymentPage _$MePaymentPageFromJson(Map<String, dynamic> json) =>
    $checkedCreate('MePaymentPage', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['items', 'nextCursor']);
      final val = MePaymentPage(
        items: $checkedConvert(
          'items',
          (v) => (v as List<dynamic>)
              .map((e) => MePaymentRow.fromJson(e as Map<String, dynamic>))
              .toList(),
        ),
        nextCursor: $checkedConvert('nextCursor', (v) => v as String?),
      );
      return val;
    });

Map<String, dynamic> _$MePaymentPageToJson(MePaymentPage instance) =>
    <String, dynamic>{
      'items': instance.items.map((e) => e.toJson()).toList(),
      'nextCursor': instance.nextCursor,
    };
