// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'audit_log_page.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AuditLogPageCWProxy {
  AuditLogPage items(List<AuditLogEntry> items);

  AuditLogPage nextCursor(String? nextCursor);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AuditLogPage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AuditLogPage(...).copyWith(id: 12, name: "My name")
  /// ````
  AuditLogPage call({List<AuditLogEntry> items, String? nextCursor});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAuditLogPage.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAuditLogPage.copyWith.fieldName(...)`
class _$AuditLogPageCWProxyImpl implements _$AuditLogPageCWProxy {
  const _$AuditLogPageCWProxyImpl(this._value);

  final AuditLogPage _value;

  @override
  AuditLogPage items(List<AuditLogEntry> items) => this(items: items);

  @override
  AuditLogPage nextCursor(String? nextCursor) => this(nextCursor: nextCursor);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AuditLogPage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AuditLogPage(...).copyWith(id: 12, name: "My name")
  /// ````
  AuditLogPage call({
    Object? items = const $CopyWithPlaceholder(),
    Object? nextCursor = const $CopyWithPlaceholder(),
  }) {
    return AuditLogPage(
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<AuditLogEntry>,
      nextCursor: nextCursor == const $CopyWithPlaceholder()
          ? _value.nextCursor
          // ignore: cast_nullable_to_non_nullable
          : nextCursor as String?,
    );
  }
}

extension $AuditLogPageCopyWith on AuditLogPage {
  /// Returns a callable class that can be used as follows: `instanceOfAuditLogPage.copyWith(...)` or like so:`instanceOfAuditLogPage.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AuditLogPageCWProxy get copyWith => _$AuditLogPageCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AuditLogPage _$AuditLogPageFromJson(Map<String, dynamic> json) =>
    $checkedCreate('AuditLogPage', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['items', 'nextCursor']);
      final val = AuditLogPage(
        items: $checkedConvert(
          'items',
          (v) => (v as List<dynamic>)
              .map((e) => AuditLogEntry.fromJson(e as Map<String, dynamic>))
              .toList(),
        ),
        nextCursor: $checkedConvert('nextCursor', (v) => v as String?),
      );
      return val;
    });

Map<String, dynamic> _$AuditLogPageToJson(AuditLogPage instance) =>
    <String, dynamic>{
      'items': instance.items.map((e) => e.toJson()).toList(),
      'nextCursor': instance.nextCursor,
    };
