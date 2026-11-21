// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'me_appointment.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MeAppointmentCWProxy {
  MeAppointment id(String id);

  MeAppointment reference(String? reference);

  MeAppointment brand(MeBrandRef brand);

  MeAppointment branch(MeAppointmentBranch branch);

  MeAppointment timeZone(String timeZone);

  MeAppointment startUtc(DateTime startUtc);

  MeAppointment endUtc(DateTime endUtc);

  MeAppointment status(AppointmentStatus status);

  MeAppointment displayStatus(DisplayStatus displayStatus);

  MeAppointment holdExpiresUtc(DateTime? holdExpiresUtc);

  MeAppointment items(List<MeAppointmentItem> items);

  MeAppointment total(Money total);

  MeAppointment amountDue(Money amountDue);

  MeAppointment deposit(MeAppointmentDeposit? deposit);

  MeAppointment canCancel(bool canCancel);

  MeAppointment cancelDeadlineUtc(DateTime? cancelDeadlineUtc);

  MeAppointment canReschedule(bool canReschedule);

  MeAppointment rescheduleDeadlineUtc(DateTime? rescheduleDeadlineUtc);

  MeAppointment canReview(bool canReview);

  MeAppointment canRebook(bool canRebook);

  MeAppointment review(MeAppointmentReview? review);

  MeAppointment preAppointmentNote(String? preAppointmentNote);

  MeAppointment customerNote(String? customerNote);

  MeAppointment cancellation(PublicAppointmentCancellation? cancellation);

  MeAppointment version(String version);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeAppointment(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeAppointment(...).copyWith(id: 12, name: "My name")
  /// ````
  MeAppointment call({
    String id,
    String? reference,
    MeBrandRef brand,
    MeAppointmentBranch branch,
    String timeZone,
    DateTime startUtc,
    DateTime endUtc,
    AppointmentStatus status,
    DisplayStatus displayStatus,
    DateTime? holdExpiresUtc,
    List<MeAppointmentItem> items,
    Money total,
    Money amountDue,
    MeAppointmentDeposit? deposit,
    bool canCancel,
    DateTime? cancelDeadlineUtc,
    bool canReschedule,
    DateTime? rescheduleDeadlineUtc,
    bool canReview,
    bool canRebook,
    MeAppointmentReview? review,
    String? preAppointmentNote,
    String? customerNote,
    PublicAppointmentCancellation? cancellation,
    String version,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMeAppointment.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMeAppointment.copyWith.fieldName(...)`
class _$MeAppointmentCWProxyImpl implements _$MeAppointmentCWProxy {
  const _$MeAppointmentCWProxyImpl(this._value);

  final MeAppointment _value;

  @override
  MeAppointment id(String id) => this(id: id);

  @override
  MeAppointment reference(String? reference) => this(reference: reference);

  @override
  MeAppointment brand(MeBrandRef brand) => this(brand: brand);

  @override
  MeAppointment branch(MeAppointmentBranch branch) => this(branch: branch);

  @override
  MeAppointment timeZone(String timeZone) => this(timeZone: timeZone);

  @override
  MeAppointment startUtc(DateTime startUtc) => this(startUtc: startUtc);

  @override
  MeAppointment endUtc(DateTime endUtc) => this(endUtc: endUtc);

  @override
  MeAppointment status(AppointmentStatus status) => this(status: status);

  @override
  MeAppointment displayStatus(DisplayStatus displayStatus) =>
      this(displayStatus: displayStatus);

  @override
  MeAppointment holdExpiresUtc(DateTime? holdExpiresUtc) =>
      this(holdExpiresUtc: holdExpiresUtc);

  @override
  MeAppointment items(List<MeAppointmentItem> items) => this(items: items);

  @override
  MeAppointment total(Money total) => this(total: total);

  @override
  MeAppointment amountDue(Money amountDue) => this(amountDue: amountDue);

  @override
  MeAppointment deposit(MeAppointmentDeposit? deposit) =>
      this(deposit: deposit);

  @override
  MeAppointment canCancel(bool canCancel) => this(canCancel: canCancel);

  @override
  MeAppointment cancelDeadlineUtc(DateTime? cancelDeadlineUtc) =>
      this(cancelDeadlineUtc: cancelDeadlineUtc);

  @override
  MeAppointment canReschedule(bool canReschedule) =>
      this(canReschedule: canReschedule);

  @override
  MeAppointment rescheduleDeadlineUtc(DateTime? rescheduleDeadlineUtc) =>
      this(rescheduleDeadlineUtc: rescheduleDeadlineUtc);

  @override
  MeAppointment canReview(bool canReview) => this(canReview: canReview);

  @override
  MeAppointment canRebook(bool canRebook) => this(canRebook: canRebook);

  @override
  MeAppointment review(MeAppointmentReview? review) => this(review: review);

  @override
  MeAppointment preAppointmentNote(String? preAppointmentNote) =>
      this(preAppointmentNote: preAppointmentNote);

  @override
  MeAppointment customerNote(String? customerNote) =>
      this(customerNote: customerNote);

  @override
  MeAppointment cancellation(PublicAppointmentCancellation? cancellation) =>
      this(cancellation: cancellation);

  @override
  MeAppointment version(String version) => this(version: version);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeAppointment(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeAppointment(...).copyWith(id: 12, name: "My name")
  /// ````
  MeAppointment call({
    Object? id = const $CopyWithPlaceholder(),
    Object? reference = const $CopyWithPlaceholder(),
    Object? brand = const $CopyWithPlaceholder(),
    Object? branch = const $CopyWithPlaceholder(),
    Object? timeZone = const $CopyWithPlaceholder(),
    Object? startUtc = const $CopyWithPlaceholder(),
    Object? endUtc = const $CopyWithPlaceholder(),
    Object? status = const $CopyWithPlaceholder(),
    Object? displayStatus = const $CopyWithPlaceholder(),
    Object? holdExpiresUtc = const $CopyWithPlaceholder(),
    Object? items = const $CopyWithPlaceholder(),
    Object? total = const $CopyWithPlaceholder(),
    Object? amountDue = const $CopyWithPlaceholder(),
    Object? deposit = const $CopyWithPlaceholder(),
    Object? canCancel = const $CopyWithPlaceholder(),
    Object? cancelDeadlineUtc = const $CopyWithPlaceholder(),
    Object? canReschedule = const $CopyWithPlaceholder(),
    Object? rescheduleDeadlineUtc = const $CopyWithPlaceholder(),
    Object? canReview = const $CopyWithPlaceholder(),
    Object? canRebook = const $CopyWithPlaceholder(),
    Object? review = const $CopyWithPlaceholder(),
    Object? preAppointmentNote = const $CopyWithPlaceholder(),
    Object? customerNote = const $CopyWithPlaceholder(),
    Object? cancellation = const $CopyWithPlaceholder(),
    Object? version = const $CopyWithPlaceholder(),
  }) {
    return MeAppointment(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      reference: reference == const $CopyWithPlaceholder()
          ? _value.reference
          // ignore: cast_nullable_to_non_nullable
          : reference as String?,
      brand: brand == const $CopyWithPlaceholder()
          ? _value.brand
          // ignore: cast_nullable_to_non_nullable
          : brand as MeBrandRef,
      branch: branch == const $CopyWithPlaceholder()
          ? _value.branch
          // ignore: cast_nullable_to_non_nullable
          : branch as MeAppointmentBranch,
      timeZone: timeZone == const $CopyWithPlaceholder()
          ? _value.timeZone
          // ignore: cast_nullable_to_non_nullable
          : timeZone as String,
      startUtc: startUtc == const $CopyWithPlaceholder()
          ? _value.startUtc
          // ignore: cast_nullable_to_non_nullable
          : startUtc as DateTime,
      endUtc: endUtc == const $CopyWithPlaceholder()
          ? _value.endUtc
          // ignore: cast_nullable_to_non_nullable
          : endUtc as DateTime,
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as AppointmentStatus,
      displayStatus: displayStatus == const $CopyWithPlaceholder()
          ? _value.displayStatus
          // ignore: cast_nullable_to_non_nullable
          : displayStatus as DisplayStatus,
      holdExpiresUtc: holdExpiresUtc == const $CopyWithPlaceholder()
          ? _value.holdExpiresUtc
          // ignore: cast_nullable_to_non_nullable
          : holdExpiresUtc as DateTime?,
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<MeAppointmentItem>,
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
          : deposit as MeAppointmentDeposit?,
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
      canReview: canReview == const $CopyWithPlaceholder()
          ? _value.canReview
          // ignore: cast_nullable_to_non_nullable
          : canReview as bool,
      canRebook: canRebook == const $CopyWithPlaceholder()
          ? _value.canRebook
          // ignore: cast_nullable_to_non_nullable
          : canRebook as bool,
      review: review == const $CopyWithPlaceholder()
          ? _value.review
          // ignore: cast_nullable_to_non_nullable
          : review as MeAppointmentReview?,
      preAppointmentNote: preAppointmentNote == const $CopyWithPlaceholder()
          ? _value.preAppointmentNote
          // ignore: cast_nullable_to_non_nullable
          : preAppointmentNote as String?,
      customerNote: customerNote == const $CopyWithPlaceholder()
          ? _value.customerNote
          // ignore: cast_nullable_to_non_nullable
          : customerNote as String?,
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

extension $MeAppointmentCopyWith on MeAppointment {
  /// Returns a callable class that can be used as follows: `instanceOfMeAppointment.copyWith(...)` or like so:`instanceOfMeAppointment.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MeAppointmentCWProxy get copyWith => _$MeAppointmentCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MeAppointment _$MeAppointmentFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('MeAppointment', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'id',
      'brand',
      'branch',
      'timeZone',
      'startUtc',
      'endUtc',
      'status',
      'displayStatus',
      'items',
      'total',
      'amountDue',
      'deposit',
      'canCancel',
      'cancelDeadlineUtc',
      'canReschedule',
      'rescheduleDeadlineUtc',
      'canReview',
      'canRebook',
      'review',
      'cancellation',
      'version',
    ],
  );
  final val = MeAppointment(
    id: $checkedConvert('id', (v) => v as String),
    reference: $checkedConvert('reference', (v) => v as String?),
    brand: $checkedConvert(
      'brand',
      (v) => MeBrandRef.fromJson(v as Map<String, dynamic>),
    ),
    branch: $checkedConvert(
      'branch',
      (v) => MeAppointmentBranch.fromJson(v as Map<String, dynamic>),
    ),
    timeZone: $checkedConvert('timeZone', (v) => v as String),
    startUtc: $checkedConvert('startUtc', (v) => DateTime.parse(v as String)),
    endUtc: $checkedConvert('endUtc', (v) => DateTime.parse(v as String)),
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
    holdExpiresUtc: $checkedConvert(
      'holdExpiresUtc',
      (v) => v == null ? null : DateTime.parse(v as String),
    ),
    items: $checkedConvert(
      'items',
      (v) => (v as List<dynamic>)
          .map((e) => MeAppointmentItem.fromJson(e as Map<String, dynamic>))
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
          : MeAppointmentDeposit.fromJson(v as Map<String, dynamic>),
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
    canReview: $checkedConvert('canReview', (v) => v as bool),
    canRebook: $checkedConvert('canRebook', (v) => v as bool),
    review: $checkedConvert(
      'review',
      (v) => v == null
          ? null
          : MeAppointmentReview.fromJson(v as Map<String, dynamic>),
    ),
    preAppointmentNote: $checkedConvert(
      'preAppointmentNote',
      (v) => v as String?,
    ),
    customerNote: $checkedConvert('customerNote', (v) => v as String?),
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

Map<String, dynamic> _$MeAppointmentToJson(
  MeAppointment instance,
) => <String, dynamic>{
  'id': instance.id,
  'reference': ?instance.reference,
  'brand': instance.brand.toJson(),
  'branch': instance.branch.toJson(),
  'timeZone': instance.timeZone,
  'startUtc': instance.startUtc.toIso8601String(),
  'endUtc': instance.endUtc.toIso8601String(),
  'status': _$AppointmentStatusEnumMap[instance.status]!,
  'displayStatus': _$DisplayStatusEnumMap[instance.displayStatus]!,
  'holdExpiresUtc': ?instance.holdExpiresUtc?.toIso8601String(),
  'items': instance.items.map((e) => e.toJson()).toList(),
  'total': instance.total.toJson(),
  'amountDue': instance.amountDue.toJson(),
  'deposit': instance.deposit?.toJson(),
  'canCancel': instance.canCancel,
  'cancelDeadlineUtc': instance.cancelDeadlineUtc?.toIso8601String(),
  'canReschedule': instance.canReschedule,
  'rescheduleDeadlineUtc': instance.rescheduleDeadlineUtc?.toIso8601String(),
  'canReview': instance.canReview,
  'canRebook': instance.canRebook,
  'review': instance.review?.toJson(),
  'preAppointmentNote': ?instance.preAppointmentNote,
  'customerNote': ?instance.customerNote,
  'cancellation': instance.cancellation?.toJson(),
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
