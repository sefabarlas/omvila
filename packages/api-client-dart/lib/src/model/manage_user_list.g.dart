// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_user_list.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageUserListCWProxy {
  ManageUserList items(List<ManageUser> items);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageUserList(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageUserList(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageUserList call({List<ManageUser> items});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageUserList.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageUserList.copyWith.fieldName(...)`
class _$ManageUserListCWProxyImpl implements _$ManageUserListCWProxy {
  const _$ManageUserListCWProxyImpl(this._value);

  final ManageUserList _value;

  @override
  ManageUserList items(List<ManageUser> items) => this(items: items);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageUserList(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageUserList(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageUserList call({Object? items = const $CopyWithPlaceholder()}) {
    return ManageUserList(
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<ManageUser>,
    );
  }
}

extension $ManageUserListCopyWith on ManageUserList {
  /// Returns a callable class that can be used as follows: `instanceOfManageUserList.copyWith(...)` or like so:`instanceOfManageUserList.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageUserListCWProxy get copyWith => _$ManageUserListCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageUserList _$ManageUserListFromJson(Map<String, dynamic> json) =>
    $checkedCreate('ManageUserList', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['items']);
      final val = ManageUserList(
        items: $checkedConvert(
          'items',
          (v) => (v as List<dynamic>)
              .map((e) => ManageUser.fromJson(e as Map<String, dynamic>))
              .toList(),
        ),
      );
      return val;
    });

Map<String, dynamic> _$ManageUserListToJson(ManageUserList instance) =>
    <String, dynamic>{'items': instance.items.map((e) => e.toJson()).toList()};
