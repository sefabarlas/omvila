// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'data_export_request_result.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$DataExportRequestResultCWProxy {
  DataExportRequestResult exportId(String exportId);

  DataExportRequestResult status(DataExportRequestResultStatusEnum status);

  DataExportRequestResult requestedUtc(DateTime requestedUtc);

  DataExportRequestResult emailMasked(String emailMasked);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `DataExportRequestResult(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// DataExportRequestResult(...).copyWith(id: 12, name: "My name")
  /// ````
  DataExportRequestResult call({
    String exportId,
    DataExportRequestResultStatusEnum status,
    DateTime requestedUtc,
    String emailMasked,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfDataExportRequestResult.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfDataExportRequestResult.copyWith.fieldName(...)`
class _$DataExportRequestResultCWProxyImpl
    implements _$DataExportRequestResultCWProxy {
  const _$DataExportRequestResultCWProxyImpl(this._value);

  final DataExportRequestResult _value;

  @override
  DataExportRequestResult exportId(String exportId) => this(exportId: exportId);

  @override
  DataExportRequestResult status(DataExportRequestResultStatusEnum status) =>
      this(status: status);

  @override
  DataExportRequestResult requestedUtc(DateTime requestedUtc) =>
      this(requestedUtc: requestedUtc);

  @override
  DataExportRequestResult emailMasked(String emailMasked) =>
      this(emailMasked: emailMasked);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `DataExportRequestResult(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// DataExportRequestResult(...).copyWith(id: 12, name: "My name")
  /// ````
  DataExportRequestResult call({
    Object? exportId = const $CopyWithPlaceholder(),
    Object? status = const $CopyWithPlaceholder(),
    Object? requestedUtc = const $CopyWithPlaceholder(),
    Object? emailMasked = const $CopyWithPlaceholder(),
  }) {
    return DataExportRequestResult(
      exportId: exportId == const $CopyWithPlaceholder()
          ? _value.exportId
          // ignore: cast_nullable_to_non_nullable
          : exportId as String,
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as DataExportRequestResultStatusEnum,
      requestedUtc: requestedUtc == const $CopyWithPlaceholder()
          ? _value.requestedUtc
          // ignore: cast_nullable_to_non_nullable
          : requestedUtc as DateTime,
      emailMasked: emailMasked == const $CopyWithPlaceholder()
          ? _value.emailMasked
          // ignore: cast_nullable_to_non_nullable
          : emailMasked as String,
    );
  }
}

extension $DataExportRequestResultCopyWith on DataExportRequestResult {
  /// Returns a callable class that can be used as follows: `instanceOfDataExportRequestResult.copyWith(...)` or like so:`instanceOfDataExportRequestResult.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$DataExportRequestResultCWProxy get copyWith =>
      _$DataExportRequestResultCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

DataExportRequestResult _$DataExportRequestResultFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('DataExportRequestResult', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const ['exportId', 'status', 'requestedUtc', 'emailMasked'],
  );
  final val = DataExportRequestResult(
    exportId: $checkedConvert('exportId', (v) => v as String),
    status: $checkedConvert(
      'status',
      (v) => $enumDecode(
        _$DataExportRequestResultStatusEnumEnumMap,
        v,
        unknownValue: DataExportRequestResultStatusEnum.unknownDefaultOpenApi,
      ),
    ),
    requestedUtc: $checkedConvert(
      'requestedUtc',
      (v) => DateTime.parse(v as String),
    ),
    emailMasked: $checkedConvert('emailMasked', (v) => v as String),
  );
  return val;
});

Map<String, dynamic> _$DataExportRequestResultToJson(
  DataExportRequestResult instance,
) => <String, dynamic>{
  'exportId': instance.exportId,
  'status': _$DataExportRequestResultStatusEnumEnumMap[instance.status]!,
  'requestedUtc': instance.requestedUtc.toIso8601String(),
  'emailMasked': instance.emailMasked,
};

const _$DataExportRequestResultStatusEnumEnumMap = {
  DataExportRequestResultStatusEnum.queued: 'Queued',
  DataExportRequestResultStatusEnum.processing: 'Processing',
  DataExportRequestResultStatusEnum.sent: 'Sent',
  DataExportRequestResultStatusEnum.failed: 'Failed',
  DataExportRequestResultStatusEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
