// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'my_commission_summary_next_accrual.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MyCommissionSummaryNextAccrualCWProxy {
  MyCommissionSummaryNextAccrual appointmentId(String appointmentId);

  MyCommissionSummaryNextAccrual customerName(String customerName);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MyCommissionSummaryNextAccrual(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MyCommissionSummaryNextAccrual(...).copyWith(id: 12, name: "My name")
  /// ````
  MyCommissionSummaryNextAccrual call({
    String appointmentId,
    String customerName,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMyCommissionSummaryNextAccrual.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMyCommissionSummaryNextAccrual.copyWith.fieldName(...)`
class _$MyCommissionSummaryNextAccrualCWProxyImpl
    implements _$MyCommissionSummaryNextAccrualCWProxy {
  const _$MyCommissionSummaryNextAccrualCWProxyImpl(this._value);

  final MyCommissionSummaryNextAccrual _value;

  @override
  MyCommissionSummaryNextAccrual appointmentId(String appointmentId) =>
      this(appointmentId: appointmentId);

  @override
  MyCommissionSummaryNextAccrual customerName(String customerName) =>
      this(customerName: customerName);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MyCommissionSummaryNextAccrual(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MyCommissionSummaryNextAccrual(...).copyWith(id: 12, name: "My name")
  /// ````
  MyCommissionSummaryNextAccrual call({
    Object? appointmentId = const $CopyWithPlaceholder(),
    Object? customerName = const $CopyWithPlaceholder(),
  }) {
    return MyCommissionSummaryNextAccrual(
      appointmentId: appointmentId == const $CopyWithPlaceholder()
          ? _value.appointmentId
          // ignore: cast_nullable_to_non_nullable
          : appointmentId as String,
      customerName: customerName == const $CopyWithPlaceholder()
          ? _value.customerName
          // ignore: cast_nullable_to_non_nullable
          : customerName as String,
    );
  }
}

extension $MyCommissionSummaryNextAccrualCopyWith
    on MyCommissionSummaryNextAccrual {
  /// Returns a callable class that can be used as follows: `instanceOfMyCommissionSummaryNextAccrual.copyWith(...)` or like so:`instanceOfMyCommissionSummaryNextAccrual.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MyCommissionSummaryNextAccrualCWProxy get copyWith =>
      _$MyCommissionSummaryNextAccrualCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MyCommissionSummaryNextAccrual _$MyCommissionSummaryNextAccrualFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('MyCommissionSummaryNextAccrual', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['appointmentId', 'customerName']);
  final val = MyCommissionSummaryNextAccrual(
    appointmentId: $checkedConvert('appointmentId', (v) => v as String),
    customerName: $checkedConvert('customerName', (v) => v as String),
  );
  return val;
});

Map<String, dynamic> _$MyCommissionSummaryNextAccrualToJson(
  MyCommissionSummaryNextAccrual instance,
) => <String, dynamic>{
  'appointmentId': instance.appointmentId,
  'customerName': instance.customerName,
};
