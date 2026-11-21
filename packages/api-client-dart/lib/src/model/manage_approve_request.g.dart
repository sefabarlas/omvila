// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_approve_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageApproveRequestCWProxy {
  ManageApproveRequest waiveDeposit(bool? waiveDeposit);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageApproveRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageApproveRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageApproveRequest call({bool? waiveDeposit});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageApproveRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageApproveRequest.copyWith.fieldName(...)`
class _$ManageApproveRequestCWProxyImpl
    implements _$ManageApproveRequestCWProxy {
  const _$ManageApproveRequestCWProxyImpl(this._value);

  final ManageApproveRequest _value;

  @override
  ManageApproveRequest waiveDeposit(bool? waiveDeposit) =>
      this(waiveDeposit: waiveDeposit);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageApproveRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageApproveRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageApproveRequest call({
    Object? waiveDeposit = const $CopyWithPlaceholder(),
  }) {
    return ManageApproveRequest(
      waiveDeposit: waiveDeposit == const $CopyWithPlaceholder()
          ? _value.waiveDeposit
          // ignore: cast_nullable_to_non_nullable
          : waiveDeposit as bool?,
    );
  }
}

extension $ManageApproveRequestCopyWith on ManageApproveRequest {
  /// Returns a callable class that can be used as follows: `instanceOfManageApproveRequest.copyWith(...)` or like so:`instanceOfManageApproveRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageApproveRequestCWProxy get copyWith =>
      _$ManageApproveRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageApproveRequest _$ManageApproveRequestFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('ManageApproveRequest', json, ($checkedConvert) {
  final val = ManageApproveRequest(
    waiveDeposit: $checkedConvert('waiveDeposit', (v) => v as bool? ?? false),
  );
  return val;
});

Map<String, dynamic> _$ManageApproveRequestToJson(
  ManageApproveRequest instance,
) => <String, dynamic>{'waiveDeposit': ?instance.waiveDeposit};
