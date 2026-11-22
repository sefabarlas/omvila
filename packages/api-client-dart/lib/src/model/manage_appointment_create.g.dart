// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_appointment_create.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageAppointmentCreateCWProxy {
  ManageAppointmentCreate branchId(String branchId);

  ManageAppointmentCreate brandCustomerId(String? brandCustomerId);

  ManageAppointmentCreate newCustomer(
    ManageAppointmentCreateNewCustomer? newCustomer,
  );

  ManageAppointmentCreate channel(ManageAppointmentCreateChannelEnum channel);

  ManageAppointmentCreate startUtc(DateTime startUtc);

  ManageAppointmentCreate items(List<ManageAppointmentCreateItemsInner> items);

  ManageAppointmentCreate staffNote(String? staffNote);

  ManageAppointmentCreate recurrence(RecurrenceRequest? recurrence);

  ManageAppointmentCreate notifyCustomer(bool? notifyCustomer);

  ManageAppointmentCreate allowOutsideWorkingHours(
    bool? allowOutsideWorkingHours,
  );

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageAppointmentCreate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageAppointmentCreate(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageAppointmentCreate call({
    String branchId,
    String? brandCustomerId,
    ManageAppointmentCreateNewCustomer? newCustomer,
    ManageAppointmentCreateChannelEnum channel,
    DateTime startUtc,
    List<ManageAppointmentCreateItemsInner> items,
    String? staffNote,
    RecurrenceRequest? recurrence,
    bool? notifyCustomer,
    bool? allowOutsideWorkingHours,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageAppointmentCreate.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageAppointmentCreate.copyWith.fieldName(...)`
class _$ManageAppointmentCreateCWProxyImpl
    implements _$ManageAppointmentCreateCWProxy {
  const _$ManageAppointmentCreateCWProxyImpl(this._value);

  final ManageAppointmentCreate _value;

  @override
  ManageAppointmentCreate branchId(String branchId) => this(branchId: branchId);

  @override
  ManageAppointmentCreate brandCustomerId(String? brandCustomerId) =>
      this(brandCustomerId: brandCustomerId);

  @override
  ManageAppointmentCreate newCustomer(
    ManageAppointmentCreateNewCustomer? newCustomer,
  ) => this(newCustomer: newCustomer);

  @override
  ManageAppointmentCreate channel(ManageAppointmentCreateChannelEnum channel) =>
      this(channel: channel);

  @override
  ManageAppointmentCreate startUtc(DateTime startUtc) =>
      this(startUtc: startUtc);

  @override
  ManageAppointmentCreate items(
    List<ManageAppointmentCreateItemsInner> items,
  ) => this(items: items);

  @override
  ManageAppointmentCreate staffNote(String? staffNote) =>
      this(staffNote: staffNote);

  @override
  ManageAppointmentCreate recurrence(RecurrenceRequest? recurrence) =>
      this(recurrence: recurrence);

  @override
  ManageAppointmentCreate notifyCustomer(bool? notifyCustomer) =>
      this(notifyCustomer: notifyCustomer);

  @override
  ManageAppointmentCreate allowOutsideWorkingHours(
    bool? allowOutsideWorkingHours,
  ) => this(allowOutsideWorkingHours: allowOutsideWorkingHours);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageAppointmentCreate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageAppointmentCreate(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageAppointmentCreate call({
    Object? branchId = const $CopyWithPlaceholder(),
    Object? brandCustomerId = const $CopyWithPlaceholder(),
    Object? newCustomer = const $CopyWithPlaceholder(),
    Object? channel = const $CopyWithPlaceholder(),
    Object? startUtc = const $CopyWithPlaceholder(),
    Object? items = const $CopyWithPlaceholder(),
    Object? staffNote = const $CopyWithPlaceholder(),
    Object? recurrence = const $CopyWithPlaceholder(),
    Object? notifyCustomer = const $CopyWithPlaceholder(),
    Object? allowOutsideWorkingHours = const $CopyWithPlaceholder(),
  }) {
    return ManageAppointmentCreate(
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
      channel: channel == const $CopyWithPlaceholder()
          ? _value.channel
          // ignore: cast_nullable_to_non_nullable
          : channel as ManageAppointmentCreateChannelEnum,
      startUtc: startUtc == const $CopyWithPlaceholder()
          ? _value.startUtc
          // ignore: cast_nullable_to_non_nullable
          : startUtc as DateTime,
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<ManageAppointmentCreateItemsInner>,
      staffNote: staffNote == const $CopyWithPlaceholder()
          ? _value.staffNote
          // ignore: cast_nullable_to_non_nullable
          : staffNote as String?,
      recurrence: recurrence == const $CopyWithPlaceholder()
          ? _value.recurrence
          // ignore: cast_nullable_to_non_nullable
          : recurrence as RecurrenceRequest?,
      notifyCustomer: notifyCustomer == const $CopyWithPlaceholder()
          ? _value.notifyCustomer
          // ignore: cast_nullable_to_non_nullable
          : notifyCustomer as bool?,
      allowOutsideWorkingHours:
          allowOutsideWorkingHours == const $CopyWithPlaceholder()
          ? _value.allowOutsideWorkingHours
          // ignore: cast_nullable_to_non_nullable
          : allowOutsideWorkingHours as bool?,
    );
  }
}

extension $ManageAppointmentCreateCopyWith on ManageAppointmentCreate {
  /// Returns a callable class that can be used as follows: `instanceOfManageAppointmentCreate.copyWith(...)` or like so:`instanceOfManageAppointmentCreate.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageAppointmentCreateCWProxy get copyWith =>
      _$ManageAppointmentCreateCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageAppointmentCreate _$ManageAppointmentCreateFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('ManageAppointmentCreate', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const ['branchId', 'channel', 'startUtc', 'items'],
  );
  final val = ManageAppointmentCreate(
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
    channel: $checkedConvert(
      'channel',
      (v) => $enumDecode(
        _$ManageAppointmentCreateChannelEnumEnumMap,
        v,
        unknownValue: ManageAppointmentCreateChannelEnum.unknownDefaultOpenApi,
      ),
    ),
    startUtc: $checkedConvert('startUtc', (v) => DateTime.parse(v as String)),
    items: $checkedConvert(
      'items',
      (v) => (v as List<dynamic>)
          .map(
            (e) => ManageAppointmentCreateItemsInner.fromJson(
              e as Map<String, dynamic>,
            ),
          )
          .toList(),
    ),
    staffNote: $checkedConvert('staffNote', (v) => v as String?),
    recurrence: $checkedConvert(
      'recurrence',
      (v) => v == null
          ? null
          : RecurrenceRequest.fromJson(v as Map<String, dynamic>),
    ),
    notifyCustomer: $checkedConvert(
      'notifyCustomer',
      (v) => v as bool? ?? true,
    ),
    allowOutsideWorkingHours: $checkedConvert(
      'allowOutsideWorkingHours',
      (v) => v as bool? ?? false,
    ),
  );
  return val;
});

Map<String, dynamic> _$ManageAppointmentCreateToJson(
  ManageAppointmentCreate instance,
) => <String, dynamic>{
  'branchId': instance.branchId,
  'brandCustomerId': ?instance.brandCustomerId,
  'newCustomer': ?instance.newCustomer?.toJson(),
  'channel': _$ManageAppointmentCreateChannelEnumEnumMap[instance.channel]!,
  'startUtc': instance.startUtc.toIso8601String(),
  'items': instance.items.map((e) => e.toJson()).toList(),
  'staffNote': instance.staffNote,
  'recurrence': ?instance.recurrence?.toJson(),
  'notifyCustomer': ?instance.notifyCustomer,
  'allowOutsideWorkingHours': ?instance.allowOutsideWorkingHours,
};

const _$ManageAppointmentCreateChannelEnumEnumMap = {
  ManageAppointmentCreateChannelEnum.panel: 'Panel',
  ManageAppointmentCreateChannelEnum.phone: 'Phone',
  ManageAppointmentCreateChannelEnum.walkIn: 'WalkIn',
  ManageAppointmentCreateChannelEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
