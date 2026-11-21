// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'me_account_deletion_result.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MeAccountDeletionResultCWProxy {
  MeAccountDeletionResult deletedUtc(DateTime deletedUtc);

  MeAccountDeletionResult cancelledAppointmentCount(
    int cancelledAppointmentCount,
  );

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeAccountDeletionResult(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeAccountDeletionResult(...).copyWith(id: 12, name: "My name")
  /// ````
  MeAccountDeletionResult call({
    DateTime deletedUtc,
    int cancelledAppointmentCount,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMeAccountDeletionResult.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMeAccountDeletionResult.copyWith.fieldName(...)`
class _$MeAccountDeletionResultCWProxyImpl
    implements _$MeAccountDeletionResultCWProxy {
  const _$MeAccountDeletionResultCWProxyImpl(this._value);

  final MeAccountDeletionResult _value;

  @override
  MeAccountDeletionResult deletedUtc(DateTime deletedUtc) =>
      this(deletedUtc: deletedUtc);

  @override
  MeAccountDeletionResult cancelledAppointmentCount(
    int cancelledAppointmentCount,
  ) => this(cancelledAppointmentCount: cancelledAppointmentCount);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeAccountDeletionResult(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeAccountDeletionResult(...).copyWith(id: 12, name: "My name")
  /// ````
  MeAccountDeletionResult call({
    Object? deletedUtc = const $CopyWithPlaceholder(),
    Object? cancelledAppointmentCount = const $CopyWithPlaceholder(),
  }) {
    return MeAccountDeletionResult(
      deletedUtc: deletedUtc == const $CopyWithPlaceholder()
          ? _value.deletedUtc
          // ignore: cast_nullable_to_non_nullable
          : deletedUtc as DateTime,
      cancelledAppointmentCount:
          cancelledAppointmentCount == const $CopyWithPlaceholder()
          ? _value.cancelledAppointmentCount
          // ignore: cast_nullable_to_non_nullable
          : cancelledAppointmentCount as int,
    );
  }
}

extension $MeAccountDeletionResultCopyWith on MeAccountDeletionResult {
  /// Returns a callable class that can be used as follows: `instanceOfMeAccountDeletionResult.copyWith(...)` or like so:`instanceOfMeAccountDeletionResult.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MeAccountDeletionResultCWProxy get copyWith =>
      _$MeAccountDeletionResultCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MeAccountDeletionResult _$MeAccountDeletionResultFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('MeAccountDeletionResult', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const ['deletedUtc', 'cancelledAppointmentCount'],
  );
  final val = MeAccountDeletionResult(
    deletedUtc: $checkedConvert(
      'deletedUtc',
      (v) => DateTime.parse(v as String),
    ),
    cancelledAppointmentCount: $checkedConvert(
      'cancelledAppointmentCount',
      (v) => (v as num).toInt(),
    ),
  );
  return val;
});

Map<String, dynamic> _$MeAccountDeletionResultToJson(
  MeAccountDeletionResult instance,
) => <String, dynamic>{
  'deletedUtc': instance.deletedUtc.toIso8601String(),
  'cancelledAppointmentCount': instance.cancelledAppointmentCount,
};
