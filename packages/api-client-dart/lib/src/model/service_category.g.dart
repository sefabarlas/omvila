// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'service_category.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ServiceCategoryCWProxy {
  ServiceCategory id(String id);

  ServiceCategory name(String name);

  ServiceCategory sortOrder(int sortOrder);

  ServiceCategory isActive(bool isActive);

  ServiceCategory serviceCount(int serviceCount);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ServiceCategory(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ServiceCategory(...).copyWith(id: 12, name: "My name")
  /// ````
  ServiceCategory call({
    String id,
    String name,
    int sortOrder,
    bool isActive,
    int serviceCount,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfServiceCategory.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfServiceCategory.copyWith.fieldName(...)`
class _$ServiceCategoryCWProxyImpl implements _$ServiceCategoryCWProxy {
  const _$ServiceCategoryCWProxyImpl(this._value);

  final ServiceCategory _value;

  @override
  ServiceCategory id(String id) => this(id: id);

  @override
  ServiceCategory name(String name) => this(name: name);

  @override
  ServiceCategory sortOrder(int sortOrder) => this(sortOrder: sortOrder);

  @override
  ServiceCategory isActive(bool isActive) => this(isActive: isActive);

  @override
  ServiceCategory serviceCount(int serviceCount) =>
      this(serviceCount: serviceCount);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ServiceCategory(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ServiceCategory(...).copyWith(id: 12, name: "My name")
  /// ````
  ServiceCategory call({
    Object? id = const $CopyWithPlaceholder(),
    Object? name = const $CopyWithPlaceholder(),
    Object? sortOrder = const $CopyWithPlaceholder(),
    Object? isActive = const $CopyWithPlaceholder(),
    Object? serviceCount = const $CopyWithPlaceholder(),
  }) {
    return ServiceCategory(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      name: name == const $CopyWithPlaceholder()
          ? _value.name
          // ignore: cast_nullable_to_non_nullable
          : name as String,
      sortOrder: sortOrder == const $CopyWithPlaceholder()
          ? _value.sortOrder
          // ignore: cast_nullable_to_non_nullable
          : sortOrder as int,
      isActive: isActive == const $CopyWithPlaceholder()
          ? _value.isActive
          // ignore: cast_nullable_to_non_nullable
          : isActive as bool,
      serviceCount: serviceCount == const $CopyWithPlaceholder()
          ? _value.serviceCount
          // ignore: cast_nullable_to_non_nullable
          : serviceCount as int,
    );
  }
}

extension $ServiceCategoryCopyWith on ServiceCategory {
  /// Returns a callable class that can be used as follows: `instanceOfServiceCategory.copyWith(...)` or like so:`instanceOfServiceCategory.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ServiceCategoryCWProxy get copyWith => _$ServiceCategoryCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ServiceCategory _$ServiceCategoryFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('ServiceCategory', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const ['id', 'name', 'sortOrder', 'isActive', 'serviceCount'],
  );
  final val = ServiceCategory(
    id: $checkedConvert('id', (v) => v as String),
    name: $checkedConvert('name', (v) => v as String),
    sortOrder: $checkedConvert('sortOrder', (v) => (v as num).toInt()),
    isActive: $checkedConvert('isActive', (v) => v as bool),
    serviceCount: $checkedConvert('serviceCount', (v) => (v as num).toInt()),
  );
  return val;
});

Map<String, dynamic> _$ServiceCategoryToJson(ServiceCategory instance) =>
    <String, dynamic>{
      'id': instance.id,
      'name': instance.name,
      'sortOrder': instance.sortOrder,
      'isActive': instance.isActive,
      'serviceCount': instance.serviceCount,
    };
