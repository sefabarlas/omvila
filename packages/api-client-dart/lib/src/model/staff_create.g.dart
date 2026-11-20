// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'staff_create.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$StaffCreateCWProxy {
  StaffCreate fullName(String fullName);

  StaffCreate phone(String? phone);

  StaffCreate title(String? title);

  StaffCreate role(StaffCreateRoleEnum role);

  StaffCreate calendarColor(String calendarColor);

  StaffCreate branchIds(List<String> branchIds);

  StaffCreate services(List<StaffServiceAssignment>? services);

  StaffCreate copyBranchWorkingHours(bool? copyBranchWorkingHours);

  StaffCreate serviceCommissionPercent(num? serviceCommissionPercent);

  StaffCreate saleCommissionPercent(num? saleCommissionPercent);

  StaffCreate isBookableOnline(bool? isBookableOnline);

  StaffCreate sendInvite(bool? sendInvite);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `StaffCreate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// StaffCreate(...).copyWith(id: 12, name: "My name")
  /// ````
  StaffCreate call({
    String fullName,
    String? phone,
    String? title,
    StaffCreateRoleEnum role,
    String calendarColor,
    List<String> branchIds,
    List<StaffServiceAssignment>? services,
    bool? copyBranchWorkingHours,
    num? serviceCommissionPercent,
    num? saleCommissionPercent,
    bool? isBookableOnline,
    bool? sendInvite,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfStaffCreate.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfStaffCreate.copyWith.fieldName(...)`
class _$StaffCreateCWProxyImpl implements _$StaffCreateCWProxy {
  const _$StaffCreateCWProxyImpl(this._value);

  final StaffCreate _value;

  @override
  StaffCreate fullName(String fullName) => this(fullName: fullName);

  @override
  StaffCreate phone(String? phone) => this(phone: phone);

  @override
  StaffCreate title(String? title) => this(title: title);

  @override
  StaffCreate role(StaffCreateRoleEnum role) => this(role: role);

  @override
  StaffCreate calendarColor(String calendarColor) =>
      this(calendarColor: calendarColor);

  @override
  StaffCreate branchIds(List<String> branchIds) => this(branchIds: branchIds);

  @override
  StaffCreate services(List<StaffServiceAssignment>? services) =>
      this(services: services);

  @override
  StaffCreate copyBranchWorkingHours(bool? copyBranchWorkingHours) =>
      this(copyBranchWorkingHours: copyBranchWorkingHours);

  @override
  StaffCreate serviceCommissionPercent(num? serviceCommissionPercent) =>
      this(serviceCommissionPercent: serviceCommissionPercent);

  @override
  StaffCreate saleCommissionPercent(num? saleCommissionPercent) =>
      this(saleCommissionPercent: saleCommissionPercent);

  @override
  StaffCreate isBookableOnline(bool? isBookableOnline) =>
      this(isBookableOnline: isBookableOnline);

  @override
  StaffCreate sendInvite(bool? sendInvite) => this(sendInvite: sendInvite);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `StaffCreate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// StaffCreate(...).copyWith(id: 12, name: "My name")
  /// ````
  StaffCreate call({
    Object? fullName = const $CopyWithPlaceholder(),
    Object? phone = const $CopyWithPlaceholder(),
    Object? title = const $CopyWithPlaceholder(),
    Object? role = const $CopyWithPlaceholder(),
    Object? calendarColor = const $CopyWithPlaceholder(),
    Object? branchIds = const $CopyWithPlaceholder(),
    Object? services = const $CopyWithPlaceholder(),
    Object? copyBranchWorkingHours = const $CopyWithPlaceholder(),
    Object? serviceCommissionPercent = const $CopyWithPlaceholder(),
    Object? saleCommissionPercent = const $CopyWithPlaceholder(),
    Object? isBookableOnline = const $CopyWithPlaceholder(),
    Object? sendInvite = const $CopyWithPlaceholder(),
  }) {
    return StaffCreate(
      fullName: fullName == const $CopyWithPlaceholder()
          ? _value.fullName
          // ignore: cast_nullable_to_non_nullable
          : fullName as String,
      phone: phone == const $CopyWithPlaceholder()
          ? _value.phone
          // ignore: cast_nullable_to_non_nullable
          : phone as String?,
      title: title == const $CopyWithPlaceholder()
          ? _value.title
          // ignore: cast_nullable_to_non_nullable
          : title as String?,
      role: role == const $CopyWithPlaceholder()
          ? _value.role
          // ignore: cast_nullable_to_non_nullable
          : role as StaffCreateRoleEnum,
      calendarColor: calendarColor == const $CopyWithPlaceholder()
          ? _value.calendarColor
          // ignore: cast_nullable_to_non_nullable
          : calendarColor as String,
      branchIds: branchIds == const $CopyWithPlaceholder()
          ? _value.branchIds
          // ignore: cast_nullable_to_non_nullable
          : branchIds as List<String>,
      services: services == const $CopyWithPlaceholder()
          ? _value.services
          // ignore: cast_nullable_to_non_nullable
          : services as List<StaffServiceAssignment>?,
      copyBranchWorkingHours:
          copyBranchWorkingHours == const $CopyWithPlaceholder()
          ? _value.copyBranchWorkingHours
          // ignore: cast_nullable_to_non_nullable
          : copyBranchWorkingHours as bool?,
      serviceCommissionPercent:
          serviceCommissionPercent == const $CopyWithPlaceholder()
          ? _value.serviceCommissionPercent
          // ignore: cast_nullable_to_non_nullable
          : serviceCommissionPercent as num?,
      saleCommissionPercent:
          saleCommissionPercent == const $CopyWithPlaceholder()
          ? _value.saleCommissionPercent
          // ignore: cast_nullable_to_non_nullable
          : saleCommissionPercent as num?,
      isBookableOnline: isBookableOnline == const $CopyWithPlaceholder()
          ? _value.isBookableOnline
          // ignore: cast_nullable_to_non_nullable
          : isBookableOnline as bool?,
      sendInvite: sendInvite == const $CopyWithPlaceholder()
          ? _value.sendInvite
          // ignore: cast_nullable_to_non_nullable
          : sendInvite as bool?,
    );
  }
}

extension $StaffCreateCopyWith on StaffCreate {
  /// Returns a callable class that can be used as follows: `instanceOfStaffCreate.copyWith(...)` or like so:`instanceOfStaffCreate.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$StaffCreateCWProxy get copyWith => _$StaffCreateCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

StaffCreate _$StaffCreateFromJson(Map<String, dynamic> json) =>
    $checkedCreate('StaffCreate', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const ['fullName', 'role', 'calendarColor', 'branchIds'],
      );
      final val = StaffCreate(
        fullName: $checkedConvert('fullName', (v) => v as String),
        phone: $checkedConvert('phone', (v) => v as String?),
        title: $checkedConvert('title', (v) => v as String?),
        role: $checkedConvert(
          'role',
          (v) => $enumDecode(
            _$StaffCreateRoleEnumEnumMap,
            v,
            unknownValue: StaffCreateRoleEnum.unknownDefaultOpenApi,
          ),
        ),
        calendarColor: $checkedConvert('calendarColor', (v) => v as String),
        branchIds: $checkedConvert(
          'branchIds',
          (v) => (v as List<dynamic>).map((e) => e as String).toList(),
        ),
        services: $checkedConvert(
          'services',
          (v) => (v as List<dynamic>?)
              ?.map(
                (e) =>
                    StaffServiceAssignment.fromJson(e as Map<String, dynamic>),
              )
              .toList(),
        ),
        copyBranchWorkingHours: $checkedConvert(
          'copyBranchWorkingHours',
          (v) => v as bool? ?? true,
        ),
        serviceCommissionPercent: $checkedConvert(
          'serviceCommissionPercent',
          (v) => v as num? ?? 0,
        ),
        saleCommissionPercent: $checkedConvert(
          'saleCommissionPercent',
          (v) => v as num? ?? 0,
        ),
        isBookableOnline: $checkedConvert(
          'isBookableOnline',
          (v) => v as bool? ?? true,
        ),
        sendInvite: $checkedConvert('sendInvite', (v) => v as bool? ?? true),
      );
      return val;
    });

Map<String, dynamic> _$StaffCreateToJson(StaffCreate instance) =>
    <String, dynamic>{
      'fullName': instance.fullName,
      'phone': ?instance.phone,
      'title': ?instance.title,
      'role': _$StaffCreateRoleEnumEnumMap[instance.role]!,
      'calendarColor': instance.calendarColor,
      'branchIds': instance.branchIds,
      'services': ?instance.services?.map((e) => e.toJson()).toList(),
      'copyBranchWorkingHours': ?instance.copyBranchWorkingHours,
      'serviceCommissionPercent': ?instance.serviceCommissionPercent,
      'saleCommissionPercent': ?instance.saleCommissionPercent,
      'isBookableOnline': ?instance.isBookableOnline,
      'sendInvite': ?instance.sendInvite,
    };

const _$StaffCreateRoleEnumEnumMap = {
  StaffCreateRoleEnum.branchManager: 'BranchManager',
  StaffCreateRoleEnum.staff: 'Staff',
  StaffCreateRoleEnum.unknownDefaultOpenApi: 'unknown_default_open_api',
};
