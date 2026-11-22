// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_brand_page.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminBrandPageCWProxy {
  AdminBrandPage counts(AdminBrandPageCounts counts);

  AdminBrandPage items(List<AdminBrandListItem> items);

  AdminBrandPage nextCursor(String? nextCursor);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminBrandPage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminBrandPage(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminBrandPage call({
    AdminBrandPageCounts counts,
    List<AdminBrandListItem> items,
    String? nextCursor,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminBrandPage.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminBrandPage.copyWith.fieldName(...)`
class _$AdminBrandPageCWProxyImpl implements _$AdminBrandPageCWProxy {
  const _$AdminBrandPageCWProxyImpl(this._value);

  final AdminBrandPage _value;

  @override
  AdminBrandPage counts(AdminBrandPageCounts counts) => this(counts: counts);

  @override
  AdminBrandPage items(List<AdminBrandListItem> items) => this(items: items);

  @override
  AdminBrandPage nextCursor(String? nextCursor) => this(nextCursor: nextCursor);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminBrandPage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminBrandPage(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminBrandPage call({
    Object? counts = const $CopyWithPlaceholder(),
    Object? items = const $CopyWithPlaceholder(),
    Object? nextCursor = const $CopyWithPlaceholder(),
  }) {
    return AdminBrandPage(
      counts: counts == const $CopyWithPlaceholder()
          ? _value.counts
          // ignore: cast_nullable_to_non_nullable
          : counts as AdminBrandPageCounts,
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<AdminBrandListItem>,
      nextCursor: nextCursor == const $CopyWithPlaceholder()
          ? _value.nextCursor
          // ignore: cast_nullable_to_non_nullable
          : nextCursor as String?,
    );
  }
}

extension $AdminBrandPageCopyWith on AdminBrandPage {
  /// Returns a callable class that can be used as follows: `instanceOfAdminBrandPage.copyWith(...)` or like so:`instanceOfAdminBrandPage.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminBrandPageCWProxy get copyWith => _$AdminBrandPageCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminBrandPage _$AdminBrandPageFromJson(Map<String, dynamic> json) =>
    $checkedCreate('AdminBrandPage', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['counts', 'items', 'nextCursor']);
      final val = AdminBrandPage(
        counts: $checkedConvert(
          'counts',
          (v) => AdminBrandPageCounts.fromJson(v as Map<String, dynamic>),
        ),
        items: $checkedConvert(
          'items',
          (v) => (v as List<dynamic>)
              .map(
                (e) => AdminBrandListItem.fromJson(e as Map<String, dynamic>),
              )
              .toList(),
        ),
        nextCursor: $checkedConvert('nextCursor', (v) => v as String?),
      );
      return val;
    });

Map<String, dynamic> _$AdminBrandPageToJson(AdminBrandPage instance) =>
    <String, dynamic>{
      'counts': instance.counts.toJson(),
      'items': instance.items.map((e) => e.toJson()).toList(),
      'nextCursor': instance.nextCursor,
    };
