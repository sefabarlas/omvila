//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'segment_view.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class SegmentView {
  /// Returns a new [SegmentView] instance.
  SegmentView({

    required  this.startUtc,

    required  this.endUtc,

    required  this.staffBusy,
  });

  @JsonKey(
    
    name: r'startUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime startUtc;



  @JsonKey(
    
    name: r'endUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime endUtc;



  @JsonKey(
    
    name: r'staffBusy',
    required: true,
    includeIfNull: false,
  )


  final bool staffBusy;





    @override
    bool operator ==(Object other) => identical(this, other) || other is SegmentView &&
      other.startUtc == startUtc &&
      other.endUtc == endUtc &&
      other.staffBusy == staffBusy;

    @override
    int get hashCode =>
        startUtc.hashCode +
        endUtc.hashCode +
        staffBusy.hashCode;

  factory SegmentView.fromJson(Map<String, dynamic> json) => _$SegmentViewFromJson(json);

  Map<String, dynamic> toJson() => _$SegmentViewToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

