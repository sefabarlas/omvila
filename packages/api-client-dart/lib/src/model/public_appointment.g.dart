// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_appointment.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PublicAppointmentCWProxy {
  PublicAppointment id(String id);

  PublicAppointment accessCode(String accessCode);

  PublicAppointment manageUrl(String manageUrl);

  PublicAppointment status(AppointmentStatus status);

  PublicAppointment displayStatus(DisplayStatus displayStatus);

  PublicAppointment brand(PublicAppointmentBrand brand);

  PublicAppointment branch(PublicAppointmentBranch branch);

  PublicAppointment startUtc(DateTime startUtc);

  PublicAppointment endUtc(DateTime endUtc);

  PublicAppointment timeZone(String timeZone);

  PublicAppointment holdExpiresUtc(DateTime? holdExpiresUtc);

  PublicAppointment items(List<PublicAppointmentItemsInner> items);

  PublicAppointment total(Money total);

  PublicAppointment amountDue(Money amountDue);

  PublicAppointment deposit(PublicAppointmentDeposit? deposit);

  PublicAppointment canCancel(bool canCancel);

  PublicAppointment cancelDeadlineUtc(DateTime? cancelDeadlineUtc);

  PublicAppointment canReschedule(bool canReschedule);

  PublicAppointment rescheduleDeadlineUtc(DateTime? rescheduleDeadlineUtc);

  PublicAppointment preAppointmentNote(String? preAppointmentNote);

  PublicAppointment cancellation(PublicAppointmentCancellation? cancellation);

  PublicAppointment version(String version);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicAppointment(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicAppointment(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicAppointment call({
    String id,
    String accessCode,
    String manageUrl,
    AppointmentStatus status,
    DisplayStatus displayStatus,
    PublicAppointmentBrand brand,
    PublicAppointmentBranch branch,
    DateTime startUtc,
    DateTime endUtc,
    String timeZone,
    DateTime? holdExpiresUtc,
    List<PublicAppointmentItemsInner> items,
    Money total,
    Money amountDue,
    PublicAppointmentDeposit? deposit,
    bool canCancel,
    DateTime? cancelDeadlineUtc,
    bool canReschedule,
    DateTime? rescheduleDeadlineUtc,
    String? preAppointmentNote,
    PublicAppointmentCancellation? cancellation,
    String version,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPublicAppointment.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPublicAppointment.copyWith.fieldName(...)`
class _$PublicAppointmentCWProxyImpl implements _$PublicAppointmentCWProxy {
  const _$PublicAppointmentCWProxyImpl(this._value);

  final PublicAppointment _value;

  @override
  PublicAppointment id(String id) => this(id: id);

  @override
  PublicAppointment accessCode(String accessCode) =>
      this(accessCode: accessCode);

  @override
  PublicAppointment manageUrl(String manageUrl) => this(manageUrl: manageUrl);

  @override
  PublicAppointment status(AppointmentStatus status) => this(status: status);

  @override
  PublicAppointment displayStatus(DisplayStatus displayStatus) =>
      this(displayStatus: displayStatus);

  @override
  PublicAppointment brand(PublicAppointmentBrand brand) => this(brand: brand);

  @override
  PublicAppointment branch(PublicAppointmentBranch branch) =>
      this(branch: branch);

  @override
  PublicAppointment startUtc(DateTime startUtc) => this(startUtc: startUtc);

  @override
  PublicAppointment endUtc(DateTime endUtc) => this(endUtc: endUtc);

  @override
  PublicAppointment timeZone(String timeZone) => this(timeZone: timeZone);

  @override
  PublicAppointment holdExpiresUtc(DateTime? holdExpiresUtc) =>
      this(holdExpiresUtc: holdExpiresUtc);

  @override
  PublicAppointment items(List<PublicAppointmentItemsInner> items) =>
      this(items: items);

  @override
  PublicAppointment total(Money total) => this(total: total);

  @override
  PublicAppointment amountDue(Money amountDue) => this(amountDue: amountDue);

  @override
  PublicAppointment deposit(PublicAppointmentDeposit? deposit) =>
      this(deposit: deposit);

  @override
  PublicAppointment canCancel(bool canCancel) => this(canCancel: canCancel);

  @override
  PublicAppointment cancelDeadlineUtc(DateTime? cancelDeadlineUtc) =>
      this(cancelDeadlineUtc: cancelDeadlineUtc);

  @override
  PublicAppointment canReschedule(bool canReschedule) =>
      this(canReschedule: canReschedule);

  @override
  PublicAppointment rescheduleDeadlineUtc(DateTime? rescheduleDeadlineUtc) =>
      this(rescheduleDeadlineUtc: rescheduleDeadlineUtc);

  @override
  PublicAppointment preAppointmentNote(String? preAppointmentNote) =>
      this(preAppointmentNote: preAppointmentNote);

  @override
  PublicAppointment cancellation(PublicAppointmentCancellation? cancellation) =>
      this(cancellation: cancellation);

  @override
  PublicAppointment version(String version) => this(version: version);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicAppointment(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicAppointment(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicAppointment call({
    Object? id = const $CopyWithPlaceholder(),
    Object? accessCode = const $CopyWithPlaceholder(),
    Object? manageUrl = const $CopyWithPlaceholder(),
    Object? status = const $CopyWithPlaceholder(),
    Object? displayStatus = const $CopyWithPlaceholder(),
    Object? brand = const $CopyWithPlaceholder(),
    Object? branch = const $CopyWithPlaceholder(),
    Object? startUtc = const $CopyWithPlaceholder(),
    Object? endUtc = const $CopyWithPlaceholder(),
    Object? timeZone = const $CopyWithPlaceholder(),
    Object? holdExpiresUtc = const $CopyWithPlaceholder(),
    Object? items = const $CopyWithPlaceholder(),
    Object? total = const $CopyWithPlaceholder(),
    Object? amountDue = const $CopyWithPlaceholder(),
    Object? deposit = const $CopyWithPlaceholder(),
    Object? canCancel = const $CopyWithPlaceholder(),
    Object? cancelDeadlineUtc = const $CopyWithPlaceholder(),
    Object? canReschedule = const $CopyWithPlaceholder(),
    Object? rescheduleDeadlineUtc = const $CopyWithPlaceholder(),
    Object? preAppointmentNote = const $CopyWithPlaceholder(),
    Object? cancellation = const $CopyWithPlaceholder(),
    Object? version = const $CopyWithPlaceholder(),
  }) {
    return PublicAppointment(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      accessCode: accessCode == const $CopyWithPlaceholder()
          ? _value.accessCode
          // ignore: cast_nullable_to_non_nullable
          : accessCode as String,
      manageUrl: manageUrl == const $CopyWithPlaceholder()
          ? _value.manageUrl
          // ignore: cast_nullable_to_non_nullable
          : manageUrl as String,
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as AppointmentStatus,
      displayStatus: displayStatus == const $CopyWithPlaceholder()
          ? _value.displayStatus
          // ignore: cast_nullable_to_non_nullable
          : displayStatus as DisplayStatus,
      brand: brand == const $CopyWithPlaceholder()
          ? _value.brand
          // ignore: cast_nullable_to_non_nullable
          : brand as PublicAppointmentBrand,
      branch: branch == const $CopyWithPlaceholder()
          ? _value.branch
          // ignore: cast_nullable_to_non_nullable
          : branch as PublicAppointmentBranch,
      startUtc: startUtc == const $CopyWithPlaceholder()
          ? _value.startUtc
          // ignore: cast_nullable_to_non_nullable
          : startUtc as DateTime,
      endUtc: endUtc == const $CopyWithPlaceholder()
          ? _value.endUtc
          // ignore: cast_nullable_to_non_nullable
          : endUtc as DateTime,
      timeZone: timeZone == const $CopyWithPlaceholder()
          ? _value.timeZone
          // ignore: cast_nullable_to_non_nullable
          : timeZone as String,
      holdExpiresUtc: holdExpiresUtc == const $CopyWithPlaceholder()
          ? _value.holdExpiresUtc
          // ignore: cast_nullable_to_non_nullable
          : holdExpiresUtc as DateTime?,
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<PublicAppointmentItemsInner>,
      total: total == const $CopyWithPlaceholder()
          ? _value.total
          // ignore: cast_nullable_to_non_nullable
          : total as Money,
      amountDue: amountDue == const $CopyWithPlaceholder()
          ? _value.amountDue
          // ignore: cast_nullable_to_non_nullable
          : amountDue as Money,
      deposit: deposit == const $CopyWithPlaceholder()
          ? _value.deposit
          // ignore: cast_nullable_to_non_nullable
          : deposit as PublicAppointmentDeposit?,
      canCancel: canCancel == const $CopyWithPlaceholder()
          ? _value.canCancel
          // ignore: cast_nullable_to_non_nullable
          : canCancel as bool,
      cancelDeadlineUtc: cancelDeadlineUtc == const $CopyWithPlaceholder()
          ? _value.cancelDeadlineUtc
          // ignore: cast_nullable_to_non_nullable
          : cancelDeadlineUtc as DateTime?,
      canReschedule: canReschedule == const $CopyWithPlaceholder()
          ? _value.canReschedule
          // ignore: cast_nullable_to_non_nullable
          : canReschedule as bool,
      rescheduleDeadlineUtc:
          rescheduleDeadlineUtc == const $CopyWithPlaceholder()
          ? _value.rescheduleDeadlineUtc
          // ignore: cast_nullable_to_non_nullable
          : rescheduleDeadlineUtc as DateTime?,
      preAppointmentNote: preAppointmentNote == const $CopyWithPlaceholder()
          ? _value.preAppointmentNote
          // ignore: cast_nullable_to_non_nullable
          : preAppointmentNote as String?,
      cancellation: cancellation == const $CopyWithPlaceholder()
          ? _value.cancellation
          // ignore: cast_nullable_to_non_nullable
          : cancellation as PublicAppointmentCancellation?,
      version: version == const $CopyWithPlaceholder()
          ? _value.version
          // ignore: cast_nullable_to_non_nullable
          : version as String,
    );
  }
}

extension $PublicAppointmentCopyWith on PublicAppointment {
  /// Returns a callable class that can be used as follows: `instanceOfPublicAppointment.copyWith(...)` or like so:`instanceOfPublicAppointment.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PublicAppointmentCWProxy get copyWith =>
      _$PublicAppointmentCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PublicAppointment _$PublicAppointmentFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('PublicAppointment', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'id',
      'accessCode',
      'manageUrl',
      'status',
      'displayStatus',
      'brand',
      'branch',
      'startUtc',
      'endUtc',
      'timeZone',
      'items',
      'total',
      'amountDue',
      'canCancel',
      'canReschedule',
      'version',
    ],
  );
  final val = PublicAppointment(
    id: $checkedConvert('id', (v) => v as String),
    accessCode: $checkedConvert('accessCode', (v) => v as String),
    manageUrl: $checkedConvert('manageUrl', (v) => v as String),
    status: $checkedConvert(
      'status',
      (v) => $enumDecode(
        _$AppointmentStatusEnumMap,
        v,
        unknownValue: AppointmentStatus.unknownDefaultOpenApi,
      ),
    ),
    displayStatus: $checkedConvert(
      'displayStatus',
      (v) => $enumDecode(
        _$DisplayStatusEnumMap,
        v,
        unknownValue: DisplayStatus.unknownDefaultOpenApi,
      ),
    ),
    brand: $checkedConvert(
      'brand',
      (v) => PublicAppointmentBrand.fromJson(v as Map<String, dynamic>),
    ),
    branch: $checkedConvert(
      'branch',
      (v) => PublicAppointmentBranch.fromJson(v as Map<String, dynamic>),
    ),
    startUtc: $checkedConvert('startUtc', (v) => DateTime.parse(v as String)),
    endUtc: $checkedConvert('endUtc', (v) => DateTime.parse(v as String)),
    timeZone: $checkedConvert('timeZone', (v) => v as String),
    holdExpiresUtc: $checkedConvert(
      'holdExpiresUtc',
      (v) => v == null ? null : DateTime.parse(v as String),
    ),
    items: $checkedConvert(
      'items',
      (v) => (v as List<dynamic>)
          .map(
            (e) =>
                PublicAppointmentItemsInner.fromJson(e as Map<String, dynamic>),
          )
          .toList(),
    ),
    total: $checkedConvert(
      'total',
      (v) => Money.fromJson(v as Map<String, dynamic>),
    ),
    amountDue: $checkedConvert(
      'amountDue',
      (v) => Money.fromJson(v as Map<String, dynamic>),
    ),
    deposit: $checkedConvert(
      'deposit',
      (v) => v == null
          ? null
          : PublicAppointmentDeposit.fromJson(v as Map<String, dynamic>),
    ),
    canCancel: $checkedConvert('canCancel', (v) => v as bool),
    cancelDeadlineUtc: $checkedConvert(
      'cancelDeadlineUtc',
      (v) => v == null ? null : DateTime.parse(v as String),
    ),
    canReschedule: $checkedConvert('canReschedule', (v) => v as bool),
    rescheduleDeadlineUtc: $checkedConvert(
      'rescheduleDeadlineUtc',
      (v) => v == null ? null : DateTime.parse(v as String),
    ),
    preAppointmentNote: $checkedConvert(
      'preAppointmentNote',
      (v) => v as String?,
    ),
    cancellation: $checkedConvert(
      'cancellation',
      (v) => v == null
          ? null
          : PublicAppointmentCancellation.fromJson(v as Map<String, dynamic>),
    ),
    version: $checkedConvert('version', (v) => v as String),
  );
  return val;
});

Map<String, dynamic> _$PublicAppointmentToJson(
  PublicAppointment instance,
) => <String, dynamic>{
  'id': instance.id,
  'accessCode': instance.accessCode,
  'manageUrl': instance.manageUrl,
  'status': _$AppointmentStatusEnumMap[instance.status]!,
  'displayStatus': _$DisplayStatusEnumMap[instance.displayStatus]!,
  'brand': instance.brand.toJson(),
  'branch': instance.branch.toJson(),
  'startUtc': instance.startUtc.toIso8601String(),
  'endUtc': instance.endUtc.toIso8601String(),
  'timeZone': instance.timeZone,
  'holdExpiresUtc': ?instance.holdExpiresUtc?.toIso8601String(),
  'items': instance.items.map((e) => e.toJson()).toList(),
  'total': instance.total.toJson(),
  'amountDue': instance.amountDue.toJson(),
  'deposit': ?instance.deposit?.toJson(),
  'canCancel': instance.canCancel,
  'cancelDeadlineUtc': ?instance.cancelDeadlineUtc?.toIso8601String(),
  'canReschedule': instance.canReschedule,
  'rescheduleDeadlineUtc': ?instance.rescheduleDeadlineUtc?.toIso8601String(),
  'preAppointmentNote': ?instance.preAppointmentNote,
  'cancellation': ?instance.cancellation?.toJson(),
  'version': instance.version,
};

const _$AppointmentStatusEnumMap = {
  AppointmentStatus.draft: 'Draft',
  AppointmentStatus.pendingApproval: 'PendingApproval',
  AppointmentStatus.confirmed: 'Confirmed',
  AppointmentStatus.checkedIn: 'CheckedIn',
  AppointmentStatus.completed: 'Completed',
  AppointmentStatus.cancelled: 'Cancelled',
  AppointmentStatus.noShow: 'NoShow',
  AppointmentStatus.unknownDefaultOpenApi: 'unknown_default_open_api',
};

const _$DisplayStatusEnumMap = {
  DisplayStatus.pendingApproval: 'PendingApproval',
  DisplayStatus.depositPending: 'DepositPending',
  DisplayStatus.confirmed: 'Confirmed',
  DisplayStatus.checkedIn: 'CheckedIn',
  DisplayStatus.completed: 'Completed',
  DisplayStatus.cancelled: 'Cancelled',
  DisplayStatus.noShow: 'NoShow',
  DisplayStatus.unknownDefaultOpenApi: 'unknown_default_open_api',
};
