// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_payment_notice_page.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminPaymentNoticePageCWProxy {
  AdminPaymentNoticePage items(List<PaymentNoticeSummary> items);

  AdminPaymentNoticePage nextCursor(String? nextCursor);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminPaymentNoticePage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminPaymentNoticePage(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminPaymentNoticePage call({
    List<PaymentNoticeSummary> items,
    String? nextCursor,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminPaymentNoticePage.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminPaymentNoticePage.copyWith.fieldName(...)`
class _$AdminPaymentNoticePageCWProxyImpl
    implements _$AdminPaymentNoticePageCWProxy {
  const _$AdminPaymentNoticePageCWProxyImpl(this._value);

  final AdminPaymentNoticePage _value;

  @override
  AdminPaymentNoticePage items(List<PaymentNoticeSummary> items) =>
      this(items: items);

  @override
  AdminPaymentNoticePage nextCursor(String? nextCursor) =>
      this(nextCursor: nextCursor);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminPaymentNoticePage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminPaymentNoticePage(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminPaymentNoticePage call({
    Object? items = const $CopyWithPlaceholder(),
    Object? nextCursor = const $CopyWithPlaceholder(),
  }) {
    return AdminPaymentNoticePage(
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<PaymentNoticeSummary>,
      nextCursor: nextCursor == const $CopyWithPlaceholder()
          ? _value.nextCursor
          // ignore: cast_nullable_to_non_nullable
          : nextCursor as String?,
    );
  }
}

extension $AdminPaymentNoticePageCopyWith on AdminPaymentNoticePage {
  /// Returns a callable class that can be used as follows: `instanceOfAdminPaymentNoticePage.copyWith(...)` or like so:`instanceOfAdminPaymentNoticePage.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminPaymentNoticePageCWProxy get copyWith =>
      _$AdminPaymentNoticePageCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminPaymentNoticePage _$AdminPaymentNoticePageFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AdminPaymentNoticePage', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['items', 'nextCursor']);
  final val = AdminPaymentNoticePage(
    items: $checkedConvert(
      'items',
      (v) => (v as List<dynamic>)
          .map((e) => PaymentNoticeSummary.fromJson(e as Map<String, dynamic>))
          .toList(),
    ),
    nextCursor: $checkedConvert('nextCursor', (v) => v as String?),
  );
  return val;
});

Map<String, dynamic> _$AdminPaymentNoticePageToJson(
  AdminPaymentNoticePage instance,
) => <String, dynamic>{
  'items': instance.items.map((e) => e.toJson()).toList(),
  'nextCursor': instance.nextCursor,
};
