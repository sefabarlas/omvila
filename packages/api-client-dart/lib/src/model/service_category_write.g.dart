// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'service_category_write.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ServiceCategoryWriteCWProxy {
  ServiceCategoryWrite name(String name);

  ServiceCategoryWrite sortOrder(int? sortOrder);

  ServiceCategoryWrite isActive(bool? isActive);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ServiceCategoryWrite(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ServiceCategoryWrite(...).copyWith(id: 12, name: "My name")
  /// ````
  ServiceCategoryWrite call({String name, int? sortOrder, bool? isActive});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfServiceCategoryWrite.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfServiceCategoryWrite.copyWith.fieldName(...)`
class _$ServiceCategoryWriteCWProxyImpl
    implements _$ServiceCategoryWriteCWProxy {
  const _$ServiceCategoryWriteCWProxyImpl(this._value);

  final ServiceCategoryWrite _value;

  @override
  ServiceCategoryWrite name(String name) => this(name: name);

  @override
  ServiceCategoryWrite sortOrder(int? sortOrder) => this(sortOrder: sortOrder);

  @override
  ServiceCategoryWrite isActive(bool? isActive) => this(isActive: isActive);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ServiceCategoryWrite(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ServiceCategoryWrite(...).copyWith(id: 12, name: "My name")
  /// ````
  ServiceCategoryWrite call({
    Object? name = const $CopyWithPlaceholder(),
    Object? sortOrder = const $CopyWithPlaceholder(),
    Object? isActive = const $CopyWithPlaceholder(),
  }) {
    return ServiceCategoryWrite(
      name: name == const $CopyWithPlaceholder()
          ? _value.name
          // ignore: cast_nullable_to_non_nullable
          : name as String,
      sortOrder: sortOrder == const $CopyWithPlaceholder()
          ? _value.sortOrder
          // ignore: cast_nullable_to_non_nullable
          : sortOrder as int?,
      isActive: isActive == const $CopyWithPlaceholder()
          ? _value.isActive
          // ignore: cast_nullable_to_non_nullable
          : isActive as bool?,
    );
  }
}

extension $ServiceCategoryWriteCopyWith on ServiceCategoryWrite {
  /// Returns a callable class that can be used as follows: `instanceOfServiceCategoryWrite.copyWith(...)` or like so:`instanceOfServiceCategoryWrite.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ServiceCategoryWriteCWProxy get copyWith =>
      _$ServiceCategoryWriteCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ServiceCategoryWrite _$ServiceCategoryWriteFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('ServiceCategoryWrite', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['name']);
  final val = ServiceCategoryWrite(
    name: $checkedConvert('name', (v) => v as String),
    sortOrder: $checkedConvert('sortOrder', (v) => (v as num?)?.toInt()),
    isActive: $checkedConvert('isActive', (v) => v as bool?),
  );
  return val;
});

Map<String, dynamic> _$ServiceCategoryWriteToJson(
  ServiceCategoryWrite instance,
) => <String, dynamic>{
  'name': instance.name,
  'sortOrder': ?instance.sortOrder,
  'isActive': ?instance.isActive,
};
