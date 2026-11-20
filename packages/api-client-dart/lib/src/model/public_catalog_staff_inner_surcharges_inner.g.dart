// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_catalog_staff_inner_surcharges_inner.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PublicCatalogStaffInnerSurchargesInnerCWProxy {
  PublicCatalogStaffInnerSurchargesInner serviceId(String serviceId);

  PublicCatalogStaffInnerSurchargesInner amount(Money amount);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicCatalogStaffInnerSurchargesInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicCatalogStaffInnerSurchargesInner(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicCatalogStaffInnerSurchargesInner call({String serviceId, Money amount});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPublicCatalogStaffInnerSurchargesInner.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPublicCatalogStaffInnerSurchargesInner.copyWith.fieldName(...)`
class _$PublicCatalogStaffInnerSurchargesInnerCWProxyImpl
    implements _$PublicCatalogStaffInnerSurchargesInnerCWProxy {
  const _$PublicCatalogStaffInnerSurchargesInnerCWProxyImpl(this._value);

  final PublicCatalogStaffInnerSurchargesInner _value;

  @override
  PublicCatalogStaffInnerSurchargesInner serviceId(String serviceId) =>
      this(serviceId: serviceId);

  @override
  PublicCatalogStaffInnerSurchargesInner amount(Money amount) =>
      this(amount: amount);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicCatalogStaffInnerSurchargesInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicCatalogStaffInnerSurchargesInner(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicCatalogStaffInnerSurchargesInner call({
    Object? serviceId = const $CopyWithPlaceholder(),
    Object? amount = const $CopyWithPlaceholder(),
  }) {
    return PublicCatalogStaffInnerSurchargesInner(
      serviceId: serviceId == const $CopyWithPlaceholder()
          ? _value.serviceId
          // ignore: cast_nullable_to_non_nullable
          : serviceId as String,
      amount: amount == const $CopyWithPlaceholder()
          ? _value.amount
          // ignore: cast_nullable_to_non_nullable
          : amount as Money,
    );
  }
}

extension $PublicCatalogStaffInnerSurchargesInnerCopyWith
    on PublicCatalogStaffInnerSurchargesInner {
  /// Returns a callable class that can be used as follows: `instanceOfPublicCatalogStaffInnerSurchargesInner.copyWith(...)` or like so:`instanceOfPublicCatalogStaffInnerSurchargesInner.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PublicCatalogStaffInnerSurchargesInnerCWProxy get copyWith =>
      _$PublicCatalogStaffInnerSurchargesInnerCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PublicCatalogStaffInnerSurchargesInner
_$PublicCatalogStaffInnerSurchargesInnerFromJson(Map<String, dynamic> json) =>
    $checkedCreate('PublicCatalogStaffInnerSurchargesInner', json, (
      $checkedConvert,
    ) {
      $checkKeys(json, requiredKeys: const ['serviceId', 'amount']);
      final val = PublicCatalogStaffInnerSurchargesInner(
        serviceId: $checkedConvert('serviceId', (v) => v as String),
        amount: $checkedConvert(
          'amount',
          (v) => Money.fromJson(v as Map<String, dynamic>),
        ),
      );
      return val;
    });

Map<String, dynamic> _$PublicCatalogStaffInnerSurchargesInnerToJson(
  PublicCatalogStaffInnerSurchargesInner instance,
) => <String, dynamic>{
  'serviceId': instance.serviceId,
  'amount': instance.amount.toJson(),
};
