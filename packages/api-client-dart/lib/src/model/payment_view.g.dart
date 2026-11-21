// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'payment_view.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PaymentViewCWProxy {
  PaymentView id(String id);

  PaymentView branchId(String branchId);

  PaymentView method(PaymentMethod method);

  PaymentView amount(Money amount);

  PaymentView isDeposit(bool isDeposit);

  PaymentView status(PaymentStatus status);

  PaymentView refundedAmount(Money? refundedAmount);

  PaymentView paidUtc(DateTime paidUtc);

  PaymentView collectedByName(String? collectedByName);

  PaymentView reference(String? reference);

  PaymentView purpose(PaymentPurpose purpose);

  PaymentView customer(CustomerRef? customer);

  PaymentView appointmentId(String? appointmentId);

  PaymentView appointmentStartUtc(DateTime? appointmentStartUtc);

  PaymentView serviceNames(List<String>? serviceNames);

  PaymentView isBalanceAfterDeposit(bool? isBalanceAfterDeposit);

  PaymentView staffName(String? staffName);

  PaymentView customerPackageId(String? customerPackageId);

  PaymentView packageName(String? packageName);

  PaymentView packageSessionCount(int? packageSessionCount);

  PaymentView recordedAfterDayClose(bool? recordedAfterDayClose);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PaymentView(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PaymentView(...).copyWith(id: 12, name: "My name")
  /// ````
  PaymentView call({
    String id,
    String branchId,
    PaymentMethod method,
    Money amount,
    bool isDeposit,
    PaymentStatus status,
    Money? refundedAmount,
    DateTime paidUtc,
    String? collectedByName,
    String? reference,
    PaymentPurpose purpose,
    CustomerRef? customer,
    String? appointmentId,
    DateTime? appointmentStartUtc,
    List<String>? serviceNames,
    bool? isBalanceAfterDeposit,
    String? staffName,
    String? customerPackageId,
    String? packageName,
    int? packageSessionCount,
    bool? recordedAfterDayClose,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPaymentView.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPaymentView.copyWith.fieldName(...)`
class _$PaymentViewCWProxyImpl implements _$PaymentViewCWProxy {
  const _$PaymentViewCWProxyImpl(this._value);

  final PaymentView _value;

  @override
  PaymentView id(String id) => this(id: id);

  @override
  PaymentView branchId(String branchId) => this(branchId: branchId);

  @override
  PaymentView method(PaymentMethod method) => this(method: method);

  @override
  PaymentView amount(Money amount) => this(amount: amount);

  @override
  PaymentView isDeposit(bool isDeposit) => this(isDeposit: isDeposit);

  @override
  PaymentView status(PaymentStatus status) => this(status: status);

  @override
  PaymentView refundedAmount(Money? refundedAmount) =>
      this(refundedAmount: refundedAmount);

  @override
  PaymentView paidUtc(DateTime paidUtc) => this(paidUtc: paidUtc);

  @override
  PaymentView collectedByName(String? collectedByName) =>
      this(collectedByName: collectedByName);

  @override
  PaymentView reference(String? reference) => this(reference: reference);

  @override
  PaymentView purpose(PaymentPurpose purpose) => this(purpose: purpose);

  @override
  PaymentView customer(CustomerRef? customer) => this(customer: customer);

  @override
  PaymentView appointmentId(String? appointmentId) =>
      this(appointmentId: appointmentId);

  @override
  PaymentView appointmentStartUtc(DateTime? appointmentStartUtc) =>
      this(appointmentStartUtc: appointmentStartUtc);

  @override
  PaymentView serviceNames(List<String>? serviceNames) =>
      this(serviceNames: serviceNames);

  @override
  PaymentView isBalanceAfterDeposit(bool? isBalanceAfterDeposit) =>
      this(isBalanceAfterDeposit: isBalanceAfterDeposit);

  @override
  PaymentView staffName(String? staffName) => this(staffName: staffName);

  @override
  PaymentView customerPackageId(String? customerPackageId) =>
      this(customerPackageId: customerPackageId);

  @override
  PaymentView packageName(String? packageName) =>
      this(packageName: packageName);

  @override
  PaymentView packageSessionCount(int? packageSessionCount) =>
      this(packageSessionCount: packageSessionCount);

  @override
  PaymentView recordedAfterDayClose(bool? recordedAfterDayClose) =>
      this(recordedAfterDayClose: recordedAfterDayClose);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PaymentView(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PaymentView(...).copyWith(id: 12, name: "My name")
  /// ````
  PaymentView call({
    Object? id = const $CopyWithPlaceholder(),
    Object? branchId = const $CopyWithPlaceholder(),
    Object? method = const $CopyWithPlaceholder(),
    Object? amount = const $CopyWithPlaceholder(),
    Object? isDeposit = const $CopyWithPlaceholder(),
    Object? status = const $CopyWithPlaceholder(),
    Object? refundedAmount = const $CopyWithPlaceholder(),
    Object? paidUtc = const $CopyWithPlaceholder(),
    Object? collectedByName = const $CopyWithPlaceholder(),
    Object? reference = const $CopyWithPlaceholder(),
    Object? purpose = const $CopyWithPlaceholder(),
    Object? customer = const $CopyWithPlaceholder(),
    Object? appointmentId = const $CopyWithPlaceholder(),
    Object? appointmentStartUtc = const $CopyWithPlaceholder(),
    Object? serviceNames = const $CopyWithPlaceholder(),
    Object? isBalanceAfterDeposit = const $CopyWithPlaceholder(),
    Object? staffName = const $CopyWithPlaceholder(),
    Object? customerPackageId = const $CopyWithPlaceholder(),
    Object? packageName = const $CopyWithPlaceholder(),
    Object? packageSessionCount = const $CopyWithPlaceholder(),
    Object? recordedAfterDayClose = const $CopyWithPlaceholder(),
  }) {
    return PaymentView(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      branchId: branchId == const $CopyWithPlaceholder()
          ? _value.branchId
          // ignore: cast_nullable_to_non_nullable
          : branchId as String,
      method: method == const $CopyWithPlaceholder()
          ? _value.method
          // ignore: cast_nullable_to_non_nullable
          : method as PaymentMethod,
      amount: amount == const $CopyWithPlaceholder()
          ? _value.amount
          // ignore: cast_nullable_to_non_nullable
          : amount as Money,
      isDeposit: isDeposit == const $CopyWithPlaceholder()
          ? _value.isDeposit
          // ignore: cast_nullable_to_non_nullable
          : isDeposit as bool,
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as PaymentStatus,
      refundedAmount: refundedAmount == const $CopyWithPlaceholder()
          ? _value.refundedAmount
          // ignore: cast_nullable_to_non_nullable
          : refundedAmount as Money?,
      paidUtc: paidUtc == const $CopyWithPlaceholder()
          ? _value.paidUtc
          // ignore: cast_nullable_to_non_nullable
          : paidUtc as DateTime,
      collectedByName: collectedByName == const $CopyWithPlaceholder()
          ? _value.collectedByName
          // ignore: cast_nullable_to_non_nullable
          : collectedByName as String?,
      reference: reference == const $CopyWithPlaceholder()
          ? _value.reference
          // ignore: cast_nullable_to_non_nullable
          : reference as String?,
      purpose: purpose == const $CopyWithPlaceholder()
          ? _value.purpose
          // ignore: cast_nullable_to_non_nullable
          : purpose as PaymentPurpose,
      customer: customer == const $CopyWithPlaceholder()
          ? _value.customer
          // ignore: cast_nullable_to_non_nullable
          : customer as CustomerRef?,
      appointmentId: appointmentId == const $CopyWithPlaceholder()
          ? _value.appointmentId
          // ignore: cast_nullable_to_non_nullable
          : appointmentId as String?,
      appointmentStartUtc: appointmentStartUtc == const $CopyWithPlaceholder()
          ? _value.appointmentStartUtc
          // ignore: cast_nullable_to_non_nullable
          : appointmentStartUtc as DateTime?,
      serviceNames: serviceNames == const $CopyWithPlaceholder()
          ? _value.serviceNames
          // ignore: cast_nullable_to_non_nullable
          : serviceNames as List<String>?,
      isBalanceAfterDeposit:
          isBalanceAfterDeposit == const $CopyWithPlaceholder()
          ? _value.isBalanceAfterDeposit
          // ignore: cast_nullable_to_non_nullable
          : isBalanceAfterDeposit as bool?,
      staffName: staffName == const $CopyWithPlaceholder()
          ? _value.staffName
          // ignore: cast_nullable_to_non_nullable
          : staffName as String?,
      customerPackageId: customerPackageId == const $CopyWithPlaceholder()
          ? _value.customerPackageId
          // ignore: cast_nullable_to_non_nullable
          : customerPackageId as String?,
      packageName: packageName == const $CopyWithPlaceholder()
          ? _value.packageName
          // ignore: cast_nullable_to_non_nullable
          : packageName as String?,
      packageSessionCount: packageSessionCount == const $CopyWithPlaceholder()
          ? _value.packageSessionCount
          // ignore: cast_nullable_to_non_nullable
          : packageSessionCount as int?,
      recordedAfterDayClose:
          recordedAfterDayClose == const $CopyWithPlaceholder()
          ? _value.recordedAfterDayClose
          // ignore: cast_nullable_to_non_nullable
          : recordedAfterDayClose as bool?,
    );
  }
}

extension $PaymentViewCopyWith on PaymentView {
  /// Returns a callable class that can be used as follows: `instanceOfPaymentView.copyWith(...)` or like so:`instanceOfPaymentView.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PaymentViewCWProxy get copyWith => _$PaymentViewCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PaymentView _$PaymentViewFromJson(Map<String, dynamic> json) => $checkedCreate(
  'PaymentView',
  json,
  ($checkedConvert) {
    $checkKeys(
      json,
      requiredKeys: const [
        'id',
        'branchId',
        'method',
        'amount',
        'isDeposit',
        'status',
        'paidUtc',
        'purpose',
      ],
    );
    final val = PaymentView(
      id: $checkedConvert('id', (v) => v as String),
      branchId: $checkedConvert('branchId', (v) => v as String),
      method: $checkedConvert(
        'method',
        (v) => $enumDecode(
          _$PaymentMethodEnumMap,
          v,
          unknownValue: PaymentMethod.unknownDefaultOpenApi,
        ),
      ),
      amount: $checkedConvert(
        'amount',
        (v) => Money.fromJson(v as Map<String, dynamic>),
      ),
      isDeposit: $checkedConvert('isDeposit', (v) => v as bool),
      status: $checkedConvert(
        'status',
        (v) => $enumDecode(
          _$PaymentStatusEnumMap,
          v,
          unknownValue: PaymentStatus.unknownDefaultOpenApi,
        ),
      ),
      refundedAmount: $checkedConvert(
        'refundedAmount',
        (v) => v == null ? null : Money.fromJson(v as Map<String, dynamic>),
      ),
      paidUtc: $checkedConvert('paidUtc', (v) => DateTime.parse(v as String)),
      collectedByName: $checkedConvert('collectedByName', (v) => v as String?),
      reference: $checkedConvert('reference', (v) => v as String?),
      purpose: $checkedConvert(
        'purpose',
        (v) => $enumDecode(
          _$PaymentPurposeEnumMap,
          v,
          unknownValue: PaymentPurpose.unknownDefaultOpenApi,
        ),
      ),
      customer: $checkedConvert(
        'customer',
        (v) =>
            v == null ? null : CustomerRef.fromJson(v as Map<String, dynamic>),
      ),
      appointmentId: $checkedConvert('appointmentId', (v) => v as String?),
      appointmentStartUtc: $checkedConvert(
        'appointmentStartUtc',
        (v) => v == null ? null : DateTime.parse(v as String),
      ),
      serviceNames: $checkedConvert(
        'serviceNames',
        (v) => (v as List<dynamic>?)?.map((e) => e as String).toList(),
      ),
      isBalanceAfterDeposit: $checkedConvert(
        'isBalanceAfterDeposit',
        (v) => v as bool?,
      ),
      staffName: $checkedConvert('staffName', (v) => v as String?),
      customerPackageId: $checkedConvert(
        'customerPackageId',
        (v) => v as String?,
      ),
      packageName: $checkedConvert('packageName', (v) => v as String?),
      packageSessionCount: $checkedConvert(
        'packageSessionCount',
        (v) => (v as num?)?.toInt(),
      ),
      recordedAfterDayClose: $checkedConvert(
        'recordedAfterDayClose',
        (v) => v as bool?,
      ),
    );
    return val;
  },
);

Map<String, dynamic> _$PaymentViewToJson(PaymentView instance) =>
    <String, dynamic>{
      'id': instance.id,
      'branchId': instance.branchId,
      'method': _$PaymentMethodEnumMap[instance.method]!,
      'amount': instance.amount.toJson(),
      'isDeposit': instance.isDeposit,
      'status': _$PaymentStatusEnumMap[instance.status]!,
      'refundedAmount': ?instance.refundedAmount?.toJson(),
      'paidUtc': instance.paidUtc.toIso8601String(),
      'collectedByName': ?instance.collectedByName,
      'reference': ?instance.reference,
      'purpose': _$PaymentPurposeEnumMap[instance.purpose]!,
      'customer': ?instance.customer?.toJson(),
      'appointmentId': ?instance.appointmentId,
      'appointmentStartUtc': ?instance.appointmentStartUtc?.toIso8601String(),
      'serviceNames': ?instance.serviceNames,
      'isBalanceAfterDeposit': ?instance.isBalanceAfterDeposit,
      'staffName': ?instance.staffName,
      'customerPackageId': ?instance.customerPackageId,
      'packageName': ?instance.packageName,
      'packageSessionCount': ?instance.packageSessionCount,
      'recordedAfterDayClose': ?instance.recordedAfterDayClose,
    };

const _$PaymentMethodEnumMap = {
  PaymentMethod.cash: 'Cash',
  PaymentMethod.card: 'Card',
  PaymentMethod.transfer: 'Transfer',
  PaymentMethod.packageRedemption: 'PackageRedemption',
  PaymentMethod.unknownDefaultOpenApi: 'unknown_default_open_api',
};

const _$PaymentStatusEnumMap = {
  PaymentStatus.recorded: 'Recorded',
  PaymentStatus.partiallyRefunded: 'PartiallyRefunded',
  PaymentStatus.refunded: 'Refunded',
  PaymentStatus.voided: 'Voided',
  PaymentStatus.unknownDefaultOpenApi: 'unknown_default_open_api',
};

const _$PaymentPurposeEnumMap = {
  PaymentPurpose.appointment: 'Appointment',
  PaymentPurpose.customerPackage: 'CustomerPackage',
  PaymentPurpose.unknownDefaultOpenApi: 'unknown_default_open_api',
};
