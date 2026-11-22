// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_brand_detail_owner.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminBrandDetailOwnerCWProxy {
  AdminBrandDetailOwner fullName(String fullName);

  AdminBrandDetailOwner phoneMasked(String phoneMasked);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminBrandDetailOwner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminBrandDetailOwner(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminBrandDetailOwner call({String fullName, String phoneMasked});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminBrandDetailOwner.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminBrandDetailOwner.copyWith.fieldName(...)`
class _$AdminBrandDetailOwnerCWProxyImpl
    implements _$AdminBrandDetailOwnerCWProxy {
  const _$AdminBrandDetailOwnerCWProxyImpl(this._value);

  final AdminBrandDetailOwner _value;

  @override
  AdminBrandDetailOwner fullName(String fullName) => this(fullName: fullName);

  @override
  AdminBrandDetailOwner phoneMasked(String phoneMasked) =>
      this(phoneMasked: phoneMasked);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminBrandDetailOwner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminBrandDetailOwner(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminBrandDetailOwner call({
    Object? fullName = const $CopyWithPlaceholder(),
    Object? phoneMasked = const $CopyWithPlaceholder(),
  }) {
    return AdminBrandDetailOwner(
      fullName: fullName == const $CopyWithPlaceholder()
          ? _value.fullName
          // ignore: cast_nullable_to_non_nullable
          : fullName as String,
      phoneMasked: phoneMasked == const $CopyWithPlaceholder()
          ? _value.phoneMasked
          // ignore: cast_nullable_to_non_nullable
          : phoneMasked as String,
    );
  }
}

extension $AdminBrandDetailOwnerCopyWith on AdminBrandDetailOwner {
  /// Returns a callable class that can be used as follows: `instanceOfAdminBrandDetailOwner.copyWith(...)` or like so:`instanceOfAdminBrandDetailOwner.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminBrandDetailOwnerCWProxy get copyWith =>
      _$AdminBrandDetailOwnerCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminBrandDetailOwner _$AdminBrandDetailOwnerFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AdminBrandDetailOwner', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['fullName', 'phoneMasked']);
  final val = AdminBrandDetailOwner(
    fullName: $checkedConvert('fullName', (v) => v as String),
    phoneMasked: $checkedConvert('phoneMasked', (v) => v as String),
  );
  return val;
});

Map<String, dynamic> _$AdminBrandDetailOwnerToJson(
  AdminBrandDetailOwner instance,
) => <String, dynamic>{
  'fullName': instance.fullName,
  'phoneMasked': instance.phoneMasked,
};
