// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'subscription_invoice.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$SubscriptionInvoiceCWProxy {
  SubscriptionInvoice id(String id);

  SubscriptionInvoice invoiceNumber(String invoiceNumber);

  SubscriptionInvoice periodStart(DateTime periodStart);

  SubscriptionInvoice periodEnd(DateTime periodEnd);

  SubscriptionInvoice amount(Money amount);

  SubscriptionInvoice tax(Money tax);

  SubscriptionInvoice total(Money total);

  SubscriptionInvoice status(SubscriptionInvoiceStatus status);

  SubscriptionInvoice issuedUtc(DateTime issuedUtc);

  SubscriptionInvoice paidUtc(DateTime? paidUtc);

  SubscriptionInvoice paidVia(SubscriptionPaymentMethod? paidVia);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `SubscriptionInvoice(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// SubscriptionInvoice(...).copyWith(id: 12, name: "My name")
  /// ````
  SubscriptionInvoice call({
    String id,
    String invoiceNumber,
    DateTime periodStart,
    DateTime periodEnd,
    Money amount,
    Money tax,
    Money total,
    SubscriptionInvoiceStatus status,
    DateTime issuedUtc,
    DateTime? paidUtc,
    SubscriptionPaymentMethod? paidVia,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfSubscriptionInvoice.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfSubscriptionInvoice.copyWith.fieldName(...)`
class _$SubscriptionInvoiceCWProxyImpl implements _$SubscriptionInvoiceCWProxy {
  const _$SubscriptionInvoiceCWProxyImpl(this._value);

  final SubscriptionInvoice _value;

  @override
  SubscriptionInvoice id(String id) => this(id: id);

  @override
  SubscriptionInvoice invoiceNumber(String invoiceNumber) =>
      this(invoiceNumber: invoiceNumber);

  @override
  SubscriptionInvoice periodStart(DateTime periodStart) =>
      this(periodStart: periodStart);

  @override
  SubscriptionInvoice periodEnd(DateTime periodEnd) =>
      this(periodEnd: periodEnd);

  @override
  SubscriptionInvoice amount(Money amount) => this(amount: amount);

  @override
  SubscriptionInvoice tax(Money tax) => this(tax: tax);

  @override
  SubscriptionInvoice total(Money total) => this(total: total);

  @override
  SubscriptionInvoice status(SubscriptionInvoiceStatus status) =>
      this(status: status);

  @override
  SubscriptionInvoice issuedUtc(DateTime issuedUtc) =>
      this(issuedUtc: issuedUtc);

  @override
  SubscriptionInvoice paidUtc(DateTime? paidUtc) => this(paidUtc: paidUtc);

  @override
  SubscriptionInvoice paidVia(SubscriptionPaymentMethod? paidVia) =>
      this(paidVia: paidVia);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `SubscriptionInvoice(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// SubscriptionInvoice(...).copyWith(id: 12, name: "My name")
  /// ````
  SubscriptionInvoice call({
    Object? id = const $CopyWithPlaceholder(),
    Object? invoiceNumber = const $CopyWithPlaceholder(),
    Object? periodStart = const $CopyWithPlaceholder(),
    Object? periodEnd = const $CopyWithPlaceholder(),
    Object? amount = const $CopyWithPlaceholder(),
    Object? tax = const $CopyWithPlaceholder(),
    Object? total = const $CopyWithPlaceholder(),
    Object? status = const $CopyWithPlaceholder(),
    Object? issuedUtc = const $CopyWithPlaceholder(),
    Object? paidUtc = const $CopyWithPlaceholder(),
    Object? paidVia = const $CopyWithPlaceholder(),
  }) {
    return SubscriptionInvoice(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      invoiceNumber: invoiceNumber == const $CopyWithPlaceholder()
          ? _value.invoiceNumber
          // ignore: cast_nullable_to_non_nullable
          : invoiceNumber as String,
      periodStart: periodStart == const $CopyWithPlaceholder()
          ? _value.periodStart
          // ignore: cast_nullable_to_non_nullable
          : periodStart as DateTime,
      periodEnd: periodEnd == const $CopyWithPlaceholder()
          ? _value.periodEnd
          // ignore: cast_nullable_to_non_nullable
          : periodEnd as DateTime,
      amount: amount == const $CopyWithPlaceholder()
          ? _value.amount
          // ignore: cast_nullable_to_non_nullable
          : amount as Money,
      tax: tax == const $CopyWithPlaceholder()
          ? _value.tax
          // ignore: cast_nullable_to_non_nullable
          : tax as Money,
      total: total == const $CopyWithPlaceholder()
          ? _value.total
          // ignore: cast_nullable_to_non_nullable
          : total as Money,
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as SubscriptionInvoiceStatus,
      issuedUtc: issuedUtc == const $CopyWithPlaceholder()
          ? _value.issuedUtc
          // ignore: cast_nullable_to_non_nullable
          : issuedUtc as DateTime,
      paidUtc: paidUtc == const $CopyWithPlaceholder()
          ? _value.paidUtc
          // ignore: cast_nullable_to_non_nullable
          : paidUtc as DateTime?,
      paidVia: paidVia == const $CopyWithPlaceholder()
          ? _value.paidVia
          // ignore: cast_nullable_to_non_nullable
          : paidVia as SubscriptionPaymentMethod?,
    );
  }
}

extension $SubscriptionInvoiceCopyWith on SubscriptionInvoice {
  /// Returns a callable class that can be used as follows: `instanceOfSubscriptionInvoice.copyWith(...)` or like so:`instanceOfSubscriptionInvoice.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$SubscriptionInvoiceCWProxy get copyWith =>
      _$SubscriptionInvoiceCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

SubscriptionInvoice _$SubscriptionInvoiceFromJson(Map<String, dynamic> json) =>
    $checkedCreate('SubscriptionInvoice', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'id',
          'invoiceNumber',
          'periodStart',
          'periodEnd',
          'amount',
          'tax',
          'total',
          'status',
          'issuedUtc',
          'paidUtc',
          'paidVia',
        ],
      );
      final val = SubscriptionInvoice(
        id: $checkedConvert('id', (v) => v as String),
        invoiceNumber: $checkedConvert('invoiceNumber', (v) => v as String),
        periodStart: $checkedConvert(
          'periodStart',
          (v) => const DateOnlyConverter().fromJson(v as String),
        ),
        periodEnd: $checkedConvert(
          'periodEnd',
          (v) => const DateOnlyConverter().fromJson(v as String),
        ),
        amount: $checkedConvert(
          'amount',
          (v) => Money.fromJson(v as Map<String, dynamic>),
        ),
        tax: $checkedConvert(
          'tax',
          (v) => Money.fromJson(v as Map<String, dynamic>),
        ),
        total: $checkedConvert(
          'total',
          (v) => Money.fromJson(v as Map<String, dynamic>),
        ),
        status: $checkedConvert(
          'status',
          (v) => $enumDecode(
            _$SubscriptionInvoiceStatusEnumMap,
            v,
            unknownValue: SubscriptionInvoiceStatus.unknownDefaultOpenApi,
          ),
        ),
        issuedUtc: $checkedConvert(
          'issuedUtc',
          (v) => DateTime.parse(v as String),
        ),
        paidUtc: $checkedConvert(
          'paidUtc',
          (v) => v == null ? null : DateTime.parse(v as String),
        ),
        paidVia: $checkedConvert(
          'paidVia',
          (v) => $enumDecodeNullable(
            _$SubscriptionPaymentMethodEnumMap,
            v,
            unknownValue: SubscriptionPaymentMethod.unknownDefaultOpenApi,
          ),
        ),
      );
      return val;
    });

Map<String, dynamic> _$SubscriptionInvoiceToJson(
  SubscriptionInvoice instance,
) => <String, dynamic>{
  'id': instance.id,
  'invoiceNumber': instance.invoiceNumber,
  'periodStart': const DateOnlyConverter().toJson(instance.periodStart),
  'periodEnd': const DateOnlyConverter().toJson(instance.periodEnd),
  'amount': instance.amount.toJson(),
  'tax': instance.tax.toJson(),
  'total': instance.total.toJson(),
  'status': _$SubscriptionInvoiceStatusEnumMap[instance.status]!,
  'issuedUtc': instance.issuedUtc.toIso8601String(),
  'paidUtc': instance.paidUtc?.toIso8601String(),
  'paidVia': _$SubscriptionPaymentMethodEnumMap[instance.paidVia],
};

const _$SubscriptionInvoiceStatusEnumMap = {
  SubscriptionInvoiceStatus.issued: 'Issued',
  SubscriptionInvoiceStatus.paid: 'Paid',
  SubscriptionInvoiceStatus.failed: 'Failed',
  SubscriptionInvoiceStatus.void_: 'Void',
  SubscriptionInvoiceStatus.unknownDefaultOpenApi: 'unknown_default_open_api',
};

const _$SubscriptionPaymentMethodEnumMap = {
  SubscriptionPaymentMethod.card: 'Card',
  SubscriptionPaymentMethod.bankTransfer: 'BankTransfer',
  SubscriptionPaymentMethod.unknownDefaultOpenApi: 'unknown_default_open_api',
};
