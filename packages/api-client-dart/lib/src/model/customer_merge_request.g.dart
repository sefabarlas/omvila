// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'customer_merge_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CustomerMergeRequestCWProxy {
  CustomerMergeRequest sourceBrandCustomerId(String sourceBrandCustomerId);

  CustomerMergeRequest mergeNotes(bool? mergeNotes);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerMergeRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerMergeRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerMergeRequest call({String sourceBrandCustomerId, bool? mergeNotes});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCustomerMergeRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCustomerMergeRequest.copyWith.fieldName(...)`
class _$CustomerMergeRequestCWProxyImpl
    implements _$CustomerMergeRequestCWProxy {
  const _$CustomerMergeRequestCWProxyImpl(this._value);

  final CustomerMergeRequest _value;

  @override
  CustomerMergeRequest sourceBrandCustomerId(String sourceBrandCustomerId) =>
      this(sourceBrandCustomerId: sourceBrandCustomerId);

  @override
  CustomerMergeRequest mergeNotes(bool? mergeNotes) =>
      this(mergeNotes: mergeNotes);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerMergeRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerMergeRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerMergeRequest call({
    Object? sourceBrandCustomerId = const $CopyWithPlaceholder(),
    Object? mergeNotes = const $CopyWithPlaceholder(),
  }) {
    return CustomerMergeRequest(
      sourceBrandCustomerId:
          sourceBrandCustomerId == const $CopyWithPlaceholder()
          ? _value.sourceBrandCustomerId
          // ignore: cast_nullable_to_non_nullable
          : sourceBrandCustomerId as String,
      mergeNotes: mergeNotes == const $CopyWithPlaceholder()
          ? _value.mergeNotes
          // ignore: cast_nullable_to_non_nullable
          : mergeNotes as bool?,
    );
  }
}

extension $CustomerMergeRequestCopyWith on CustomerMergeRequest {
  /// Returns a callable class that can be used as follows: `instanceOfCustomerMergeRequest.copyWith(...)` or like so:`instanceOfCustomerMergeRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CustomerMergeRequestCWProxy get copyWith =>
      _$CustomerMergeRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CustomerMergeRequest _$CustomerMergeRequestFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('CustomerMergeRequest', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['sourceBrandCustomerId']);
  final val = CustomerMergeRequest(
    sourceBrandCustomerId: $checkedConvert(
      'sourceBrandCustomerId',
      (v) => v as String,
    ),
    mergeNotes: $checkedConvert('mergeNotes', (v) => v as bool? ?? true),
  );
  return val;
});

Map<String, dynamic> _$CustomerMergeRequestToJson(
  CustomerMergeRequest instance,
) => <String, dynamic>{
  'sourceBrandCustomerId': instance.sourceBrandCustomerId,
  'mergeNotes': ?instance.mergeNotes,
};
