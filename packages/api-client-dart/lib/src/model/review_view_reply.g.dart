// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'review_view_reply.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ReviewViewReplyCWProxy {
  ReviewViewReply text(String text);

  ReviewViewReply repliedUtc(DateTime repliedUtc);

  ReviewViewReply repliedByName(String? repliedByName);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ReviewViewReply(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ReviewViewReply(...).copyWith(id: 12, name: "My name")
  /// ````
  ReviewViewReply call({
    String text,
    DateTime repliedUtc,
    String? repliedByName,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfReviewViewReply.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfReviewViewReply.copyWith.fieldName(...)`
class _$ReviewViewReplyCWProxyImpl implements _$ReviewViewReplyCWProxy {
  const _$ReviewViewReplyCWProxyImpl(this._value);

  final ReviewViewReply _value;

  @override
  ReviewViewReply text(String text) => this(text: text);

  @override
  ReviewViewReply repliedUtc(DateTime repliedUtc) =>
      this(repliedUtc: repliedUtc);

  @override
  ReviewViewReply repliedByName(String? repliedByName) =>
      this(repliedByName: repliedByName);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ReviewViewReply(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ReviewViewReply(...).copyWith(id: 12, name: "My name")
  /// ````
  ReviewViewReply call({
    Object? text = const $CopyWithPlaceholder(),
    Object? repliedUtc = const $CopyWithPlaceholder(),
    Object? repliedByName = const $CopyWithPlaceholder(),
  }) {
    return ReviewViewReply(
      text: text == const $CopyWithPlaceholder()
          ? _value.text
          // ignore: cast_nullable_to_non_nullable
          : text as String,
      repliedUtc: repliedUtc == const $CopyWithPlaceholder()
          ? _value.repliedUtc
          // ignore: cast_nullable_to_non_nullable
          : repliedUtc as DateTime,
      repliedByName: repliedByName == const $CopyWithPlaceholder()
          ? _value.repliedByName
          // ignore: cast_nullable_to_non_nullable
          : repliedByName as String?,
    );
  }
}

extension $ReviewViewReplyCopyWith on ReviewViewReply {
  /// Returns a callable class that can be used as follows: `instanceOfReviewViewReply.copyWith(...)` or like so:`instanceOfReviewViewReply.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ReviewViewReplyCWProxy get copyWith => _$ReviewViewReplyCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ReviewViewReply _$ReviewViewReplyFromJson(Map<String, dynamic> json) =>
    $checkedCreate('ReviewViewReply', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['text', 'repliedUtc']);
      final val = ReviewViewReply(
        text: $checkedConvert('text', (v) => v as String),
        repliedUtc: $checkedConvert(
          'repliedUtc',
          (v) => DateTime.parse(v as String),
        ),
        repliedByName: $checkedConvert('repliedByName', (v) => v as String?),
      );
      return val;
    });

Map<String, dynamic> _$ReviewViewReplyToJson(ReviewViewReply instance) =>
    <String, dynamic>{
      'text': instance.text,
      'repliedUtc': instance.repliedUtc.toIso8601String(),
      'repliedByName': ?instance.repliedByName,
    };
