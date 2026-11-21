// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'appointment_detail.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AppointmentDetailCWProxy {
  AppointmentDetail id(String id);

  AppointmentDetail reference(String? reference);

  AppointmentDetail branchId(String branchId);

  AppointmentDetail status(AppointmentStatus status);

  AppointmentDetail displayStatus(DisplayStatus displayStatus);

  AppointmentDetail channel(AppointmentChannel channel);

  AppointmentDetail startUtc(DateTime startUtc);

  AppointmentDetail endUtc(DateTime endUtc);

  AppointmentDetail timeZone(String timeZone);

  AppointmentDetail holdExpiresUtc(DateTime? holdExpiresUtc);

  AppointmentDetail customer(AppointmentDetailCustomer customer);

  AppointmentDetail items(List<AppointmentItemView> items);

  AppointmentDetail deposit(DepositView? deposit);

  AppointmentDetail total(Money total);

  AppointmentDetail amountDue(Money amountDue);

  AppointmentDetail customerNote(String? customerNote);

  AppointmentDetail staffNote(String? staffNote);

  AppointmentDetail isRecurring(bool isRecurring);

  AppointmentDetail seriesId(String? seriesId);

  AppointmentDetail noShowAtUtc(DateTime? noShowAtUtc);

  AppointmentDetail cancellation(AppointmentDetailCancellation? cancellation);

  AppointmentDetail completedUtc(DateTime? completedUtc);

  AppointmentDetail payments(List<PaymentView>? payments);

  AppointmentDetail history(List<AppointmentHistoryEntry> history);

  AppointmentDetail version(String version);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AppointmentDetail(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AppointmentDetail(...).copyWith(id: 12, name: "My name")
  /// ````
  AppointmentDetail call({
    String id,
    String? reference,
    String branchId,
    AppointmentStatus status,
    DisplayStatus displayStatus,
    AppointmentChannel channel,
    DateTime startUtc,
    DateTime endUtc,
    String timeZone,
    DateTime? holdExpiresUtc,
    AppointmentDetailCustomer customer,
    List<AppointmentItemView> items,
    DepositView? deposit,
    Money total,
    Money amountDue,
    String? customerNote,
    String? staffNote,
    bool isRecurring,
    String? seriesId,
    DateTime? noShowAtUtc,
    AppointmentDetailCancellation? cancellation,
    DateTime? completedUtc,
    List<PaymentView>? payments,
    List<AppointmentHistoryEntry> history,
    String version,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAppointmentDetail.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAppointmentDetail.copyWith.fieldName(...)`
class _$AppointmentDetailCWProxyImpl implements _$AppointmentDetailCWProxy {
  const _$AppointmentDetailCWProxyImpl(this._value);

  final AppointmentDetail _value;

  @override
  AppointmentDetail id(String id) => this(id: id);

  @override
  AppointmentDetail reference(String? reference) => this(reference: reference);

  @override
  AppointmentDetail branchId(String branchId) => this(branchId: branchId);

  @override
  AppointmentDetail status(AppointmentStatus status) => this(status: status);

  @override
  AppointmentDetail displayStatus(DisplayStatus displayStatus) =>
      this(displayStatus: displayStatus);

  @override
  AppointmentDetail channel(AppointmentChannel channel) =>
      this(channel: channel);

  @override
  AppointmentDetail startUtc(DateTime startUtc) => this(startUtc: startUtc);

  @override
  AppointmentDetail endUtc(DateTime endUtc) => this(endUtc: endUtc);

  @override
  AppointmentDetail timeZone(String timeZone) => this(timeZone: timeZone);

  @override
  AppointmentDetail holdExpiresUtc(DateTime? holdExpiresUtc) =>
      this(holdExpiresUtc: holdExpiresUtc);

  @override
  AppointmentDetail customer(AppointmentDetailCustomer customer) =>
      this(customer: customer);

  @override
  AppointmentDetail items(List<AppointmentItemView> items) =>
      this(items: items);

  @override
  AppointmentDetail deposit(DepositView? deposit) => this(deposit: deposit);

  @override
  AppointmentDetail total(Money total) => this(total: total);

  @override
  AppointmentDetail amountDue(Money amountDue) => this(amountDue: amountDue);

  @override
  AppointmentDetail customerNote(String? customerNote) =>
      this(customerNote: customerNote);

  @override
  AppointmentDetail staffNote(String? staffNote) => this(staffNote: staffNote);

  @override
  AppointmentDetail isRecurring(bool isRecurring) =>
      this(isRecurring: isRecurring);

  @override
  AppointmentDetail seriesId(String? seriesId) => this(seriesId: seriesId);

  @override
  AppointmentDetail noShowAtUtc(DateTime? noShowAtUtc) =>
      this(noShowAtUtc: noShowAtUtc);

  @override
  AppointmentDetail cancellation(AppointmentDetailCancellation? cancellation) =>
      this(cancellation: cancellation);

  @override
  AppointmentDetail completedUtc(DateTime? completedUtc) =>
      this(completedUtc: completedUtc);

  @override
  AppointmentDetail payments(List<PaymentView>? payments) =>
      this(payments: payments);

  @override
  AppointmentDetail history(List<AppointmentHistoryEntry> history) =>
      this(history: history);

  @override
  AppointmentDetail version(String version) => this(version: version);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AppointmentDetail(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AppointmentDetail(...).copyWith(id: 12, name: "My name")
  /// ````
  AppointmentDetail call({
    Object? id = const $CopyWithPlaceholder(),
    Object? reference = const $CopyWithPlaceholder(),
    Object? branchId = const $CopyWithPlaceholder(),
    Object? status = const $CopyWithPlaceholder(),
    Object? displayStatus = const $CopyWithPlaceholder(),
    Object? channel = const $CopyWithPlaceholder(),
    Object? startUtc = const $CopyWithPlaceholder(),
    Object? endUtc = const $CopyWithPlaceholder(),
    Object? timeZone = const $CopyWithPlaceholder(),
    Object? holdExpiresUtc = const $CopyWithPlaceholder(),
    Object? customer = const $CopyWithPlaceholder(),
    Object? items = const $CopyWithPlaceholder(),
    Object? deposit = const $CopyWithPlaceholder(),
    Object? total = const $CopyWithPlaceholder(),
    Object? amountDue = const $CopyWithPlaceholder(),
    Object? customerNote = const $CopyWithPlaceholder(),
    Object? staffNote = const $CopyWithPlaceholder(),
    Object? isRecurring = const $CopyWithPlaceholder(),
    Object? seriesId = const $CopyWithPlaceholder(),
    Object? noShowAtUtc = const $CopyWithPlaceholder(),
    Object? cancellation = const $CopyWithPlaceholder(),
    Object? completedUtc = const $CopyWithPlaceholder(),
    Object? payments = const $CopyWithPlaceholder(),
    Object? history = const $CopyWithPlaceholder(),
    Object? version = const $CopyWithPlaceholder(),
  }) {
    return AppointmentDetail(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      reference: reference == const $CopyWithPlaceholder()
          ? _value.reference
          // ignore: cast_nullable_to_non_nullable
          : reference as String?,
      branchId: branchId == const $CopyWithPlaceholder()
          ? _value.branchId
          // ignore: cast_nullable_to_non_nullable
          : branchId as String,
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as AppointmentStatus,
      displayStatus: displayStatus == const $CopyWithPlaceholder()
          ? _value.displayStatus
          // ignore: cast_nullable_to_non_nullable
          : displayStatus as DisplayStatus,
      channel: channel == const $CopyWithPlaceholder()
          ? _value.channel
          // ignore: cast_nullable_to_non_nullable
          : channel as AppointmentChannel,
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
      customer: customer == const $CopyWithPlaceholder()
          ? _value.customer
          // ignore: cast_nullable_to_non_nullable
          : customer as AppointmentDetailCustomer,
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<AppointmentItemView>,
      deposit: deposit == const $CopyWithPlaceholder()
          ? _value.deposit
          // ignore: cast_nullable_to_non_nullable
          : deposit as DepositView?,
      total: total == const $CopyWithPlaceholder()
          ? _value.total
          // ignore: cast_nullable_to_non_nullable
          : total as Money,
      amountDue: amountDue == const $CopyWithPlaceholder()
          ? _value.amountDue
          // ignore: cast_nullable_to_non_nullable
          : amountDue as Money,
      customerNote: customerNote == const $CopyWithPlaceholder()
          ? _value.customerNote
          // ignore: cast_nullable_to_non_nullable
          : customerNote as String?,
      staffNote: staffNote == const $CopyWithPlaceholder()
          ? _value.staffNote
          // ignore: cast_nullable_to_non_nullable
          : staffNote as String?,
      isRecurring: isRecurring == const $CopyWithPlaceholder()
          ? _value.isRecurring
          // ignore: cast_nullable_to_non_nullable
          : isRecurring as bool,
      seriesId: seriesId == const $CopyWithPlaceholder()
          ? _value.seriesId
          // ignore: cast_nullable_to_non_nullable
          : seriesId as String?,
      noShowAtUtc: noShowAtUtc == const $CopyWithPlaceholder()
          ? _value.noShowAtUtc
          // ignore: cast_nullable_to_non_nullable
          : noShowAtUtc as DateTime?,
      cancellation: cancellation == const $CopyWithPlaceholder()
          ? _value.cancellation
          // ignore: cast_nullable_to_non_nullable
          : cancellation as AppointmentDetailCancellation?,
      completedUtc: completedUtc == const $CopyWithPlaceholder()
          ? _value.completedUtc
          // ignore: cast_nullable_to_non_nullable
          : completedUtc as DateTime?,
      payments: payments == const $CopyWithPlaceholder()
          ? _value.payments
          // ignore: cast_nullable_to_non_nullable
          : payments as List<PaymentView>?,
      history: history == const $CopyWithPlaceholder()
          ? _value.history
          // ignore: cast_nullable_to_non_nullable
          : history as List<AppointmentHistoryEntry>,
      version: version == const $CopyWithPlaceholder()
          ? _value.version
          // ignore: cast_nullable_to_non_nullable
          : version as String,
    );
  }
}

extension $AppointmentDetailCopyWith on AppointmentDetail {
  /// Returns a callable class that can be used as follows: `instanceOfAppointmentDetail.copyWith(...)` or like so:`instanceOfAppointmentDetail.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AppointmentDetailCWProxy get copyWith =>
      _$AppointmentDetailCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AppointmentDetail _$AppointmentDetailFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AppointmentDetail', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'id',
      'branchId',
      'status',
      'displayStatus',
      'channel',
      'startUtc',
      'endUtc',
      'timeZone',
      'customer',
      'items',
      'total',
      'amountDue',
      'isRecurring',
      'history',
      'version',
    ],
  );
  final val = AppointmentDetail(
    id: $checkedConvert('id', (v) => v as String),
    reference: $checkedConvert('reference', (v) => v as String?),
    branchId: $checkedConvert('branchId', (v) => v as String),
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
    channel: $checkedConvert(
      'channel',
      (v) => $enumDecode(
        _$AppointmentChannelEnumMap,
        v,
        unknownValue: AppointmentChannel.unknownDefaultOpenApi,
      ),
    ),
    startUtc: $checkedConvert('startUtc', (v) => DateTime.parse(v as String)),
    endUtc: $checkedConvert('endUtc', (v) => DateTime.parse(v as String)),
    timeZone: $checkedConvert('timeZone', (v) => v as String),
    holdExpiresUtc: $checkedConvert(
      'holdExpiresUtc',
      (v) => v == null ? null : DateTime.parse(v as String),
    ),
    customer: $checkedConvert(
      'customer',
      (v) => AppointmentDetailCustomer.fromJson(v as Map<String, dynamic>),
    ),
    items: $checkedConvert(
      'items',
      (v) => (v as List<dynamic>)
          .map((e) => AppointmentItemView.fromJson(e as Map<String, dynamic>))
          .toList(),
    ),
    deposit: $checkedConvert(
      'deposit',
      (v) => v == null ? null : DepositView.fromJson(v as Map<String, dynamic>),
    ),
    total: $checkedConvert(
      'total',
      (v) => Money.fromJson(v as Map<String, dynamic>),
    ),
    amountDue: $checkedConvert(
      'amountDue',
      (v) => Money.fromJson(v as Map<String, dynamic>),
    ),
    customerNote: $checkedConvert('customerNote', (v) => v as String?),
    staffNote: $checkedConvert('staffNote', (v) => v as String?),
    isRecurring: $checkedConvert('isRecurring', (v) => v as bool),
    seriesId: $checkedConvert('seriesId', (v) => v as String?),
    noShowAtUtc: $checkedConvert(
      'noShowAtUtc',
      (v) => v == null ? null : DateTime.parse(v as String),
    ),
    cancellation: $checkedConvert(
      'cancellation',
      (v) => v == null
          ? null
          : AppointmentDetailCancellation.fromJson(v as Map<String, dynamic>),
    ),
    completedUtc: $checkedConvert(
      'completedUtc',
      (v) => v == null ? null : DateTime.parse(v as String),
    ),
    payments: $checkedConvert(
      'payments',
      (v) => (v as List<dynamic>?)
          ?.map((e) => PaymentView.fromJson(e as Map<String, dynamic>))
          .toList(),
    ),
    history: $checkedConvert(
      'history',
      (v) => (v as List<dynamic>)
          .map(
            (e) => AppointmentHistoryEntry.fromJson(e as Map<String, dynamic>),
          )
          .toList(),
    ),
    version: $checkedConvert('version', (v) => v as String),
  );
  return val;
});

Map<String, dynamic> _$AppointmentDetailToJson(AppointmentDetail instance) =>
    <String, dynamic>{
      'id': instance.id,
      'reference': ?instance.reference,
      'branchId': instance.branchId,
      'status': _$AppointmentStatusEnumMap[instance.status]!,
      'displayStatus': _$DisplayStatusEnumMap[instance.displayStatus]!,
      'channel': _$AppointmentChannelEnumMap[instance.channel]!,
      'startUtc': instance.startUtc.toIso8601String(),
      'endUtc': instance.endUtc.toIso8601String(),
      'timeZone': instance.timeZone,
      'holdExpiresUtc': ?instance.holdExpiresUtc?.toIso8601String(),
      'customer': instance.customer.toJson(),
      'items': instance.items.map((e) => e.toJson()).toList(),
      'deposit': ?instance.deposit?.toJson(),
      'total': instance.total.toJson(),
      'amountDue': instance.amountDue.toJson(),
      'customerNote': ?instance.customerNote,
      'staffNote': ?instance.staffNote,
      'isRecurring': instance.isRecurring,
      'seriesId': ?instance.seriesId,
      'noShowAtUtc': ?instance.noShowAtUtc?.toIso8601String(),
      'cancellation': ?instance.cancellation?.toJson(),
      'completedUtc': ?instance.completedUtc?.toIso8601String(),
      'payments': ?instance.payments?.map((e) => e.toJson()).toList(),
      'history': instance.history.map((e) => e.toJson()).toList(),
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

const _$AppointmentChannelEnumMap = {
  AppointmentChannel.online: 'Online',
  AppointmentChannel.panel: 'Panel',
  AppointmentChannel.phone: 'Phone',
  AppointmentChannel.walkIn: 'WalkIn',
  AppointmentChannel.unknownDefaultOpenApi: 'unknown_default_open_api',
};
