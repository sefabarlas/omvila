// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'staff_service_assignment.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$StaffServiceAssignmentCWProxy {
  StaffServiceAssignment serviceId(String serviceId);

  StaffServiceAssignment durationOverrideMinutes(int? durationOverrideMinutes);

  StaffServiceAssignment surcharge(Money? surcharge);

  StaffServiceAssignment commissionPercentOverride(
    num? commissionPercentOverride,
  );

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `StaffServiceAssignment(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// StaffServiceAssignment(...).copyWith(id: 12, name: "My name")
  /// ````
  StaffServiceAssignment call({
    String serviceId,
    int? durationOverrideMinutes,
    Money? surcharge,
    num? commissionPercentOverride,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfStaffServiceAssignment.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfStaffServiceAssignment.copyWith.fieldName(...)`
class _$StaffServiceAssignmentCWProxyImpl
    implements _$StaffServiceAssignmentCWProxy {
  const _$StaffServiceAssignmentCWProxyImpl(this._value);

  final StaffServiceAssignment _value;

  @override
  StaffServiceAssignment serviceId(String serviceId) =>
      this(serviceId: serviceId);

  @override
  StaffServiceAssignment durationOverrideMinutes(
    int? durationOverrideMinutes,
  ) => this(durationOverrideMinutes: durationOverrideMinutes);

  @override
  StaffServiceAssignment surcharge(Money? surcharge) =>
      this(surcharge: surcharge);

  @override
  StaffServiceAssignment commissionPercentOverride(
    num? commissionPercentOverride,
  ) => this(commissionPercentOverride: commissionPercentOverride);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `StaffServiceAssignment(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// StaffServiceAssignment(...).copyWith(id: 12, name: "My name")
  /// ````
  StaffServiceAssignment call({
    Object? serviceId = const $CopyWithPlaceholder(),
    Object? durationOverrideMinutes = const $CopyWithPlaceholder(),
    Object? surcharge = const $CopyWithPlaceholder(),
    Object? commissionPercentOverride = const $CopyWithPlaceholder(),
  }) {
    return StaffServiceAssignment(
      serviceId: serviceId == const $CopyWithPlaceholder()
          ? _value.serviceId
          // ignore: cast_nullable_to_non_nullable
          : serviceId as String,
      durationOverrideMinutes:
          durationOverrideMinutes == const $CopyWithPlaceholder()
          ? _value.durationOverrideMinutes
          // ignore: cast_nullable_to_non_nullable
          : durationOverrideMinutes as int?,
      surcharge: surcharge == const $CopyWithPlaceholder()
          ? _value.surcharge
          // ignore: cast_nullable_to_non_nullable
          : surcharge as Money?,
      commissionPercentOverride:
          commissionPercentOverride == const $CopyWithPlaceholder()
          ? _value.commissionPercentOverride
          // ignore: cast_nullable_to_non_nullable
          : commissionPercentOverride as num?,
    );
  }
}

extension $StaffServiceAssignmentCopyWith on StaffServiceAssignment {
  /// Returns a callable class that can be used as follows: `instanceOfStaffServiceAssignment.copyWith(...)` or like so:`instanceOfStaffServiceAssignment.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$StaffServiceAssignmentCWProxy get copyWith =>
      _$StaffServiceAssignmentCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

StaffServiceAssignment _$StaffServiceAssignmentFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('StaffServiceAssignment', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['serviceId']);
  final val = StaffServiceAssignment(
    serviceId: $checkedConvert('serviceId', (v) => v as String),
    durationOverrideMinutes: $checkedConvert(
      'durationOverrideMinutes',
      (v) => (v as num?)?.toInt(),
    ),
    surcharge: $checkedConvert(
      'surcharge',
      (v) => v == null ? null : Money.fromJson(v as Map<String, dynamic>),
    ),
    commissionPercentOverride: $checkedConvert(
      'commissionPercentOverride',
      (v) => v as num?,
    ),
  );
  return val;
});

Map<String, dynamic> _$StaffServiceAssignmentToJson(
  StaffServiceAssignment instance,
) => <String, dynamic>{
  'serviceId': instance.serviceId,
  'durationOverrideMinutes': instance.durationOverrideMinutes,
  'surcharge': ?instance.surcharge?.toJson(),
  'commissionPercentOverride': instance.commissionPercentOverride,
};
