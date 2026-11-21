// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'cash_day.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CashDayCWProxy {
  CashDay branchId(String branchId);

  CashDay date(DateTime date);

  CashDay timeZone(String timeZone);

  CashDay status(CashDayStatusEnum status);

  CashDay total(CashMethodTotal total);

  CashDay cash(CashMethodTotal cash);

  CashDay card(CashMethodTotal card);

  CashDay transfer(CashDayTransfer transfer);

  CashDay cashRefunded(Money? cashRefunded);

  CashDay expectedCash(Money expectedCash);

  CashDay payments(List<PaymentView> payments);

  CashDay closing(CashDayClosing? closing);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CashDay(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CashDay(...).copyWith(id: 12, name: "My name")
  /// ````
  CashDay call({
    String branchId,
    DateTime date,
    String timeZone,
    CashDayStatusEnum status,
    CashMethodTotal total,
    CashMethodTotal cash,
    CashMethodTotal card,
    CashDayTransfer transfer,
    Money? cashRefunded,
    Money expectedCash,
    List<PaymentView> payments,
    CashDayClosing? closing,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCashDay.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCashDay.copyWith.fieldName(...)`
class _$CashDayCWProxyImpl implements _$CashDayCWProxy {
  const _$CashDayCWProxyImpl(this._value);

  final CashDay _value;

  @override
  CashDay branchId(String branchId) => this(branchId: branchId);

  @override
  CashDay date(DateTime date) => this(date: date);

  @override
  CashDay timeZone(String timeZone) => this(timeZone: timeZone);

  @override
  CashDay status(CashDayStatusEnum status) => this(status: status);

  @override
  CashDay total(CashMethodTotal total) => this(total: total);

  @override
  CashDay cash(CashMethodTotal cash) => this(cash: cash);

  @override
  CashDay card(CashMethodTotal card) => this(card: card);

  @override
  CashDay transfer(CashDayTransfer transfer) => this(transfer: transfer);

  @override
  CashDay cashRefunded(Money? cashRefunded) => this(cashRefunded: cashRefunded);

  @override
  CashDay expectedCash(Money expectedCash) => this(expectedCash: expectedCash);

  @override
  CashDay payments(List<PaymentView> payments) => this(payments: payments);

  @override
  CashDay closing(CashDayClosing? closing) => this(closing: closing);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CashDay(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CashDay(...).copyWith(id: 12, name: "My name")
  /// ````
  CashDay call({
    Object? branchId = const $CopyWithPlaceholder(),
    Object? date = const $CopyWithPlaceholder(),
    Object? timeZone = const $CopyWithPlaceholder(),
    Object? status = const $CopyWithPlaceholder(),
    Object? total = const $CopyWithPlaceholder(),
    Object? cash = const $CopyWithPlaceholder(),
    Object? card = const $CopyWithPlaceholder(),
    Object? transfer = const $CopyWithPlaceholder(),
    Object? cashRefunded = const $CopyWithPlaceholder(),
    Object? expectedCash = const $CopyWithPlaceholder(),
    Object? payments = const $CopyWithPlaceholder(),
    Object? closing = const $CopyWithPlaceholder(),
  }) {
    return CashDay(
      branchId: branchId == const $CopyWithPlaceholder()
          ? _value.branchId
          // ignore: cast_nullable_to_non_nullable
          : branchId as String,
      date: date == const $CopyWithPlaceholder()
          ? _value.date
          // ignore: cast_nullable_to_non_nullable
          : date as DateTime,
      timeZone: timeZone == const $CopyWithPlaceholder()
          ? _value.timeZone
          // ignore: cast_nullable_to_non_nullable
          : timeZone as String,
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as CashDayStatusEnum,
      total: total == const $CopyWithPlaceholder()
          ? _value.total
          // ignore: cast_nullable_to_non_nullable
          : total as CashMethodTotal,
      cash: cash == const $CopyWithPlaceholder()
          ? _value.cash
          // ignore: cast_nullable_to_non_nullable
          : cash as CashMethodTotal,
      card: card == const $CopyWithPlaceholder()
          ? _value.card
          // ignore: cast_nullable_to_non_nullable
          : card as CashMethodTotal,
      transfer: transfer == const $CopyWithPlaceholder()
          ? _value.transfer
          // ignore: cast_nullable_to_non_nullable
          : transfer as CashDayTransfer,
      cashRefunded: cashRefunded == const $CopyWithPlaceholder()
          ? _value.cashRefunded
          // ignore: cast_nullable_to_non_nullable
          : cashRefunded as Money?,
      expectedCash: expectedCash == const $CopyWithPlaceholder()
          ? _value.expectedCash
          // ignore: cast_nullable_to_non_nullable
          : expectedCash as Money,
      payments: payments == const $CopyWithPlaceholder()
          ? _value.payments
          // ignore: cast_nullable_to_non_nullable
          : payments as List<PaymentView>,
      closing: closing == const $CopyWithPlaceholder()
          ? _value.closing
          // ignore: cast_nullable_to_non_nullable
          : closing as CashDayClosing?,
    );
  }
}

extension $CashDayCopyWith on CashDay {
  /// Returns a callable class that can be used as follows: `instanceOfCashDay.copyWith(...)` or like so:`instanceOfCashDay.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CashDayCWProxy get copyWith => _$CashDayCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CashDay _$CashDayFromJson(Map<String, dynamic> json) =>
    $checkedCreate('CashDay', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'branchId',
          'date',
          'timeZone',
          'status',
          'total',
          'cash',
          'card',
          'transfer',
          'expectedCash',
          'payments',
          'closing',
        ],
      );
      final val = CashDay(
        branchId: $checkedConvert('branchId', (v) => v as String),
        date: $checkedConvert('date', (v) => DateTime.parse(v as String)),
        timeZone: $checkedConvert('timeZone', (v) => v as String),
        status: $checkedConvert(
          'status',
          (v) => $enumDecode(
            _$CashDayStatusEnumEnumMap,
            v,
            unknownValue: CashDayStatusEnum.unknownDefaultOpenApi,
          ),
        ),
        total: $checkedConvert(
          'total',
          (v) => CashMethodTotal.fromJson(v as Map<String, dynamic>),
        ),
        cash: $checkedConvert(
          'cash',
          (v) => CashMethodTotal.fromJson(v as Map<String, dynamic>),
        ),
        card: $checkedConvert(
          'card',
          (v) => CashMethodTotal.fromJson(v as Map<String, dynamic>),
        ),
        transfer: $checkedConvert(
          'transfer',
          (v) => CashDayTransfer.fromJson(v as Map<String, dynamic>),
        ),
        cashRefunded: $checkedConvert(
          'cashRefunded',
          (v) => v == null ? null : Money.fromJson(v as Map<String, dynamic>),
        ),
        expectedCash: $checkedConvert(
          'expectedCash',
          (v) => Money.fromJson(v as Map<String, dynamic>),
        ),
        payments: $checkedConvert(
          'payments',
          (v) => (v as List<dynamic>)
              .map((e) => PaymentView.fromJson(e as Map<String, dynamic>))
              .toList(),
        ),
        closing: $checkedConvert(
          'closing',
          (v) => v == null
              ? null
              : CashDayClosing.fromJson(v as Map<String, dynamic>),
        ),
      );
      return val;
    });

Map<String, dynamic> _$CashDayToJson(CashDay instance) => <String, dynamic>{
  'branchId': instance.branchId,
  'date': instance.date.toIso8601String(),
  'timeZone': instance.timeZone,
  'status': _$CashDayStatusEnumEnumMap[instance.status]!,
  'total': instance.total.toJson(),
  'cash': instance.cash.toJson(),
  'card': instance.card.toJson(),
  'transfer': instance.transfer.toJson(),
  'cashRefunded': ?instance.cashRefunded?.toJson(),
  'expectedCash': instance.expectedCash.toJson(),
  'payments': instance.payments.map((e) => e.toJson()).toList(),
  'closing': instance.closing?.toJson(),
};

const _$CashDayStatusEnumEnumMap = {
  CashDayStatusEnum.open: 'Open',
  CashDayStatusEnum.closed: 'Closed',
  CashDayStatusEnum.unknownDefaultOpenApi: 'unknown_default_open_api',
};
