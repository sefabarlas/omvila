// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'credit_purchase_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CreditPurchaseRequestCWProxy {
  CreditPurchaseRequest creditPackageId(String creditPackageId);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CreditPurchaseRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CreditPurchaseRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  CreditPurchaseRequest call({String creditPackageId});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCreditPurchaseRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCreditPurchaseRequest.copyWith.fieldName(...)`
class _$CreditPurchaseRequestCWProxyImpl
    implements _$CreditPurchaseRequestCWProxy {
  const _$CreditPurchaseRequestCWProxyImpl(this._value);

  final CreditPurchaseRequest _value;

  @override
  CreditPurchaseRequest creditPackageId(String creditPackageId) =>
      this(creditPackageId: creditPackageId);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CreditPurchaseRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CreditPurchaseRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  CreditPurchaseRequest call({
    Object? creditPackageId = const $CopyWithPlaceholder(),
  }) {
    return CreditPurchaseRequest(
      creditPackageId: creditPackageId == const $CopyWithPlaceholder()
          ? _value.creditPackageId
          // ignore: cast_nullable_to_non_nullable
          : creditPackageId as String,
    );
  }
}

extension $CreditPurchaseRequestCopyWith on CreditPurchaseRequest {
  /// Returns a callable class that can be used as follows: `instanceOfCreditPurchaseRequest.copyWith(...)` or like so:`instanceOfCreditPurchaseRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CreditPurchaseRequestCWProxy get copyWith =>
      _$CreditPurchaseRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CreditPurchaseRequest _$CreditPurchaseRequestFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('CreditPurchaseRequest', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['creditPackageId']);
  final val = CreditPurchaseRequest(
    creditPackageId: $checkedConvert('creditPackageId', (v) => v as String),
  );
  return val;
});

Map<String, dynamic> _$CreditPurchaseRequestToJson(
  CreditPurchaseRequest instance,
) => <String, dynamic>{'creditPackageId': instance.creditPackageId};
