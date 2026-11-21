// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_complete_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageCompleteRequestCWProxy {
  ManageCompleteRequest payments(List<CompletionPayment> payments);

  ManageCompleteRequest packageRedemptions(
    List<PackageRedemptionRequest>? packageRedemptions,
  );

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageCompleteRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageCompleteRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageCompleteRequest call({
    List<CompletionPayment> payments,
    List<PackageRedemptionRequest>? packageRedemptions,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageCompleteRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageCompleteRequest.copyWith.fieldName(...)`
class _$ManageCompleteRequestCWProxyImpl
    implements _$ManageCompleteRequestCWProxy {
  const _$ManageCompleteRequestCWProxyImpl(this._value);

  final ManageCompleteRequest _value;

  @override
  ManageCompleteRequest payments(List<CompletionPayment> payments) =>
      this(payments: payments);

  @override
  ManageCompleteRequest packageRedemptions(
    List<PackageRedemptionRequest>? packageRedemptions,
  ) => this(packageRedemptions: packageRedemptions);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageCompleteRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageCompleteRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageCompleteRequest call({
    Object? payments = const $CopyWithPlaceholder(),
    Object? packageRedemptions = const $CopyWithPlaceholder(),
  }) {
    return ManageCompleteRequest(
      payments: payments == const $CopyWithPlaceholder()
          ? _value.payments
          // ignore: cast_nullable_to_non_nullable
          : payments as List<CompletionPayment>,
      packageRedemptions: packageRedemptions == const $CopyWithPlaceholder()
          ? _value.packageRedemptions
          // ignore: cast_nullable_to_non_nullable
          : packageRedemptions as List<PackageRedemptionRequest>?,
    );
  }
}

extension $ManageCompleteRequestCopyWith on ManageCompleteRequest {
  /// Returns a callable class that can be used as follows: `instanceOfManageCompleteRequest.copyWith(...)` or like so:`instanceOfManageCompleteRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageCompleteRequestCWProxy get copyWith =>
      _$ManageCompleteRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageCompleteRequest _$ManageCompleteRequestFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('ManageCompleteRequest', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['payments']);
  final val = ManageCompleteRequest(
    payments: $checkedConvert(
      'payments',
      (v) => (v as List<dynamic>)
          .map((e) => CompletionPayment.fromJson(e as Map<String, dynamic>))
          .toList(),
    ),
    packageRedemptions: $checkedConvert(
      'packageRedemptions',
      (v) => (v as List<dynamic>?)
          ?.map(
            (e) => PackageRedemptionRequest.fromJson(e as Map<String, dynamic>),
          )
          .toList(),
    ),
  );
  return val;
});

Map<String, dynamic> _$ManageCompleteRequestToJson(
  ManageCompleteRequest instance,
) => <String, dynamic>{
  'payments': instance.payments.map((e) => e.toJson()).toList(),
  'packageRedemptions': ?instance.packageRedemptions
      ?.map((e) => e.toJson())
      .toList(),
};
