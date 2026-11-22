// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'role_matrix.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$RoleMatrixCWProxy {
  RoleMatrix rows(List<RoleMatrixRowsInner> rows);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `RoleMatrix(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// RoleMatrix(...).copyWith(id: 12, name: "My name")
  /// ````
  RoleMatrix call({List<RoleMatrixRowsInner> rows});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfRoleMatrix.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfRoleMatrix.copyWith.fieldName(...)`
class _$RoleMatrixCWProxyImpl implements _$RoleMatrixCWProxy {
  const _$RoleMatrixCWProxyImpl(this._value);

  final RoleMatrix _value;

  @override
  RoleMatrix rows(List<RoleMatrixRowsInner> rows) => this(rows: rows);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `RoleMatrix(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// RoleMatrix(...).copyWith(id: 12, name: "My name")
  /// ````
  RoleMatrix call({Object? rows = const $CopyWithPlaceholder()}) {
    return RoleMatrix(
      rows: rows == const $CopyWithPlaceholder()
          ? _value.rows
          // ignore: cast_nullable_to_non_nullable
          : rows as List<RoleMatrixRowsInner>,
    );
  }
}

extension $RoleMatrixCopyWith on RoleMatrix {
  /// Returns a callable class that can be used as follows: `instanceOfRoleMatrix.copyWith(...)` or like so:`instanceOfRoleMatrix.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$RoleMatrixCWProxy get copyWith => _$RoleMatrixCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

RoleMatrix _$RoleMatrixFromJson(Map<String, dynamic> json) => $checkedCreate(
  'RoleMatrix',
  json,
  ($checkedConvert) {
    $checkKeys(json, requiredKeys: const ['rows']);
    final val = RoleMatrix(
      rows: $checkedConvert(
        'rows',
        (v) => (v as List<dynamic>)
            .map((e) => RoleMatrixRowsInner.fromJson(e as Map<String, dynamic>))
            .toList(),
      ),
    );
    return val;
  },
);

Map<String, dynamic> _$RoleMatrixToJson(RoleMatrix instance) =>
    <String, dynamic>{'rows': instance.rows.map((e) => e.toJson()).toList()};
