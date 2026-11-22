// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_subscription_list_item_next_step.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminSubscriptionListItemNextStepCWProxy {
  AdminSubscriptionListItemNextStep type(
    AdminSubscriptionListItemNextStepTypeEnum type,
  );

  AdminSubscriptionListItemNextStep date(DateTime? date);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminSubscriptionListItemNextStep(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminSubscriptionListItemNextStep(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminSubscriptionListItemNextStep call({
    AdminSubscriptionListItemNextStepTypeEnum type,
    DateTime? date,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminSubscriptionListItemNextStep.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminSubscriptionListItemNextStep.copyWith.fieldName(...)`
class _$AdminSubscriptionListItemNextStepCWProxyImpl
    implements _$AdminSubscriptionListItemNextStepCWProxy {
  const _$AdminSubscriptionListItemNextStepCWProxyImpl(this._value);

  final AdminSubscriptionListItemNextStep _value;

  @override
  AdminSubscriptionListItemNextStep type(
    AdminSubscriptionListItemNextStepTypeEnum type,
  ) => this(type: type);

  @override
  AdminSubscriptionListItemNextStep date(DateTime? date) => this(date: date);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminSubscriptionListItemNextStep(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminSubscriptionListItemNextStep(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminSubscriptionListItemNextStep call({
    Object? type = const $CopyWithPlaceholder(),
    Object? date = const $CopyWithPlaceholder(),
  }) {
    return AdminSubscriptionListItemNextStep(
      type: type == const $CopyWithPlaceholder()
          ? _value.type
          // ignore: cast_nullable_to_non_nullable
          : type as AdminSubscriptionListItemNextStepTypeEnum,
      date: date == const $CopyWithPlaceholder()
          ? _value.date
          // ignore: cast_nullable_to_non_nullable
          : date as DateTime?,
    );
  }
}

extension $AdminSubscriptionListItemNextStepCopyWith
    on AdminSubscriptionListItemNextStep {
  /// Returns a callable class that can be used as follows: `instanceOfAdminSubscriptionListItemNextStep.copyWith(...)` or like so:`instanceOfAdminSubscriptionListItemNextStep.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminSubscriptionListItemNextStepCWProxy get copyWith =>
      _$AdminSubscriptionListItemNextStepCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminSubscriptionListItemNextStep _$AdminSubscriptionListItemNextStepFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AdminSubscriptionListItemNextStep', json, (
  $checkedConvert,
) {
  $checkKeys(json, requiredKeys: const ['type', 'date']);
  final val = AdminSubscriptionListItemNextStep(
    type: $checkedConvert(
      'type',
      (v) => $enumDecode(
        _$AdminSubscriptionListItemNextStepTypeEnumEnumMap,
        v,
        unknownValue:
            AdminSubscriptionListItemNextStepTypeEnum.unknownDefaultOpenApi,
      ),
    ),
    date: $checkedConvert(
      'date',
      (v) => _$JsonConverterFromJson<String, DateTime>(
        v,
        const DateOnlyConverter().fromJson,
      ),
    ),
  );
  return val;
});

Map<String, dynamic> _$AdminSubscriptionListItemNextStepToJson(
  AdminSubscriptionListItemNextStep instance,
) => <String, dynamic>{
  'type': _$AdminSubscriptionListItemNextStepTypeEnumEnumMap[instance.type]!,
  'date': _$JsonConverterToJson<String, DateTime>(
    instance.date,
    const DateOnlyConverter().toJson,
  ),
};

const _$AdminSubscriptionListItemNextStepTypeEnumEnumMap = {
  AdminSubscriptionListItemNextStepTypeEnum.trialEnds: 'TrialEnds',
  AdminSubscriptionListItemNextStepTypeEnum.readOnlyOn: 'ReadOnlyOn',
  AdminSubscriptionListItemNextStepTypeEnum.renews: 'Renews',
  AdminSubscriptionListItemNextStepTypeEnum.reactivatesOnPayment:
      'ReactivatesOnPayment',
  AdminSubscriptionListItemNextStepTypeEnum.smsStopped: 'SmsStopped',
  AdminSubscriptionListItemNextStepTypeEnum.archivesOn: 'ArchivesOn',
  AdminSubscriptionListItemNextStepTypeEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};

Value? _$JsonConverterFromJson<Json, Value>(
  Object? json,
  Value? Function(Json json) fromJson,
) => json == null ? null : fromJson(json as Json);

Json? _$JsonConverterToJson<Json, Value>(
  Value? value,
  Json? Function(Value value) toJson,
) => value == null ? null : toJson(value);
