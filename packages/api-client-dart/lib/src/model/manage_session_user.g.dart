// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_session_user.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageSessionUserCWProxy {
  ManageSessionUser id(String id);

  ManageSessionUser fullName(String fullName);

  ManageSessionUser phoneMasked(String phoneMasked);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageSessionUser(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageSessionUser(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageSessionUser call({String id, String fullName, String phoneMasked});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageSessionUser.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageSessionUser.copyWith.fieldName(...)`
class _$ManageSessionUserCWProxyImpl implements _$ManageSessionUserCWProxy {
  const _$ManageSessionUserCWProxyImpl(this._value);

  final ManageSessionUser _value;

  @override
  ManageSessionUser id(String id) => this(id: id);

  @override
  ManageSessionUser fullName(String fullName) => this(fullName: fullName);

  @override
  ManageSessionUser phoneMasked(String phoneMasked) =>
      this(phoneMasked: phoneMasked);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageSessionUser(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageSessionUser(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageSessionUser call({
    Object? id = const $CopyWithPlaceholder(),
    Object? fullName = const $CopyWithPlaceholder(),
    Object? phoneMasked = const $CopyWithPlaceholder(),
  }) {
    return ManageSessionUser(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
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

extension $ManageSessionUserCopyWith on ManageSessionUser {
  /// Returns a callable class that can be used as follows: `instanceOfManageSessionUser.copyWith(...)` or like so:`instanceOfManageSessionUser.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageSessionUserCWProxy get copyWith =>
      _$ManageSessionUserCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageSessionUser _$ManageSessionUserFromJson(Map<String, dynamic> json) =>
    $checkedCreate('ManageSessionUser', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['id', 'fullName', 'phoneMasked']);
      final val = ManageSessionUser(
        id: $checkedConvert('id', (v) => v as String),
        fullName: $checkedConvert('fullName', (v) => v as String),
        phoneMasked: $checkedConvert('phoneMasked', (v) => v as String),
      );
      return val;
    });

Map<String, dynamic> _$ManageSessionUserToJson(ManageSessionUser instance) =>
    <String, dynamic>{
      'id': instance.id,
      'fullName': instance.fullName,
      'phoneMasked': instance.phoneMasked,
    };
