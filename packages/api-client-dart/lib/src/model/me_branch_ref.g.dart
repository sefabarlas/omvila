// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'me_branch_ref.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MeBranchRefCWProxy {
  MeBranchRef id(String id);

  MeBranchRef name(String name);

  MeBranchRef district(String district);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeBranchRef(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeBranchRef(...).copyWith(id: 12, name: "My name")
  /// ````
  MeBranchRef call({String id, String name, String district});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMeBranchRef.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMeBranchRef.copyWith.fieldName(...)`
class _$MeBranchRefCWProxyImpl implements _$MeBranchRefCWProxy {
  const _$MeBranchRefCWProxyImpl(this._value);

  final MeBranchRef _value;

  @override
  MeBranchRef id(String id) => this(id: id);

  @override
  MeBranchRef name(String name) => this(name: name);

  @override
  MeBranchRef district(String district) => this(district: district);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeBranchRef(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeBranchRef(...).copyWith(id: 12, name: "My name")
  /// ````
  MeBranchRef call({
    Object? id = const $CopyWithPlaceholder(),
    Object? name = const $CopyWithPlaceholder(),
    Object? district = const $CopyWithPlaceholder(),
  }) {
    return MeBranchRef(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      name: name == const $CopyWithPlaceholder()
          ? _value.name
          // ignore: cast_nullable_to_non_nullable
          : name as String,
      district: district == const $CopyWithPlaceholder()
          ? _value.district
          // ignore: cast_nullable_to_non_nullable
          : district as String,
    );
  }
}

extension $MeBranchRefCopyWith on MeBranchRef {
  /// Returns a callable class that can be used as follows: `instanceOfMeBranchRef.copyWith(...)` or like so:`instanceOfMeBranchRef.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MeBranchRefCWProxy get copyWith => _$MeBranchRefCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MeBranchRef _$MeBranchRefFromJson(Map<String, dynamic> json) =>
    $checkedCreate('MeBranchRef', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['id', 'name', 'district']);
      final val = MeBranchRef(
        id: $checkedConvert('id', (v) => v as String),
        name: $checkedConvert('name', (v) => v as String),
        district: $checkedConvert('district', (v) => v as String),
      );
      return val;
    });

Map<String, dynamic> _$MeBranchRefToJson(MeBranchRef instance) =>
    <String, dynamic>{
      'id': instance.id,
      'name': instance.name,
      'district': instance.district,
    };
