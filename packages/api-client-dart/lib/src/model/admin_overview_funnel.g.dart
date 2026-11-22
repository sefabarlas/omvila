// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_overview_funnel.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminOverviewFunnelCWProxy {
  AdminOverviewFunnel windowDays(int windowDays);

  AdminOverviewFunnel signedUp(int signedUp);

  AdminOverviewFunnel addedServices(int addedServices);

  AdminOverviewFunnel setWorkingHours(int setWorkingHours);

  AdminOverviewFunnel firstOnlineAppointment(int firstOnlineAppointment);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminOverviewFunnel(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminOverviewFunnel(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminOverviewFunnel call({
    int windowDays,
    int signedUp,
    int addedServices,
    int setWorkingHours,
    int firstOnlineAppointment,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminOverviewFunnel.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminOverviewFunnel.copyWith.fieldName(...)`
class _$AdminOverviewFunnelCWProxyImpl implements _$AdminOverviewFunnelCWProxy {
  const _$AdminOverviewFunnelCWProxyImpl(this._value);

  final AdminOverviewFunnel _value;

  @override
  AdminOverviewFunnel windowDays(int windowDays) =>
      this(windowDays: windowDays);

  @override
  AdminOverviewFunnel signedUp(int signedUp) => this(signedUp: signedUp);

  @override
  AdminOverviewFunnel addedServices(int addedServices) =>
      this(addedServices: addedServices);

  @override
  AdminOverviewFunnel setWorkingHours(int setWorkingHours) =>
      this(setWorkingHours: setWorkingHours);

  @override
  AdminOverviewFunnel firstOnlineAppointment(int firstOnlineAppointment) =>
      this(firstOnlineAppointment: firstOnlineAppointment);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminOverviewFunnel(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminOverviewFunnel(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminOverviewFunnel call({
    Object? windowDays = const $CopyWithPlaceholder(),
    Object? signedUp = const $CopyWithPlaceholder(),
    Object? addedServices = const $CopyWithPlaceholder(),
    Object? setWorkingHours = const $CopyWithPlaceholder(),
    Object? firstOnlineAppointment = const $CopyWithPlaceholder(),
  }) {
    return AdminOverviewFunnel(
      windowDays: windowDays == const $CopyWithPlaceholder()
          ? _value.windowDays
          // ignore: cast_nullable_to_non_nullable
          : windowDays as int,
      signedUp: signedUp == const $CopyWithPlaceholder()
          ? _value.signedUp
          // ignore: cast_nullable_to_non_nullable
          : signedUp as int,
      addedServices: addedServices == const $CopyWithPlaceholder()
          ? _value.addedServices
          // ignore: cast_nullable_to_non_nullable
          : addedServices as int,
      setWorkingHours: setWorkingHours == const $CopyWithPlaceholder()
          ? _value.setWorkingHours
          // ignore: cast_nullable_to_non_nullable
          : setWorkingHours as int,
      firstOnlineAppointment:
          firstOnlineAppointment == const $CopyWithPlaceholder()
          ? _value.firstOnlineAppointment
          // ignore: cast_nullable_to_non_nullable
          : firstOnlineAppointment as int,
    );
  }
}

extension $AdminOverviewFunnelCopyWith on AdminOverviewFunnel {
  /// Returns a callable class that can be used as follows: `instanceOfAdminOverviewFunnel.copyWith(...)` or like so:`instanceOfAdminOverviewFunnel.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminOverviewFunnelCWProxy get copyWith =>
      _$AdminOverviewFunnelCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminOverviewFunnel _$AdminOverviewFunnelFromJson(Map<String, dynamic> json) =>
    $checkedCreate('AdminOverviewFunnel', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'windowDays',
          'signedUp',
          'addedServices',
          'setWorkingHours',
          'firstOnlineAppointment',
        ],
      );
      final val = AdminOverviewFunnel(
        windowDays: $checkedConvert('windowDays', (v) => (v as num).toInt()),
        signedUp: $checkedConvert('signedUp', (v) => (v as num).toInt()),
        addedServices: $checkedConvert(
          'addedServices',
          (v) => (v as num).toInt(),
        ),
        setWorkingHours: $checkedConvert(
          'setWorkingHours',
          (v) => (v as num).toInt(),
        ),
        firstOnlineAppointment: $checkedConvert(
          'firstOnlineAppointment',
          (v) => (v as num).toInt(),
        ),
      );
      return val;
    });

Map<String, dynamic> _$AdminOverviewFunnelToJson(
  AdminOverviewFunnel instance,
) => <String, dynamic>{
  'windowDays': instance.windowDays,
  'signedUp': instance.signedUp,
  'addedServices': instance.addedServices,
  'setWorkingHours': instance.setWorkingHours,
  'firstOnlineAppointment': instance.firstOnlineAppointment,
};
