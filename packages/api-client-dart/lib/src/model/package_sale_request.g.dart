// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'package_sale_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PackageSaleRequestCWProxy {
  PackageSaleRequest packageId(String packageId);

  PackageSaleRequest branchId(String branchId);

  PackageSaleRequest soldByStaffId(String soldByStaffId);

  PackageSaleRequest paymentMethod(CollectedPaymentMethod paymentMethod);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PackageSaleRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PackageSaleRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  PackageSaleRequest call({
    String packageId,
    String branchId,
    String soldByStaffId,
    CollectedPaymentMethod paymentMethod,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPackageSaleRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPackageSaleRequest.copyWith.fieldName(...)`
class _$PackageSaleRequestCWProxyImpl implements _$PackageSaleRequestCWProxy {
  const _$PackageSaleRequestCWProxyImpl(this._value);

  final PackageSaleRequest _value;

  @override
  PackageSaleRequest packageId(String packageId) => this(packageId: packageId);

  @override
  PackageSaleRequest branchId(String branchId) => this(branchId: branchId);

  @override
  PackageSaleRequest soldByStaffId(String soldByStaffId) =>
      this(soldByStaffId: soldByStaffId);

  @override
  PackageSaleRequest paymentMethod(CollectedPaymentMethod paymentMethod) =>
      this(paymentMethod: paymentMethod);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PackageSaleRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PackageSaleRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  PackageSaleRequest call({
    Object? packageId = const $CopyWithPlaceholder(),
    Object? branchId = const $CopyWithPlaceholder(),
    Object? soldByStaffId = const $CopyWithPlaceholder(),
    Object? paymentMethod = const $CopyWithPlaceholder(),
  }) {
    return PackageSaleRequest(
      packageId: packageId == const $CopyWithPlaceholder()
          ? _value.packageId
          // ignore: cast_nullable_to_non_nullable
          : packageId as String,
      branchId: branchId == const $CopyWithPlaceholder()
          ? _value.branchId
          // ignore: cast_nullable_to_non_nullable
          : branchId as String,
      soldByStaffId: soldByStaffId == const $CopyWithPlaceholder()
          ? _value.soldByStaffId
          // ignore: cast_nullable_to_non_nullable
          : soldByStaffId as String,
      paymentMethod: paymentMethod == const $CopyWithPlaceholder()
          ? _value.paymentMethod
          // ignore: cast_nullable_to_non_nullable
          : paymentMethod as CollectedPaymentMethod,
    );
  }
}

extension $PackageSaleRequestCopyWith on PackageSaleRequest {
  /// Returns a callable class that can be used as follows: `instanceOfPackageSaleRequest.copyWith(...)` or like so:`instanceOfPackageSaleRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PackageSaleRequestCWProxy get copyWith =>
      _$PackageSaleRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PackageSaleRequest _$PackageSaleRequestFromJson(Map<String, dynamic> json) =>
    $checkedCreate('PackageSaleRequest', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'packageId',
          'branchId',
          'soldByStaffId',
          'paymentMethod',
        ],
      );
      final val = PackageSaleRequest(
        packageId: $checkedConvert('packageId', (v) => v as String),
        branchId: $checkedConvert('branchId', (v) => v as String),
        soldByStaffId: $checkedConvert('soldByStaffId', (v) => v as String),
        paymentMethod: $checkedConvert(
          'paymentMethod',
          (v) => $enumDecode(
            _$CollectedPaymentMethodEnumMap,
            v,
            unknownValue: CollectedPaymentMethod.unknownDefaultOpenApi,
          ),
        ),
      );
      return val;
    });

Map<String, dynamic> _$PackageSaleRequestToJson(PackageSaleRequest instance) =>
    <String, dynamic>{
      'packageId': instance.packageId,
      'branchId': instance.branchId,
      'soldByStaffId': instance.soldByStaffId,
      'paymentMethod': _$CollectedPaymentMethodEnumMap[instance.paymentMethod]!,
    };

const _$CollectedPaymentMethodEnumMap = {
  CollectedPaymentMethod.cash: 'Cash',
  CollectedPaymentMethod.card: 'Card',
  CollectedPaymentMethod.transfer: 'Transfer',
  CollectedPaymentMethod.unknownDefaultOpenApi: 'unknown_default_open_api',
};
