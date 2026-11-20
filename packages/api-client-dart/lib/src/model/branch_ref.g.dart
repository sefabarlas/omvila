// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'branch_ref.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$BranchRefCWProxy {
  BranchRef id(String id);

  BranchRef name(String name);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `BranchRef(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// BranchRef(...).copyWith(id: 12, name: "My name")
  /// ````
  BranchRef call({String id, String name});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfBranchRef.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfBranchRef.copyWith.fieldName(...)`
class _$BranchRefCWProxyImpl implements _$BranchRefCWProxy {
  const _$BranchRefCWProxyImpl(this._value);

  final BranchRef _value;

  @override
  BranchRef id(String id) => this(id: id);

  @override
  BranchRef name(String name) => this(name: name);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `BranchRef(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// BranchRef(...).copyWith(id: 12, name: "My name")
  /// ````
  BranchRef call({
    Object? id = const $CopyWithPlaceholder(),
    Object? name = const $CopyWithPlaceholder(),
  }) {
    return BranchRef(
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

extension $BranchRefCopyWith on BranchRef {
  /// Returns a callable class that can be used as follows: `instanceOfBranchRef.copyWith(...)` or like so:`instanceOfBranchRef.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$BranchRefCWProxy get copyWith => _$BranchRefCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

BranchRef _$BranchRefFromJson(Map<String, dynamic> json) =>
    $checkedCreate('BranchRef', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['id', 'name']);
      final val = BranchRef(
        id: $checkedConvert('id', (v) => v as String),
        name: $checkedConvert('name', (v) => v as String),
      );
      return val;
    });

Map<String, dynamic> _$BranchRefToJson(BranchRef instance) => <String, dynamic>{
  'id': instance.id,
  'name': instance.name,
};
