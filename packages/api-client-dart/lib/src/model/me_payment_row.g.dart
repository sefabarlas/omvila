// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'me_payment_row.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MePaymentRowCWProxy {
  MePaymentRow id(String id);

  MePaymentRow kind(MePaymentKind kind);

  MePaymentRow status(MePaymentStatus status);

  MePaymentRow brand(MeBrandRef brand);

  MePaymentRow amount(Money amount);

  MePaymentRow method(PaymentMethod method);

  MePaymentRow occurredUtc(DateTime occurredUtc);

  MePaymentRow timeZone(String timeZone);

  MePaymentRow refundedAmount(Money? refundedAmount);

  MePaymentRow appointmentId(String? appointmentId);

  MePaymentRow appointmentStartUtc(DateTime? appointmentStartUtc);

  MePaymentRow serviceNames(List<String>? serviceNames);

  MePaymentRow packageName(String? packageName);

  MePaymentRow packageSessionCount(int? packageSessionCount);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MePaymentRow(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MePaymentRow(...).copyWith(id: 12, name: "My name")
  /// ````
  MePaymentRow call({
    String id,
    MePaymentKind kind,
    MePaymentStatus status,
    MeBrandRef brand,
    Money amount,
    PaymentMethod method,
    DateTime occurredUtc,
    String timeZone,
    Money? refundedAmount,
    String? appointmentId,
    DateTime? appointmentStartUtc,
    List<String>? serviceNames,
    String? packageName,
    int? packageSessionCount,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMePaymentRow.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMePaymentRow.copyWith.fieldName(...)`
class _$MePaymentRowCWProxyImpl implements _$MePaymentRowCWProxy {
  const _$MePaymentRowCWProxyImpl(this._value);

  final MePaymentRow _value;

  @override
  MePaymentRow id(String id) => this(id: id);

  @override
  MePaymentRow kind(MePaymentKind kind) => this(kind: kind);

  @override
  MePaymentRow status(MePaymentStatus status) => this(status: status);

  @override
  MePaymentRow brand(MeBrandRef brand) => this(brand: brand);

  @override
  MePaymentRow amount(Money amount) => this(amount: amount);

  @override
  MePaymentRow method(PaymentMethod method) => this(method: method);

  @override
  MePaymentRow occurredUtc(DateTime occurredUtc) =>
      this(occurredUtc: occurredUtc);

  @override
  MePaymentRow timeZone(String timeZone) => this(timeZone: timeZone);

  @override
  MePaymentRow refundedAmount(Money? refundedAmount) =>
      this(refundedAmount: refundedAmount);

  @override
  MePaymentRow appointmentId(String? appointmentId) =>
      this(appointmentId: appointmentId);

  @override
  MePaymentRow appointmentStartUtc(DateTime? appointmentStartUtc) =>
      this(appointmentStartUtc: appointmentStartUtc);

  @override
  MePaymentRow serviceNames(List<String>? serviceNames) =>
      this(serviceNames: serviceNames);

  @override
  MePaymentRow packageName(String? packageName) =>
      this(packageName: packageName);

  @override
  MePaymentRow packageSessionCount(int? packageSessionCount) =>
      this(packageSessionCount: packageSessionCount);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MePaymentRow(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MePaymentRow(...).copyWith(id: 12, name: "My name")
  /// ````
  MePaymentRow call({
    Object? id = const $CopyWithPlaceholder(),
    Object? kind = const $CopyWithPlaceholder(),
    Object? status = const $CopyWithPlaceholder(),
    Object? brand = const $CopyWithPlaceholder(),
    Object? amount = const $CopyWithPlaceholder(),
    Object? method = const $CopyWithPlaceholder(),
    Object? occurredUtc = const $CopyWithPlaceholder(),
    Object? timeZone = const $CopyWithPlaceholder(),
    Object? refundedAmount = const $CopyWithPlaceholder(),
    Object? appointmentId = const $CopyWithPlaceholder(),
    Object? appointmentStartUtc = const $CopyWithPlaceholder(),
    Object? serviceNames = const $CopyWithPlaceholder(),
    Object? packageName = const $CopyWithPlaceholder(),
    Object? packageSessionCount = const $CopyWithPlaceholder(),
  }) {
    return MePaymentRow(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      kind: kind == const $CopyWithPlaceholder()
          ? _value.kind
          // ignore: cast_nullable_to_non_nullable
          : kind as MePaymentKind,
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as MePaymentStatus,
      brand: brand == const $CopyWithPlaceholder()
          ? _value.brand
          // ignore: cast_nullable_to_non_nullable
          : brand as MeBrandRef,
      amount: amount == const $CopyWithPlaceholder()
          ? _value.amount
          // ignore: cast_nullable_to_non_nullable
          : amount as Money,
      method: method == const $CopyWithPlaceholder()
          ? _value.method
          // ignore: cast_nullable_to_non_nullable
          : method as PaymentMethod,
      occurredUtc: occurredUtc == const $CopyWithPlaceholder()
          ? _value.occurredUtc
          // ignore: cast_nullable_to_non_nullable
          : occurredUtc as DateTime,
      timeZone: timeZone == const $CopyWithPlaceholder()
          ? _value.timeZone
          // ignore: cast_nullable_to_non_nullable
          : timeZone as String,
      refundedAmount: refundedAmount == const $CopyWithPlaceholder()
          ? _value.refundedAmount
          // ignore: cast_nullable_to_non_nullable
          : refundedAmount as Money?,
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
      packageName: packageName == const $CopyWithPlaceholder()
          ? _value.packageName
          // ignore: cast_nullable_to_non_nullable
          : packageName as String?,
      packageSessionCount: packageSessionCount == const $CopyWithPlaceholder()
          ? _value.packageSessionCount
          // ignore: cast_nullable_to_non_nullable
          : packageSessionCount as int?,
    );
  }
}

extension $MePaymentRowCopyWith on MePaymentRow {
  /// Returns a callable class that can be used as follows: `instanceOfMePaymentRow.copyWith(...)` or like so:`instanceOfMePaymentRow.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MePaymentRowCWProxy get copyWith => _$MePaymentRowCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MePaymentRow _$MePaymentRowFromJson(Map<String, dynamic> json) =>
    $checkedCreate('MePaymentRow', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'id',
          'kind',
          'status',
          'brand',
          'amount',
          'method',
          'occurredUtc',
          'timeZone',
        ],
      );
      final val = MePaymentRow(
        id: $checkedConvert('id', (v) => v as String),
        kind: $checkedConvert(
          'kind',
          (v) => $enumDecode(
            _$MePaymentKindEnumMap,
            v,
            unknownValue: MePaymentKind.unknownDefaultOpenApi,
          ),
        ),
        status: $checkedConvert(
          'status',
          (v) => $enumDecode(
            _$MePaymentStatusEnumMap,
            v,
            unknownValue: MePaymentStatus.unknownDefaultOpenApi,
          ),
        ),
        brand: $checkedConvert(
          'brand',
          (v) => MeBrandRef.fromJson(v as Map<String, dynamic>),
        ),
        amount: $checkedConvert(
          'amount',
          (v) => Money.fromJson(v as Map<String, dynamic>),
        ),
        method: $checkedConvert(
          'method',
          (v) => $enumDecode(
            _$PaymentMethodEnumMap,
            v,
            unknownValue: PaymentMethod.unknownDefaultOpenApi,
          ),
        ),
        occurredUtc: $checkedConvert(
          'occurredUtc',
          (v) => DateTime.parse(v as String),
        ),
        timeZone: $checkedConvert('timeZone', (v) => v as String),
        refundedAmount: $checkedConvert(
          'refundedAmount',
          (v) => v == null ? null : Money.fromJson(v as Map<String, dynamic>),
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
        packageName: $checkedConvert('packageName', (v) => v as String?),
        packageSessionCount: $checkedConvert(
          'packageSessionCount',
          (v) => (v as num?)?.toInt(),
        ),
      );
      return val;
    });

Map<String, dynamic> _$MePaymentRowToJson(MePaymentRow instance) =>
    <String, dynamic>{
      'id': instance.id,
      'kind': _$MePaymentKindEnumMap[instance.kind]!,
      'status': _$MePaymentStatusEnumMap[instance.status]!,
      'brand': instance.brand.toJson(),
      'amount': instance.amount.toJson(),
      'method': _$PaymentMethodEnumMap[instance.method]!,
      'occurredUtc': instance.occurredUtc.toIso8601String(),
      'timeZone': instance.timeZone,
      'refundedAmount': ?instance.refundedAmount?.toJson(),
      'appointmentId': ?instance.appointmentId,
      'appointmentStartUtc': ?instance.appointmentStartUtc?.toIso8601String(),
      'serviceNames': ?instance.serviceNames,
      'packageName': ?instance.packageName,
      'packageSessionCount': ?instance.packageSessionCount,
    };

const _$MePaymentKindEnumMap = {
  MePaymentKind.deposit: 'Deposit',
  MePaymentKind.appointmentPayment: 'AppointmentPayment',
  MePaymentKind.packagePurchase: 'PackagePurchase',
  MePaymentKind.unknownDefaultOpenApi: 'unknown_default_open_api',
};

const _$MePaymentStatusEnumMap = {
  MePaymentStatus.awaitingConfirmation: 'AwaitingConfirmation',
  MePaymentStatus.recorded: 'Recorded',
  MePaymentStatus.partiallyRefunded: 'PartiallyRefunded',
  MePaymentStatus.refunded: 'Refunded',
  MePaymentStatus.unknownDefaultOpenApi: 'unknown_default_open_api',
};

const _$PaymentMethodEnumMap = {
  PaymentMethod.cash: 'Cash',
  PaymentMethod.card: 'Card',
  PaymentMethod.transfer: 'Transfer',
  PaymentMethod.packageRedemption: 'PackageRedemption',
  PaymentMethod.unknownDefaultOpenApi: 'unknown_default_open_api',
};
