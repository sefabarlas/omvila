// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'bank_transaction_import_result_errors_inner.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$BankTransactionImportResultErrorsInnerCWProxy {
  BankTransactionImportResultErrorsInner rowNumber(int rowNumber);

  BankTransactionImportResultErrorsInner column(String column);

  BankTransactionImportResultErrorsInner code(String code);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `BankTransactionImportResultErrorsInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// BankTransactionImportResultErrorsInner(...).copyWith(id: 12, name: "My name")
  /// ````
  BankTransactionImportResultErrorsInner call({
    int rowNumber,
    String column,
    String code,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfBankTransactionImportResultErrorsInner.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfBankTransactionImportResultErrorsInner.copyWith.fieldName(...)`
class _$BankTransactionImportResultErrorsInnerCWProxyImpl
    implements _$BankTransactionImportResultErrorsInnerCWProxy {
  const _$BankTransactionImportResultErrorsInnerCWProxyImpl(this._value);

  final BankTransactionImportResultErrorsInner _value;

  @override
  BankTransactionImportResultErrorsInner rowNumber(int rowNumber) =>
      this(rowNumber: rowNumber);

  @override
  BankTransactionImportResultErrorsInner column(String column) =>
      this(column: column);

  @override
  BankTransactionImportResultErrorsInner code(String code) => this(code: code);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `BankTransactionImportResultErrorsInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// BankTransactionImportResultErrorsInner(...).copyWith(id: 12, name: "My name")
  /// ````
  BankTransactionImportResultErrorsInner call({
    Object? rowNumber = const $CopyWithPlaceholder(),
    Object? column = const $CopyWithPlaceholder(),
    Object? code = const $CopyWithPlaceholder(),
  }) {
    return BankTransactionImportResultErrorsInner(
      rowNumber: rowNumber == const $CopyWithPlaceholder()
          ? _value.rowNumber
          // ignore: cast_nullable_to_non_nullable
          : rowNumber as int,
      column: column == const $CopyWithPlaceholder()
          ? _value.column
          // ignore: cast_nullable_to_non_nullable
          : column as String,
      code: code == const $CopyWithPlaceholder()
          ? _value.code
          // ignore: cast_nullable_to_non_nullable
          : code as String,
    );
  }
}

extension $BankTransactionImportResultErrorsInnerCopyWith
    on BankTransactionImportResultErrorsInner {
  /// Returns a callable class that can be used as follows: `instanceOfBankTransactionImportResultErrorsInner.copyWith(...)` or like so:`instanceOfBankTransactionImportResultErrorsInner.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$BankTransactionImportResultErrorsInnerCWProxy get copyWith =>
      _$BankTransactionImportResultErrorsInnerCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

BankTransactionImportResultErrorsInner
_$BankTransactionImportResultErrorsInnerFromJson(Map<String, dynamic> json) =>
    $checkedCreate('BankTransactionImportResultErrorsInner', json, (
      $checkedConvert,
    ) {
      $checkKeys(json, requiredKeys: const ['rowNumber', 'column', 'code']);
      final val = BankTransactionImportResultErrorsInner(
        rowNumber: $checkedConvert('rowNumber', (v) => (v as num).toInt()),
        column: $checkedConvert('column', (v) => v as String),
        code: $checkedConvert('code', (v) => v as String),
      );
      return val;
    });

Map<String, dynamic> _$BankTransactionImportResultErrorsInnerToJson(
  BankTransactionImportResultErrorsInner instance,
) => <String, dynamic>{
  'rowNumber': instance.rowNumber,
  'column': instance.column,
  'code': instance.code,
};
