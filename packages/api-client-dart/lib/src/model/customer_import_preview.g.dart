// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'customer_import_preview.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CustomerImportPreviewCWProxy {
  CustomerImportPreview importId(String importId);

  CustomerImportPreview fileName(String fileName);

  CustomerImportPreview totalRows(int totalRows);

  CustomerImportPreview toCreate(int toCreate);

  CustomerImportPreview toMerge(int toMerge);

  CustomerImportPreview invalidRows(int invalidRows);

  CustomerImportPreview mappedColumns(
    List<CustomerImportPreviewMappedColumnsEnum> mappedColumns,
  );

  CustomerImportPreview errors(List<CustomerImportRowError> errors);

  CustomerImportPreview status(CustomerImportPreviewStatusEnum status);

  CustomerImportPreview expiresUtc(DateTime? expiresUtc);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerImportPreview(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerImportPreview(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerImportPreview call({
    String importId,
    String fileName,
    int totalRows,
    int toCreate,
    int toMerge,
    int invalidRows,
    List<CustomerImportPreviewMappedColumnsEnum> mappedColumns,
    List<CustomerImportRowError> errors,
    CustomerImportPreviewStatusEnum status,
    DateTime? expiresUtc,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCustomerImportPreview.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCustomerImportPreview.copyWith.fieldName(...)`
class _$CustomerImportPreviewCWProxyImpl
    implements _$CustomerImportPreviewCWProxy {
  const _$CustomerImportPreviewCWProxyImpl(this._value);

  final CustomerImportPreview _value;

  @override
  CustomerImportPreview importId(String importId) => this(importId: importId);

  @override
  CustomerImportPreview fileName(String fileName) => this(fileName: fileName);

  @override
  CustomerImportPreview totalRows(int totalRows) => this(totalRows: totalRows);

  @override
  CustomerImportPreview toCreate(int toCreate) => this(toCreate: toCreate);

  @override
  CustomerImportPreview toMerge(int toMerge) => this(toMerge: toMerge);

  @override
  CustomerImportPreview invalidRows(int invalidRows) =>
      this(invalidRows: invalidRows);

  @override
  CustomerImportPreview mappedColumns(
    List<CustomerImportPreviewMappedColumnsEnum> mappedColumns,
  ) => this(mappedColumns: mappedColumns);

  @override
  CustomerImportPreview errors(List<CustomerImportRowError> errors) =>
      this(errors: errors);

  @override
  CustomerImportPreview status(CustomerImportPreviewStatusEnum status) =>
      this(status: status);

  @override
  CustomerImportPreview expiresUtc(DateTime? expiresUtc) =>
      this(expiresUtc: expiresUtc);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerImportPreview(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerImportPreview(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerImportPreview call({
    Object? importId = const $CopyWithPlaceholder(),
    Object? fileName = const $CopyWithPlaceholder(),
    Object? totalRows = const $CopyWithPlaceholder(),
    Object? toCreate = const $CopyWithPlaceholder(),
    Object? toMerge = const $CopyWithPlaceholder(),
    Object? invalidRows = const $CopyWithPlaceholder(),
    Object? mappedColumns = const $CopyWithPlaceholder(),
    Object? errors = const $CopyWithPlaceholder(),
    Object? status = const $CopyWithPlaceholder(),
    Object? expiresUtc = const $CopyWithPlaceholder(),
  }) {
    return CustomerImportPreview(
      importId: importId == const $CopyWithPlaceholder()
          ? _value.importId
          // ignore: cast_nullable_to_non_nullable
          : importId as String,
      fileName: fileName == const $CopyWithPlaceholder()
          ? _value.fileName
          // ignore: cast_nullable_to_non_nullable
          : fileName as String,
      totalRows: totalRows == const $CopyWithPlaceholder()
          ? _value.totalRows
          // ignore: cast_nullable_to_non_nullable
          : totalRows as int,
      toCreate: toCreate == const $CopyWithPlaceholder()
          ? _value.toCreate
          // ignore: cast_nullable_to_non_nullable
          : toCreate as int,
      toMerge: toMerge == const $CopyWithPlaceholder()
          ? _value.toMerge
          // ignore: cast_nullable_to_non_nullable
          : toMerge as int,
      invalidRows: invalidRows == const $CopyWithPlaceholder()
          ? _value.invalidRows
          // ignore: cast_nullable_to_non_nullable
          : invalidRows as int,
      mappedColumns: mappedColumns == const $CopyWithPlaceholder()
          ? _value.mappedColumns
          // ignore: cast_nullable_to_non_nullable
          : mappedColumns as List<CustomerImportPreviewMappedColumnsEnum>,
      errors: errors == const $CopyWithPlaceholder()
          ? _value.errors
          // ignore: cast_nullable_to_non_nullable
          : errors as List<CustomerImportRowError>,
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as CustomerImportPreviewStatusEnum,
      expiresUtc: expiresUtc == const $CopyWithPlaceholder()
          ? _value.expiresUtc
          // ignore: cast_nullable_to_non_nullable
          : expiresUtc as DateTime?,
    );
  }
}

extension $CustomerImportPreviewCopyWith on CustomerImportPreview {
  /// Returns a callable class that can be used as follows: `instanceOfCustomerImportPreview.copyWith(...)` or like so:`instanceOfCustomerImportPreview.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CustomerImportPreviewCWProxy get copyWith =>
      _$CustomerImportPreviewCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CustomerImportPreview _$CustomerImportPreviewFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('CustomerImportPreview', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'importId',
      'fileName',
      'totalRows',
      'toCreate',
      'toMerge',
      'invalidRows',
      'mappedColumns',
      'errors',
      'status',
    ],
  );
  final val = CustomerImportPreview(
    importId: $checkedConvert('importId', (v) => v as String),
    fileName: $checkedConvert('fileName', (v) => v as String),
    totalRows: $checkedConvert('totalRows', (v) => (v as num).toInt()),
    toCreate: $checkedConvert('toCreate', (v) => (v as num).toInt()),
    toMerge: $checkedConvert('toMerge', (v) => (v as num).toInt()),
    invalidRows: $checkedConvert('invalidRows', (v) => (v as num).toInt()),
    mappedColumns: $checkedConvert(
      'mappedColumns',
      (v) => (v as List<dynamic>)
          .map(
            (e) => $enumDecode(
              _$CustomerImportPreviewMappedColumnsEnumEnumMap,
              e,
              unknownValue:
                  CustomerImportPreviewMappedColumnsEnum.unknownDefaultOpenApi,
            ),
          )
          .toList(),
    ),
    errors: $checkedConvert(
      'errors',
      (v) => (v as List<dynamic>)
          .map(
            (e) => CustomerImportRowError.fromJson(e as Map<String, dynamic>),
          )
          .toList(),
    ),
    status: $checkedConvert(
      'status',
      (v) => $enumDecode(
        _$CustomerImportPreviewStatusEnumEnumMap,
        v,
        unknownValue: CustomerImportPreviewStatusEnum.unknownDefaultOpenApi,
      ),
    ),
    expiresUtc: $checkedConvert(
      'expiresUtc',
      (v) => v == null ? null : DateTime.parse(v as String),
    ),
  );
  return val;
});

Map<String, dynamic> _$CustomerImportPreviewToJson(
  CustomerImportPreview instance,
) => <String, dynamic>{
  'importId': instance.importId,
  'fileName': instance.fileName,
  'totalRows': instance.totalRows,
  'toCreate': instance.toCreate,
  'toMerge': instance.toMerge,
  'invalidRows': instance.invalidRows,
  'mappedColumns': instance.mappedColumns
      .map((e) => _$CustomerImportPreviewMappedColumnsEnumEnumMap[e]!)
      .toList(),
  'errors': instance.errors.map((e) => e.toJson()).toList(),
  'status': _$CustomerImportPreviewStatusEnumEnumMap[instance.status]!,
  'expiresUtc': ?instance.expiresUtc?.toIso8601String(),
};

const _$CustomerImportPreviewMappedColumnsEnumEnumMap = {
  CustomerImportPreviewMappedColumnsEnum.fullName: 'FullName',
  CustomerImportPreviewMappedColumnsEnum.phone: 'Phone',
  CustomerImportPreviewMappedColumnsEnum.notes: 'Notes',
  CustomerImportPreviewMappedColumnsEnum.birthYear: 'BirthYear',
  CustomerImportPreviewMappedColumnsEnum.email: 'Email',
  CustomerImportPreviewMappedColumnsEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};

const _$CustomerImportPreviewStatusEnumEnumMap = {
  CustomerImportPreviewStatusEnum.previewed: 'Previewed',
  CustomerImportPreviewStatusEnum.committed: 'Committed',
  CustomerImportPreviewStatusEnum.failed: 'Failed',
  CustomerImportPreviewStatusEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
