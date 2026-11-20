// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_catalog_services_inner.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PublicCatalogServicesInnerCWProxy {
  PublicCatalogServicesInner id(String id);

  PublicCatalogServicesInner categoryId(String categoryId);

  PublicCatalogServicesInner name(String name);

  PublicCatalogServicesInner description(String? description);

  PublicCatalogServicesInner durationMinutes(int durationMinutes);

  PublicCatalogServicesInner price(Money? price);

  PublicCatalogServicesInner priceDisplay(
    PublicCatalogServicesInnerPriceDisplayEnum priceDisplay,
  );

  PublicCatalogServicesInner hasDeposit(bool hasDeposit);

  PublicCatalogServicesInner depositHint(Money? depositHint);

  PublicCatalogServicesInner staffIds(List<String> staffIds);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicCatalogServicesInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicCatalogServicesInner(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicCatalogServicesInner call({
    String id,
    String categoryId,
    String name,
    String? description,
    int durationMinutes,
    Money? price,
    PublicCatalogServicesInnerPriceDisplayEnum priceDisplay,
    bool hasDeposit,
    Money? depositHint,
    List<String> staffIds,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPublicCatalogServicesInner.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPublicCatalogServicesInner.copyWith.fieldName(...)`
class _$PublicCatalogServicesInnerCWProxyImpl
    implements _$PublicCatalogServicesInnerCWProxy {
  const _$PublicCatalogServicesInnerCWProxyImpl(this._value);

  final PublicCatalogServicesInner _value;

  @override
  PublicCatalogServicesInner id(String id) => this(id: id);

  @override
  PublicCatalogServicesInner categoryId(String categoryId) =>
      this(categoryId: categoryId);

  @override
  PublicCatalogServicesInner name(String name) => this(name: name);

  @override
  PublicCatalogServicesInner description(String? description) =>
      this(description: description);

  @override
  PublicCatalogServicesInner durationMinutes(int durationMinutes) =>
      this(durationMinutes: durationMinutes);

  @override
  PublicCatalogServicesInner price(Money? price) => this(price: price);

  @override
  PublicCatalogServicesInner priceDisplay(
    PublicCatalogServicesInnerPriceDisplayEnum priceDisplay,
  ) => this(priceDisplay: priceDisplay);

  @override
  PublicCatalogServicesInner hasDeposit(bool hasDeposit) =>
      this(hasDeposit: hasDeposit);

  @override
  PublicCatalogServicesInner depositHint(Money? depositHint) =>
      this(depositHint: depositHint);

  @override
  PublicCatalogServicesInner staffIds(List<String> staffIds) =>
      this(staffIds: staffIds);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicCatalogServicesInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicCatalogServicesInner(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicCatalogServicesInner call({
    Object? id = const $CopyWithPlaceholder(),
    Object? categoryId = const $CopyWithPlaceholder(),
    Object? name = const $CopyWithPlaceholder(),
    Object? description = const $CopyWithPlaceholder(),
    Object? durationMinutes = const $CopyWithPlaceholder(),
    Object? price = const $CopyWithPlaceholder(),
    Object? priceDisplay = const $CopyWithPlaceholder(),
    Object? hasDeposit = const $CopyWithPlaceholder(),
    Object? depositHint = const $CopyWithPlaceholder(),
    Object? staffIds = const $CopyWithPlaceholder(),
  }) {
    return PublicCatalogServicesInner(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      categoryId: categoryId == const $CopyWithPlaceholder()
          ? _value.categoryId
          // ignore: cast_nullable_to_non_nullable
          : categoryId as String,
      name: name == const $CopyWithPlaceholder()
          ? _value.name
          // ignore: cast_nullable_to_non_nullable
          : name as String,
      description: description == const $CopyWithPlaceholder()
          ? _value.description
          // ignore: cast_nullable_to_non_nullable
          : description as String?,
      durationMinutes: durationMinutes == const $CopyWithPlaceholder()
          ? _value.durationMinutes
          // ignore: cast_nullable_to_non_nullable
          : durationMinutes as int,
      price: price == const $CopyWithPlaceholder()
          ? _value.price
          // ignore: cast_nullable_to_non_nullable
          : price as Money?,
      priceDisplay: priceDisplay == const $CopyWithPlaceholder()
          ? _value.priceDisplay
          // ignore: cast_nullable_to_non_nullable
          : priceDisplay as PublicCatalogServicesInnerPriceDisplayEnum,
      hasDeposit: hasDeposit == const $CopyWithPlaceholder()
          ? _value.hasDeposit
          // ignore: cast_nullable_to_non_nullable
          : hasDeposit as bool,
      depositHint: depositHint == const $CopyWithPlaceholder()
          ? _value.depositHint
          // ignore: cast_nullable_to_non_nullable
          : depositHint as Money?,
      staffIds: staffIds == const $CopyWithPlaceholder()
          ? _value.staffIds
          // ignore: cast_nullable_to_non_nullable
          : staffIds as List<String>,
    );
  }
}

extension $PublicCatalogServicesInnerCopyWith on PublicCatalogServicesInner {
  /// Returns a callable class that can be used as follows: `instanceOfPublicCatalogServicesInner.copyWith(...)` or like so:`instanceOfPublicCatalogServicesInner.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PublicCatalogServicesInnerCWProxy get copyWith =>
      _$PublicCatalogServicesInnerCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PublicCatalogServicesInner _$PublicCatalogServicesInnerFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('PublicCatalogServicesInner', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'id',
      'categoryId',
      'name',
      'durationMinutes',
      'priceDisplay',
      'hasDeposit',
      'staffIds',
    ],
  );
  final val = PublicCatalogServicesInner(
    id: $checkedConvert('id', (v) => v as String),
    categoryId: $checkedConvert('categoryId', (v) => v as String),
    name: $checkedConvert('name', (v) => v as String),
    description: $checkedConvert('description', (v) => v as String?),
    durationMinutes: $checkedConvert(
      'durationMinutes',
      (v) => (v as num).toInt(),
    ),
    price: $checkedConvert(
      'price',
      (v) => v == null ? null : Money.fromJson(v as Map<String, dynamic>),
    ),
    priceDisplay: $checkedConvert(
      'priceDisplay',
      (v) => $enumDecode(
        _$PublicCatalogServicesInnerPriceDisplayEnumEnumMap,
        v,
        unknownValue:
            PublicCatalogServicesInnerPriceDisplayEnum.unknownDefaultOpenApi,
      ),
    ),
    hasDeposit: $checkedConvert('hasDeposit', (v) => v as bool),
    depositHint: $checkedConvert(
      'depositHint',
      (v) => v == null ? null : Money.fromJson(v as Map<String, dynamic>),
    ),
    staffIds: $checkedConvert(
      'staffIds',
      (v) => (v as List<dynamic>).map((e) => e as String).toList(),
    ),
  );
  return val;
});

Map<String, dynamic> _$PublicCatalogServicesInnerToJson(
  PublicCatalogServicesInner instance,
) => <String, dynamic>{
  'id': instance.id,
  'categoryId': instance.categoryId,
  'name': instance.name,
  'description': ?instance.description,
  'durationMinutes': instance.durationMinutes,
  'price': ?instance.price?.toJson(),
  'priceDisplay':
      _$PublicCatalogServicesInnerPriceDisplayEnumEnumMap[instance
          .priceDisplay]!,
  'hasDeposit': instance.hasDeposit,
  'depositHint': ?instance.depositHint?.toJson(),
  'staffIds': instance.staffIds,
};

const _$PublicCatalogServicesInnerPriceDisplayEnumEnumMap = {
  PublicCatalogServicesInnerPriceDisplayEnum.fixed: 'Fixed',
  PublicCatalogServicesInnerPriceDisplayEnum.startingFrom: 'StartingFrom',
  PublicCatalogServicesInnerPriceDisplayEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
