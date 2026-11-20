// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'service.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ServiceCWProxy {
  Service id(String id);

  Service categoryId(String categoryId);

  Service name(String name);

  Service description(String? description);

  Service durationMinutes(int durationMinutes);

  Service price(Money price);

  Service priceDisplay(ServicePriceDisplayEnum priceDisplay);

  Service isBranchPrice(bool? isBranchPrice);

  Service bufferBeforeMinutes(int bufferBeforeMinutes);

  Service bufferAfterMinutes(int bufferAfterMinutes);

  Service deposit(DepositRule deposit);

  Service requiresApproval(bool? requiresApproval);

  Service minNoticeMinutes(int? minNoticeMinutes);

  Service maxAdvanceDays(int? maxAdvanceDays);

  Service resourceTypeId(String? resourceTypeId);

  Service isOnlineBookable(bool isOnlineBookable);

  Service isActive(bool isActive);

  Service sortOrder(int? sortOrder);

  Service stages(List<ServiceStage> stages);

  Service staffIds(List<String> staffIds);

  Service version(String version);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `Service(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// Service(...).copyWith(id: 12, name: "My name")
  /// ````
  Service call({
    String id,
    String categoryId,
    String name,
    String? description,
    int durationMinutes,
    Money price,
    ServicePriceDisplayEnum priceDisplay,
    bool? isBranchPrice,
    int bufferBeforeMinutes,
    int bufferAfterMinutes,
    DepositRule deposit,
    bool? requiresApproval,
    int? minNoticeMinutes,
    int? maxAdvanceDays,
    String? resourceTypeId,
    bool isOnlineBookable,
    bool isActive,
    int? sortOrder,
    List<ServiceStage> stages,
    List<String> staffIds,
    String version,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfService.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfService.copyWith.fieldName(...)`
class _$ServiceCWProxyImpl implements _$ServiceCWProxy {
  const _$ServiceCWProxyImpl(this._value);

  final Service _value;

  @override
  Service id(String id) => this(id: id);

  @override
  Service categoryId(String categoryId) => this(categoryId: categoryId);

  @override
  Service name(String name) => this(name: name);

  @override
  Service description(String? description) => this(description: description);

  @override
  Service durationMinutes(int durationMinutes) =>
      this(durationMinutes: durationMinutes);

  @override
  Service price(Money price) => this(price: price);

  @override
  Service priceDisplay(ServicePriceDisplayEnum priceDisplay) =>
      this(priceDisplay: priceDisplay);

  @override
  Service isBranchPrice(bool? isBranchPrice) =>
      this(isBranchPrice: isBranchPrice);

  @override
  Service bufferBeforeMinutes(int bufferBeforeMinutes) =>
      this(bufferBeforeMinutes: bufferBeforeMinutes);

  @override
  Service bufferAfterMinutes(int bufferAfterMinutes) =>
      this(bufferAfterMinutes: bufferAfterMinutes);

  @override
  Service deposit(DepositRule deposit) => this(deposit: deposit);

  @override
  Service requiresApproval(bool? requiresApproval) =>
      this(requiresApproval: requiresApproval);

  @override
  Service minNoticeMinutes(int? minNoticeMinutes) =>
      this(minNoticeMinutes: minNoticeMinutes);

  @override
  Service maxAdvanceDays(int? maxAdvanceDays) =>
      this(maxAdvanceDays: maxAdvanceDays);

  @override
  Service resourceTypeId(String? resourceTypeId) =>
      this(resourceTypeId: resourceTypeId);

  @override
  Service isOnlineBookable(bool isOnlineBookable) =>
      this(isOnlineBookable: isOnlineBookable);

  @override
  Service isActive(bool isActive) => this(isActive: isActive);

  @override
  Service sortOrder(int? sortOrder) => this(sortOrder: sortOrder);

  @override
  Service stages(List<ServiceStage> stages) => this(stages: stages);

  @override
  Service staffIds(List<String> staffIds) => this(staffIds: staffIds);

  @override
  Service version(String version) => this(version: version);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `Service(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// Service(...).copyWith(id: 12, name: "My name")
  /// ````
  Service call({
    Object? id = const $CopyWithPlaceholder(),
    Object? categoryId = const $CopyWithPlaceholder(),
    Object? name = const $CopyWithPlaceholder(),
    Object? description = const $CopyWithPlaceholder(),
    Object? durationMinutes = const $CopyWithPlaceholder(),
    Object? price = const $CopyWithPlaceholder(),
    Object? priceDisplay = const $CopyWithPlaceholder(),
    Object? isBranchPrice = const $CopyWithPlaceholder(),
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
    Object? version = const $CopyWithPlaceholder(),
  }) {
    return Service(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
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
          : priceDisplay as ServicePriceDisplayEnum,
      isBranchPrice: isBranchPrice == const $CopyWithPlaceholder()
          ? _value.isBranchPrice
          // ignore: cast_nullable_to_non_nullable
          : isBranchPrice as bool?,
      bufferBeforeMinutes: bufferBeforeMinutes == const $CopyWithPlaceholder()
          ? _value.bufferBeforeMinutes
          // ignore: cast_nullable_to_non_nullable
          : bufferBeforeMinutes as int,
      bufferAfterMinutes: bufferAfterMinutes == const $CopyWithPlaceholder()
          ? _value.bufferAfterMinutes
          // ignore: cast_nullable_to_non_nullable
          : bufferAfterMinutes as int,
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
          : isActive as bool,
      sortOrder: sortOrder == const $CopyWithPlaceholder()
          ? _value.sortOrder
          // ignore: cast_nullable_to_non_nullable
          : sortOrder as int?,
      stages: stages == const $CopyWithPlaceholder()
          ? _value.stages
          // ignore: cast_nullable_to_non_nullable
          : stages as List<ServiceStage>,
      staffIds: staffIds == const $CopyWithPlaceholder()
          ? _value.staffIds
          // ignore: cast_nullable_to_non_nullable
          : staffIds as List<String>,
      version: version == const $CopyWithPlaceholder()
          ? _value.version
          // ignore: cast_nullable_to_non_nullable
          : version as String,
    );
  }
}

extension $ServiceCopyWith on Service {
  /// Returns a callable class that can be used as follows: `instanceOfService.copyWith(...)` or like so:`instanceOfService.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ServiceCWProxy get copyWith => _$ServiceCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Service _$ServiceFromJson(Map<String, dynamic> json) => $checkedCreate(
  'Service',
  json,
  ($checkedConvert) {
    $checkKeys(
      json,
      requiredKeys: const [
        'id',
        'categoryId',
        'name',
        'durationMinutes',
        'price',
        'priceDisplay',
        'bufferBeforeMinutes',
        'bufferAfterMinutes',
        'deposit',
        'isOnlineBookable',
        'isActive',
        'stages',
        'staffIds',
        'version',
      ],
    );
    final val = Service(
      id: $checkedConvert('id', (v) => v as String),
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
          _$ServicePriceDisplayEnumEnumMap,
          v,
          unknownValue: ServicePriceDisplayEnum.unknownDefaultOpenApi,
        ),
      ),
      isBranchPrice: $checkedConvert('isBranchPrice', (v) => v as bool?),
      bufferBeforeMinutes: $checkedConvert(
        'bufferBeforeMinutes',
        (v) => (v as num).toInt(),
      ),
      bufferAfterMinutes: $checkedConvert(
        'bufferAfterMinutes',
        (v) => (v as num).toInt(),
      ),
      deposit: $checkedConvert(
        'deposit',
        (v) => DepositRule.fromJson(v as Map<String, dynamic>),
      ),
      requiresApproval: $checkedConvert('requiresApproval', (v) => v as bool?),
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
      isActive: $checkedConvert('isActive', (v) => v as bool),
      sortOrder: $checkedConvert('sortOrder', (v) => (v as num?)?.toInt()),
      stages: $checkedConvert(
        'stages',
        (v) => (v as List<dynamic>)
            .map((e) => ServiceStage.fromJson(e as Map<String, dynamic>))
            .toList(),
      ),
      staffIds: $checkedConvert(
        'staffIds',
        (v) => (v as List<dynamic>).map((e) => e as String).toList(),
      ),
      version: $checkedConvert('version', (v) => v as String),
    );
    return val;
  },
);

Map<String, dynamic> _$ServiceToJson(Service instance) => <String, dynamic>{
  'id': instance.id,
  'categoryId': instance.categoryId,
  'name': instance.name,
  'description': ?instance.description,
  'durationMinutes': instance.durationMinutes,
  'price': instance.price.toJson(),
  'priceDisplay': _$ServicePriceDisplayEnumEnumMap[instance.priceDisplay]!,
  'isBranchPrice': ?instance.isBranchPrice,
  'bufferBeforeMinutes': instance.bufferBeforeMinutes,
  'bufferAfterMinutes': instance.bufferAfterMinutes,
  'deposit': instance.deposit.toJson(),
  'requiresApproval': ?instance.requiresApproval,
  'minNoticeMinutes': ?instance.minNoticeMinutes,
  'maxAdvanceDays': ?instance.maxAdvanceDays,
  'resourceTypeId': ?instance.resourceTypeId,
  'isOnlineBookable': instance.isOnlineBookable,
  'isActive': instance.isActive,
  'sortOrder': ?instance.sortOrder,
  'stages': instance.stages.map((e) => e.toJson()).toList(),
  'staffIds': instance.staffIds,
  'version': instance.version,
};

const _$ServicePriceDisplayEnumEnumMap = {
  ServicePriceDisplayEnum.fixed: 'Fixed',
  ServicePriceDisplayEnum.startingFrom: 'StartingFrom',
  ServicePriceDisplayEnum.unknownDefaultOpenApi: 'unknown_default_open_api',
};
