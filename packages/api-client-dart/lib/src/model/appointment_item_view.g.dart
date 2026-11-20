// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'appointment_item_view.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AppointmentItemViewCWProxy {
  AppointmentItemView id(String id);

  AppointmentItemView serviceId(String serviceId);

  AppointmentItemView serviceName(String serviceName);

  AppointmentItemView staffId(String? staffId);

  AppointmentItemView staffName(String? staffName);

  AppointmentItemView startUtc(DateTime startUtc);

  AppointmentItemView endUtc(DateTime endUtc);

  AppointmentItemView price(Money price);

  AppointmentItemView staffSurcharge(Money? staffSurcharge);

  AppointmentItemView discount(Money? discount);

  AppointmentItemView paidFromPackage(bool? paidFromPackage);

  AppointmentItemView status(AppointmentItemViewStatusEnum status);

  AppointmentItemView segments(List<SegmentView> segments);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AppointmentItemView(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AppointmentItemView(...).copyWith(id: 12, name: "My name")
  /// ````
  AppointmentItemView call({
    String id,
    String serviceId,
    String serviceName,
    String? staffId,
    String? staffName,
    DateTime startUtc,
    DateTime endUtc,
    Money price,
    Money? staffSurcharge,
    Money? discount,
    bool? paidFromPackage,
    AppointmentItemViewStatusEnum status,
    List<SegmentView> segments,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAppointmentItemView.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAppointmentItemView.copyWith.fieldName(...)`
class _$AppointmentItemViewCWProxyImpl implements _$AppointmentItemViewCWProxy {
  const _$AppointmentItemViewCWProxyImpl(this._value);

  final AppointmentItemView _value;

  @override
  AppointmentItemView id(String id) => this(id: id);

  @override
  AppointmentItemView serviceId(String serviceId) => this(serviceId: serviceId);

  @override
  AppointmentItemView serviceName(String serviceName) =>
      this(serviceName: serviceName);

  @override
  AppointmentItemView staffId(String? staffId) => this(staffId: staffId);

  @override
  AppointmentItemView staffName(String? staffName) =>
      this(staffName: staffName);

  @override
  AppointmentItemView startUtc(DateTime startUtc) => this(startUtc: startUtc);

  @override
  AppointmentItemView endUtc(DateTime endUtc) => this(endUtc: endUtc);

  @override
  AppointmentItemView price(Money price) => this(price: price);

  @override
  AppointmentItemView staffSurcharge(Money? staffSurcharge) =>
      this(staffSurcharge: staffSurcharge);

  @override
  AppointmentItemView discount(Money? discount) => this(discount: discount);

  @override
  AppointmentItemView paidFromPackage(bool? paidFromPackage) =>
      this(paidFromPackage: paidFromPackage);

  @override
  AppointmentItemView status(AppointmentItemViewStatusEnum status) =>
      this(status: status);

  @override
  AppointmentItemView segments(List<SegmentView> segments) =>
      this(segments: segments);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AppointmentItemView(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AppointmentItemView(...).copyWith(id: 12, name: "My name")
  /// ````
  AppointmentItemView call({
    Object? id = const $CopyWithPlaceholder(),
    Object? serviceId = const $CopyWithPlaceholder(),
    Object? serviceName = const $CopyWithPlaceholder(),
    Object? staffId = const $CopyWithPlaceholder(),
    Object? staffName = const $CopyWithPlaceholder(),
    Object? startUtc = const $CopyWithPlaceholder(),
    Object? endUtc = const $CopyWithPlaceholder(),
    Object? price = const $CopyWithPlaceholder(),
    Object? staffSurcharge = const $CopyWithPlaceholder(),
    Object? discount = const $CopyWithPlaceholder(),
    Object? paidFromPackage = const $CopyWithPlaceholder(),
    Object? status = const $CopyWithPlaceholder(),
    Object? segments = const $CopyWithPlaceholder(),
  }) {
    return AppointmentItemView(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      serviceId: serviceId == const $CopyWithPlaceholder()
          ? _value.serviceId
          // ignore: cast_nullable_to_non_nullable
          : serviceId as String,
      serviceName: serviceName == const $CopyWithPlaceholder()
          ? _value.serviceName
          // ignore: cast_nullable_to_non_nullable
          : serviceName as String,
      staffId: staffId == const $CopyWithPlaceholder()
          ? _value.staffId
          // ignore: cast_nullable_to_non_nullable
          : staffId as String?,
      staffName: staffName == const $CopyWithPlaceholder()
          ? _value.staffName
          // ignore: cast_nullable_to_non_nullable
          : staffName as String?,
      startUtc: startUtc == const $CopyWithPlaceholder()
          ? _value.startUtc
          // ignore: cast_nullable_to_non_nullable
          : startUtc as DateTime,
      endUtc: endUtc == const $CopyWithPlaceholder()
          ? _value.endUtc
          // ignore: cast_nullable_to_non_nullable
          : endUtc as DateTime,
      price: price == const $CopyWithPlaceholder()
          ? _value.price
          // ignore: cast_nullable_to_non_nullable
          : price as Money,
      staffSurcharge: staffSurcharge == const $CopyWithPlaceholder()
          ? _value.staffSurcharge
          // ignore: cast_nullable_to_non_nullable
          : staffSurcharge as Money?,
      discount: discount == const $CopyWithPlaceholder()
          ? _value.discount
          // ignore: cast_nullable_to_non_nullable
          : discount as Money?,
      paidFromPackage: paidFromPackage == const $CopyWithPlaceholder()
          ? _value.paidFromPackage
          // ignore: cast_nullable_to_non_nullable
          : paidFromPackage as bool?,
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as AppointmentItemViewStatusEnum,
      segments: segments == const $CopyWithPlaceholder()
          ? _value.segments
          // ignore: cast_nullable_to_non_nullable
          : segments as List<SegmentView>,
    );
  }
}

extension $AppointmentItemViewCopyWith on AppointmentItemView {
  /// Returns a callable class that can be used as follows: `instanceOfAppointmentItemView.copyWith(...)` or like so:`instanceOfAppointmentItemView.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AppointmentItemViewCWProxy get copyWith =>
      _$AppointmentItemViewCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AppointmentItemView _$AppointmentItemViewFromJson(Map<String, dynamic> json) =>
    $checkedCreate('AppointmentItemView', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'id',
          'serviceId',
          'serviceName',
          'startUtc',
          'endUtc',
          'price',
          'status',
          'segments',
        ],
      );
      final val = AppointmentItemView(
        id: $checkedConvert('id', (v) => v as String),
        serviceId: $checkedConvert('serviceId', (v) => v as String),
        serviceName: $checkedConvert('serviceName', (v) => v as String),
        staffId: $checkedConvert('staffId', (v) => v as String?),
        staffName: $checkedConvert('staffName', (v) => v as String?),
        startUtc: $checkedConvert(
          'startUtc',
          (v) => DateTime.parse(v as String),
        ),
        endUtc: $checkedConvert('endUtc', (v) => DateTime.parse(v as String)),
        price: $checkedConvert(
          'price',
          (v) => Money.fromJson(v as Map<String, dynamic>),
        ),
        staffSurcharge: $checkedConvert(
          'staffSurcharge',
          (v) => v == null ? null : Money.fromJson(v as Map<String, dynamic>),
        ),
        discount: $checkedConvert(
          'discount',
          (v) => v == null ? null : Money.fromJson(v as Map<String, dynamic>),
        ),
        paidFromPackage: $checkedConvert('paidFromPackage', (v) => v as bool?),
        status: $checkedConvert(
          'status',
          (v) => $enumDecode(
            _$AppointmentItemViewStatusEnumEnumMap,
            v,
            unknownValue: AppointmentItemViewStatusEnum.unknownDefaultOpenApi,
          ),
        ),
        segments: $checkedConvert(
          'segments',
          (v) => (v as List<dynamic>)
              .map((e) => SegmentView.fromJson(e as Map<String, dynamic>))
              .toList(),
        ),
      );
      return val;
    });

Map<String, dynamic> _$AppointmentItemViewToJson(
  AppointmentItemView instance,
) => <String, dynamic>{
  'id': instance.id,
  'serviceId': instance.serviceId,
  'serviceName': instance.serviceName,
  'staffId': ?instance.staffId,
  'staffName': ?instance.staffName,
  'startUtc': instance.startUtc.toIso8601String(),
  'endUtc': instance.endUtc.toIso8601String(),
  'price': instance.price.toJson(),
  'staffSurcharge': ?instance.staffSurcharge?.toJson(),
  'discount': ?instance.discount?.toJson(),
  'paidFromPackage': ?instance.paidFromPackage,
  'status': _$AppointmentItemViewStatusEnumEnumMap[instance.status]!,
  'segments': instance.segments.map((e) => e.toJson()).toList(),
};

const _$AppointmentItemViewStatusEnumEnumMap = {
  AppointmentItemViewStatusEnum.active: 'Active',
  AppointmentItemViewStatusEnum.completed: 'Completed',
  AppointmentItemViewStatusEnum.cancelled: 'Cancelled',
  AppointmentItemViewStatusEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
