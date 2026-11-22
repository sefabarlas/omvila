// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_appointment_create.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PublicAppointmentCreateCWProxy {
  PublicAppointmentCreate items(List<AppointmentItemRequest> items);

  PublicAppointmentCreate sameStaffForAll(bool? sameStaffForAll);

  PublicAppointmentCreate startUtc(DateTime startUtc);

  PublicAppointmentCreate fullName(String fullName);

  PublicAppointmentCreate customerNote(String? customerNote);

  PublicAppointmentCreate usePackageItems(
    List<PublicAppointmentCreateUsePackageItemsInner>? usePackageItems,
  );

  PublicAppointmentCreate privacyNoticeAcknowledged(
    bool privacyNoticeAcknowledged,
  );

  PublicAppointmentCreate privacyNoticeVersion(String privacyNoticeVersion);

  PublicAppointmentCreate marketingConsent(bool? marketingConsent);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicAppointmentCreate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicAppointmentCreate(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicAppointmentCreate call({
    List<AppointmentItemRequest> items,
    bool? sameStaffForAll,
    DateTime startUtc,
    String fullName,
    String? customerNote,
    List<PublicAppointmentCreateUsePackageItemsInner>? usePackageItems,
    bool privacyNoticeAcknowledged,
    String privacyNoticeVersion,
    bool? marketingConsent,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPublicAppointmentCreate.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPublicAppointmentCreate.copyWith.fieldName(...)`
class _$PublicAppointmentCreateCWProxyImpl
    implements _$PublicAppointmentCreateCWProxy {
  const _$PublicAppointmentCreateCWProxyImpl(this._value);

  final PublicAppointmentCreate _value;

  @override
  PublicAppointmentCreate items(List<AppointmentItemRequest> items) =>
      this(items: items);

  @override
  PublicAppointmentCreate sameStaffForAll(bool? sameStaffForAll) =>
      this(sameStaffForAll: sameStaffForAll);

  @override
  PublicAppointmentCreate startUtc(DateTime startUtc) =>
      this(startUtc: startUtc);

  @override
  PublicAppointmentCreate fullName(String fullName) => this(fullName: fullName);

  @override
  PublicAppointmentCreate customerNote(String? customerNote) =>
      this(customerNote: customerNote);

  @override
  PublicAppointmentCreate usePackageItems(
    List<PublicAppointmentCreateUsePackageItemsInner>? usePackageItems,
  ) => this(usePackageItems: usePackageItems);

  @override
  PublicAppointmentCreate privacyNoticeAcknowledged(
    bool privacyNoticeAcknowledged,
  ) => this(privacyNoticeAcknowledged: privacyNoticeAcknowledged);

  @override
  PublicAppointmentCreate privacyNoticeVersion(String privacyNoticeVersion) =>
      this(privacyNoticeVersion: privacyNoticeVersion);

  @override
  PublicAppointmentCreate marketingConsent(bool? marketingConsent) =>
      this(marketingConsent: marketingConsent);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicAppointmentCreate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicAppointmentCreate(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicAppointmentCreate call({
    Object? items = const $CopyWithPlaceholder(),
    Object? sameStaffForAll = const $CopyWithPlaceholder(),
    Object? startUtc = const $CopyWithPlaceholder(),
    Object? fullName = const $CopyWithPlaceholder(),
    Object? customerNote = const $CopyWithPlaceholder(),
    Object? usePackageItems = const $CopyWithPlaceholder(),
    Object? privacyNoticeAcknowledged = const $CopyWithPlaceholder(),
    Object? privacyNoticeVersion = const $CopyWithPlaceholder(),
    Object? marketingConsent = const $CopyWithPlaceholder(),
  }) {
    return PublicAppointmentCreate(
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<AppointmentItemRequest>,
      sameStaffForAll: sameStaffForAll == const $CopyWithPlaceholder()
          ? _value.sameStaffForAll
          // ignore: cast_nullable_to_non_nullable
          : sameStaffForAll as bool?,
      startUtc: startUtc == const $CopyWithPlaceholder()
          ? _value.startUtc
          // ignore: cast_nullable_to_non_nullable
          : startUtc as DateTime,
      fullName: fullName == const $CopyWithPlaceholder()
          ? _value.fullName
          // ignore: cast_nullable_to_non_nullable
          : fullName as String,
      customerNote: customerNote == const $CopyWithPlaceholder()
          ? _value.customerNote
          // ignore: cast_nullable_to_non_nullable
          : customerNote as String?,
      usePackageItems: usePackageItems == const $CopyWithPlaceholder()
          ? _value.usePackageItems
          // ignore: cast_nullable_to_non_nullable
          : usePackageItems
                as List<PublicAppointmentCreateUsePackageItemsInner>?,
      privacyNoticeAcknowledged:
          privacyNoticeAcknowledged == const $CopyWithPlaceholder()
          ? _value.privacyNoticeAcknowledged
          // ignore: cast_nullable_to_non_nullable
          : privacyNoticeAcknowledged as bool,
      privacyNoticeVersion: privacyNoticeVersion == const $CopyWithPlaceholder()
          ? _value.privacyNoticeVersion
          // ignore: cast_nullable_to_non_nullable
          : privacyNoticeVersion as String,
      marketingConsent: marketingConsent == const $CopyWithPlaceholder()
          ? _value.marketingConsent
          // ignore: cast_nullable_to_non_nullable
          : marketingConsent as bool?,
    );
  }
}

extension $PublicAppointmentCreateCopyWith on PublicAppointmentCreate {
  /// Returns a callable class that can be used as follows: `instanceOfPublicAppointmentCreate.copyWith(...)` or like so:`instanceOfPublicAppointmentCreate.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PublicAppointmentCreateCWProxy get copyWith =>
      _$PublicAppointmentCreateCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PublicAppointmentCreate _$PublicAppointmentCreateFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('PublicAppointmentCreate', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'items',
      'startUtc',
      'fullName',
      'privacyNoticeAcknowledged',
      'privacyNoticeVersion',
    ],
  );
  final val = PublicAppointmentCreate(
    items: $checkedConvert(
      'items',
      (v) => (v as List<dynamic>)
          .map(
            (e) => AppointmentItemRequest.fromJson(e as Map<String, dynamic>),
          )
          .toList(),
    ),
    sameStaffForAll: $checkedConvert(
      'sameStaffForAll',
      (v) => v as bool? ?? true,
    ),
    startUtc: $checkedConvert('startUtc', (v) => DateTime.parse(v as String)),
    fullName: $checkedConvert('fullName', (v) => v as String),
    customerNote: $checkedConvert('customerNote', (v) => v as String?),
    usePackageItems: $checkedConvert(
      'usePackageItems',
      (v) => (v as List<dynamic>?)
          ?.map(
            (e) => PublicAppointmentCreateUsePackageItemsInner.fromJson(
              e as Map<String, dynamic>,
            ),
          )
          .toList(),
    ),
    privacyNoticeAcknowledged: $checkedConvert(
      'privacyNoticeAcknowledged',
      (v) => v as bool,
    ),
    privacyNoticeVersion: $checkedConvert(
      'privacyNoticeVersion',
      (v) => v as String,
    ),
    marketingConsent: $checkedConvert(
      'marketingConsent',
      (v) => v as bool? ?? false,
    ),
  );
  return val;
});

Map<String, dynamic> _$PublicAppointmentCreateToJson(
  PublicAppointmentCreate instance,
) => <String, dynamic>{
  'items': instance.items.map((e) => e.toJson()).toList(),
  'sameStaffForAll': ?instance.sameStaffForAll,
  'startUtc': instance.startUtc.toIso8601String(),
  'fullName': instance.fullName,
  'customerNote': instance.customerNote,
  'usePackageItems': ?instance.usePackageItems?.map((e) => e.toJson()).toList(),
  'privacyNoticeAcknowledged': instance.privacyNoticeAcknowledged,
  'privacyNoticeVersion': instance.privacyNoticeVersion,
  'marketingConsent': ?instance.marketingConsent,
};
