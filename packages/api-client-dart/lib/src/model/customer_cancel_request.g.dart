// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'customer_cancel_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CustomerCancelRequestCWProxy {
  CustomerCancelRequest reason(CustomerCancelRequestReasonEnum reason);

  CustomerCancelRequest note(String? note);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerCancelRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerCancelRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerCancelRequest call({
    CustomerCancelRequestReasonEnum reason,
    String? note,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCustomerCancelRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCustomerCancelRequest.copyWith.fieldName(...)`
class _$CustomerCancelRequestCWProxyImpl
    implements _$CustomerCancelRequestCWProxy {
  const _$CustomerCancelRequestCWProxyImpl(this._value);

  final CustomerCancelRequest _value;

  @override
  CustomerCancelRequest reason(CustomerCancelRequestReasonEnum reason) =>
      this(reason: reason);

  @override
  CustomerCancelRequest note(String? note) => this(note: note);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerCancelRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerCancelRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerCancelRequest call({
    Object? reason = const $CopyWithPlaceholder(),
    Object? note = const $CopyWithPlaceholder(),
  }) {
    return CustomerCancelRequest(
      reason: reason == const $CopyWithPlaceholder()
          ? _value.reason
          // ignore: cast_nullable_to_non_nullable
          : reason as CustomerCancelRequestReasonEnum,
      note: note == const $CopyWithPlaceholder()
          ? _value.note
          // ignore: cast_nullable_to_non_nullable
          : note as String?,
    );
  }
}

extension $CustomerCancelRequestCopyWith on CustomerCancelRequest {
  /// Returns a callable class that can be used as follows: `instanceOfCustomerCancelRequest.copyWith(...)` or like so:`instanceOfCustomerCancelRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CustomerCancelRequestCWProxy get copyWith =>
      _$CustomerCancelRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CustomerCancelRequest _$CustomerCancelRequestFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('CustomerCancelRequest', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['reason']);
  final val = CustomerCancelRequest(
    reason: $checkedConvert(
      'reason',
      (v) => $enumDecode(
        _$CustomerCancelRequestReasonEnumEnumMap,
        v,
        unknownValue: CustomerCancelRequestReasonEnum.unknownDefaultOpenApi,
      ),
    ),
    note: $checkedConvert('note', (v) => v as String?),
  );
  return val;
});

Map<String, dynamic> _$CustomerCancelRequestToJson(
  CustomerCancelRequest instance,
) => <String, dynamic>{
  'reason': _$CustomerCancelRequestReasonEnumEnumMap[instance.reason]!,
  'note': ?instance.note,
};

const _$CustomerCancelRequestReasonEnumEnumMap = {
  CustomerCancelRequestReasonEnum.planChanged: 'PlanChanged',
  CustomerCancelRequestReasonEnum.sick: 'Sick',
  CustomerCancelRequestReasonEnum.wantsDifferentTime: 'WantsDifferentTime',
  CustomerCancelRequestReasonEnum.other: 'Other',
  CustomerCancelRequestReasonEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
