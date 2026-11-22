// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_brand_ref.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminBrandRefCWProxy {
  AdminBrandRef id(String id);

  AdminBrandRef name(String name);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminBrandRef(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminBrandRef(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminBrandRef call({String id, String name});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminBrandRef.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminBrandRef.copyWith.fieldName(...)`
class _$AdminBrandRefCWProxyImpl implements _$AdminBrandRefCWProxy {
  const _$AdminBrandRefCWProxyImpl(this._value);

  final AdminBrandRef _value;

  @override
  AdminBrandRef id(String id) => this(id: id);

  @override
  AdminBrandRef name(String name) => this(name: name);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminBrandRef(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminBrandRef(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminBrandRef call({
    Object? id = const $CopyWithPlaceholder(),
    Object? name = const $CopyWithPlaceholder(),
  }) {
    return AdminBrandRef(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      name: name == const $CopyWithPlaceholder()
          ? _value.name
          // ignore: cast_nullable_to_non_nullable
          : name as String,
    );
  }
}

extension $AdminBrandRefCopyWith on AdminBrandRef {
  /// Returns a callable class that can be used as follows: `instanceOfAdminBrandRef.copyWith(...)` or like so:`instanceOfAdminBrandRef.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminBrandRefCWProxy get copyWith => _$AdminBrandRefCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminBrandRef _$AdminBrandRefFromJson(Map<String, dynamic> json) =>
    $checkedCreate('AdminBrandRef', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['id', 'name']);
      final val = AdminBrandRef(
        id: $checkedConvert('id', (v) => v as String),
        name: $checkedConvert('name', (v) => v as String),
      );
      return val;
    });

Map<String, dynamic> _$AdminBrandRefToJson(AdminBrandRef instance) =>
    <String, dynamic>{'id': instance.id, 'name': instance.name};
