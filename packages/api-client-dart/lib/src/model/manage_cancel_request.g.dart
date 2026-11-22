// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_cancel_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageCancelRequestCWProxy {
  ManageCancelRequest reason(ManageCancelRequestReasonEnum reason);

  ManageCancelRequest note(String? note);

  ManageCancelRequest scope(ManageCancelRequestScopeEnum? scope);

  ManageCancelRequest notifyCustomer(bool? notifyCustomer);

  ManageCancelRequest offerToWaitlist(bool? offerToWaitlist);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageCancelRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageCancelRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageCancelRequest call({
    ManageCancelRequestReasonEnum reason,
    String? note,
    ManageCancelRequestScopeEnum? scope,
    bool? notifyCustomer,
    bool? offerToWaitlist,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageCancelRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageCancelRequest.copyWith.fieldName(...)`
class _$ManageCancelRequestCWProxyImpl implements _$ManageCancelRequestCWProxy {
  const _$ManageCancelRequestCWProxyImpl(this._value);

  final ManageCancelRequest _value;

  @override
  ManageCancelRequest reason(ManageCancelRequestReasonEnum reason) =>
      this(reason: reason);

  @override
  ManageCancelRequest note(String? note) => this(note: note);

  @override
  ManageCancelRequest scope(ManageCancelRequestScopeEnum? scope) =>
      this(scope: scope);

  @override
  ManageCancelRequest notifyCustomer(bool? notifyCustomer) =>
      this(notifyCustomer: notifyCustomer);

  @override
  ManageCancelRequest offerToWaitlist(bool? offerToWaitlist) =>
      this(offerToWaitlist: offerToWaitlist);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageCancelRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageCancelRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageCancelRequest call({
    Object? reason = const $CopyWithPlaceholder(),
    Object? note = const $CopyWithPlaceholder(),
    Object? scope = const $CopyWithPlaceholder(),
    Object? notifyCustomer = const $CopyWithPlaceholder(),
    Object? offerToWaitlist = const $CopyWithPlaceholder(),
  }) {
    return ManageCancelRequest(
      reason: reason == const $CopyWithPlaceholder()
          ? _value.reason
          // ignore: cast_nullable_to_non_nullable
          : reason as ManageCancelRequestReasonEnum,
      note: note == const $CopyWithPlaceholder()
          ? _value.note
          // ignore: cast_nullable_to_non_nullable
          : note as String?,
      scope: scope == const $CopyWithPlaceholder()
          ? _value.scope
          // ignore: cast_nullable_to_non_nullable
          : scope as ManageCancelRequestScopeEnum?,
      notifyCustomer: notifyCustomer == const $CopyWithPlaceholder()
          ? _value.notifyCustomer
          // ignore: cast_nullable_to_non_nullable
          : notifyCustomer as bool?,
      offerToWaitlist: offerToWaitlist == const $CopyWithPlaceholder()
          ? _value.offerToWaitlist
          // ignore: cast_nullable_to_non_nullable
          : offerToWaitlist as bool?,
    );
  }
}

extension $ManageCancelRequestCopyWith on ManageCancelRequest {
  /// Returns a callable class that can be used as follows: `instanceOfManageCancelRequest.copyWith(...)` or like so:`instanceOfManageCancelRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageCancelRequestCWProxy get copyWith =>
      _$ManageCancelRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageCancelRequest _$ManageCancelRequestFromJson(Map<String, dynamic> json) =>
    $checkedCreate('ManageCancelRequest', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['reason']);
      final val = ManageCancelRequest(
        reason: $checkedConvert(
          'reason',
          (v) => $enumDecode(
            _$ManageCancelRequestReasonEnumEnumMap,
            v,
            unknownValue: ManageCancelRequestReasonEnum.unknownDefaultOpenApi,
          ),
        ),
        note: $checkedConvert('note', (v) => v as String?),
        scope: $checkedConvert(
          'scope',
          (v) => $enumDecodeNullable(
            _$ManageCancelRequestScopeEnumEnumMap,
            v,
            unknownValue: ManageCancelRequestScopeEnum.unknownDefaultOpenApi,
          ),
        ),
        notifyCustomer: $checkedConvert(
          'notifyCustomer',
          (v) => v as bool? ?? true,
        ),
        offerToWaitlist: $checkedConvert(
          'offerToWaitlist',
          (v) => v as bool? ?? true,
        ),
      );
      return val;
    });

Map<String, dynamic> _$ManageCancelRequestToJson(
  ManageCancelRequest instance,
) => <String, dynamic>{
  'reason': _$ManageCancelRequestReasonEnumEnumMap[instance.reason]!,
  'note': instance.note,
  'scope': ?_$ManageCancelRequestScopeEnumEnumMap[instance.scope],
  'notifyCustomer': ?instance.notifyCustomer,
  'offerToWaitlist': ?instance.offerToWaitlist,
};

const _$ManageCancelRequestReasonEnumEnumMap = {
  ManageCancelRequestReasonEnum.customerRequested: 'CustomerRequested',
  ManageCancelRequestReasonEnum.staffUnavailable: 'StaffUnavailable',
  ManageCancelRequestReasonEnum.other: 'Other',
  ManageCancelRequestReasonEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};

const _$ManageCancelRequestScopeEnumEnumMap = {
  ManageCancelRequestScopeEnum.thisOnly: 'ThisOnly',
  ManageCancelRequestScopeEnum.thisAndFollowing: 'ThisAndFollowing',
  ManageCancelRequestScopeEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
