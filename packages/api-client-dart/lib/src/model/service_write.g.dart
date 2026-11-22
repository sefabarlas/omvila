// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'service_write.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ServiceWriteCWProxy {
  ServiceWrite categoryId(String categoryId);

  ServiceWrite name(String name);

  ServiceWrite description(String? description);

  ServiceWrite durationMinutes(int durationMinutes);

  ServiceWrite price(Money price);

  ServiceWrite priceDisplay(ServiceWritePriceDisplayEnum priceDisplay);

  ServiceWrite bufferBeforeMinutes(int? bufferBeforeMinutes);

  ServiceWrite bufferAfterMinutes(int? bufferAfterMinutes);

  ServiceWrite deposit(DepositRule deposit);

  ServiceWrite requiresApproval(bool? requiresApproval);

  ServiceWrite minNoticeMinutes(int? minNoticeMinutes);

  ServiceWrite maxAdvanceDays(int? maxAdvanceDays);

  ServiceWrite resourceTypeId(String? resourceTypeId);

  ServiceWrite isOnlineBookable(bool isOnlineBookable);

  ServiceWrite isActive(bool? isActive);

  ServiceWrite sortOrder(int? sortOrder);

  ServiceWrite stages(List<ServiceStage>? stages);

  ServiceWrite staffIds(List<String> staffIds);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ServiceWrite(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ServiceWrite(...).copyWith(id: 12, name: "My name")
  /// ````
  ServiceWrite call({
    String categoryId,
    String name,
    String? description,
    int durationMinutes,
    Money price,
    ServiceWritePriceDisplayEnum priceDisplay,
    int? bufferBeforeMinutes,
    int? bufferAfterMinutes,
    DepositRule deposit,
    bool? requiresApproval,
    int? minNoticeMinutes,
    int? maxAdvanceDays,
    String? resourceTypeId,
    bool isOnlineBookable,
    bool? isActive,
    int? sortOrder,
    List<ServiceStage>? stages,
    List<String> staffIds,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfServiceWrite.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfServiceWrite.copyWith.fieldName(...)`
class _$ServiceWriteCWProxyImpl implements _$ServiceWriteCWProxy {
  const _$ServiceWriteCWProxyImpl(this._value);

  final ServiceWrite _value;

  @override
  ServiceWrite categoryId(String categoryId) => this(categoryId: categoryId);

  @override
  ServiceWrite name(String name) => this(name: name);

  @override
  ServiceWrite description(String? description) =>
      this(description: description);

  @override
  ServiceWrite durationMinutes(int durationMinutes) =>
      this(durationMinutes: durationMinutes);

  @override
  ServiceWrite price(Money price) => this(price: price);

  @override
  ServiceWrite priceDisplay(ServiceWritePriceDisplayEnum priceDisplay) =>
      this(priceDisplay: priceDisplay);

  @override
  ServiceWrite bufferBeforeMinutes(int? bufferBeforeMinutes) =>
      this(bufferBeforeMinutes: bufferBeforeMinutes);

  @override
  ServiceWrite bufferAfterMinutes(int? bufferAfterMinutes) =>
      this(bufferAfterMinutes: bufferAfterMinutes);

  @override
  ServiceWrite deposit(DepositRule deposit) => this(deposit: deposit);

  @override
  ServiceWrite requiresApproval(bool? requiresApproval) =>
      this(requiresApproval: requiresApproval);

  @override
  ServiceWrite minNoticeMinutes(int? minNoticeMinutes) =>
      this(minNoticeMinutes: minNoticeMinutes);

  @override
  ServiceWrite maxAdvanceDays(int? maxAdvanceDays) =>
      this(maxAdvanceDays: maxAdvanceDays);

  @override
  ServiceWrite resourceTypeId(String? resourceTypeId) =>
      this(resourceTypeId: resourceTypeId);

  @override
  ServiceWrite isOnlineBookable(bool isOnlineBookable) =>
      this(isOnlineBookable: isOnlineBookable);

  @override
  ServiceWrite isActive(bool? isActive) => this(isActive: isActive);

  @override
  ServiceWrite sortOrder(int? sortOrder) => this(sortOrder: sortOrder);

  @override
  ServiceWrite stages(List<ServiceStage>? stages) => this(stages: stages);

  @override
  ServiceWrite staffIds(List<String> staffIds) => this(staffIds: staffIds);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ServiceWrite(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ServiceWrite(...).copyWith(id: 12, name: "My name")
  /// ````
  ServiceWrite call({
    Object? categoryId = const $CopyWithPlaceholder(),
    Object? name = const $CopyWithPlaceholder(),
    Object? description = const $CopyWithPlaceholder(),
    Object? durationMinutes = const $CopyWithPlaceholder(),
    Object? price = const $CopyWithPlaceholder(),
    Object? priceDisplay = const $CopyWithPlaceholder(),
    Object? bufferBeforeMinutes = const $CopyWithPlaceholder(),
    Object? bufferAfterMinutes = const $CopyWithPlaceholder(),
    Object? deposit = const $CopyWithPlaceholder(),
    Object? requiresApproval = const $CopyWithPlaceholder(),
    Object? minNoticeMinutes = const $CopyWithPlaceholder(),
    Object? maxAdvanceDays = const $CopyWithPlaceholder(),
    Object? resourceTypeId = const $CopyWithPlaceholder(),
    Object? isOnlineBookable = const $CopyWithPlaceholder(),
    Object? isActive = const $CopyWithPlaceholder(),
    Object? sortOrder = const $CopyWithPlaceholder(),
    Object? stages = const $CopyWithPlaceholder(),
    Object? staffIds = const $CopyWithPlaceholder(),
  }) {
    return ServiceWrite(
      categoryId: categoryId == const $CopyWithPlaceholder()
          ? _value.categoryId
          // ignore: cast_nullable_to_non_nullable
          : categoryId as String,
      name: name == const $CopyWithPlaceholder()
          ? _value.name
          // ignore: cast_nullable_to_non_nullable
          : name as String,
      description: description == const $CopyWithPlaceholder()
          ? _value.description
          // ignore: cast_nullable_to_non_nullable
          : description as String?,
      durationMinutes: durationMinutes == const $CopyWithPlaceholder()
          ? _value.durationMinutes
          // ignore: cast_nullable_to_non_nullable
          : durationMinutes as int,
      price: price == const $CopyWithPlaceholder()
          ? _value.price
          // ignore: cast_nullable_to_non_nullable
          : price as Money,
      priceDisplay: priceDisplay == const $CopyWithPlaceholder()
          ? _value.priceDisplay
          // ignore: cast_nullable_to_non_nullable
          : priceDisplay as ServiceWritePriceDisplayEnum,
      bufferBeforeMinutes: bufferBeforeMinutes == const $CopyWithPlaceholder()
          ? _value.bufferBeforeMinutes
          // ignore: cast_nullable_to_non_nullable
          : bufferBeforeMinutes as int?,
      bufferAfterMinutes: bufferAfterMinutes == const $CopyWithPlaceholder()
          ? _value.bufferAfterMinutes
          // ignore: cast_nullable_to_non_nullable
          : bufferAfterMinutes as int?,
      deposit: deposit == const $CopyWithPlaceholder()
          ? _value.deposit
          // ignore: cast_nullable_to_non_nullable
          : deposit as DepositRule,
      requiresApproval: requiresApproval == const $CopyWithPlaceholder()
          ? _value.requiresApproval
          // ignore: cast_nullable_to_non_nullable
          : requiresApproval as bool?,
      minNoticeMinutes: minNoticeMinutes == const $CopyWithPlaceholder()
          ? _value.minNoticeMinutes
          // ignore: cast_nullable_to_non_nullable
          : minNoticeMinutes as int?,
      maxAdvanceDays: maxAdvanceDays == const $CopyWithPlaceholder()
          ? _value.maxAdvanceDays
          // ignore: cast_nullable_to_non_nullable
          : maxAdvanceDays as int?,
      resourceTypeId: resourceTypeId == const $CopyWithPlaceholder()
          ? _value.resourceTypeId
          // ignore: cast_nullable_to_non_nullable
          : resourceTypeId as String?,
      isOnlineBookable: isOnlineBookable == const $CopyWithPlaceholder()
          ? _value.isOnlineBookable
          // ignore: cast_nullable_to_non_nullable
          : isOnlineBookable as bool,
      isActive: isActive == const $CopyWithPlaceholder()
          ? _value.isActive
          // ignore: cast_nullable_to_non_nullable
          : isActive as bool?,
      sortOrder: sortOrder == const $CopyWithPlaceholder()
          ? _value.sortOrder
          // ignore: cast_nullable_to_non_nullable
          : sortOrder as int?,
      stages: stages == const $CopyWithPlaceholder()
          ? _value.stages
          // ignore: cast_nullable_to_non_nullable
          : stages as List<ServiceStage>?,
      staffIds: staffIds == const $CopyWithPlaceholder()
          ? _value.staffIds
          // ignore: cast_nullable_to_non_nullable
          : staffIds as List<String>,
    );
  }
}

extension $ServiceWriteCopyWith on ServiceWrite {
  /// Returns a callable class that can be used as follows: `instanceOfServiceWrite.copyWith(...)` or like so:`instanceOfServiceWrite.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ServiceWriteCWProxy get copyWith => _$ServiceWriteCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ServiceWrite _$ServiceWriteFromJson(Map<String, dynamic> json) =>
    $checkedCreate('ServiceWrite', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'categoryId',
          'name',
          'durationMinutes',
          'price',
          'priceDisplay',
          'deposit',
          'isOnlineBookable',
          'staffIds',
        ],
      );
      final val = ServiceWrite(
        categoryId: $checkedConvert('categoryId', (v) => v as String),
        name: $checkedConvert('name', (v) => v as String),
        description: $checkedConvert('description', (v) => v as String?),
        durationMinutes: $checkedConvert(
          'durationMinutes',
          (v) => (v as num).toInt(),
        ),
        price: $checkedConvert(
          'price',
          (v) => Money.fromJson(v as Map<String, dynamic>),
        ),
        priceDisplay: $checkedConvert(
          'priceDisplay',
          (v) => $enumDecode(
            _$ServiceWritePriceDisplayEnumEnumMap,
            v,
            unknownValue: ServiceWritePriceDisplayEnum.unknownDefaultOpenApi,
          ),
        ),
        bufferBeforeMinutes: $checkedConvert(
          'bufferBeforeMinutes',
          (v) => (v as num?)?.toInt() ?? 0,
        ),
        bufferAfterMinutes: $checkedConvert(
          'bufferAfterMinutes',
          (v) => (v as num?)?.toInt() ?? 0,
        ),
        deposit: $checkedConvert(
          'deposit',
          (v) => DepositRule.fromJson(v as Map<String, dynamic>),
        ),
        requiresApproval: $checkedConvert(
          'requiresApproval',
          (v) => v as bool?,
        ),
        minNoticeMinutes: $checkedConvert(
          'minNoticeMinutes',
          (v) => (v as num?)?.toInt(),
        ),
        maxAdvanceDays: $checkedConvert(
          'maxAdvanceDays',
          (v) => (v as num?)?.toInt(),
        ),
        resourceTypeId: $checkedConvert('resourceTypeId', (v) => v as String?),
        isOnlineBookable: $checkedConvert('isOnlineBookable', (v) => v as bool),
        isActive: $checkedConvert('isActive', (v) => v as bool? ?? true),
        sortOrder: $checkedConvert('sortOrder', (v) => (v as num?)?.toInt()),
        stages: $checkedConvert(
          'stages',
          (v) => (v as List<dynamic>?)
              ?.map((e) => ServiceStage.fromJson(e as Map<String, dynamic>))
              .toList(),
        ),
        staffIds: $checkedConvert(
          'staffIds',
          (v) => (v as List<dynamic>).map((e) => e as String).toList(),
        ),
      );
      return val;
    });

Map<String, dynamic> _$ServiceWriteToJson(
  ServiceWrite instance,
) => <String, dynamic>{
  'categoryId': instance.categoryId,
  'name': instance.name,
  'description': instance.description,
  'durationMinutes': instance.durationMinutes,
  'price': instance.price.toJson(),
  'priceDisplay': _$ServiceWritePriceDisplayEnumEnumMap[instance.priceDisplay]!,
  'bufferBeforeMinutes': ?instance.bufferBeforeMinutes,
  'bufferAfterMinutes': ?instance.bufferAfterMinutes,
  'deposit': instance.deposit.toJson(),
  'requiresApproval': instance.requiresApproval,
  'minNoticeMinutes': instance.minNoticeMinutes,
  'maxAdvanceDays': instance.maxAdvanceDays,
  'resourceTypeId': instance.resourceTypeId,
  'isOnlineBookable': instance.isOnlineBookable,
  'isActive': ?instance.isActive,
  'sortOrder': ?instance.sortOrder,
  'stages': ?instance.stages?.map((e) => e.toJson()).toList(),
  'staffIds': instance.staffIds,
};

const _$ServiceWritePriceDisplayEnumEnumMap = {
  ServiceWritePriceDisplayEnum.fixed: 'Fixed',
  ServiceWritePriceDisplayEnum.startingFrom: 'StartingFrom',
  ServiceWritePriceDisplayEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
