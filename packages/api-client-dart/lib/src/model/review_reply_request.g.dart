// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'review_reply_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ReviewReplyRequestCWProxy {
  ReviewReplyRequest text(String text);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ReviewReplyRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ReviewReplyRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  ReviewReplyRequest call({String text});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfReviewReplyRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfReviewReplyRequest.copyWith.fieldName(...)`
class _$ReviewReplyRequestCWProxyImpl implements _$ReviewReplyRequestCWProxy {
  const _$ReviewReplyRequestCWProxyImpl(this._value);

  final ReviewReplyRequest _value;

  @override
  ReviewReplyRequest text(String text) => this(text: text);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ReviewReplyRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ReviewReplyRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  ReviewReplyRequest call({Object? text = const $CopyWithPlaceholder()}) {
    return ReviewReplyRequest(
      text: text == const $CopyWithPlaceholder()
          ? _value.text
          // ignore: cast_nullable_to_non_nullable
          : text as String,
    );
  }
}

extension $ReviewReplyRequestCopyWith on ReviewReplyRequest {
  /// Returns a callable class that can be used as follows: `instanceOfReviewReplyRequest.copyWith(...)` or like so:`instanceOfReviewReplyRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ReviewReplyRequestCWProxy get copyWith =>
      _$ReviewReplyRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ReviewReplyRequest _$ReviewReplyRequestFromJson(Map<String, dynamic> json) =>
    $checkedCreate('ReviewReplyRequest', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['text']);
      final val = ReviewReplyRequest(
        text: $checkedConvert('text', (v) => v as String),
      );
      return val;
    });

Map<String, dynamic> _$ReviewReplyRequestToJson(ReviewReplyRequest instance) =>
    <String, dynamic>{'text': instance.text};
