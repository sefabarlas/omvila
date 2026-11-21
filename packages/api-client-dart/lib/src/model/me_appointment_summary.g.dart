// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'me_appointment_summary.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MeAppointmentSummaryCWProxy {
  MeAppointmentSummary id(String id);

  MeAppointmentSummary brand(MeBrandRef brand);

  MeAppointmentSummary branch(MeBranchRef branch);

  MeAppointmentSummary timeZone(String timeZone);

  MeAppointmentSummary startUtc(DateTime startUtc);

  MeAppointmentSummary endUtc(DateTime endUtc);

  MeAppointmentSummary status(AppointmentStatus status);

  MeAppointmentSummary displayStatus(DisplayStatus displayStatus);

  MeAppointmentSummary holdExpiresUtc(DateTime? holdExpiresUtc);

  MeAppointmentSummary depositStatus(DepositStatus? depositStatus);

  MeAppointmentSummary items(List<MeAppointmentItem> items);

  MeAppointmentSummary total(Money total);

  MeAppointmentSummary amountDue(Money amountDue);

  MeAppointmentSummary review(MeAppointmentReview? review);

  MeAppointmentSummary canReview(bool canReview);

  MeAppointmentSummary canRebook(bool canRebook);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeAppointmentSummary(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeAppointmentSummary(...).copyWith(id: 12, name: "My name")
  /// ````
  MeAppointmentSummary call({
    String id,
    MeBrandRef brand,
    MeBranchRef branch,
    String timeZone,
    DateTime startUtc,
    DateTime endUtc,
    AppointmentStatus status,
    DisplayStatus displayStatus,
    DateTime? holdExpiresUtc,
    DepositStatus? depositStatus,
    List<MeAppointmentItem> items,
    Money total,
    Money amountDue,
    MeAppointmentReview? review,
    bool canReview,
    bool canRebook,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMeAppointmentSummary.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMeAppointmentSummary.copyWith.fieldName(...)`
class _$MeAppointmentSummaryCWProxyImpl
    implements _$MeAppointmentSummaryCWProxy {
  const _$MeAppointmentSummaryCWProxyImpl(this._value);

  final MeAppointmentSummary _value;

  @override
  MeAppointmentSummary id(String id) => this(id: id);

  @override
  MeAppointmentSummary brand(MeBrandRef brand) => this(brand: brand);

  @override
  MeAppointmentSummary branch(MeBranchRef branch) => this(branch: branch);

  @override
  MeAppointmentSummary timeZone(String timeZone) => this(timeZone: timeZone);

  @override
  MeAppointmentSummary startUtc(DateTime startUtc) => this(startUtc: startUtc);

  @override
  MeAppointmentSummary endUtc(DateTime endUtc) => this(endUtc: endUtc);

  @override
  MeAppointmentSummary status(AppointmentStatus status) => this(status: status);

  @override
  MeAppointmentSummary displayStatus(DisplayStatus displayStatus) =>
      this(displayStatus: displayStatus);

  @override
  MeAppointmentSummary holdExpiresUtc(DateTime? holdExpiresUtc) =>
      this(holdExpiresUtc: holdExpiresUtc);

  @override
  MeAppointmentSummary depositStatus(DepositStatus? depositStatus) =>
      this(depositStatus: depositStatus);

  @override
  MeAppointmentSummary items(List<MeAppointmentItem> items) =>
      this(items: items);

  @override
  MeAppointmentSummary total(Money total) => this(total: total);

  @override
  MeAppointmentSummary amountDue(Money amountDue) => this(amountDue: amountDue);

  @override
  MeAppointmentSummary review(MeAppointmentReview? review) =>
      this(review: review);

  @override
  MeAppointmentSummary canReview(bool canReview) => this(canReview: canReview);

  @override
  MeAppointmentSummary canRebook(bool canRebook) => this(canRebook: canRebook);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeAppointmentSummary(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeAppointmentSummary(...).copyWith(id: 12, name: "My name")
  /// ````
  MeAppointmentSummary call({
    Object? id = const $CopyWithPlaceholder(),
    Object? brand = const $CopyWithPlaceholder(),
    Object? branch = const $CopyWithPlaceholder(),
    Object? timeZone = const $CopyWithPlaceholder(),
    Object? startUtc = const $CopyWithPlaceholder(),
    Object? endUtc = const $CopyWithPlaceholder(),
    Object? status = const $CopyWithPlaceholder(),
    Object? displayStatus = const $CopyWithPlaceholder(),
    Object? holdExpiresUtc = const $CopyWithPlaceholder(),
    Object? depositStatus = const $CopyWithPlaceholder(),
    Object? items = const $CopyWithPlaceholder(),
    Object? total = const $CopyWithPlaceholder(),
    Object? amountDue = const $CopyWithPlaceholder(),
    Object? review = const $CopyWithPlaceholder(),
    Object? canReview = const $CopyWithPlaceholder(),
    Object? canRebook = const $CopyWithPlaceholder(),
  }) {
    return MeAppointmentSummary(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      brand: brand == const $CopyWithPlaceholder()
          ? _value.brand
          // ignore: cast_nullable_to_non_nullable
          : brand as MeBrandRef,
      branch: branch == const $CopyWithPlaceholder()
          ? _value.branch
          // ignore: cast_nullable_to_non_nullable
          : branch as MeBranchRef,
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
      depositStatus: depositStatus == const $CopyWithPlaceholder()
          ? _value.depositStatus
          // ignore: cast_nullable_to_non_nullable
          : depositStatus as DepositStatus?,
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
      review: review == const $CopyWithPlaceholder()
          ? _value.review
          // ignore: cast_nullable_to_non_nullable
          : review as MeAppointmentReview?,
      canReview: canReview == const $CopyWithPlaceholder()
          ? _value.canReview
          // ignore: cast_nullable_to_non_nullable
          : canReview as bool,
      canRebook: canRebook == const $CopyWithPlaceholder()
          ? _value.canRebook
          // ignore: cast_nullable_to_non_nullable
          : canRebook as bool,
    );
  }
}

extension $MeAppointmentSummaryCopyWith on MeAppointmentSummary {
  /// Returns a callable class that can be used as follows: `instanceOfMeAppointmentSummary.copyWith(...)` or like so:`instanceOfMeAppointmentSummary.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MeAppointmentSummaryCWProxy get copyWith =>
      _$MeAppointmentSummaryCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MeAppointmentSummary _$MeAppointmentSummaryFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('MeAppointmentSummary', json, ($checkedConvert) {
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
      'review',
      'canReview',
      'canRebook',
    ],
  );
  final val = MeAppointmentSummary(
    id: $checkedConvert('id', (v) => v as String),
    brand: $checkedConvert(
      'brand',
      (v) => MeBrandRef.fromJson(v as Map<String, dynamic>),
    ),
    branch: $checkedConvert(
      'branch',
      (v) => MeBranchRef.fromJson(v as Map<String, dynamic>),
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
    depositStatus: $checkedConvert(
      'depositStatus',
      (v) => $enumDecodeNullable(
        _$DepositStatusEnumMap,
        v,
        unknownValue: DepositStatus.unknownDefaultOpenApi,
      ),
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
    review: $checkedConvert(
      'review',
      (v) => v == null
          ? null
          : MeAppointmentReview.fromJson(v as Map<String, dynamic>),
    ),
    canReview: $checkedConvert('canReview', (v) => v as bool),
    canRebook: $checkedConvert('canRebook', (v) => v as bool),
  );
  return val;
});

Map<String, dynamic> _$MeAppointmentSummaryToJson(
  MeAppointmentSummary instance,
) => <String, dynamic>{
  'id': instance.id,
  'brand': instance.brand.toJson(),
  'branch': instance.branch.toJson(),
  'timeZone': instance.timeZone,
  'startUtc': instance.startUtc.toIso8601String(),
  'endUtc': instance.endUtc.toIso8601String(),
  'status': _$AppointmentStatusEnumMap[instance.status]!,
  'displayStatus': _$DisplayStatusEnumMap[instance.displayStatus]!,
  'holdExpiresUtc': ?instance.holdExpiresUtc?.toIso8601String(),
  'depositStatus': ?_$DepositStatusEnumMap[instance.depositStatus],
  'items': instance.items.map((e) => e.toJson()).toList(),
  'total': instance.total.toJson(),
  'amountDue': instance.amountDue.toJson(),
  'review': instance.review?.toJson(),
  'canReview': instance.canReview,
  'canRebook': instance.canRebook,
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

const _$DepositStatusEnumMap = {
  DepositStatus.awaiting: 'Awaiting',
  DepositStatus.customerReported: 'CustomerReported',
  DepositStatus.received: 'Received',
  DepositStatus.refunded: 'Refunded',
  DepositStatus.waived: 'Waived',
  DepositStatus.unknownDefaultOpenApi: 'unknown_default_open_api',
};
