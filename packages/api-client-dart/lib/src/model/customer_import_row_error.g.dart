// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'customer_import_row_error.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CustomerImportRowErrorCWProxy {
  CustomerImportRowError rowNumber(int rowNumber);

  CustomerImportRowError column(String? column);

  CustomerImportRowError code(CustomerImportRowErrorCodeEnum code);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerImportRowError(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerImportRowError(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerImportRowError call({
    int rowNumber,
    String? column,
    CustomerImportRowErrorCodeEnum code,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCustomerImportRowError.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCustomerImportRowError.copyWith.fieldName(...)`
class _$CustomerImportRowErrorCWProxyImpl
    implements _$CustomerImportRowErrorCWProxy {
  const _$CustomerImportRowErrorCWProxyImpl(this._value);

  final CustomerImportRowError _value;

  @override
  CustomerImportRowError rowNumber(int rowNumber) => this(rowNumber: rowNumber);

  @override
  CustomerImportRowError column(String? column) => this(column: column);

  @override
  CustomerImportRowError code(CustomerImportRowErrorCodeEnum code) =>
      this(code: code);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerImportRowError(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerImportRowError(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerImportRowError call({
    Object? rowNumber = const $CopyWithPlaceholder(),
    Object? column = const $CopyWithPlaceholder(),
    Object? code = const $CopyWithPlaceholder(),
  }) {
    return CustomerImportRowError(
      rowNumber: rowNumber == const $CopyWithPlaceholder()
          ? _value.rowNumber
          // ignore: cast_nullable_to_non_nullable
          : rowNumber as int,
      column: column == const $CopyWithPlaceholder()
          ? _value.column
          // ignore: cast_nullable_to_non_nullable
          : column as String?,
      code: code == const $CopyWithPlaceholder()
          ? _value.code
          // ignore: cast_nullable_to_non_nullable
          : code as CustomerImportRowErrorCodeEnum,
    );
  }
}

extension $CustomerImportRowErrorCopyWith on CustomerImportRowError {
  /// Returns a callable class that can be used as follows: `instanceOfCustomerImportRowError.copyWith(...)` or like so:`instanceOfCustomerImportRowError.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CustomerImportRowErrorCWProxy get copyWith =>
      _$CustomerImportRowErrorCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CustomerImportRowError _$CustomerImportRowErrorFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('CustomerImportRowError', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['rowNumber', 'code']);
  final val = CustomerImportRowError(
    rowNumber: $checkedConvert('rowNumber', (v) => (v as num).toInt()),
    column: $checkedConvert('column', (v) => v as String?),
    code: $checkedConvert(
      'code',
      (v) => $enumDecode(
        _$CustomerImportRowErrorCodeEnumEnumMap,
        v,
        unknownValue: CustomerImportRowErrorCodeEnum.unknownDefaultOpenApi,
      ),
    ),
  );
  return val;
});

Map<String, dynamic> _$CustomerImportRowErrorToJson(
  CustomerImportRowError instance,
) => <String, dynamic>{
  'rowNumber': instance.rowNumber,
  'column': ?instance.column,
  'code': _$CustomerImportRowErrorCodeEnumEnumMap[instance.code]!,
};

const _$CustomerImportRowErrorCodeEnumEnumMap = {
  CustomerImportRowErrorCodeEnum.invalidPhone: 'invalid_phone',
  CustomerImportRowErrorCodeEnum.missingName: 'missing_name',
  CustomerImportRowErrorCodeEnum.duplicateInFile: 'duplicate_in_file',
  CustomerImportRowErrorCodeEnum.tooLong: 'too_long',
  CustomerImportRowErrorCodeEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
