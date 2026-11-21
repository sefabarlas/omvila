// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'data_deletion_request_page.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$DataDeletionRequestPageCWProxy {
  DataDeletionRequestPage items(List<DataDeletionRequestView> items);

  DataDeletionRequestPage nextCursor(String? nextCursor);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `DataDeletionRequestPage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// DataDeletionRequestPage(...).copyWith(id: 12, name: "My name")
  /// ````
  DataDeletionRequestPage call({
    List<DataDeletionRequestView> items,
    String? nextCursor,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfDataDeletionRequestPage.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfDataDeletionRequestPage.copyWith.fieldName(...)`
class _$DataDeletionRequestPageCWProxyImpl
    implements _$DataDeletionRequestPageCWProxy {
  const _$DataDeletionRequestPageCWProxyImpl(this._value);

  final DataDeletionRequestPage _value;

  @override
  DataDeletionRequestPage items(List<DataDeletionRequestView> items) =>
      this(items: items);

  @override
  DataDeletionRequestPage nextCursor(String? nextCursor) =>
      this(nextCursor: nextCursor);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `DataDeletionRequestPage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// DataDeletionRequestPage(...).copyWith(id: 12, name: "My name")
  /// ````
  DataDeletionRequestPage call({
    Object? items = const $CopyWithPlaceholder(),
    Object? nextCursor = const $CopyWithPlaceholder(),
  }) {
    return DataDeletionRequestPage(
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<DataDeletionRequestView>,
      nextCursor: nextCursor == const $CopyWithPlaceholder()
          ? _value.nextCursor
          // ignore: cast_nullable_to_non_nullable
          : nextCursor as String?,
    );
  }
}

extension $DataDeletionRequestPageCopyWith on DataDeletionRequestPage {
  /// Returns a callable class that can be used as follows: `instanceOfDataDeletionRequestPage.copyWith(...)` or like so:`instanceOfDataDeletionRequestPage.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$DataDeletionRequestPageCWProxy get copyWith =>
      _$DataDeletionRequestPageCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

DataDeletionRequestPage _$DataDeletionRequestPageFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('DataDeletionRequestPage', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['items', 'nextCursor']);
  final val = DataDeletionRequestPage(
    items: $checkedConvert(
      'items',
      (v) => (v as List<dynamic>)
          .map(
            (e) => DataDeletionRequestView.fromJson(e as Map<String, dynamic>),
          )
          .toList(),
    ),
    nextCursor: $checkedConvert('nextCursor', (v) => v as String?),
  );
  return val;
});

Map<String, dynamic> _$DataDeletionRequestPageToJson(
  DataDeletionRequestPage instance,
) => <String, dynamic>{
  'items': instance.items.map((e) => e.toJson()).toList(),
  'nextCursor': instance.nextCursor,
};
