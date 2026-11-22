// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_waitlist_entry.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PublicWaitlistEntryCWProxy {
  PublicWaitlistEntry id(String id);

  PublicWaitlistEntry accessCode(String accessCode);

  PublicWaitlistEntry status(WaitlistEntryStatus status);

  PublicWaitlistEntry branchId(String branchId);

  PublicWaitlistEntry timeZone(String timeZone);

  PublicWaitlistEntry serviceNames(List<String> serviceNames);

  PublicWaitlistEntry preferredStaffName(String? preferredStaffName);

  PublicWaitlistEntry acceptOtherStaff(bool acceptOtherStaff);

  PublicWaitlistEntry dates(List<DateTime> dates);

  PublicWaitlistEntry timeWindows(List<WaitlistTimeWindow> timeWindows);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicWaitlistEntry(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicWaitlistEntry(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicWaitlistEntry call({
    String id,
    String accessCode,
    WaitlistEntryStatus status,
    String branchId,
    String timeZone,
    List<String> serviceNames,
    String? preferredStaffName,
    bool acceptOtherStaff,
    List<DateTime> dates,
    List<WaitlistTimeWindow> timeWindows,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPublicWaitlistEntry.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPublicWaitlistEntry.copyWith.fieldName(...)`
class _$PublicWaitlistEntryCWProxyImpl implements _$PublicWaitlistEntryCWProxy {
  const _$PublicWaitlistEntryCWProxyImpl(this._value);

  final PublicWaitlistEntry _value;

  @override
  PublicWaitlistEntry id(String id) => this(id: id);

  @override
  PublicWaitlistEntry accessCode(String accessCode) =>
      this(accessCode: accessCode);

  @override
  PublicWaitlistEntry status(WaitlistEntryStatus status) =>
      this(status: status);

  @override
  PublicWaitlistEntry branchId(String branchId) => this(branchId: branchId);

  @override
  PublicWaitlistEntry timeZone(String timeZone) => this(timeZone: timeZone);

  @override
  PublicWaitlistEntry serviceNames(List<String> serviceNames) =>
      this(serviceNames: serviceNames);

  @override
  PublicWaitlistEntry preferredStaffName(String? preferredStaffName) =>
      this(preferredStaffName: preferredStaffName);

  @override
  PublicWaitlistEntry acceptOtherStaff(bool acceptOtherStaff) =>
      this(acceptOtherStaff: acceptOtherStaff);

  @override
  PublicWaitlistEntry dates(List<DateTime> dates) => this(dates: dates);

  @override
  PublicWaitlistEntry timeWindows(List<WaitlistTimeWindow> timeWindows) =>
      this(timeWindows: timeWindows);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicWaitlistEntry(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicWaitlistEntry(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicWaitlistEntry call({
    Object? id = const $CopyWithPlaceholder(),
    Object? accessCode = const $CopyWithPlaceholder(),
    Object? status = const $CopyWithPlaceholder(),
    Object? branchId = const $CopyWithPlaceholder(),
    Object? timeZone = const $CopyWithPlaceholder(),
    Object? serviceNames = const $CopyWithPlaceholder(),
    Object? preferredStaffName = const $CopyWithPlaceholder(),
    Object? acceptOtherStaff = const $CopyWithPlaceholder(),
    Object? dates = const $CopyWithPlaceholder(),
    Object? timeWindows = const $CopyWithPlaceholder(),
  }) {
    return PublicWaitlistEntry(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      accessCode: accessCode == const $CopyWithPlaceholder()
          ? _value.accessCode
          // ignore: cast_nullable_to_non_nullable
          : accessCode as String,
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as WaitlistEntryStatus,
      branchId: branchId == const $CopyWithPlaceholder()
          ? _value.branchId
          // ignore: cast_nullable_to_non_nullable
          : branchId as String,
      timeZone: timeZone == const $CopyWithPlaceholder()
          ? _value.timeZone
          // ignore: cast_nullable_to_non_nullable
          : timeZone as String,
      serviceNames: serviceNames == const $CopyWithPlaceholder()
          ? _value.serviceNames
          // ignore: cast_nullable_to_non_nullable
          : serviceNames as List<String>,
      preferredStaffName: preferredStaffName == const $CopyWithPlaceholder()
          ? _value.preferredStaffName
          // ignore: cast_nullable_to_non_nullable
          : preferredStaffName as String?,
      acceptOtherStaff: acceptOtherStaff == const $CopyWithPlaceholder()
          ? _value.acceptOtherStaff
          // ignore: cast_nullable_to_non_nullable
          : acceptOtherStaff as bool,
      dates: dates == const $CopyWithPlaceholder()
          ? _value.dates
          // ignore: cast_nullable_to_non_nullable
          : dates as List<DateTime>,
      timeWindows: timeWindows == const $CopyWithPlaceholder()
          ? _value.timeWindows
          // ignore: cast_nullable_to_non_nullable
          : timeWindows as List<WaitlistTimeWindow>,
    );
  }
}

extension $PublicWaitlistEntryCopyWith on PublicWaitlistEntry {
  /// Returns a callable class that can be used as follows: `instanceOfPublicWaitlistEntry.copyWith(...)` or like so:`instanceOfPublicWaitlistEntry.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PublicWaitlistEntryCWProxy get copyWith =>
      _$PublicWaitlistEntryCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PublicWaitlistEntry _$PublicWaitlistEntryFromJson(Map<String, dynamic> json) =>
    $checkedCreate('PublicWaitlistEntry', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'id',
          'accessCode',
          'status',
          'branchId',
          'timeZone',
          'serviceNames',
          'acceptOtherStaff',
          'dates',
          'timeWindows',
        ],
      );
      final val = PublicWaitlistEntry(
        id: $checkedConvert('id', (v) => v as String),
        accessCode: $checkedConvert('accessCode', (v) => v as String),
        status: $checkedConvert(
          'status',
          (v) => $enumDecode(
            _$WaitlistEntryStatusEnumMap,
            v,
            unknownValue: WaitlistEntryStatus.unknownDefaultOpenApi,
          ),
        ),
        branchId: $checkedConvert('branchId', (v) => v as String),
        timeZone: $checkedConvert('timeZone', (v) => v as String),
        serviceNames: $checkedConvert(
          'serviceNames',
          (v) => (v as List<dynamic>).map((e) => e as String).toList(),
        ),
        preferredStaffName: $checkedConvert(
          'preferredStaffName',
          (v) => v as String?,
        ),
        acceptOtherStaff: $checkedConvert('acceptOtherStaff', (v) => v as bool),
        dates: $checkedConvert(
          'dates',
          (v) => (v as List<dynamic>)
              .map((e) => const DateOnlyConverter().fromJson(e as String))
              .toList(),
        ),
        timeWindows: $checkedConvert(
          'timeWindows',
          (v) => (v as List<dynamic>)
              .map(
                (e) => $enumDecode(
                  _$WaitlistTimeWindowEnumMap,
                  e,
                  unknownValue: WaitlistTimeWindow.unknownDefaultOpenApi,
                ),
              )
              .toList(),
        ),
      );
      return val;
    });

Map<String, dynamic> _$PublicWaitlistEntryToJson(
  PublicWaitlistEntry instance,
) => <String, dynamic>{
  'id': instance.id,
  'accessCode': instance.accessCode,
  'status': _$WaitlistEntryStatusEnumMap[instance.status]!,
  'branchId': instance.branchId,
  'timeZone': instance.timeZone,
  'serviceNames': instance.serviceNames,
  'preferredStaffName': ?instance.preferredStaffName,
  'acceptOtherStaff': instance.acceptOtherStaff,
  'dates': instance.dates.map(const DateOnlyConverter().toJson).toList(),
  'timeWindows': instance.timeWindows
      .map((e) => _$WaitlistTimeWindowEnumMap[e]!)
      .toList(),
};

const _$WaitlistEntryStatusEnumMap = {
  WaitlistEntryStatus.waiting: 'Waiting',
  WaitlistEntryStatus.converted: 'Converted',
  WaitlistEntryStatus.cancelled: 'Cancelled',
  WaitlistEntryStatus.expired: 'Expired',
  WaitlistEntryStatus.unknownDefaultOpenApi: 'unknown_default_open_api',
};

const _$WaitlistTimeWindowEnumMap = {
  WaitlistTimeWindow.morning: 'Morning',
  WaitlistTimeWindow.afternoon: 'Afternoon',
  WaitlistTimeWindow.evening: 'Evening',
  WaitlistTimeWindow.unknownDefaultOpenApi: 'unknown_default_open_api',
};
