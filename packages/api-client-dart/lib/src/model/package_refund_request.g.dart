// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'package_refund_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PackageRefundRequestCWProxy {
  PackageRefundRequest amount(Money amount);

  PackageRefundRequest note(String? note);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PackageRefundRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PackageRefundRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  PackageRefundRequest call({Money amount, String? note});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPackageRefundRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPackageRefundRequest.copyWith.fieldName(...)`
class _$PackageRefundRequestCWProxyImpl
    implements _$PackageRefundRequestCWProxy {
  const _$PackageRefundRequestCWProxyImpl(this._value);

  final PackageRefundRequest _value;

  @override
  PackageRefundRequest amount(Money amount) => this(amount: amount);

  @override
  PackageRefundRequest note(String? note) => this(note: note);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PackageRefundRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PackageRefundRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  PackageRefundRequest call({
    Object? amount = const $CopyWithPlaceholder(),
    Object? note = const $CopyWithPlaceholder(),
  }) {
    return PackageRefundRequest(
      amount: amount == const $CopyWithPlaceholder()
          ? _value.amount
          // ignore: cast_nullable_to_non_nullable
          : amount as Money,
      note: note == const $CopyWithPlaceholder()
          ? _value.note
          // ignore: cast_nullable_to_non_nullable
          : note as String?,
    );
  }
}

extension $PackageRefundRequestCopyWith on PackageRefundRequest {
  /// Returns a callable class that can be used as follows: `instanceOfPackageRefundRequest.copyWith(...)` or like so:`instanceOfPackageRefundRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PackageRefundRequestCWProxy get copyWith =>
      _$PackageRefundRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PackageRefundRequest _$PackageRefundRequestFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('PackageRefundRequest', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['amount']);
  final val = PackageRefundRequest(
    amount: $checkedConvert(
      'amount',
      (v) => Money.fromJson(v as Map<String, dynamic>),
    ),
    note: $checkedConvert('note', (v) => v as String?),
  );
  return val;
});

Map<String, dynamic> _$PackageRefundRequestToJson(
  PackageRefundRequest instance,
) => <String, dynamic>{
  'amount': instance.amount.toJson(),
  'note': ?instance.note,
};
