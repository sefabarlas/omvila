// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_credit_grant.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminCreditGrantCWProxy {
  AdminCreditGrant credits(int credits);

  AdminCreditGrant reason(String reason);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminCreditGrant(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminCreditGrant(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminCreditGrant call({int credits, String reason});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminCreditGrant.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminCreditGrant.copyWith.fieldName(...)`
class _$AdminCreditGrantCWProxyImpl implements _$AdminCreditGrantCWProxy {
  const _$AdminCreditGrantCWProxyImpl(this._value);

  final AdminCreditGrant _value;

  @override
  AdminCreditGrant credits(int credits) => this(credits: credits);

  @override
  AdminCreditGrant reason(String reason) => this(reason: reason);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminCreditGrant(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminCreditGrant(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminCreditGrant call({
    Object? credits = const $CopyWithPlaceholder(),
    Object? reason = const $CopyWithPlaceholder(),
  }) {
    return AdminCreditGrant(
      credits: credits == const $CopyWithPlaceholder()
          ? _value.credits
          // ignore: cast_nullable_to_non_nullable
          : credits as int,
      reason: reason == const $CopyWithPlaceholder()
          ? _value.reason
          // ignore: cast_nullable_to_non_nullable
          : reason as String,
    );
  }
}

extension $AdminCreditGrantCopyWith on AdminCreditGrant {
  /// Returns a callable class that can be used as follows: `instanceOfAdminCreditGrant.copyWith(...)` or like so:`instanceOfAdminCreditGrant.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminCreditGrantCWProxy get copyWith => _$AdminCreditGrantCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminCreditGrant _$AdminCreditGrantFromJson(Map<String, dynamic> json) =>
    $checkedCreate('AdminCreditGrant', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['credits', 'reason']);
      final val = AdminCreditGrant(
        credits: $checkedConvert('credits', (v) => (v as num).toInt()),
        reason: $checkedConvert('reason', (v) => v as String),
      );
      return val;
    });

Map<String, dynamic> _$AdminCreditGrantToJson(AdminCreditGrant instance) =>
    <String, dynamic>{'credits': instance.credits, 'reason': instance.reason};
