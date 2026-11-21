// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'customer_import_result.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CustomerImportResultCWProxy {
  CustomerImportResult importId(String importId);

  CustomerImportResult status(CustomerImportResultStatusEnum status);

  CustomerImportResult created(int created);

  CustomerImportResult merged(int merged);

  CustomerImportResult skipped(int skipped);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerImportResult(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerImportResult(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerImportResult call({
    String importId,
    CustomerImportResultStatusEnum status,
    int created,
    int merged,
    int skipped,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCustomerImportResult.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCustomerImportResult.copyWith.fieldName(...)`
class _$CustomerImportResultCWProxyImpl
    implements _$CustomerImportResultCWProxy {
  const _$CustomerImportResultCWProxyImpl(this._value);

  final CustomerImportResult _value;

  @override
  CustomerImportResult importId(String importId) => this(importId: importId);

  @override
  CustomerImportResult status(CustomerImportResultStatusEnum status) =>
      this(status: status);

  @override
  CustomerImportResult created(int created) => this(created: created);

  @override
  CustomerImportResult merged(int merged) => this(merged: merged);

  @override
  CustomerImportResult skipped(int skipped) => this(skipped: skipped);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerImportResult(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerImportResult(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerImportResult call({
    Object? importId = const $CopyWithPlaceholder(),
    Object? status = const $CopyWithPlaceholder(),
    Object? created = const $CopyWithPlaceholder(),
    Object? merged = const $CopyWithPlaceholder(),
    Object? skipped = const $CopyWithPlaceholder(),
  }) {
    return CustomerImportResult(
      importId: importId == const $CopyWithPlaceholder()
          ? _value.importId
          // ignore: cast_nullable_to_non_nullable
          : importId as String,
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as CustomerImportResultStatusEnum,
      created: created == const $CopyWithPlaceholder()
          ? _value.created
          // ignore: cast_nullable_to_non_nullable
          : created as int,
      merged: merged == const $CopyWithPlaceholder()
          ? _value.merged
          // ignore: cast_nullable_to_non_nullable
          : merged as int,
      skipped: skipped == const $CopyWithPlaceholder()
          ? _value.skipped
          // ignore: cast_nullable_to_non_nullable
          : skipped as int,
    );
  }
}

extension $CustomerImportResultCopyWith on CustomerImportResult {
  /// Returns a callable class that can be used as follows: `instanceOfCustomerImportResult.copyWith(...)` or like so:`instanceOfCustomerImportResult.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CustomerImportResultCWProxy get copyWith =>
      _$CustomerImportResultCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CustomerImportResult _$CustomerImportResultFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('CustomerImportResult', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const ['importId', 'status', 'created', 'merged', 'skipped'],
  );
  final val = CustomerImportResult(
    importId: $checkedConvert('importId', (v) => v as String),
    status: $checkedConvert(
      'status',
      (v) => $enumDecode(
        _$CustomerImportResultStatusEnumEnumMap,
        v,
        unknownValue: CustomerImportResultStatusEnum.unknownDefaultOpenApi,
      ),
    ),
    created: $checkedConvert('created', (v) => (v as num).toInt()),
    merged: $checkedConvert('merged', (v) => (v as num).toInt()),
    skipped: $checkedConvert('skipped', (v) => (v as num).toInt()),
  );
  return val;
});

Map<String, dynamic> _$CustomerImportResultToJson(
  CustomerImportResult instance,
) => <String, dynamic>{
  'importId': instance.importId,
  'status': _$CustomerImportResultStatusEnumEnumMap[instance.status]!,
  'created': instance.created,
  'merged': instance.merged,
  'skipped': instance.skipped,
};

const _$CustomerImportResultStatusEnumEnumMap = {
  CustomerImportResultStatusEnum.committed: 'Committed',
  CustomerImportResultStatusEnum.failed: 'Failed',
  CustomerImportResultStatusEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
