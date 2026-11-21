// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_waitlist_offer_items_inner.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PublicWaitlistOfferItemsInnerCWProxy {
  PublicWaitlistOfferItemsInner serviceName(String serviceName);

  PublicWaitlistOfferItemsInner staffName(String? staffName);

  PublicWaitlistOfferItemsInner durationMinutes(int durationMinutes);

  PublicWaitlistOfferItemsInner price(Money price);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicWaitlistOfferItemsInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicWaitlistOfferItemsInner(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicWaitlistOfferItemsInner call({
    String serviceName,
    String? staffName,
    int durationMinutes,
    Money price,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPublicWaitlistOfferItemsInner.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPublicWaitlistOfferItemsInner.copyWith.fieldName(...)`
class _$PublicWaitlistOfferItemsInnerCWProxyImpl
    implements _$PublicWaitlistOfferItemsInnerCWProxy {
  const _$PublicWaitlistOfferItemsInnerCWProxyImpl(this._value);

  final PublicWaitlistOfferItemsInner _value;

  @override
  PublicWaitlistOfferItemsInner serviceName(String serviceName) =>
      this(serviceName: serviceName);

  @override
  PublicWaitlistOfferItemsInner staffName(String? staffName) =>
      this(staffName: staffName);

  @override
  PublicWaitlistOfferItemsInner durationMinutes(int durationMinutes) =>
      this(durationMinutes: durationMinutes);

  @override
  PublicWaitlistOfferItemsInner price(Money price) => this(price: price);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicWaitlistOfferItemsInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicWaitlistOfferItemsInner(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicWaitlistOfferItemsInner call({
    Object? serviceName = const $CopyWithPlaceholder(),
    Object? staffName = const $CopyWithPlaceholder(),
    Object? durationMinutes = const $CopyWithPlaceholder(),
    Object? price = const $CopyWithPlaceholder(),
  }) {
    return PublicWaitlistOfferItemsInner(
      serviceName: serviceName == const $CopyWithPlaceholder()
          ? _value.serviceName
          // ignore: cast_nullable_to_non_nullable
          : serviceName as String,
      staffName: staffName == const $CopyWithPlaceholder()
          ? _value.staffName
          // ignore: cast_nullable_to_non_nullable
          : staffName as String?,
      durationMinutes: durationMinutes == const $CopyWithPlaceholder()
          ? _value.durationMinutes
          // ignore: cast_nullable_to_non_nullable
          : durationMinutes as int,
      price: price == const $CopyWithPlaceholder()
          ? _value.price
          // ignore: cast_nullable_to_non_nullable
          : price as Money,
    );
  }
}

extension $PublicWaitlistOfferItemsInnerCopyWith
    on PublicWaitlistOfferItemsInner {
  /// Returns a callable class that can be used as follows: `instanceOfPublicWaitlistOfferItemsInner.copyWith(...)` or like so:`instanceOfPublicWaitlistOfferItemsInner.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PublicWaitlistOfferItemsInnerCWProxy get copyWith =>
      _$PublicWaitlistOfferItemsInnerCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PublicWaitlistOfferItemsInner _$PublicWaitlistOfferItemsInnerFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('PublicWaitlistOfferItemsInner', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const ['serviceName', 'durationMinutes', 'price'],
  );
  final val = PublicWaitlistOfferItemsInner(
    serviceName: $checkedConvert('serviceName', (v) => v as String),
    staffName: $checkedConvert('staffName', (v) => v as String?),
    durationMinutes: $checkedConvert(
      'durationMinutes',
      (v) => (v as num).toInt(),
    ),
    price: $checkedConvert(
      'price',
      (v) => Money.fromJson(v as Map<String, dynamic>),
    ),
  );
  return val;
});

Map<String, dynamic> _$PublicWaitlistOfferItemsInnerToJson(
  PublicWaitlistOfferItemsInner instance,
) => <String, dynamic>{
  'serviceName': instance.serviceName,
  'staffName': ?instance.staffName,
  'durationMinutes': instance.durationMinutes,
  'price': instance.price.toJson(),
};
