// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'customer_deletion_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CustomerDeletionRequestCWProxy {
  CustomerDeletionRequest confirmFullName(String confirmFullName);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerDeletionRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerDeletionRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerDeletionRequest call({String confirmFullName});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCustomerDeletionRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCustomerDeletionRequest.copyWith.fieldName(...)`
class _$CustomerDeletionRequestCWProxyImpl
    implements _$CustomerDeletionRequestCWProxy {
  const _$CustomerDeletionRequestCWProxyImpl(this._value);

  final CustomerDeletionRequest _value;

  @override
  CustomerDeletionRequest confirmFullName(String confirmFullName) =>
      this(confirmFullName: confirmFullName);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerDeletionRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerDeletionRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerDeletionRequest call({
    Object? confirmFullName = const $CopyWithPlaceholder(),
  }) {
    return CustomerDeletionRequest(
      confirmFullName: confirmFullName == const $CopyWithPlaceholder()
          ? _value.confirmFullName
          // ignore: cast_nullable_to_non_nullable
          : confirmFullName as String,
    );
  }
}

extension $CustomerDeletionRequestCopyWith on CustomerDeletionRequest {
  /// Returns a callable class that can be used as follows: `instanceOfCustomerDeletionRequest.copyWith(...)` or like so:`instanceOfCustomerDeletionRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CustomerDeletionRequestCWProxy get copyWith =>
      _$CustomerDeletionRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CustomerDeletionRequest _$CustomerDeletionRequestFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('CustomerDeletionRequest', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['confirmFullName']);
  final val = CustomerDeletionRequest(
    confirmFullName: $checkedConvert('confirmFullName', (v) => v as String),
  );
  return val;
});

Map<String, dynamic> _$CustomerDeletionRequestToJson(
  CustomerDeletionRequest instance,
) => <String, dynamic>{'confirmFullName': instance.confirmFullName};
