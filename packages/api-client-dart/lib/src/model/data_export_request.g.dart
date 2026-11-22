// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'data_export_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$DataExportRequestCWProxy {
  DataExportRequest email(String? email);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `DataExportRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// DataExportRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  DataExportRequest call({String? email});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfDataExportRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfDataExportRequest.copyWith.fieldName(...)`
class _$DataExportRequestCWProxyImpl implements _$DataExportRequestCWProxy {
  const _$DataExportRequestCWProxyImpl(this._value);

  final DataExportRequest _value;

  @override
  DataExportRequest email(String? email) => this(email: email);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `DataExportRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// DataExportRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  DataExportRequest call({Object? email = const $CopyWithPlaceholder()}) {
    return DataExportRequest(
      email: email == const $CopyWithPlaceholder()
          ? _value.email
          // ignore: cast_nullable_to_non_nullable
          : email as String?,
    );
  }
}

extension $DataExportRequestCopyWith on DataExportRequest {
  /// Returns a callable class that can be used as follows: `instanceOfDataExportRequest.copyWith(...)` or like so:`instanceOfDataExportRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$DataExportRequestCWProxy get copyWith =>
      _$DataExportRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

DataExportRequest _$DataExportRequestFromJson(Map<String, dynamic> json) =>
    $checkedCreate('DataExportRequest', json, ($checkedConvert) {
      final val = DataExportRequest(
        email: $checkedConvert('email', (v) => v as String?),
      );
      return val;
    });

Map<String, dynamic> _$DataExportRequestToJson(DataExportRequest instance) =>
    <String, dynamic>{'email': ?instance.email};
