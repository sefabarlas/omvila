// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_session_user.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminSessionUserCWProxy {
  AdminSessionUser id(String id);

  AdminSessionUser fullName(String fullName);

  AdminSessionUser phoneMasked(String phoneMasked);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminSessionUser(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminSessionUser(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminSessionUser call({String id, String fullName, String phoneMasked});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminSessionUser.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminSessionUser.copyWith.fieldName(...)`
class _$AdminSessionUserCWProxyImpl implements _$AdminSessionUserCWProxy {
  const _$AdminSessionUserCWProxyImpl(this._value);

  final AdminSessionUser _value;

  @override
  AdminSessionUser id(String id) => this(id: id);

  @override
  AdminSessionUser fullName(String fullName) => this(fullName: fullName);

  @override
  AdminSessionUser phoneMasked(String phoneMasked) =>
      this(phoneMasked: phoneMasked);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminSessionUser(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminSessionUser(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminSessionUser call({
    Object? id = const $CopyWithPlaceholder(),
    Object? fullName = const $CopyWithPlaceholder(),
    Object? phoneMasked = const $CopyWithPlaceholder(),
  }) {
    return AdminSessionUser(
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

extension $AdminSessionUserCopyWith on AdminSessionUser {
  /// Returns a callable class that can be used as follows: `instanceOfAdminSessionUser.copyWith(...)` or like so:`instanceOfAdminSessionUser.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminSessionUserCWProxy get copyWith => _$AdminSessionUserCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminSessionUser _$AdminSessionUserFromJson(Map<String, dynamic> json) =>
    $checkedCreate('AdminSessionUser', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['id', 'fullName', 'phoneMasked']);
      final val = AdminSessionUser(
        id: $checkedConvert('id', (v) => v as String),
        fullName: $checkedConvert('fullName', (v) => v as String),
        phoneMasked: $checkedConvert('phoneMasked', (v) => v as String),
      );
      return val;
    });

Map<String, dynamic> _$AdminSessionUserToJson(AdminSessionUser instance) =>
    <String, dynamic>{
      'id': instance.id,
      'fullName': instance.fullName,
      'phoneMasked': instance.phoneMasked,
    };
