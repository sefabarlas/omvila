// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'waitlist_preferences.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$WaitlistPreferencesCWProxy {
  WaitlistPreferences serviceIds(List<String> serviceIds);

  WaitlistPreferences preferredStaffId(String? preferredStaffId);

  WaitlistPreferences acceptOtherStaff(bool acceptOtherStaff);

  WaitlistPreferences dates(List<DateTime> dates);

  WaitlistPreferences timeWindows(List<WaitlistTimeWindow> timeWindows);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `WaitlistPreferences(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// WaitlistPreferences(...).copyWith(id: 12, name: "My name")
  /// ````
  WaitlistPreferences call({
    List<String> serviceIds,
    String? preferredStaffId,
    bool acceptOtherStaff,
    List<DateTime> dates,
    List<WaitlistTimeWindow> timeWindows,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfWaitlistPreferences.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfWaitlistPreferences.copyWith.fieldName(...)`
class _$WaitlistPreferencesCWProxyImpl implements _$WaitlistPreferencesCWProxy {
  const _$WaitlistPreferencesCWProxyImpl(this._value);

  final WaitlistPreferences _value;

  @override
  WaitlistPreferences serviceIds(List<String> serviceIds) =>
      this(serviceIds: serviceIds);

  @override
  WaitlistPreferences preferredStaffId(String? preferredStaffId) =>
      this(preferredStaffId: preferredStaffId);

  @override
  WaitlistPreferences acceptOtherStaff(bool acceptOtherStaff) =>
      this(acceptOtherStaff: acceptOtherStaff);

  @override
  WaitlistPreferences dates(List<DateTime> dates) => this(dates: dates);

  @override
  WaitlistPreferences timeWindows(List<WaitlistTimeWindow> timeWindows) =>
      this(timeWindows: timeWindows);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `WaitlistPreferences(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// WaitlistPreferences(...).copyWith(id: 12, name: "My name")
  /// ````
  WaitlistPreferences call({
    Object? serviceIds = const $CopyWithPlaceholder(),
    Object? preferredStaffId = const $CopyWithPlaceholder(),
    Object? acceptOtherStaff = const $CopyWithPlaceholder(),
    Object? dates = const $CopyWithPlaceholder(),
    Object? timeWindows = const $CopyWithPlaceholder(),
  }) {
    return WaitlistPreferences(
      serviceIds: serviceIds == const $CopyWithPlaceholder()
          ? _value.serviceIds
          // ignore: cast_nullable_to_non_nullable
          : serviceIds as List<String>,
      preferredStaffId: preferredStaffId == const $CopyWithPlaceholder()
          ? _value.preferredStaffId
          // ignore: cast_nullable_to_non_nullable
          : preferredStaffId as String?,
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

extension $WaitlistPreferencesCopyWith on WaitlistPreferences {
  /// Returns a callable class that can be used as follows: `instanceOfWaitlistPreferences.copyWith(...)` or like so:`instanceOfWaitlistPreferences.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$WaitlistPreferencesCWProxy get copyWith =>
      _$WaitlistPreferencesCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

WaitlistPreferences _$WaitlistPreferencesFromJson(Map<String, dynamic> json) =>
    $checkedCreate('WaitlistPreferences', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'serviceIds',
          'acceptOtherStaff',
          'dates',
          'timeWindows',
        ],
      );
      final val = WaitlistPreferences(
        serviceIds: $checkedConvert(
          'serviceIds',
          (v) => (v as List<dynamic>).map((e) => e as String).toList(),
        ),
        preferredStaffId: $checkedConvert(
          'preferredStaffId',
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

Map<String, dynamic> _$WaitlistPreferencesToJson(
  WaitlistPreferences instance,
) => <String, dynamic>{
  'serviceIds': instance.serviceIds,
  'preferredStaffId': ?instance.preferredStaffId,
  'acceptOtherStaff': instance.acceptOtherStaff,
  'dates': instance.dates.map(const DateOnlyConverter().toJson).toList(),
  'timeWindows': instance.timeWindows
      .map((e) => _$WaitlistTimeWindowEnumMap[e]!)
      .toList(),
};

const _$WaitlistTimeWindowEnumMap = {
  WaitlistTimeWindow.morning: 'Morning',
  WaitlistTimeWindow.afternoon: 'Afternoon',
  WaitlistTimeWindow.evening: 'Evening',
  WaitlistTimeWindow.unknownDefaultOpenApi: 'unknown_default_open_api',
};
