// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'customer_import_commit_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CustomerImportCommitRequestCWProxy {
  CustomerImportCommitRequest sendInfoSms(bool? sendInfoSms);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerImportCommitRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerImportCommitRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerImportCommitRequest call({bool? sendInfoSms});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCustomerImportCommitRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCustomerImportCommitRequest.copyWith.fieldName(...)`
class _$CustomerImportCommitRequestCWProxyImpl
    implements _$CustomerImportCommitRequestCWProxy {
  const _$CustomerImportCommitRequestCWProxyImpl(this._value);

  final CustomerImportCommitRequest _value;

  @override
  CustomerImportCommitRequest sendInfoSms(bool? sendInfoSms) =>
      this(sendInfoSms: sendInfoSms);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerImportCommitRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerImportCommitRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerImportCommitRequest call({
    Object? sendInfoSms = const $CopyWithPlaceholder(),
  }) {
    return CustomerImportCommitRequest(
      sendInfoSms: sendInfoSms == const $CopyWithPlaceholder()
          ? _value.sendInfoSms
          // ignore: cast_nullable_to_non_nullable
          : sendInfoSms as bool?,
    );
  }
}

extension $CustomerImportCommitRequestCopyWith on CustomerImportCommitRequest {
  /// Returns a callable class that can be used as follows: `instanceOfCustomerImportCommitRequest.copyWith(...)` or like so:`instanceOfCustomerImportCommitRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CustomerImportCommitRequestCWProxy get copyWith =>
      _$CustomerImportCommitRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CustomerImportCommitRequest _$CustomerImportCommitRequestFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('CustomerImportCommitRequest', json, ($checkedConvert) {
  final val = CustomerImportCommitRequest(
    sendInfoSms: $checkedConvert('sendInfoSms', (v) => v as bool? ?? false),
  );
  return val;
});

Map<String, dynamic> _$CustomerImportCommitRequestToJson(
  CustomerImportCommitRequest instance,
) => <String, dynamic>{'sendInfoSms': ?instance.sendInfoSms};
