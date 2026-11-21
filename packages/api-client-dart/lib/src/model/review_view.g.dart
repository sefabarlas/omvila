// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'review_view.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ReviewViewCWProxy {
  ReviewView id(String id);

  ReviewView appointmentId(String appointmentId);

  ReviewView branchId(String branchId);

  ReviewView rating(int rating);

  ReviewView comment(String? comment);

  ReviewView customer(CustomerRef customer);

  ReviewView serviceNames(List<String> serviceNames);

  ReviewView staffId(String? staffId);

  ReviewView staffName(String? staffName);

  ReviewView createdUtc(DateTime createdUtc);

  ReviewView isPublished(bool isPublished);

  ReviewView reply(ReviewViewReply? reply);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ReviewView(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ReviewView(...).copyWith(id: 12, name: "My name")
  /// ````
  ReviewView call({
    String id,
    String appointmentId,
    String branchId,
    int rating,
    String? comment,
    CustomerRef customer,
    List<String> serviceNames,
    String? staffId,
    String? staffName,
    DateTime createdUtc,
    bool isPublished,
    ReviewViewReply? reply,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfReviewView.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfReviewView.copyWith.fieldName(...)`
class _$ReviewViewCWProxyImpl implements _$ReviewViewCWProxy {
  const _$ReviewViewCWProxyImpl(this._value);

  final ReviewView _value;

  @override
  ReviewView id(String id) => this(id: id);

  @override
  ReviewView appointmentId(String appointmentId) =>
      this(appointmentId: appointmentId);

  @override
  ReviewView branchId(String branchId) => this(branchId: branchId);

  @override
  ReviewView rating(int rating) => this(rating: rating);

  @override
  ReviewView comment(String? comment) => this(comment: comment);

  @override
  ReviewView customer(CustomerRef customer) => this(customer: customer);

  @override
  ReviewView serviceNames(List<String> serviceNames) =>
      this(serviceNames: serviceNames);

  @override
  ReviewView staffId(String? staffId) => this(staffId: staffId);

  @override
  ReviewView staffName(String? staffName) => this(staffName: staffName);

  @override
  ReviewView createdUtc(DateTime createdUtc) => this(createdUtc: createdUtc);

  @override
  ReviewView isPublished(bool isPublished) => this(isPublished: isPublished);

  @override
  ReviewView reply(ReviewViewReply? reply) => this(reply: reply);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ReviewView(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ReviewView(...).copyWith(id: 12, name: "My name")
  /// ````
  ReviewView call({
    Object? id = const $CopyWithPlaceholder(),
    Object? appointmentId = const $CopyWithPlaceholder(),
    Object? branchId = const $CopyWithPlaceholder(),
    Object? rating = const $CopyWithPlaceholder(),
    Object? comment = const $CopyWithPlaceholder(),
    Object? customer = const $CopyWithPlaceholder(),
    Object? serviceNames = const $CopyWithPlaceholder(),
    Object? staffId = const $CopyWithPlaceholder(),
    Object? staffName = const $CopyWithPlaceholder(),
    Object? createdUtc = const $CopyWithPlaceholder(),
    Object? isPublished = const $CopyWithPlaceholder(),
    Object? reply = const $CopyWithPlaceholder(),
  }) {
    return ReviewView(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      appointmentId: appointmentId == const $CopyWithPlaceholder()
          ? _value.appointmentId
          // ignore: cast_nullable_to_non_nullable
          : appointmentId as String,
      branchId: branchId == const $CopyWithPlaceholder()
          ? _value.branchId
          // ignore: cast_nullable_to_non_nullable
          : branchId as String,
      rating: rating == const $CopyWithPlaceholder()
          ? _value.rating
          // ignore: cast_nullable_to_non_nullable
          : rating as int,
      comment: comment == const $CopyWithPlaceholder()
          ? _value.comment
          // ignore: cast_nullable_to_non_nullable
          : comment as String?,
      customer: customer == const $CopyWithPlaceholder()
          ? _value.customer
          // ignore: cast_nullable_to_non_nullable
          : customer as CustomerRef,
      serviceNames: serviceNames == const $CopyWithPlaceholder()
          ? _value.serviceNames
          // ignore: cast_nullable_to_non_nullable
          : serviceNames as List<String>,
      staffId: staffId == const $CopyWithPlaceholder()
          ? _value.staffId
          // ignore: cast_nullable_to_non_nullable
          : staffId as String?,
      staffName: staffName == const $CopyWithPlaceholder()
          ? _value.staffName
          // ignore: cast_nullable_to_non_nullable
          : staffName as String?,
      createdUtc: createdUtc == const $CopyWithPlaceholder()
          ? _value.createdUtc
          // ignore: cast_nullable_to_non_nullable
          : createdUtc as DateTime,
      isPublished: isPublished == const $CopyWithPlaceholder()
          ? _value.isPublished
          // ignore: cast_nullable_to_non_nullable
          : isPublished as bool,
      reply: reply == const $CopyWithPlaceholder()
          ? _value.reply
          // ignore: cast_nullable_to_non_nullable
          : reply as ReviewViewReply?,
    );
  }
}

extension $ReviewViewCopyWith on ReviewView {
  /// Returns a callable class that can be used as follows: `instanceOfReviewView.copyWith(...)` or like so:`instanceOfReviewView.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ReviewViewCWProxy get copyWith => _$ReviewViewCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ReviewView _$ReviewViewFromJson(Map<String, dynamic> json) =>
    $checkedCreate('ReviewView', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'id',
          'appointmentId',
          'branchId',
          'rating',
          'customer',
          'serviceNames',
          'createdUtc',
          'isPublished',
          'reply',
        ],
      );
      final val = ReviewView(
        id: $checkedConvert('id', (v) => v as String),
        appointmentId: $checkedConvert('appointmentId', (v) => v as String),
        branchId: $checkedConvert('branchId', (v) => v as String),
        rating: $checkedConvert('rating', (v) => (v as num).toInt()),
        comment: $checkedConvert('comment', (v) => v as String?),
        customer: $checkedConvert(
          'customer',
          (v) => CustomerRef.fromJson(v as Map<String, dynamic>),
        ),
        serviceNames: $checkedConvert(
          'serviceNames',
          (v) => (v as List<dynamic>).map((e) => e as String).toList(),
        ),
        staffId: $checkedConvert('staffId', (v) => v as String?),
        staffName: $checkedConvert('staffName', (v) => v as String?),
        createdUtc: $checkedConvert(
          'createdUtc',
          (v) => DateTime.parse(v as String),
        ),
        isPublished: $checkedConvert('isPublished', (v) => v as bool),
        reply: $checkedConvert(
          'reply',
          (v) => v == null
              ? null
              : ReviewViewReply.fromJson(v as Map<String, dynamic>),
        ),
      );
      return val;
    });

Map<String, dynamic> _$ReviewViewToJson(ReviewView instance) =>
    <String, dynamic>{
      'id': instance.id,
      'appointmentId': instance.appointmentId,
      'branchId': instance.branchId,
      'rating': instance.rating,
      'comment': ?instance.comment,
      'customer': instance.customer.toJson(),
      'serviceNames': instance.serviceNames,
      'staffId': ?instance.staffId,
      'staffName': ?instance.staffName,
      'createdUtc': instance.createdUtc.toIso8601String(),
      'isPublished': instance.isPublished,
      'reply': instance.reply?.toJson(),
    };
