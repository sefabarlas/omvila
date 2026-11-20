// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'segment_view.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$SegmentViewCWProxy {
  SegmentView startUtc(DateTime startUtc);

  SegmentView endUtc(DateTime endUtc);

  SegmentView staffBusy(bool staffBusy);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `SegmentView(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// SegmentView(...).copyWith(id: 12, name: "My name")
  /// ````
  SegmentView call({DateTime startUtc, DateTime endUtc, bool staffBusy});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfSegmentView.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfSegmentView.copyWith.fieldName(...)`
class _$SegmentViewCWProxyImpl implements _$SegmentViewCWProxy {
  const _$SegmentViewCWProxyImpl(this._value);

  final SegmentView _value;

  @override
  SegmentView startUtc(DateTime startUtc) => this(startUtc: startUtc);

  @override
  SegmentView endUtc(DateTime endUtc) => this(endUtc: endUtc);

  @override
  SegmentView staffBusy(bool staffBusy) => this(staffBusy: staffBusy);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `SegmentView(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// SegmentView(...).copyWith(id: 12, name: "My name")
  /// ````
  SegmentView call({
    Object? startUtc = const $CopyWithPlaceholder(),
    Object? endUtc = const $CopyWithPlaceholder(),
    Object? staffBusy = const $CopyWithPlaceholder(),
  }) {
    return SegmentView(
      startUtc: startUtc == const $CopyWithPlaceholder()
          ? _value.startUtc
          // ignore: cast_nullable_to_non_nullable
          : startUtc as DateTime,
      endUtc: endUtc == const $CopyWithPlaceholder()
          ? _value.endUtc
          // ignore: cast_nullable_to_non_nullable
          : endUtc as DateTime,
      staffBusy: staffBusy == const $CopyWithPlaceholder()
          ? _value.staffBusy
          // ignore: cast_nullable_to_non_nullable
          : staffBusy as bool,
    );
  }
}

extension $SegmentViewCopyWith on SegmentView {
  /// Returns a callable class that can be used as follows: `instanceOfSegmentView.copyWith(...)` or like so:`instanceOfSegmentView.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$SegmentViewCWProxy get copyWith => _$SegmentViewCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

SegmentView _$SegmentViewFromJson(Map<String, dynamic> json) => $checkedCreate(
  'SegmentView',
  json,
  ($checkedConvert) {
    $checkKeys(json, requiredKeys: const ['startUtc', 'endUtc', 'staffBusy']);
    final val = SegmentView(
      startUtc: $checkedConvert('startUtc', (v) => DateTime.parse(v as String)),
      endUtc: $checkedConvert('endUtc', (v) => DateTime.parse(v as String)),
      staffBusy: $checkedConvert('staffBusy', (v) => v as bool),
    );
    return val;
  },
);

Map<String, dynamic> _$SegmentViewToJson(SegmentView instance) =>
    <String, dynamic>{
      'startUtc': instance.startUtc.toIso8601String(),
      'endUtc': instance.endUtc.toIso8601String(),
      'staffBusy': instance.staffBusy,
    };
