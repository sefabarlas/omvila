// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'package_redemption_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PackageRedemptionRequestCWProxy {
  PackageRedemptionRequest appointmentItemId(String appointmentItemId);

  PackageRedemptionRequest customerPackageItemId(String customerPackageItemId);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PackageRedemptionRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PackageRedemptionRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  PackageRedemptionRequest call({
    String appointmentItemId,
    String customerPackageItemId,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPackageRedemptionRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPackageRedemptionRequest.copyWith.fieldName(...)`
class _$PackageRedemptionRequestCWProxyImpl
    implements _$PackageRedemptionRequestCWProxy {
  const _$PackageRedemptionRequestCWProxyImpl(this._value);

  final PackageRedemptionRequest _value;

  @override
  PackageRedemptionRequest appointmentItemId(String appointmentItemId) =>
      this(appointmentItemId: appointmentItemId);

  @override
  PackageRedemptionRequest customerPackageItemId(
    String customerPackageItemId,
  ) => this(customerPackageItemId: customerPackageItemId);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PackageRedemptionRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PackageRedemptionRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  PackageRedemptionRequest call({
    Object? appointmentItemId = const $CopyWithPlaceholder(),
    Object? customerPackageItemId = const $CopyWithPlaceholder(),
  }) {
    return PackageRedemptionRequest(
      appointmentItemId: appointmentItemId == const $CopyWithPlaceholder()
          ? _value.appointmentItemId
          // ignore: cast_nullable_to_non_nullable
          : appointmentItemId as String,
      customerPackageItemId:
          customerPackageItemId == const $CopyWithPlaceholder()
          ? _value.customerPackageItemId
          // ignore: cast_nullable_to_non_nullable
          : customerPackageItemId as String,
    );
  }
}

extension $PackageRedemptionRequestCopyWith on PackageRedemptionRequest {
  /// Returns a callable class that can be used as follows: `instanceOfPackageRedemptionRequest.copyWith(...)` or like so:`instanceOfPackageRedemptionRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PackageRedemptionRequestCWProxy get copyWith =>
      _$PackageRedemptionRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PackageRedemptionRequest _$PackageRedemptionRequestFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('PackageRedemptionRequest', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const ['appointmentItemId', 'customerPackageItemId'],
  );
  final val = PackageRedemptionRequest(
    appointmentItemId: $checkedConvert('appointmentItemId', (v) => v as String),
    customerPackageItemId: $checkedConvert(
      'customerPackageItemId',
      (v) => v as String,
    ),
  );
  return val;
});

Map<String, dynamic> _$PackageRedemptionRequestToJson(
  PackageRedemptionRequest instance,
) => <String, dynamic>{
  'appointmentItemId': instance.appointmentItemId,
  'customerPackageItemId': instance.customerPackageItemId,
};
