// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'waitlist_entry_view.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$WaitlistEntryViewCWProxy {
  WaitlistEntryView id(String id);

  WaitlistEntryView branchId(String branchId);

  WaitlistEntryView status(WaitlistEntryStatus status);

  WaitlistEntryView customer(CustomerRef customer);

  WaitlistEntryView serviceIds(List<String> serviceIds);

  WaitlistEntryView serviceNames(List<String> serviceNames);

  WaitlistEntryView preferredStaffId(String? preferredStaffId);

  WaitlistEntryView preferredStaffName(String? preferredStaffName);

  WaitlistEntryView acceptOtherStaff(bool acceptOtherStaff);

  WaitlistEntryView dates(List<DateTime> dates);

  WaitlistEntryView timeWindows(List<WaitlistTimeWindow> timeWindows);

  WaitlistEntryView note(String? note);

  WaitlistEntryView joinedUtc(DateTime joinedUtc);

  WaitlistEntryView source_(WaitlistEntryViewSource_Enum source_);

  WaitlistEntryView pendingOffer(WaitlistOfferView? pendingOffer);

  WaitlistEntryView version(String version);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `WaitlistEntryView(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// WaitlistEntryView(...).copyWith(id: 12, name: "My name")
  /// ````
  WaitlistEntryView call({
    String id,
    String branchId,
    WaitlistEntryStatus status,
    CustomerRef customer,
    List<String> serviceIds,
    List<String> serviceNames,
    String? preferredStaffId,
    String? preferredStaffName,
    bool acceptOtherStaff,
    List<DateTime> dates,
    List<WaitlistTimeWindow> timeWindows,
    String? note,
    DateTime joinedUtc,
    WaitlistEntryViewSource_Enum source_,
    WaitlistOfferView? pendingOffer,
    String version,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfWaitlistEntryView.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfWaitlistEntryView.copyWith.fieldName(...)`
class _$WaitlistEntryViewCWProxyImpl implements _$WaitlistEntryViewCWProxy {
  const _$WaitlistEntryViewCWProxyImpl(this._value);

  final WaitlistEntryView _value;

  @override
  WaitlistEntryView id(String id) => this(id: id);

  @override
  WaitlistEntryView branchId(String branchId) => this(branchId: branchId);

  @override
  WaitlistEntryView status(WaitlistEntryStatus status) => this(status: status);

  @override
  WaitlistEntryView customer(CustomerRef customer) => this(customer: customer);

  @override
  WaitlistEntryView serviceIds(List<String> serviceIds) =>
      this(serviceIds: serviceIds);

  @override
  WaitlistEntryView serviceNames(List<String> serviceNames) =>
      this(serviceNames: serviceNames);

  @override
  WaitlistEntryView preferredStaffId(String? preferredStaffId) =>
      this(preferredStaffId: preferredStaffId);

  @override
  WaitlistEntryView preferredStaffName(String? preferredStaffName) =>
      this(preferredStaffName: preferredStaffName);

  @override
  WaitlistEntryView acceptOtherStaff(bool acceptOtherStaff) =>
      this(acceptOtherStaff: acceptOtherStaff);

  @override
  WaitlistEntryView dates(List<DateTime> dates) => this(dates: dates);

  @override
  WaitlistEntryView timeWindows(List<WaitlistTimeWindow> timeWindows) =>
      this(timeWindows: timeWindows);

  @override
  WaitlistEntryView note(String? note) => this(note: note);

  @override
  WaitlistEntryView joinedUtc(DateTime joinedUtc) => this(joinedUtc: joinedUtc);

  @override
  WaitlistEntryView source_(WaitlistEntryViewSource_Enum source_) =>
      this(source_: source_);

  @override
  WaitlistEntryView pendingOffer(WaitlistOfferView? pendingOffer) =>
      this(pendingOffer: pendingOffer);

  @override
  WaitlistEntryView version(String version) => this(version: version);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `WaitlistEntryView(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// WaitlistEntryView(...).copyWith(id: 12, name: "My name")
  /// ````
  WaitlistEntryView call({
    Object? id = const $CopyWithPlaceholder(),
    Object? branchId = const $CopyWithPlaceholder(),
    Object? status = const $CopyWithPlaceholder(),
    Object? customer = const $CopyWithPlaceholder(),
    Object? serviceIds = const $CopyWithPlaceholder(),
    Object? serviceNames = const $CopyWithPlaceholder(),
    Object? preferredStaffId = const $CopyWithPlaceholder(),
    Object? preferredStaffName = const $CopyWithPlaceholder(),
    Object? acceptOtherStaff = const $CopyWithPlaceholder(),
    Object? dates = const $CopyWithPlaceholder(),
    Object? timeWindows = const $CopyWithPlaceholder(),
    Object? note = const $CopyWithPlaceholder(),
    Object? joinedUtc = const $CopyWithPlaceholder(),
    Object? source_ = const $CopyWithPlaceholder(),
    Object? pendingOffer = const $CopyWithPlaceholder(),
    Object? version = const $CopyWithPlaceholder(),
  }) {
    return WaitlistEntryView(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      branchId: branchId == const $CopyWithPlaceholder()
          ? _value.branchId
          // ignore: cast_nullable_to_non_nullable
          : branchId as String,
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as WaitlistEntryStatus,
      customer: customer == const $CopyWithPlaceholder()
          ? _value.customer
          // ignore: cast_nullable_to_non_nullable
          : customer as CustomerRef,
      serviceIds: serviceIds == const $CopyWithPlaceholder()
          ? _value.serviceIds
          // ignore: cast_nullable_to_non_nullable
          : serviceIds as List<String>,
      serviceNames: serviceNames == const $CopyWithPlaceholder()
          ? _value.serviceNames
          // ignore: cast_nullable_to_non_nullable
          : serviceNames as List<String>,
      preferredStaffId: preferredStaffId == const $CopyWithPlaceholder()
          ? _value.preferredStaffId
          // ignore: cast_nullable_to_non_nullable
          : preferredStaffId as String?,
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
      note: note == const $CopyWithPlaceholder()
          ? _value.note
          // ignore: cast_nullable_to_non_nullable
          : note as String?,
      joinedUtc: joinedUtc == const $CopyWithPlaceholder()
          ? _value.joinedUtc
          // ignore: cast_nullable_to_non_nullable
          : joinedUtc as DateTime,
      source_: source_ == const $CopyWithPlaceholder()
          ? _value.source_
          // ignore: cast_nullable_to_non_nullable
          : source_ as WaitlistEntryViewSource_Enum,
      pendingOffer: pendingOffer == const $CopyWithPlaceholder()
          ? _value.pendingOffer
          // ignore: cast_nullable_to_non_nullable
          : pendingOffer as WaitlistOfferView?,
      version: version == const $CopyWithPlaceholder()
          ? _value.version
          // ignore: cast_nullable_to_non_nullable
          : version as String,
    );
  }
}

extension $WaitlistEntryViewCopyWith on WaitlistEntryView {
  /// Returns a callable class that can be used as follows: `instanceOfWaitlistEntryView.copyWith(...)` or like so:`instanceOfWaitlistEntryView.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$WaitlistEntryViewCWProxy get copyWith =>
      _$WaitlistEntryViewCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

WaitlistEntryView _$WaitlistEntryViewFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('WaitlistEntryView', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'id',
      'branchId',
      'status',
      'customer',
      'serviceIds',
      'serviceNames',
      'acceptOtherStaff',
      'dates',
      'timeWindows',
      'joinedUtc',
      'source',
      'pendingOffer',
      'version',
    ],
  );
  final val = WaitlistEntryView(
    id: $checkedConvert('id', (v) => v as String),
    branchId: $checkedConvert('branchId', (v) => v as String),
    status: $checkedConvert(
      'status',
      (v) => $enumDecode(
        _$WaitlistEntryStatusEnumMap,
        v,
        unknownValue: WaitlistEntryStatus.unknownDefaultOpenApi,
      ),
    ),
    customer: $checkedConvert(
      'customer',
      (v) => CustomerRef.fromJson(v as Map<String, dynamic>),
    ),
    serviceIds: $checkedConvert(
      'serviceIds',
      (v) => (v as List<dynamic>).map((e) => e as String).toList(),
    ),
    serviceNames: $checkedConvert(
      'serviceNames',
      (v) => (v as List<dynamic>).map((e) => e as String).toList(),
    ),
    preferredStaffId: $checkedConvert('preferredStaffId', (v) => v as String?),
    preferredStaffName: $checkedConvert(
      'preferredStaffName',
      (v) => v as String?,
    ),
    acceptOtherStaff: $checkedConvert('acceptOtherStaff', (v) => v as bool),
    dates: $checkedConvert(
      'dates',
      (v) =>
          (v as List<dynamic>).map((e) => DateTime.parse(e as String)).toList(),
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
    note: $checkedConvert('note', (v) => v as String?),
    joinedUtc: $checkedConvert('joinedUtc', (v) => DateTime.parse(v as String)),
    source_: $checkedConvert(
      'source',
      (v) => $enumDecode(
        _$WaitlistEntryViewSource_EnumEnumMap,
        v,
        unknownValue: WaitlistEntryViewSource_Enum.unknownDefaultOpenApi,
      ),
    ),
    pendingOffer: $checkedConvert(
      'pendingOffer',
      (v) => v == null
          ? null
          : WaitlistOfferView.fromJson(v as Map<String, dynamic>),
    ),
    version: $checkedConvert('version', (v) => v as String),
  );
  return val;
}, fieldKeyMap: const {'source_': 'source'});

Map<String, dynamic> _$WaitlistEntryViewToJson(WaitlistEntryView instance) =>
    <String, dynamic>{
      'id': instance.id,
      'branchId': instance.branchId,
      'status': _$WaitlistEntryStatusEnumMap[instance.status]!,
      'customer': instance.customer.toJson(),
      'serviceIds': instance.serviceIds,
      'serviceNames': instance.serviceNames,
      'preferredStaffId': ?instance.preferredStaffId,
      'preferredStaffName': ?instance.preferredStaffName,
      'acceptOtherStaff': instance.acceptOtherStaff,
      'dates': instance.dates.map((e) => e.toIso8601String()).toList(),
      'timeWindows': instance.timeWindows
          .map((e) => _$WaitlistTimeWindowEnumMap[e]!)
          .toList(),
      'note': ?instance.note,
      'joinedUtc': instance.joinedUtc.toIso8601String(),
      'source': _$WaitlistEntryViewSource_EnumEnumMap[instance.source_]!,
      'pendingOffer': instance.pendingOffer?.toJson(),
      'version': instance.version,
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

const _$WaitlistEntryViewSource_EnumEnumMap = {
  WaitlistEntryViewSource_Enum.online: 'Online',
  WaitlistEntryViewSource_Enum.panel: 'Panel',
  WaitlistEntryViewSource_Enum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
