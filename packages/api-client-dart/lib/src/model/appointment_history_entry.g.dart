// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'appointment_history_entry.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AppointmentHistoryEntryCWProxy {
  AppointmentHistoryEntry occurredUtc(DateTime occurredUtc);

  AppointmentHistoryEntry kind(AppointmentHistoryEntryKindEnum kind);

  AppointmentHistoryEntry actorName(String? actorName);

  AppointmentHistoryEntry detail(String? detail);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AppointmentHistoryEntry(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AppointmentHistoryEntry(...).copyWith(id: 12, name: "My name")
  /// ````
  AppointmentHistoryEntry call({
    DateTime occurredUtc,
    AppointmentHistoryEntryKindEnum kind,
    String? actorName,
    String? detail,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAppointmentHistoryEntry.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAppointmentHistoryEntry.copyWith.fieldName(...)`
class _$AppointmentHistoryEntryCWProxyImpl
    implements _$AppointmentHistoryEntryCWProxy {
  const _$AppointmentHistoryEntryCWProxyImpl(this._value);

  final AppointmentHistoryEntry _value;

  @override
  AppointmentHistoryEntry occurredUtc(DateTime occurredUtc) =>
      this(occurredUtc: occurredUtc);

  @override
  AppointmentHistoryEntry kind(AppointmentHistoryEntryKindEnum kind) =>
      this(kind: kind);

  @override
  AppointmentHistoryEntry actorName(String? actorName) =>
      this(actorName: actorName);

  @override
  AppointmentHistoryEntry detail(String? detail) => this(detail: detail);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AppointmentHistoryEntry(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AppointmentHistoryEntry(...).copyWith(id: 12, name: "My name")
  /// ````
  AppointmentHistoryEntry call({
    Object? occurredUtc = const $CopyWithPlaceholder(),
    Object? kind = const $CopyWithPlaceholder(),
    Object? actorName = const $CopyWithPlaceholder(),
    Object? detail = const $CopyWithPlaceholder(),
  }) {
    return AppointmentHistoryEntry(
      occurredUtc: occurredUtc == const $CopyWithPlaceholder()
          ? _value.occurredUtc
          // ignore: cast_nullable_to_non_nullable
          : occurredUtc as DateTime,
      kind: kind == const $CopyWithPlaceholder()
          ? _value.kind
          // ignore: cast_nullable_to_non_nullable
          : kind as AppointmentHistoryEntryKindEnum,
      actorName: actorName == const $CopyWithPlaceholder()
          ? _value.actorName
          // ignore: cast_nullable_to_non_nullable
          : actorName as String?,
      detail: detail == const $CopyWithPlaceholder()
          ? _value.detail
          // ignore: cast_nullable_to_non_nullable
          : detail as String?,
    );
  }
}

extension $AppointmentHistoryEntryCopyWith on AppointmentHistoryEntry {
  /// Returns a callable class that can be used as follows: `instanceOfAppointmentHistoryEntry.copyWith(...)` or like so:`instanceOfAppointmentHistoryEntry.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AppointmentHistoryEntryCWProxy get copyWith =>
      _$AppointmentHistoryEntryCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AppointmentHistoryEntry _$AppointmentHistoryEntryFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AppointmentHistoryEntry', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['occurredUtc', 'kind']);
  final val = AppointmentHistoryEntry(
    occurredUtc: $checkedConvert(
      'occurredUtc',
      (v) => DateTime.parse(v as String),
    ),
    kind: $checkedConvert(
      'kind',
      (v) => $enumDecode(
        _$AppointmentHistoryEntryKindEnumEnumMap,
        v,
        unknownValue: AppointmentHistoryEntryKindEnum.unknownDefaultOpenApi,
      ),
    ),
    actorName: $checkedConvert('actorName', (v) => v as String?),
    detail: $checkedConvert('detail', (v) => v as String?),
  );
  return val;
});

Map<String, dynamic> _$AppointmentHistoryEntryToJson(
  AppointmentHistoryEntry instance,
) => <String, dynamic>{
  'occurredUtc': instance.occurredUtc.toIso8601String(),
  'kind': _$AppointmentHistoryEntryKindEnumEnumMap[instance.kind]!,
  'actorName': ?instance.actorName,
  'detail': ?instance.detail,
};

const _$AppointmentHistoryEntryKindEnumEnumMap = {
  AppointmentHistoryEntryKindEnum.created: 'Created',
  AppointmentHistoryEntryKindEnum.approved: 'Approved',
  AppointmentHistoryEntryKindEnum.declined: 'Declined',
  AppointmentHistoryEntryKindEnum.rescheduled: 'Rescheduled',
  AppointmentHistoryEntryKindEnum.cancelled: 'Cancelled',
  AppointmentHistoryEntryKindEnum.checkedIn: 'CheckedIn',
  AppointmentHistoryEntryKindEnum.noShow: 'NoShow',
  AppointmentHistoryEntryKindEnum.noShowUndone: 'NoShowUndone',
  AppointmentHistoryEntryKindEnum.completed: 'Completed',
  AppointmentHistoryEntryKindEnum.depositReported: 'DepositReported',
  AppointmentHistoryEntryKindEnum.depositReceived: 'DepositReceived',
  AppointmentHistoryEntryKindEnum.notificationSent: 'NotificationSent',
  AppointmentHistoryEntryKindEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
