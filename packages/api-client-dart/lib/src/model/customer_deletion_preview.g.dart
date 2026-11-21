// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'customer_deletion_preview.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CustomerDeletionPreviewCWProxy {
  CustomerDeletionPreview fullName(String fullName);

  CustomerDeletionPreview appointmentCount(int appointmentCount);

  CustomerDeletionPreview paymentCount(int paymentCount);

  CustomerDeletionPreview upcomingAppointmentCount(
    int upcomingAppointmentCount,
  );

  CustomerDeletionPreview activePackages(
    List<CustomerDeletionPreviewActivePackagesInner> activePackages,
  );

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerDeletionPreview(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerDeletionPreview(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerDeletionPreview call({
    String fullName,
    int appointmentCount,
    int paymentCount,
    int upcomingAppointmentCount,
    List<CustomerDeletionPreviewActivePackagesInner> activePackages,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCustomerDeletionPreview.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCustomerDeletionPreview.copyWith.fieldName(...)`
class _$CustomerDeletionPreviewCWProxyImpl
    implements _$CustomerDeletionPreviewCWProxy {
  const _$CustomerDeletionPreviewCWProxyImpl(this._value);

  final CustomerDeletionPreview _value;

  @override
  CustomerDeletionPreview fullName(String fullName) => this(fullName: fullName);

  @override
  CustomerDeletionPreview appointmentCount(int appointmentCount) =>
      this(appointmentCount: appointmentCount);

  @override
  CustomerDeletionPreview paymentCount(int paymentCount) =>
      this(paymentCount: paymentCount);

  @override
  CustomerDeletionPreview upcomingAppointmentCount(
    int upcomingAppointmentCount,
  ) => this(upcomingAppointmentCount: upcomingAppointmentCount);

  @override
  CustomerDeletionPreview activePackages(
    List<CustomerDeletionPreviewActivePackagesInner> activePackages,
  ) => this(activePackages: activePackages);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerDeletionPreview(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerDeletionPreview(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerDeletionPreview call({
    Object? fullName = const $CopyWithPlaceholder(),
    Object? appointmentCount = const $CopyWithPlaceholder(),
    Object? paymentCount = const $CopyWithPlaceholder(),
    Object? upcomingAppointmentCount = const $CopyWithPlaceholder(),
    Object? activePackages = const $CopyWithPlaceholder(),
  }) {
    return CustomerDeletionPreview(
      fullName: fullName == const $CopyWithPlaceholder()
          ? _value.fullName
          // ignore: cast_nullable_to_non_nullable
          : fullName as String,
      appointmentCount: appointmentCount == const $CopyWithPlaceholder()
          ? _value.appointmentCount
          // ignore: cast_nullable_to_non_nullable
          : appointmentCount as int,
      paymentCount: paymentCount == const $CopyWithPlaceholder()
          ? _value.paymentCount
          // ignore: cast_nullable_to_non_nullable
          : paymentCount as int,
      upcomingAppointmentCount:
          upcomingAppointmentCount == const $CopyWithPlaceholder()
          ? _value.upcomingAppointmentCount
          // ignore: cast_nullable_to_non_nullable
          : upcomingAppointmentCount as int,
      activePackages: activePackages == const $CopyWithPlaceholder()
          ? _value.activePackages
          // ignore: cast_nullable_to_non_nullable
          : activePackages as List<CustomerDeletionPreviewActivePackagesInner>,
    );
  }
}

extension $CustomerDeletionPreviewCopyWith on CustomerDeletionPreview {
  /// Returns a callable class that can be used as follows: `instanceOfCustomerDeletionPreview.copyWith(...)` or like so:`instanceOfCustomerDeletionPreview.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CustomerDeletionPreviewCWProxy get copyWith =>
      _$CustomerDeletionPreviewCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CustomerDeletionPreview _$CustomerDeletionPreviewFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('CustomerDeletionPreview', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'fullName',
      'appointmentCount',
      'paymentCount',
      'upcomingAppointmentCount',
      'activePackages',
    ],
  );
  final val = CustomerDeletionPreview(
    fullName: $checkedConvert('fullName', (v) => v as String),
    appointmentCount: $checkedConvert(
      'appointmentCount',
      (v) => (v as num).toInt(),
    ),
    paymentCount: $checkedConvert('paymentCount', (v) => (v as num).toInt()),
    upcomingAppointmentCount: $checkedConvert(
      'upcomingAppointmentCount',
      (v) => (v as num).toInt(),
    ),
    activePackages: $checkedConvert(
      'activePackages',
      (v) => (v as List<dynamic>)
          .map(
            (e) => CustomerDeletionPreviewActivePackagesInner.fromJson(
              e as Map<String, dynamic>,
            ),
          )
          .toList(),
    ),
  );
  return val;
});

Map<String, dynamic> _$CustomerDeletionPreviewToJson(
  CustomerDeletionPreview instance,
) => <String, dynamic>{
  'fullName': instance.fullName,
  'appointmentCount': instance.appointmentCount,
  'paymentCount': instance.paymentCount,
  'upcomingAppointmentCount': instance.upcomingAppointmentCount,
  'activePackages': instance.activePackages.map((e) => e.toJson()).toList(),
};
