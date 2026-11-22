// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'bank_transaction_import_result.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$BankTransactionImportResultCWProxy {
  BankTransactionImportResult dryRun(bool dryRun);

  BankTransactionImportResult fileName(String fileName);

  BankTransactionImportResult totalRows(int totalRows);

  BankTransactionImportResult imported(int imported);

  BankTransactionImportResult duplicates(int duplicates);

  BankTransactionImportResult skippedOutgoing(int skippedOutgoing);

  BankTransactionImportResult invalidRows(int invalidRows);

  BankTransactionImportResult errors(
    List<BankTransactionImportResultErrorsInner> errors,
  );

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `BankTransactionImportResult(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// BankTransactionImportResult(...).copyWith(id: 12, name: "My name")
  /// ````
  BankTransactionImportResult call({
    bool dryRun,
    String fileName,
    int totalRows,
    int imported,
    int duplicates,
    int skippedOutgoing,
    int invalidRows,
    List<BankTransactionImportResultErrorsInner> errors,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfBankTransactionImportResult.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfBankTransactionImportResult.copyWith.fieldName(...)`
class _$BankTransactionImportResultCWProxyImpl
    implements _$BankTransactionImportResultCWProxy {
  const _$BankTransactionImportResultCWProxyImpl(this._value);

  final BankTransactionImportResult _value;

  @override
  BankTransactionImportResult dryRun(bool dryRun) => this(dryRun: dryRun);

  @override
  BankTransactionImportResult fileName(String fileName) =>
      this(fileName: fileName);

  @override
  BankTransactionImportResult totalRows(int totalRows) =>
      this(totalRows: totalRows);

  @override
  BankTransactionImportResult imported(int imported) =>
      this(imported: imported);

  @override
  BankTransactionImportResult duplicates(int duplicates) =>
      this(duplicates: duplicates);

  @override
  BankTransactionImportResult skippedOutgoing(int skippedOutgoing) =>
      this(skippedOutgoing: skippedOutgoing);

  @override
  BankTransactionImportResult invalidRows(int invalidRows) =>
      this(invalidRows: invalidRows);

  @override
  BankTransactionImportResult errors(
    List<BankTransactionImportResultErrorsInner> errors,
  ) => this(errors: errors);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `BankTransactionImportResult(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// BankTransactionImportResult(...).copyWith(id: 12, name: "My name")
  /// ````
  BankTransactionImportResult call({
    Object? dryRun = const $CopyWithPlaceholder(),
    Object? fileName = const $CopyWithPlaceholder(),
    Object? totalRows = const $CopyWithPlaceholder(),
    Object? imported = const $CopyWithPlaceholder(),
    Object? duplicates = const $CopyWithPlaceholder(),
    Object? skippedOutgoing = const $CopyWithPlaceholder(),
    Object? invalidRows = const $CopyWithPlaceholder(),
    Object? errors = const $CopyWithPlaceholder(),
  }) {
    return BankTransactionImportResult(
      dryRun: dryRun == const $CopyWithPlaceholder()
          ? _value.dryRun
          // ignore: cast_nullable_to_non_nullable
          : dryRun as bool,
      fileName: fileName == const $CopyWithPlaceholder()
          ? _value.fileName
          // ignore: cast_nullable_to_non_nullable
          : fileName as String,
      totalRows: totalRows == const $CopyWithPlaceholder()
          ? _value.totalRows
          // ignore: cast_nullable_to_non_nullable
          : totalRows as int,
      imported: imported == const $CopyWithPlaceholder()
          ? _value.imported
          // ignore: cast_nullable_to_non_nullable
          : imported as int,
      duplicates: duplicates == const $CopyWithPlaceholder()
          ? _value.duplicates
          // ignore: cast_nullable_to_non_nullable
          : duplicates as int,
      skippedOutgoing: skippedOutgoing == const $CopyWithPlaceholder()
          ? _value.skippedOutgoing
          // ignore: cast_nullable_to_non_nullable
          : skippedOutgoing as int,
      invalidRows: invalidRows == const $CopyWithPlaceholder()
          ? _value.invalidRows
          // ignore: cast_nullable_to_non_nullable
          : invalidRows as int,
      errors: errors == const $CopyWithPlaceholder()
          ? _value.errors
          // ignore: cast_nullable_to_non_nullable
          : errors as List<BankTransactionImportResultErrorsInner>,
    );
  }
}

extension $BankTransactionImportResultCopyWith on BankTransactionImportResult {
  /// Returns a callable class that can be used as follows: `instanceOfBankTransactionImportResult.copyWith(...)` or like so:`instanceOfBankTransactionImportResult.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$BankTransactionImportResultCWProxy get copyWith =>
      _$BankTransactionImportResultCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

BankTransactionImportResult _$BankTransactionImportResultFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('BankTransactionImportResult', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'dryRun',
      'fileName',
      'totalRows',
      'imported',
      'duplicates',
      'skippedOutgoing',
      'invalidRows',
      'errors',
    ],
  );
  final val = BankTransactionImportResult(
    dryRun: $checkedConvert('dryRun', (v) => v as bool),
    fileName: $checkedConvert('fileName', (v) => v as String),
    totalRows: $checkedConvert('totalRows', (v) => (v as num).toInt()),
    imported: $checkedConvert('imported', (v) => (v as num).toInt()),
    duplicates: $checkedConvert('duplicates', (v) => (v as num).toInt()),
    skippedOutgoing: $checkedConvert(
      'skippedOutgoing',
      (v) => (v as num).toInt(),
    ),
    invalidRows: $checkedConvert('invalidRows', (v) => (v as num).toInt()),
    errors: $checkedConvert(
      'errors',
      (v) => (v as List<dynamic>)
          .map(
            (e) => BankTransactionImportResultErrorsInner.fromJson(
              e as Map<String, dynamic>,
            ),
          )
          .toList(),
    ),
  );
  return val;
});

Map<String, dynamic> _$BankTransactionImportResultToJson(
  BankTransactionImportResult instance,
) => <String, dynamic>{
  'dryRun': instance.dryRun,
  'fileName': instance.fileName,
  'totalRows': instance.totalRows,
  'imported': instance.imported,
  'duplicates': instance.duplicates,
  'skippedOutgoing': instance.skippedOutgoing,
  'invalidRows': instance.invalidRows,
  'errors': instance.errors.map((e) => e.toJson()).toList(),
};
