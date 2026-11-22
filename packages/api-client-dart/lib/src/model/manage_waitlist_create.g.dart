// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_waitlist_create.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageWaitlistCreateCWProxy {
  ManageWaitlistCreate branchId(String branchId);

  ManageWaitlistCreate brandCustomerId(String? brandCustomerId);

  ManageWaitlistCreate newCustomer(
    ManageAppointmentCreateNewCustomer? newCustomer,
  );

  ManageWaitlistCreate preferences(WaitlistPreferences preferences);

  ManageWaitlistCreate note(String? note);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageWaitlistCreate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageWaitlistCreate(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageWaitlistCreate call({
    String branchId,
    String? brandCustomerId,
    ManageAppointmentCreateNewCustomer? newCustomer,
    WaitlistPreferences preferences,
    String? note,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageWaitlistCreate.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageWaitlistCreate.copyWith.fieldName(...)`
class _$ManageWaitlistCreateCWProxyImpl
    implements _$ManageWaitlistCreateCWProxy {
  const _$ManageWaitlistCreateCWProxyImpl(this._value);

  final ManageWaitlistCreate _value;

  @override
  ManageWaitlistCreate branchId(String branchId) => this(branchId: branchId);

  @override
  ManageWaitlistCreate brandCustomerId(String? brandCustomerId) =>
      this(brandCustomerId: brandCustomerId);

  @override
  ManageWaitlistCreate newCustomer(
    ManageAppointmentCreateNewCustomer? newCustomer,
  ) => this(newCustomer: newCustomer);

  @override
  ManageWaitlistCreate preferences(WaitlistPreferences preferences) =>
      this(preferences: preferences);

  @override
  ManageWaitlistCreate note(String? note) => this(note: note);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageWaitlistCreate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageWaitlistCreate(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageWaitlistCreate call({
    Object? branchId = const $CopyWithPlaceholder(),
    Object? brandCustomerId = const $CopyWithPlaceholder(),
    Object? newCustomer = const $CopyWithPlaceholder(),
    Object? preferences = const $CopyWithPlaceholder(),
    Object? note = const $CopyWithPlaceholder(),
  }) {
    return ManageWaitlistCreate(
      branchId: branchId == const $CopyWithPlaceholder()
          ? _value.branchId
          // ignore: cast_nullable_to_non_nullable
          : branchId as String,
      brandCustomerId: brandCustomerId == const $CopyWithPlaceholder()
          ? _value.brandCustomerId
          // ignore: cast_nullable_to_non_nullable
          : brandCustomerId as String?,
      newCustomer: newCustomer == const $CopyWithPlaceholder()
          ? _value.newCustomer
          // ignore: cast_nullable_to_non_nullable
          : newCustomer as ManageAppointmentCreateNewCustomer?,
      preferences: preferences == const $CopyWithPlaceholder()
          ? _value.preferences
          // ignore: cast_nullable_to_non_nullable
          : preferences as WaitlistPreferences,
      note: note == const $CopyWithPlaceholder()
          ? _value.note
          // ignore: cast_nullable_to_non_nullable
          : note as String?,
    );
  }
}

extension $ManageWaitlistCreateCopyWith on ManageWaitlistCreate {
  /// Returns a callable class that can be used as follows: `instanceOfManageWaitlistCreate.copyWith(...)` or like so:`instanceOfManageWaitlistCreate.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageWaitlistCreateCWProxy get copyWith =>
      _$ManageWaitlistCreateCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageWaitlistCreate _$ManageWaitlistCreateFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('ManageWaitlistCreate', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['branchId', 'preferences']);
  final val = ManageWaitlistCreate(
    branchId: $checkedConvert('branchId', (v) => v as String),
    brandCustomerId: $checkedConvert('brandCustomerId', (v) => v as String?),
    newCustomer: $checkedConvert(
      'newCustomer',
      (v) => v == null
          ? null
          : ManageAppointmentCreateNewCustomer.fromJson(
              v as Map<String, dynamic>,
            ),
    ),
    preferences: $checkedConvert(
      'preferences',
      (v) => WaitlistPreferences.fromJson(v as Map<String, dynamic>),
    ),
    note: $checkedConvert('note', (v) => v as String?),
  );
  return val;
});

Map<String, dynamic> _$ManageWaitlistCreateToJson(
  ManageWaitlistCreate instance,
) => <String, dynamic>{
  'branchId': instance.branchId,
  'brandCustomerId': ?instance.brandCustomerId,
  'newCustomer': ?instance.newCustomer?.toJson(),
  'preferences': instance.preferences.toJson(),
  'note': instance.note,
};
